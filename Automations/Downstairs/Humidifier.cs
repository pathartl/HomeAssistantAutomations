using NetDaemonApps.Extensions;
using NetDaemonApps.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetDaemonApps.Automations.Downstairs
{
    [NetDaemonApp]
    public class Humidifier : HomeAssistantAutomation
    {
        public Humidifier(IHaContext context, IScheduler scheduler, IMqttEntityManager entityManager, ITextToSpeechService tts) : base(context, scheduler, entityManager, tts)
        {
        }

        public override void Init()
        {
            Entities.BinarySensor.DownstairsHumidifierTankEmpty
                .StateChanges()
                .Where(e => e.New?.State == "on")
                .Subscribe(e =>
                {
                    var random = new Random();

                    Logger.Info("Humidifier tank is empty, sending notification...");

                    Notifications.SendPushNotification(new PushNotification()
                    {
                        Title = "Downstairs Humidifier Empty!",
                        Message = Config.Humidifier.EmptyMessages.ElementAt(random.Next(0, Config.Humidifier.EmptyMessages.Count)),
                        Tag = "DownstairsHumidifierEmpty",
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
