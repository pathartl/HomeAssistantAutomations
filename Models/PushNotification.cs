using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace NetDaemonApps.Models
{
    public static class PushNotificationPriority
    {
        public static readonly string Default = "default";
        public static readonly string Min = "min";
        public static readonly string Max = "max";
        public static readonly string Low = "low";
        public static readonly string High = "high";
    }

    public class PushNotification
    {
        [JsonPropertyName("title")]
        public string Title { get; set; }

        [JsonPropertyName("message")]
        public string Message { get; set; }

        [JsonIgnore]
        public string Tag { get; set; }

        [JsonPropertyName("data")]
        public PushNotificationData Data { get; set; }
    }

    public class PushNotificationData
    {
        [JsonPropertyName("ttl")]
        public int TTL { get; set; }

        [JsonPropertyName("priority")]
        public string Priority { get; set; }

        [JsonPropertyName("tag")]
        public string Tag { get; set; }

        [JsonPropertyName("image")]
        public string Image { get; set; }

        [JsonPropertyName("actions")]
        public IEnumerable<PushNotificationAction> Actions { get; set; }

        [JsonIgnore]
        public PushNotificationSubscription Subscription { get; set; }
    }

    public class PushNotificationAction
    {
        [JsonPropertyName("action")]
        public string Action { get; set; }

        [JsonPropertyName("title")]
        public string Title { get; set; }

        [JsonPropertyName("url")]
        public string Url { get; set; }

        public PushNotificationAction(string action, string title, string url)
        {
            Action = action;
            Title = title;
            Url = url;
        }

        public PushNotificationAction(string action, string title)
        {
            Action = action;
            Title = title;
        }
    }

    public class PushNotificationSubscription
    {
        public string Tag { get; set; }
        public string[] Services { get; set; }
    }
}
