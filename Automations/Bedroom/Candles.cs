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
    public class Candles : HomeAssistantAutomation
    {
        public Candles(IHaContext context, IScheduler scheduler, IMqttEntityManager entityManager, ITextToSpeechService tts) : base(context, scheduler, entityManager, tts)
        {
        }

        public override async void Init()
        {
            Entities.Switch.Candles
                .StateAllChanges()
                .WhenStateIsFor(s => s?.State == "on", TimeSpan.FromMinutes(Config.Candles.AutoOffMinutes))
                .Subscribe(e =>
                {
                    Entities.Button.Extinguish.Press();
                });
        }
    }
}
