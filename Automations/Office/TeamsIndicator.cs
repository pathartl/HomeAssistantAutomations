using NetDaemon.HassModel.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetDaemonApps.Automations.Office
{
    [NetDaemonApp]
    public class TeamsIndicator : HomeAssistantAutomation
    {
        public TeamsIndicator(IHaContext context, IScheduler scheduler, IMqttEntityManager entityManager, ITextToSpeechService tts) : base(context, scheduler, entityManager, tts)
        {
        }

        public override void Init()
        {
            Entities.Sensor.TeamsActivity.StateAllChanges()
                .Subscribe(s =>
                {
                    switch (s.New?.State)
                    {
                        case "In a call":
                            Entities.Light.OfficeIndicator.TurnOn(new LightTurnOnParameters()
                            {
                                Brightness = 255,
                                RgbColor = new int[] { 255, 0, 0 }
                            });
                            break;

                        default:
                            if (Entities.Light.OfficeLights.IsOn())
                            {
                                Entities.Light.OfficeIndicator.TurnOn(new LightTurnOnParameters()
                                {
                                    Brightness = (long)Entities.Light.OfficeLights.Attributes.Brightness,
                                    ColorTemp = 400
                                });
                            }
                            else
                            {
                                Entities.Light.OfficeIndicator.TurnOff();
                            }
                            break;
                    }
                });

            Entities.Light.OfficeLights.StateAllChanges()
                .Subscribe(s =>
                {
                    if (Entities.Light.OfficeIndicator.State != "In a call")
                    {
                        switch (s.New?.State)
                        {
                            case "on":
                                Entities.Light.OfficeIndicator.TurnOn(new LightTurnOnParameters()
                                {
                                    Brightness = 255,
                                    ColorTemp = 400
                                });
                                break;

                            case "off":
                                Entities.Light.OfficeIndicator.TurnOff();
                                break;
                        }
                    }
                });
        }
    }
}
