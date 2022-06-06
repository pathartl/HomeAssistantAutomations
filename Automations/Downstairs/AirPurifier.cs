using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetDaemonApps.Automations.Downstairs
{
    [NetDaemonApp]
    public class AirPurifier : HomeAssistantAutomation
    {
        public AirPurifier(IHaContext context, IScheduler scheduler, IMqttEntityManager entityManager, ITextToSpeechService tts) : base(context, scheduler, entityManager, tts)
        {
        }

        public override void Init()
        {
            Entities.MediaPlayer.LivingRoomTv
                .StateAllChanges()
                .Subscribe(s =>
                {
                    switch (s.New?.State)
                    {
                        case "playing":
                            Entities.Fan.AirPurifier.TurnOff();
                            break;

                        case "idle":
                        case "paused":
                        case "off":
                            Entities.Fan.AirPurifier.TurnOn();
                            Entities.Fan.AirPurifier.SetPresetMode("auto");
                            break;
                    }
                });
        }
    }
}
