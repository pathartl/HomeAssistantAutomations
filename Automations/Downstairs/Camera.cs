using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetDaemonApps.Automations.Downstairs
{
    [NetDaemonApp]
    public class Camera : HomeAssistantAutomation
    {
        public Camera(IHaContext context, IScheduler scheduler, IMqttEntityManager entityManager, ITextToSpeechService tts) : base(context, scheduler, entityManager, tts)
        {
        }

        public override void Init()
        {
            Entities.Person.Sierra.StateChanges()
                .Subscribe(s =>
                {
                    if (s.New?.State != s.Old?.State)
                    {
                        Logger.Info("Sierra state changed to {State}", s.New?.State);
                        SwivelCameraIfHome();
                    }
                });

            Entities.Person.PatHartl.StateChanges()
                .Subscribe(s =>
                {
                    if (s.New?.State != s.Old?.State)
                    {
                        Logger.Info("Pat state changed to {State}", s.New?.State);
                        SwivelCameraIfHome();
                    }
                });
        }

        private void SwivelCameraIfHome()
        {
            if (Entities.Person.PatHartl.State == "home" || Entities.Person.Sierra.State == "home")
            {
                Logger.Info("Someone's home, facing the camera towards the wall...");
                Entities.Cover.LivingRoomCameraMoveLeftRight.SetCoverPosition(100);
            }
            else
            {
                if (Entities.Cover.LivingRoomCameraMoveLeftRight.Attributes?.CurrentPosition <= 90 ||
                    Entities.Cover.LivingRoomCameraMoveLeftRight.Attributes?.CurrentPosition >= 10)
                {
                    Logger.Info("Nobody's home, repositioning the camera...");
                    Entities.Cover.LivingRoomCameraMoveLeftRight.SetCoverPosition(50);
                }
            }
        }
    }
}
