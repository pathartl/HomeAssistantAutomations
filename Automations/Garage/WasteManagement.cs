using NetDaemonApps.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetDaemonApps.Automations.Garage
{
    [NetDaemonApp]
    public class WasteManagement : HomeAssistantAutomation
    {
        public WasteManagement(IHaContext context, IScheduler scheduler, IMqttEntityManager entityManager, ITextToSpeechService tts) : base(context, scheduler, entityManager, tts)
        {
        }

        public override void Init()
        {
            DateTime startingDateGarbage;
            DateTime startingDateRecycling;

            if (DateTime.TryParse(Config.WasteManagement.StartingDateGarbage, out startingDateGarbage))
            {
                var notificationDateTime = startingDateGarbage.AddHours(-1 * Config.WasteManagement.NotifyHoursBefore);

                Scheduler.ScheduleCron($"{notificationDateTime.Minute} {notificationDateTime.Hour} * * *", () =>
                {
                    var since = DateTime.Now - notificationDateTime;

                    if (since.TotalDays % Config.WasteManagement.DaysBetweenGarbage == 0)
                    {
                        Notifications.SendPushNotification(new PushNotification()
                        {
                            Title = "Garbage Day",
                            Message = "Tomorrow's garbage day. The bin needs to be put out!",
                            Tag = "GarbageDay",
                            Data = new PushNotificationData()
                            {
                                TTL = 0,
                                Priority = PushNotificationPriority.High
                            }
                        });
                    }
                });
            }

            if (DateTime.TryParse(Config.WasteManagement.StartingDateRecycling, out startingDateRecycling))
            {
                var notificationDateTime = startingDateRecycling.AddHours(-1 * Config.WasteManagement.NotifyHoursBefore);

                Scheduler.ScheduleCron($"{notificationDateTime.Minute} {notificationDateTime.Hour} * * *", () =>
                {
                    var since = DateTime.Now - notificationDateTime;

                    if (since.TotalDays % Config.WasteManagement.DaysBetweenGarbage == 0)
                    {
                        Notifications.SendPushNotification(new PushNotification()
                        {
                            Title = "Recycling Day",
                            Message = "Tomorrow's recycling day. The bin needs to be put out!",
                            Tag = "RecyclingDay",
                            Data = new PushNotificationData()
                            {
                                TTL = 0,
                                Priority = PushNotificationPriority.High
                            }
                        });
                    }
                });
            }
        }
    }
}
