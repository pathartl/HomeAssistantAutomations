using NetDaemonApps.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetDaemonApps
{
    public class Config
    {
        public SleepyTimeConfig SleepyTime;
        public AlarmConfig Alarm;
        public CandlesConfig Candles;
        public HumidifierConfig Humidifier;
        public EntryConfig Entry;
        public WasteManagementConfig WasteManagement;
        public LaundryConfig Laundry;
        public MinerstatConfig Minerstat;
        public NotificationConfig Notification;
    }

    public class AlarmConfig
    {
        public string? Playlist;
        public string? SpotcastDeviceName;
        public int MinutesBeforeAlarm;
        public int MinutesBeforeLightsOn;
        public int LightsTransitionDuration;
        public int Volume;
    }

    public class SleepyTimeConfig
    {
        public string? Playlist;
        public string? SpotcastDeviceName;
        public IList<string> NotificationImages;
        public string? SunsetStartColor;
        public string? SunsetEndColor;
        public int SunsetDelayMinutes;
        public int SunsetColorTransitionMinutes;
        public int SunsetBrightnessTransitionMinutes;
    }

    public class CandlesConfig
    {
        public int AutoOffMinutes;
    }

    public class HumidifierConfig
    {
        public IList<string> EmptyMessages;
    }

    public class EntryConfig
    {
        public int LightsAutoOffMinutes;
    }

    public class WasteManagementConfig
    {
        public string? StartingDateGarbage;
        public string? StartingDateRecycling;
        public int DaysBetweenGarbage;
        public int DaysBetweenRecycling;
        public int NotifyHoursBefore;
    }

    public class LaundryConfig
    {
        public IList<WeightedMessage> DryerCompleteMessages;
        public IList<WeightedMessage> WasherCompleteMessages;
    }

    public class MinerstatConfig
    {
        public string AccessKey;
        public IList<string> Workers;
    }

    public class NotificationConfig
    {
        public IEnumerable<PushNotificationSubscription> Subscriptions;
    }

    public class WeightedMessage
    {
        public string? Message;
        public float Weight;
    }
}
