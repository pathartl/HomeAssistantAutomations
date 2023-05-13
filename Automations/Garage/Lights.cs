using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetDaemonApps.Automations.Garage
{
    [NetDaemonApp]
    public class Lights : HomeAssistantAutomation
    {
        public Lights(IHaContext context, IScheduler scheduler, IMqttEntityManager entityManager, ITextToSpeechService tts) : base(context, scheduler, entityManager, tts)
        {
        }

        public override void Init()
        {
            Entities.Switch.WallMountedSwitch
                .StateChanges()
                .Where(s => s.New?.State == "on")
                .Subscribe(s =>
                {
                    ScheduleOffIn10Minutes();
                });

            Entities.Cover.GarageDoor
                .StateChanges()
                .Where(s => s.New?.State == "open")
                .Subscribe(s =>
                {
                    Entities.Switch.WallMountedSwitch.TurnOn();
                });
        }

        private void ScheduleOffIn10Minutes()
        {
            Logger.Info("Scheduling garage lights to turn off at {ScheduledDate}", DateTime.Now.AddMinutes(10));

            Scheduler.Schedule(TimeSpan.FromMinutes(10), () =>
            {
                //switch (Entities.MediaPlayer.Treadmill.State)
                switch ("")
                {
                    case "playing":
                    case "paused":
                        // Reschedule, someone's using the treadmill
                        Logger.Info("Rescheduling garage lights, the treadmill is playing media");
                        ScheduleOffIn10Minutes();
                        break;

                    case "idle":
                    case "off":
                    default:
                        Logger.Info("Turning off garage lights");
                        Entities.Switch.WallMountedSwitch.TurnOff();
                        break;
                }
            });
        }
    }
}
