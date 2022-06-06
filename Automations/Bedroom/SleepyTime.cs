using NetDaemon.HassModel.Entities;
using NetDaemonApps.Extensions;
using NetDaemonApps.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace NetDaemonApps.Automations.Bedroom
{
    public static class SunsetState
    {
        public const string Daylight = "Daylight";
        public const string Sunset = "Sunset";
        public const string Twilight = "Twilight";
        public const string Night = "Night";
    }

    [NetDaemonApp]
    public class SleepyTime : HomeAssistantAutomation
    {
        private IDisposable DaylightSchedule;
        private IDisposable SunsetSchedule;
        private IDisposable TwilightSchedule;

        public SleepyTime(IHaContext context, IScheduler scheduler, IMqttEntityManager entityManager, ITextToSpeechService tts) : base(context, scheduler, entityManager, tts)
        {
        }

        public override async void Init()
        {
            await EntityManager.CreateAsync("button.start_sleepytime", new EntityCreationOptions()
            {
                Name = "Start Sleepy Time"
            }, new
            {
                icon = "mdi:sleep"
            });

            await EntityManager.CreateAsync("sensor.sleepytime_status", new EntityCreationOptions()
            {
                Name = "Sleepy Time Status"
            });

            Entities.Sensor.SleepytimeStatus.StateAllChanges()
                .SubscribeAsync(async s =>
                {
                    if (s.New?.State != s.Old?.State)
                    {
                        try { DaylightSchedule.Dispose(); } catch { }
                        try { SunsetSchedule.Dispose(); } catch { }
                        try { TwilightSchedule.Dispose(); } catch { }

                        switch (s.New?.State)
                        {
                            case SunsetState.Daylight:
                                TurnOffLights();
                                TurnOnNightstandLamp();
                                Scheduler.Schedule(TimeSpan.FromSeconds(18), StartPlaylist);

                                DaylightSchedule = Scheduler.Schedule(TimeSpan.FromMinutes(Entities.InputNumber.SleepytimeDaylightMinutes.State.GetValueOrDefault()), async () =>
                                {
                                    await EntityManager.SetStateAsync(Entities.Sensor.SleepytimeStatus.EntityId, SunsetState.Sunset);
                                });
                                break;

                            case SunsetState.Sunset:
                                await StartNightstandSunsetTransition();
                                break;

                            case SunsetState.Twilight:
                                await StartNightstandTwilightTransition();
                                break;

                            case SunsetState.Night:
                                NotifyPat();

                                if (Entities.Light.NightstandRgbLamp.IsOn())
                                    Entities.Light.NightstandRgbLamp.TurnOff();

                                Entities.InputBoolean.SleepytimeEnabled.TurnOff();
                                break;
                        }
                    }
                });

            Entities.Button.StartSleepytime.OnPress(async () =>
            {
                switch (Entities.Sensor.SleepytimeStatus.State)
                {
                    case SunsetState.Night:
                    default:
                        await EntityManager.SetStateAsync(Entities.Sensor.SleepytimeStatus.EntityId, SunsetState.Daylight);
                        break;

                    case SunsetState.Daylight:
                        await EntityManager.SetStateAsync(Entities.Sensor.SleepytimeStatus.EntityId, SunsetState.Sunset);
                        break;

                    case SunsetState.Sunset:
                        await EntityManager.SetStateAsync(Entities.Sensor.SleepytimeStatus.EntityId, SunsetState.Twilight);
                        break;

                    case SunsetState.Twilight:
                        await EntityManager.SetStateAsync(Entities.Sensor.SleepytimeStatus.EntityId, SunsetState.Night);
                        break;
                }
            });

            Notifications.OnAction("start-sleepy-time", (e) =>
            {
                Entities.Button.StartSleepytime.Press();
            });

            Entities.Sensor.SleepyTimeButtonAction.StateChanges()
                .Subscribe(s =>
                {
                    if (s.New?.State == "on")
                        Entities.Button.StartSleepytime.Press();
                });

            Scheduler.ScheduleCron($"* * * * *", () =>
            {
                var sleepyTime = Entities.InputDatetime.SleepytimeStartTime;
                DateTime time;

                DateTime.TryParse(sleepyTime.State, out time);

                var now = DateTime.Now;

                if (now.Minute == time.Minute && now.Hour == time.Hour)
                {
                    // Skip on weekends if sleepy time weekend is off
                    if (Entities.InputBoolean.SleepytimeNotifyWeekend.IsOff() && (now.DayOfWeek == DayOfWeek.Saturday || now.DayOfWeek == DayOfWeek.Sunday))
                    {
                        return;
                    }

                    var random = new Random();

                    Notifications.SendPushNotification(new PushNotification()
                    {
                        Title = "Sleepy Time!",
                        Message = "It's time to unwind and go to bed",
                        Tag = "SleepyTimeNag",
                        Data = new PushNotificationData()
                        {
                            Image = Config.SleepyTime.NotificationImages.ElementAt(random.Next(0, Config.SleepyTime.NotificationImages.Count)),
                            TTL = 0,
                            Priority = PushNotificationPriority.High,
                            Actions = new PushNotificationAction[]
                            {
                                new PushNotificationAction("start-sleepy-time", "Start Snoozin!")
                            }
                        }
                    });
                }
            });

            
        }

        private void StartPlaylist()
        {
            Services.Spotcast.Start(new SpotcastStartParameters()
            {
                DeviceName = Config.SleepyTime.SpotcastDeviceName,
                Uri = Config.SleepyTime.Playlist,
                RandomSong = true,
                Shuffle = true
            });
        }

        private void TurnOffLights()
        {
            Entities.Light.MasterBedroomBathroomLights.TurnOff(20);
            Entities.Light.MasterBedroomClosetLights.TurnOff(20);
            Entities.Light.MasterBedroomLights.TurnOff(20);
            Entities.Light.MasterBathOverheadLight.TurnOff(20);
        }

        private void TurnOnNightstandLamp()
        {
            Entities.Light.NightstandRgbLamp.TurnOn(new LightTurnOnParameters()
            {
                ColorTemp = 500,
                BrightnessPct = (long)Entities.InputNumber.SleepytimeMaxBrightness.State.GetValueOrDefault(),
                Transition = 30
            });
        }

        private async Task StartNightstandSunsetTransition()
        {
            var startingColor = new Color(Config.SleepyTime.SunsetStartColor).HexToRGB();
            var endingColor = new Color(Config.SleepyTime.SunsetEndColor).HexToRGB();
            var duration = Entities.InputNumber.SleepytimeSunsetMinutes.State.GetValueOrDefault();
            var brightness = (int)((Entities.InputNumber.SleepytimeMaxBrightness.State.GetValueOrDefault() / 100) * 255);

            await Entities.Light.NightstandRgbLamp.TransitionColors(startingColor, endingColor, TimeSpan.FromMinutes(duration), brightness, brightness);

            SunsetSchedule = Scheduler.Schedule(TimeSpan.FromMinutes(duration), async () =>
            {
                await EntityManager.SetStateAsync(Entities.Sensor.SleepytimeStatus.EntityId, SunsetState.Twilight);
            });
        }

        private async Task StartNightstandTwilightTransition()
        {
            var duration = Entities.InputNumber.SleepytimeTwilightMinutes.State.GetValueOrDefault();

            await Entities.Light.NightstandRgbLamp.TransitionBrightness(0, TimeSpan.FromMinutes(duration));

            TwilightSchedule = Scheduler.Schedule(TimeSpan.FromMinutes(duration), async () =>
            {
                await EntityManager.SetStateAsync(Entities.Sensor.SleepytimeStatus.EntityId, SunsetState.Night);
            });
        }

        private void NotifyPat()
        {
            string message;

            switch (Entities.Sensor.Starbuck.State)
            {
                case "living_room":
                    message = "Sierra needs a tuck tuck! Starbuck is in the living room.";
                    break;

                case "bedroom":
                    message = "Your girls need a tuck-tuck!";
                    break;

                case "office":
                default:
                    message = "Sierra needs a tuck-tuck!";
                    break;
            }

            Notifications.SendPushNotification(new PushNotification()
            {
                Title = "Tuck Tuck!",
                Message = message,
                Tag = "SleepyTimeTuckTuck",
                Data = new PushNotificationData()
                {
                    TTL = 0,
                    Priority = PushNotificationPriority.High
                }
            });
        }
    }
}
