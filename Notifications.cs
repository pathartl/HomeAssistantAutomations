using NetDaemonApps.Models;
using System.Linq;
using System.Text.Json.Serialization;

namespace NetDaemonApps
{
    record NotificationActionEvent
    {
        [JsonPropertyName("action")]
        public string Action { get; set; }

        [JsonPropertyName("command")]
        public string Command { get; set; }
    }

    public class Notifications
    {
        private IHaContext Context { get; set; }
        private Config Config { get; set; }

        public Notifications(IHaContext context, Config config)
        {
            Context = context;
            Config = config;
        }

        public void SendPushNotification(PushNotification notification)
        {
            var subscription = Config.Notification.Subscriptions.FirstOrDefault(s => s.Tag == notification.Tag);

            ClearPushNotification(notification.Tag, subscription.Services);

            if (notification.Data == null)
            {
                notification.Data = new PushNotificationData();
            }

            notification.Data.Tag = notification.Tag;

            foreach (var service in subscription.Services)
            {
                Context.CallService("notify", service, null, notification);
            }
        }

        public void SendPushNotification(PushNotification notification, params string[] services)
        {
            foreach (var service in services)
            {
                Context.CallService("notify", service, null, notification);
            }
        }

        public void ClearPushNotification(string tag, params string[] services)
        {
            var data = new PushNotificationData()
            {
                Tag = tag,
            };

            SendPushNotification(new PushNotification()
            {
                Message = "clear_notification",
                Data = data
            }, services);
        }

        public IObservable<Event> OnAction(string action)
        {
            return Context.Events
                .Filter<NotificationActionEvent>("mobile_app_notification_action")
                .Where(e => e.Data != null && e.Data.Action == action);
        }

        public void OnAction(string actionName, Action<Event> action)
        {
            OnAction(actionName).Subscribe(action);
        }
    }
}
