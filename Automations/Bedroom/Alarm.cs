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
    public static class SunriseState
    {
        public const string Daylight = "Daylight";
        public const string Sunrise = "Sunrise";
        public const string Dawn = "Dawn";
        public const string Night = "Night";
    }

    [NetDaemonApp]
    public class Alarm : HomeAssistantAutomation
    {
        private IDisposable DawnSchedule;
        private IDisposable SunriseSchedule;

        public Alarm(IHaContext context, IScheduler scheduler, IMqttEntityManager entityManager, ITextToSpeechService tts) : base(context, scheduler, entityManager, tts)
        {
        }

        public override async void Init()
        {
            await EntityManager.CreateAsync("sensor.alarm_sunrise_status", new EntityCreationOptions()
            {
                Name = "Alarm Sunrise Status"
            });

            Entities.Sensor.AlarmSunriseStatus.StateAllChanges()
                .SubscribeAsync(async s =>
                {
                    if (s.New?.State != s.Old?.State)
                    {
                        try { SunriseSchedule.Dispose(); } catch { }
                        try { DawnSchedule.Dispose(); } catch { }

                        switch (s.New?.State)
                        {
                            case SunriseState.Dawn:
                                await StartNightstandDawnTransition();

                                DawnSchedule = Scheduler.Schedule(TimeSpan.FromMinutes(Entities.InputNumber.AlarmDawnMinutes.State.GetValueOrDefault()), async () =>
                                {
                                    await EntityManager.SetStateAsync(Entities.Sensor.AlarmSunriseStatus.EntityId, SunriseState.Sunrise);
                                });
                                break;

                            case SunriseState.Sunrise:
                                await StartNightstandSunriseTransition();
                                break;
                        }
                    }
                });

            Scheduler.ScheduleCron("* * * * *", async () =>
            {
                try
                {
                    DateTime pat;
                    DateTime sierra;

                    DateTime.TryParse(Entities.Sensor.PatSPhoneNextAlarm.State, out pat);
                    DateTime.TryParse(Entities.Sensor.SierraSPhoneNextAlarm.State, out sierra);

                    var dawnDuration = Entities.InputNumber.AlarmDawnMinutes.State.GetValueOrDefault();
                    var sunriseDuration = Entities.InputNumber.AlarmSunriseMinutes.State.GetValueOrDefault();

                    var now = DateTime.Now;

                    if (IsItTime(pat, TimeSpan.FromMinutes(Config.Alarm.MinutesBeforeAlarm)))
                    {
                        if (sierra.Date != now.Date || (pat > sierra && pat.Date == now.Date))
                        {
                            await WakeUpPat();
                        }
                    }
                    else if (IsItTime(sierra, TimeSpan.FromMinutes(dawnDuration + sunriseDuration)))
                    {
                        await WakeUpSierra();
                    }
                }
                catch (Exception ex)
                { }
            });
        }

        private bool IsItTime(DateTime input, TimeSpan offset)
        {
            var now = DateTime.Now;

            if (input == DateTime.MinValue)
                return false;

            var start = input.Subtract(offset);

            return start.Hour == now.Hour && start.Minute == now.Minute;
        }

        private void StartPlaylist()
        {
            Services.Spotcast.Start(new SpotcastStartParameters()
            {
                DeviceName = Config.Alarm.SpotcastDeviceName,
                Uri = Config.Alarm.Playlist,
                RandomSong = true,
                Shuffle = true,
                StartVolume = Config.Alarm.Volume
            });
        }

        private async Task WakeUpPat()
        {
            StartPlaylist();

            await Task.Delay(TimeSpan.FromMinutes(Config.Alarm.MinutesBeforeLightsOn));

            await Entities.Light.MasterBedroomLights.TransitionBrightness(255, 128, TimeSpan.FromMinutes(Config.Alarm.LightsTransitionDuration));
        }

        private async Task WakeUpSierra()
        {
            await EntityManager.SetStateAsync(Entities.Sensor.AlarmSunriseStatus.EntityId, SunriseState.Dawn);
        }

        private async Task StartNightstandDawnTransition()
        {
            var duration = Entities.InputNumber.AlarmDawnMinutes.State.GetValueOrDefault();
            var brightness = (int)((Entities.InputNumber.AlarmMaxBrightness.State.GetValueOrDefault() / 100) * 255);
            var color = new Color(Config.SleepyTime.SunsetEndColor).HexToRGB();

            Entities.Light.NightstandRgbLamp.TurnOn(new LightTurnOnParameters()
            {
                Brightness = 1,
                RgbColor = new int[] { color.Red, color.Green, color.Blue },
                Transition = 0
            });

            await Entities.Light.NightstandRgbLamp.TransitionBrightness(brightness, TimeSpan.FromMinutes(duration));

            DawnSchedule = Scheduler.Schedule(TimeSpan.FromMinutes(duration), async () =>
            {
                await EntityManager.SetStateAsync(Entities.Sensor.AlarmSunriseStatus.EntityId, SunriseState.Sunrise);
            });
        }

        private async Task StartNightstandSunriseTransition()
        {
            var endingColor = new Color(Config.SleepyTime.SunsetStartColor).HexToRGB();
            var startingColor = new Color(Config.SleepyTime.SunsetEndColor).HexToRGB();
            var duration = Entities.InputNumber.AlarmSunriseMinutes.State.GetValueOrDefault();
            var brightness = (int)((Entities.InputNumber.AlarmMaxBrightness.State.GetValueOrDefault() / 100) * 255);

            await Entities.Light.NightstandRgbLamp.TransitionColors(endingColor, startingColor, TimeSpan.FromMinutes(duration), brightness, brightness);

            SunriseSchedule = Scheduler.Schedule(TimeSpan.FromMinutes(duration), async () =>
            {
                await EntityManager.SetStateAsync(Entities.Sensor.AlarmSunriseStatus.EntityId, SunriseState.Daylight);
            });
        }
    }
}
