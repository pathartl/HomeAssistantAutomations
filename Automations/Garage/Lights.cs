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
            Entities.Switch.GarageLights
                .StateChanges()
                .Where(s => s.New?.State == "on")
                .Subscribe(s =>
                {
                    ScheduleOffIn10Minutes();
                });
        }

        private void ScheduleOffIn10Minutes()
        {
            Scheduler.Schedule(TimeSpan.FromMinutes(10), () =>
            {
                switch (Entities.MediaPlayer.Treadmill.State)
                {
                    case "Playing":
                    case "Paused":
                        // Reschedule, someone's using the treadmill
                        ScheduleOffIn10Minutes();
                        break;

                    case "Idle":
                    case "off":
                        Entities.Switch.GarageLights.TurnOff();
                        break;
                }
            });
        }
    }
}
