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
        private bool DryerIdleValid = true;
        private IDisposable DryerIdleCompleteSubscription;

        public Dryer(IHaContext context, IScheduler scheduler, IMqttEntityManager entityManager, ITextToSpeechService tts) : base(context, scheduler, entityManager, tts)
        {
        }

        public override void Init()
        {
            Entities.Sensor.DryerStatus
                .StateAllChanges()
                .WhenStateIsFor(s => s?.State == "Running", TimeSpan.FromSeconds(30))
                .Subscribe(s =>
                {
                    DryerIdleCompleteSubscription = Entities.Sensor.DryerStatus
                        .StateAllChanges()
                        .WhenStateIsFor(s => s?.State == "Idle", TimeSpan.FromMinutes(1))
                        .Subscribe(s =>
                        {
                            Notifications.ClearPushNotification("DryerComplete");

                            Logger.Info("Dryer done, sending notification...");

                            Notifications.SendPushNotification(new PushNotification()
                            {
                                Title = "Dryer Done!",
                                Message = Config.Laundry.DryerCompleteMessages.RandomElementByWeight(m => m.Weight).Message,
                                Tag = "DryerComplete",
                                Data = new PushNotificationData()
                                {
                                    TTL = 0,
                                    Priority = PushNotificationPriority.High
                                }
                            });
                        });
                });

            Entities.Sensor.DryerStatus
                .StateAllChanges()
                .Subscribe(s =>
                {
                    if (DryerIdleCompleteSubscription != null && s.New?.State == "Running")
                    {
                        DryerIdleCompleteSubscription.Dispose();
                    }

                });
        }


    }
}
