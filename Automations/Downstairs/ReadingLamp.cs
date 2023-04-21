using NetDaemon.HassModel.Entities;
using NetDaemonApps.Extensions;
using NetDaemonApps.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace NetDaemonApps.Automations.Downstairs
{
    public static class SunsetState
    {
        public const string Daylight = "Daylight";
        public const string Sunset = "Sunset";
        public const string Twilight = "Twilight";
        public const string Night = "Night";
    }

    [NetDaemonApp]
    public class ReadingLamp : HomeAssistantAutomation
    {
        private IDisposable DaylightSchedule;
        private IDisposable SunsetSchedule;
        private IDisposable TwilightSchedule;

        public ReadingLamp(IHaContext context, IScheduler scheduler, IMqttEntityManager entityManager, ITextToSpeechService tts) : base(context, scheduler, entityManager, tts)
        {
        }

        public override async void Init()
        {
            await EntityManager.CreateAsync("button.start_cozy_sunset", new EntityCreationOptions()
            {
                Name = "Start Cozy Sunset"
            }, new
            {
                icon = "mdi:weather-sunset"
            });

            await EntityManager.CreateAsync("sensor.cozy_sunset_status", new EntityCreationOptions()
            {
                Name = "Cozy Sunset Status"
            });

            // Need physical zigbee button to trigger this
            /*Entities.Sensor.SleepyTimeButtonAction.StateAllChanges()
                .SubscribeAsync(async s =>
                {
                    if (s.New?.State == "on")
                    {
                        await EntityManager.SetStateAsync(Entities.Sensor.SleepytimeStatus.EntityId, "On");
                    }
                });*/

            Entities.Sensor.CozySunsetStatus.StateAllChanges()
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
                                TurnOnDaylight();

                                // Check if TV is on, turn it off if nobody's watching anything
                                if (Entities.Remote.LivingRoomRemote.IsOn() && Entities.MediaPlayer.LivingRoomTv.State == "Idle")
                                {
                                    Entities.Remote.LivingRoomRemote.TurnOff();
                                }

                                // Schedule sunset state change after daylight is complete
                                DaylightSchedule = Scheduler.Schedule(TimeSpan.FromMinutes(Entities.InputNumber.CozyDaylightMinutes.State.GetValueOrDefault()), () =>
                                {
                                    EntityManager.SetStateAsync(Entities.Sensor.CozySunsetStatus.EntityId, SunsetState.Sunset);
                                });
                                break;

                            case SunsetState.Sunset:
                                await StartSunsetTransition();
                                break;

                            case SunsetState.Twilight:
                                await StartTwilightTransition();
                                break;

                            case SunsetState.Night:
                                break;
                        }
                    }
                });

            Entities.Button.StartCozySunset.OnPress(async () =>
            {
                switch (Entities.Sensor.CozySunsetStatus.State)
                {
                    case SunsetState.Night:
                    default:
                        await EntityManager.SetStateAsync(Entities.Sensor.CozySunsetStatus.EntityId, SunsetState.Daylight);
                        break;

                    case SunsetState.Daylight:
                        await EntityManager.SetStateAsync(Entities.Sensor.CozySunsetStatus.EntityId, SunsetState.Sunset);
                        break;

                    case SunsetState.Sunset:
                        await EntityManager.SetStateAsync(Entities.Sensor.CozySunsetStatus.EntityId, SunsetState.Twilight);
                        break;

                    case SunsetState.Twilight:
                        await EntityManager.SetStateAsync(Entities.Sensor.CozySunsetStatus.EntityId, SunsetState.Night);
                        break;
                }
            });
        }

        private void TurnOffLights()
        {
            Entities.Switch.KitchenOverheadLights.TurnOff();
            Entities.Light.KitchenOfficeLights.TransitionBrightness(0, TimeSpan.FromSeconds(30));
            Entities.Light.KitchenLights.TransitionBrightness(0, TimeSpan.FromSeconds(30));
            Entities.Light.BathroomLights.TransitionBrightness(0, TimeSpan.FromSeconds(30));
            Entities.Light.StairLights.TransitionBrightness(0, TimeSpan.FromSeconds(30));
        }

        private void TurnOnDaylight()
        {
            Entities.Light.CozyCornerReadingLamp.TurnOn(new LightTurnOnParameters()
            {
                ColorTemp = 500,
                Brightness = 255,
                Transition = 30
            });
        }

        private async Task StartSunsetTransition()
        {
            // Refactor to separate config
            var startingColor = new Color(Config.SleepyTime.SunsetStartColor).HexToRGB();
            var endingColor = new Color(Config.SleepyTime.SunsetEndColor).HexToRGB();
            var duration = TimeSpan.FromMinutes(Entities.InputNumber.CozySunsetMinutes.State.GetValueOrDefault());

            await Entities.Light.CozyCornerReadingLamp.TransitionColors(startingColor, endingColor, duration);

            SunsetSchedule = Scheduler.Schedule(duration, async () =>
            {
                await EntityManager.SetStateAsync(Entities.Sensor.CozySunsetStatus.EntityId, SunsetState.Twilight);
            });
        }

        private async Task StartTwilightTransition()
        {
            var duration = TimeSpan.FromMinutes(Entities.InputNumber.CozyTwilightMinutes.State.GetValueOrDefault());

            await Entities.Light.CozyCornerReadingLamp.TransitionBrightness(0, duration);

            TwilightSchedule = Scheduler.Schedule(duration, async () =>
            {
                await EntityManager.SetStateAsync(Entities.Sensor.CozySunsetStatus.EntityId, SunsetState.Night);
            });
        }
    }
}
