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
                    Logger.Info("Teams activity status has changed to {State}", s.New?.State);

                    switch (s.New?.State)
                    {
                        case "In a call":
                            Logger.Info("Turning indicator lamp to red...");

                            Entities.Light.OfficeIndicator.TurnOn(new LightTurnOnParameters()
                            {
                                Brightness = 255,
                                RgbColor = new int[] { 255, 0, 0 }
                            });
                            break;

                        default:
                            if (Entities.Light.OfficeLights.IsOn())
                            {
                                Logger.Info("Office lights are on, turning on the indicator lamp");

                                Entities.Light.OfficeIndicator.TurnOn(new LightTurnOnParameters()
                                {
                                    Brightness = (long)Entities.Light.OfficeLights.Attributes.Brightness,
                                    ColorTemp = 400
                                });
                            }
                            else
                            {
                                Logger.Info("Office lights are off, turning off the indicator lamp");

                                Entities.Light.OfficeIndicator.TurnOff();
                            }
                            break;
                    }
                });

            Entities.Light.OfficeLights.StateAllChanges()
                .Subscribe(s =>
                {
                    Logger.Info("Office lights changed state, Teams activity state is {State}", Entities.Sensor.TeamsActivity.State);

                    if (Entities.Sensor.TeamsActivity.State != "In a call")
                    {
                        Logger.Info("Not in a Teams call, indicator light should match office lights state of '{State}'", s.New?.State);

                        switch (s.New?.State)
                        {
                            case "on":
                                Logger.Info("Turning Teams indicator lamp on");

                                Entities.Light.OfficeIndicator.TurnOn(new LightTurnOnParameters()
                                {
                                    Brightness = 255,
                                    ColorTemp = 400
                                });
                                break;

                            case "off":
                                Logger.Info("Turning Teams indicator lamp off");

                                Entities.Light.OfficeIndicator.TurnOff();
                                break;
                        }
                    }
                });
        }
    }
}
