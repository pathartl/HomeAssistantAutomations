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
    public class Dryer : HomeAssistantAutomation
    {
        public static readonly string CycleCompleteNotificationTag = "dryer-complete";

        public Dryer(IHaContext context, IScheduler scheduler, IMqttEntityManager entityManager, ITextToSpeechService tts) : base(context, scheduler, entityManager, tts)
        {
        }

        public override void Init()
        {
            Entities.Sensor.DryerStatus
                .StateAllChanges()
                .WhenStateIsFor(s => s?.State == "Idle", TimeSpan.FromMinutes(1))
                .Subscribe(s =>
                {
                    Notifications.ClearPushNotification(CycleCompleteNotificationTag, MobileApp.PatPhone, MobileApp.SierraPhone);

                    Notifications.SendPushNotification(new PushNotification()
                    {
                        Title = "Dryer Done!",
                        Message = Config.Laundry.DryerCompleteMessages.RandomElementByWeight(m => m.Weight).Message,
                        Tag = "DryerComplete",
                        Data = new PushNotificationData()
                        {
                            TTL = 0,
                            Priority = PushNotificationPriority.High,
                            Tag = CycleCompleteNotificationTag
                        }
                    });
                });
        }
    }
}
