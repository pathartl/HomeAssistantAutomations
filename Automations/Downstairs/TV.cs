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
            Entities.MediaPlayer.LivingRoomTv
                .StateAllChanges()
                .Where(e => e.New?.Attributes?.MediaContentType != e.Old?.Attributes?.MediaContentType || e.New?.Attributes?.AppName != e.Old?.Attributes?.AppName)
                .Subscribe(s =>
                {
                    bool mediaContentChanged = s.Old?.Attributes?.MediaContentType != s.New?.Attributes?.MediaContentType;
                    var mediaContentType = s.New?.Attributes?.MediaContentType;

                    bool appChanged = s.Old?.Attributes?.AppName != s.New?.Attributes?.AppName;
                    var appName = s.New?.Attributes?.AppName;

                    Logger.Info("TV state changed | MediaContentType: {MediaContentType} | AppName: {AppName}", mediaContentType, appName);

                    if (mediaContentChanged || appChanged)
                    {
                        if (appName == "Spotify" || mediaContentType == "music")
                        {
                            Logger.Info("Music is playing, changing to 7 channel stereo...");
                            Services.MediaPlayer.SelectSoundMode(ServiceTarget.FromEntity(Entities.MediaPlayer.Receiver.EntityId), "7ch Stereo");
                        }
                        else if (mediaContentChanged || appChanged)
                        {
                            Logger.Info("Changing receiver to default surround preset...");
                            Services.MediaPlayer.SelectSoundMode(ServiceTarget.FromEntity(Entities.MediaPlayer.Receiver.EntityId), "Surround Decoder");
                        }
                    }
                });

            Entities.Remote.LivingRoomRemote
                .StateAllChanges()
                .Where(e => e.New?.Attributes?.CurrentActivity != e.Old?.Attributes?.CurrentActivity)
                .Subscribe(s =>
                {
                    Logger.Info("Harmony remote changed activity to {CurrentActivity}", s.New?.Attributes?.CurrentActivity);

                    switch (s.New?.Attributes?.CurrentActivity)
                    {
                        case "Patflix":
                            Entities.Button.TvUsbSwitchHost1.Press();
                            break;

                        case "Steam Deck":
                            Entities.Button.TvUsbSwitchHost2.Press();
                            break;

                        case "Xbox One X":
                            Entities.Button.TvUsbSwitchHost3.Press();
                            break;

                        case "Gaming PC":
                            Entities.Button.TvUsbSwitchHost4.Press();
                            break;
                    }
                });
        }
    }
}
