using NetDaemon.HassModel.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetDaemonApps.Automations.Downstairs
{
    [NetDaemonApp]
    public class TV : HomeAssistantAutomation
    {
        public TV(IHaContext context, IScheduler scheduler, IMqttEntityManager entityManager, ITextToSpeechService tts) : base(context, scheduler, entityManager, tts)
        {
        }

        public override void Init()
        {
            Entities.MediaPlayer.LivingRoomTv.StateAllChanges()
                .Subscribe(s =>
                {
                    if (s.Old?.Attributes?.MediaContentType != s.New?.Attributes?.MediaContentType && s.New?.Attributes?.MediaContentType == "music")
                    {
                        Services.MediaPlayer.SelectSoundMode(ServiceTarget.FromEntity(Entities.MediaPlayer.Receiver.EntityId), "7ch Stereo");
                    }
                    else if (s.Old?.Attributes?.MediaContentType != s.New?.Attributes?.MediaContentType)
                    {
                        Services.MediaPlayer.SelectSoundMode(ServiceTarget.FromEntity(Entities.MediaPlayer.Receiver.EntityId), "Surround Decoder");
                    }
                });
        }
    }
}
