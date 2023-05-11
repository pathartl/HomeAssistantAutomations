using NetDaemon.HassModel.Entities;
using NetDaemonApps.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetDaemonApps.Automations.Office
{
    [NetDaemonApp]
    public class Prusa : HomeAssistantAutomation
    {
        public Prusa(IHaContext context, IScheduler scheduler, IMqttEntityManager entityManager, ITextToSpeechService tts) : base(context, scheduler, entityManager, tts)
        {
        }

        public override void Init()
        {
            Entities.BinarySensor.OctoprintPrinting
                .StateChanges()
                .Subscribe(s =>
                {
                    if (s.New?.State == "off" && s.Old?.State == "on")
                    {
                        Logger.Info("Print job has finished, sending notification...");

                        Notifications.SendPushNotification(new PushNotification()
                        {
                            Title = "Print Done!",
                            Message = "The print job sent to the Prusa has finished.",
                            Tag = "PrusaPrintFinished",
                            Data = new PushNotificationData()
                            {
                                TTL = 0,
                                Priority = PushNotificationPriority.High,
                            }
                        });
                    }
                });
        }
    }
}
