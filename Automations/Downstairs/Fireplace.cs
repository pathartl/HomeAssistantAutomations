using NetDaemon.HassModel.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetDaemonApps.Automations.Downstairs
{
    [NetDaemonApp]
    public class Fireplace : HomeAssistantAutomation
    {
        public Fireplace(IHaContext context, IScheduler scheduler, IMqttEntityManager entityManager, ITextToSpeechService tts) : base(context, scheduler, entityManager, tts)
        {

        }

        public override async void Init()
        {
            Entities.InputBoolean.Fireplace.StateChanges()
                .Subscribe(s =>
                {
                    switch (s.New?.State)
                    {
                        case "on":
                            Entities.Switch.SoundEffects.TurnOn();

                            if (Entities.Sensor.Power.State < 2)
                            {
                                Entities.Button.Fireplace.Press();
                            }
                            break;

                        case "off":
                            Entities.Switch.SoundEffects.TurnOff();

                            if (Entities.Sensor.Power.State > 2)
                            {
                                Entities.Button.Fireplace.Press();
                            }
                            break;
                    }
                });

            Entities.Sensor.Power.StateChanges()
                .Subscribe(s =>
                {
                    if (s.New?.State > 2 && Entities.InputBoolean.Fireplace.IsOff())
                    {
                        Entities.InputBoolean.Fireplace.TurnOn();
                    }
                    else if (s.New?.State < 2 && Entities.InputBoolean.Fireplace.IsOn())
                    {
                        Entities.InputBoolean.Fireplace.TurnOff();
                    }
                });
        }
    }
}
