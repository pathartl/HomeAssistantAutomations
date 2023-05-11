using NetDaemon.HassModel.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetDaemonApps.Automations.Downstairs
{
    [NetDaemonApp]
    public class KitchenLights : HomeAssistantAutomation
    {
        public KitchenLights(IHaContext context, IScheduler scheduler, IMqttEntityManager entityManager, ITextToSpeechService tts) : base(context, scheduler, entityManager, tts)
        {
        }

        public override void Init()
        {
            Entities.Light.KitchenLights.StateAllChanges()
                .Subscribe(state =>
                {
                    SetOverheadLightsState();
                });

            Entities.Light.KitchenOfficeLights.StateAllChanges()
                .Subscribe(state =>
                {
                    SetOverheadLightsState();
                });
        }

        public void SetOverheadLightsState()
        {
            var kitchenLightsAtMax = Entities.Light.KitchenLights.IsOn() && Entities.Light.KitchenLights.Attributes?.Brightness >= 255;
            var officeLightsAtMax = Entities.Light.KitchenOfficeLights.IsOn() && Entities.Light.KitchenOfficeLights.Attributes?.Brightness >= 255;

            if (kitchenLightsAtMax && officeLightsAtMax)
            {
                Logger.Info("All kitchen lights are set to max, turning on the overhead lights...");
                Entities.Switch.KitchenOverheadLights.TurnOn();
            }
            else
            {
                Logger.Info("Kitchen lights are not at max, turning off the overhead lights...");
                Entities.Switch.KitchenOverheadLights.TurnOff();
            }
        }
    }
}
