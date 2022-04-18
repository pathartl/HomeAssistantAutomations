using NetDaemonApps.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetDaemonApps.Extensions
{
    public static class NotificationsExtensions
    {
        public static void SendPushNotification(this NotifyServices notifyServices, PushNotification notification, params string[] entityIds)
        {
            foreach (var entityId in entityIds)
            {
                notifyServices.Notify(new NotifyNotifyParameters()
                {
                    Target = new
                    {
                        entity_id = entityId,
                    },
                    Title = notification.Title,
                    Message = notification.Message,
                    Data = notification.Data,
                });
            }
        }

        public static void ClearPushNotification(this NotifyServices notifyServices, string tag, params string[] entityIds)
        {
            var data = new PushNotificationData()
            {
                Tag = tag
            };

            foreach (var entityId in entityIds)
            {
                notifyServices.Notify(new NotifyNotifyParameters()
                {
                    Target = new
                    {
                        entity_id = entityId,
                    },
                    Message = "clear_notification",
                    Data = data
                });
            }
        }

        public static void NotifyPhones(this IHaContext context, PushNotification notification)
        {
            context.CallService("notify", "mobile_app_pat_s_phone", null, notification);
            context.CallService("notify", "mobile_app_sierra_s_phone", null, notification);
        }
    }
}
