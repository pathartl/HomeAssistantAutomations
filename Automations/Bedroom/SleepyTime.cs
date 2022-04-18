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
    [NetDaemonApp]
    public class SleepyTime : HomeAssistantAutomation
    {
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

            await EntityManager.CreateAsync("switch.sleepytime_tuck_tuck_weekend", new EntityCreationOptions()
            {
                Name = "Tuck-Tuck On Weekends",
            }, new
            {
                icon = "mdi:bed"
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
                        switch (s.New?.State)
                        {
                            case "On":
                                TurnOffLights();
                                TurnOnNightstandLamp();
                                Scheduler.Schedule(TimeSpan.FromSeconds(18), StartPlaylist);

                                Scheduler.Schedule(TimeSpan.FromMinutes(Config.SleepyTime.SunsetColorTransitionDelayMinutes), () =>
                                {
                                    EntityManager.SetStateAsync(Entities.Sensor.SleepytimeStatus.EntityId, "Sunset");
                                });
                                break;

                            case "Sunset":
                                await StartNightstandSunsetTransition();
                                break;

                            case "Darken":
                                await StartNightstandDarkenTransition();
                                break;

                            case "Off":
                                NotifyPat();
                                Entities.InputBoolean.SleepytimeEnabled.TurnOff();
                                break;
                        }
                    }
                });

            Entities.Button.StartSleepytime.OnPress(async () =>
            {
                await EntityManager.SetStateAsync(Entities.Sensor.SleepytimeStatus.EntityId, "On");
            });

            Notifications.OnAction("start-sleepy-time", (e) =>
            {
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
                    if (Entities.InputBoolean.SleepytimeTuckTuckWeekend.IsOff() && (now.DayOfWeek == DayOfWeek.Saturday || now.DayOfWeek == DayOfWeek.Sunday))
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
        }

        private void TurnOnNightstandLamp()
        {
            Entities.Light.NightstandRgbLamp.TurnOn(new LightTurnOnParameters()
            {
                ColorTemp = 500,
                Brightness = 255,
                Transition = 30
            });
        }

        private async Task StartNightstandSunsetTransition()
        {
            var startingColor = new Color(Config.SleepyTime.SunsetStartColor).HexToRGB();
            var endingColor = new Color(Config.SleepyTime.SunsetEndColor).HexToRGB();

            await Entities.Light.NightstandRgbLamp.TransitionColors(startingColor, endingColor, TimeSpan.FromMinutes(Config.SleepyTime.SunsetColorTransitionMinutes));

            Scheduler.Schedule(TimeSpan.FromMinutes(Config.SleepyTime.SunsetColorTransitionMinutes), async () =>
            {
                await EntityManager.SetStateAsync(Entities.Sensor.SleepytimeStatus.EntityId, "Darken");
            });
        }

        private async Task StartNightstandDarkenTransition()
        {
            await Entities.Light.NightstandRgbLamp.TransitionBrightness(0, TimeSpan.FromMinutes(Config.SleepyTime.SunsetBrightnessTransitionMinutes));

            Scheduler.Schedule(TimeSpan.FromMinutes(Config.SleepyTime.SunsetBrightnessTransitionMinutes), async () =>
            {
                await EntityManager.SetStateAsync(Entities.Sensor.SleepytimeStatus.EntityId, "Off");
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
