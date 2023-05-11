using NetDaemonApps.Extensions;
using NetDaemonApps.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetDaemonApps.Automations.Laundry
{
    [NetDaemonApp]
    public class Washer : HomeAssistantAutomation
    {
        public Washer(IHaContext context, IScheduler scheduler, IMqttEntityManager entityManager, ITextToSpeechService tts) : base(context, scheduler, entityManager, tts)
        {
        }

        public override void Init()
        {
            Entities.BinarySensor.WasherCycleComplete
                .StateAllChanges()
                .Where(s => s.New?.State == "on")
                .Subscribe(e =>
                {
                    Notifications.ClearPushNotification("WasherComplete");

                    Logger.Info("Washing machine complete, sending notification...");

                    Notifications.SendPushNotification(new PushNotification()
                    {
                        Title = "Washing Machine Done!",
                        Message = Config.Laundry.WasherCompleteMessages.RandomElementByWeight(e => e.Weight).Message,
                        Tag = "WasherComplete",
                        Data = new PushNotificationData()
                        {
                            TTL = 0,
                            Priority = PushNotificationPriority.High
                        }
                    });
                });

            Entities.BinarySensor.WasherCycleComplete
                .StateAllChanges()
                .WhenStateIsFor(s => s?.State == "on", TimeSpan.FromHours(1))
                .Subscribe(e =>
                {
                    Notifications.ClearPushNotification("WasherComplete");

                    Logger.Info("Clothes in the washing machine has been sitting for an hour, sending notification...");

                    Notifications.SendPushNotification(new PushNotification()
                    {
                        Title = "Washing Machine Done!",
                        Message = "The wet laundry has been sitting for an hour!",
                        Tag = "WasherComplete",
                        Data = new PushNotificationData()
                        {
                            TTL = 0,
                            Priority = PushNotificationPriority.High
                        }
                    });
                });
        }
    }
}
