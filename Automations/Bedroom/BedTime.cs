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
    public class BedTime : HomeAssistantAutomation
    {
        public BedTime(IHaContext context, IScheduler scheduler, IMqttEntityManager entityManager, ITextToSpeechService tts) : base(context, scheduler, entityManager, tts)
        {
        }

        public override async void Init()
        {
            Entities.MediaPlayer.MasterBathSpeaker.StateAllChanges()
                .Where(s => s.Old?.Attributes?.AppName != s.New?.Attributes?.AppName)
                .Subscribe(async s =>
                {
                    Logger.Info("Master bath speaker state changed to {State}", s.Entity.Attributes?.AppName);

                    if (s.Entity.Attributes?.AppName == "Relaxing Sounds")
                    {
                        Entities.Light.LivingRoomLightsGroup.TurnOff();
                        Entities.Light.KitchenLightsGroup.TurnOff();
                        Entities.Light.BedroomLightsGroup.TurnOff();
                        Entities.Light.EntryLights.TurnOff();
                        Entities.Switch.OutdoorLights.TurnOff();
                        Entities.Switch.WallMountedSwitch.TurnOff();
                        Entities.Light.StairLights.TurnOff();
                        Entities.Light.OfficeIndicator.TurnOff();

                        Entities.Button.Fan.Press();

                        await Task.Delay(TimeSpan.FromMilliseconds(500));

                        Entities.Button.FanSwing.Press();
                    }
                });
        }
    }
}
