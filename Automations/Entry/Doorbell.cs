using NetDaemon.Extensions.Tts;
using NetDaemonApps.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetDaemonApps.Automations.Entry
{
    [NetDaemonApp]
    public class Doorbell : HomeAssistantAutomation
    {
        public Doorbell(IHaContext context, IScheduler scheduler, IMqttEntityManager entityManager, ITextToSpeechService tts) : base(context, scheduler, entityManager, tts)
        {
        }

        public override void Init()
        {
            Entities.BinarySensor.FrontDoorDoorbell2
                .StateAllChanges()
                .Where(s => s.New?.State == "on")
                .Subscribe(e =>
                {
                    Logger.Info("Doorbell state changed to '{State}'", e.New?.State);

                    Logger.Info("Turning on entry lights...");
                    Entities.Light.EntryLights.CallService("turn_on");

                    Logger.Info("Sending notification...");
                    Notifications.SendPushNotification(new PushNotification()
                    {
                        Title = "Ding Dong!",
                        Message = "Someone's at the door",
                        Tag = "Doorbell",
                        Data = new PushNotificationData()
                        {
                            Image = $"/api/camera_proxy/{Entities.Camera.AmcrestCamera.EntityId}",
                            TTL = 0,
                            Priority = PushNotificationPriority.High,
                            Actions = new PushNotificationAction[]
                            {
                                new PushNotificationAction(FrontDoorLock.LockDoorActionName, "Unlock Door")
                            }
                        }
                    });
                });
        }
    }
}
