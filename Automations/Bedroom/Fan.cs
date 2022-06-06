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
    public class Fan : HomeAssistantAutomation
    {
        public Fan(IHaContext context, IScheduler scheduler, IMqttEntityManager entityManager, ITextToSpeechService tts) : base(context, scheduler, entityManager, tts)
        {
        }

        public override async void Init()
        {
            await EntityManager.CreateAsync("button.bedroom_fan_fix", new EntityCreationOptions()
            {
                Name = "Fix Bedroom Fan"
            }, new
            {
                icon = "mdi:fan-alert"
            });

            await EntityManager.CreateAsync("fan.bedroom_fan", new EntityCreationOptions()
            {
                Name = "Bedroom Fan"
            }, new
            {
                icon = "mdi:fan",
                supported_features = 2 | 8,
                speed_count = 8
            });

            int maxLevel = 8;
            int minLevel = 1;

            Entities.Fan.BedroomFan.WithAttributesAs<FanAttributes>()
                .StateAllChanges()
                .Subscribe(s =>
                {

                });

            Entities.InputNumber.BedroomFanSpeed.StateAllChanges()
                .Subscribe(async s =>
                {
                    var mustRollOver = s.New?.State < s.Old?.State;

                    int presses = 0;

                    if (mustRollOver && s.Old != null && s.New != null)
                    {
                        // Given we want to go from a level of 5 to 4
                        // Parameters:
                        // Max level: 8
                        // Min level: 1
                        //
                        // If current = 5, we need X button presses until we roll over to 1
                        // 6, 7, 8, 1
                        // X = 4
                        //
                        // OR
                        // X = MaxLevel - CurrentLevel + 1
                        var pressesUntilRollover = maxLevel - (int)s.Old.State + 1;

                        // NEXT:
                        //
                        // Desired level: 4
                        // If rolled over to 1, we need Y button presses until we reach desired level
                        // 2, 3, 4
                        // Y = 3
                        //
                        // OR
                        // Y = DesiredLevel - MinLevel
                        var pressesUntilDesiredLevel = (int)s.New.State - minLevel;

                        presses = pressesUntilRollover + pressesUntilDesiredLevel;
                    }
                    else
                    {
                        presses = (int)(s.New?.State - s.Old?.State);
                    }

                    for (var i = 0; i < presses; i++)
                    {
                        Entities.Button.FanSpeed.Press();

                        await Task.Delay(TimeSpan.FromMilliseconds(250));
                    }
                });

            Entities.Button.BedroomFanFix.OnPress(() =>
            {
                Entities.InputBoolean.BedroomFanPower.TurnOn();
                Entities.InputNumber.BedroomFanSpeed.SetValue(1);
            });
        }

        private int PercentToLevel(double percent)
        {
            return 0;
        }
    }
}
