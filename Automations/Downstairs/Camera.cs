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
                        SwivelCameraIfHome();
                    }
                });

            Entities.Person.PatHartl.StateChanges()
                .Subscribe(s =>
                {
                    if (s.New?.State != s.Old?.State)
                    {
                        SwivelCameraIfHome();
                    }
                });
        }

        private void SwivelCameraIfHome()
        {
            if (Entities.Person.PatHartl.State == "home" || Entities.Person.Sierra.State == "home")
            {
                Entities.Cover.LivingRoomCameraMoveLeftRight.SetCoverPosition(100);
            }
            else
            {
                if (Entities.Cover.LivingRoomCameraMoveLeftRight.Attributes?.CurrentPosition <= 90 ||
                    Entities.Cover.LivingRoomCameraMoveLeftRight.Attributes?.CurrentPosition >= 10)
                {
                    Entities.Cover.LivingRoomCameraMoveLeftRight.SetCoverPosition(50);
                }
            }
        }
    }
}
