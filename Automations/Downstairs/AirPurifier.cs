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
                .Where(e => e.New?.State != e.Old?.State)
                .Subscribe(s =>
                {
                    Logger.Info("Living room TV state changed to {State}", s.New?.State);

                    switch (s.New?.State)
                    {
                        case "playing":
                            Logger.Info("Turning off air purifier");
                            Entities.Fan.AirPurifier.TurnOff();
                            break;

                        case "idle":
                        case "paused":
                        case "off":
                            Logger.Info("Turning on air purifier");
                            Entities.Fan.AirPurifier.TurnOn();
                            Entities.Fan.AirPurifier.SetPresetMode("auto");
                            break;
                    }
                });
        }
    }
}
