using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetDaemonApps.Automations.Entry
{
    [NetDaemonApp]
    public class Lights : HomeAssistantAutomation
    {
        public Lights(IHaContext context, IScheduler scheduler, IMqttEntityManager entityManager, ITextToSpeechService tts) : base(context, scheduler, entityManager, tts)
        {
        }

        public override void Init()
        {
            Entities.Light.EntryLights
                .StateAllChanges()
                .WhenStateIsFor(s => s?.State == "on", TimeSpan.FromMinutes(Config.Entry.LightsAutoOffMinutes))
                .Subscribe(e =>
                {
                    Entities.Light.EntryLights.TurnOff();
                });
        }
    }
}
