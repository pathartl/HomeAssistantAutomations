using NetDaemon.HassModel.Entities;
using NetDaemonApps.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetDaemonApps.Automations.Entry
{
    [NetDaemonApp]
    public class FrontDoorLock : HomeAssistantAutomation
    {
        public static readonly string DoorLeftUnlockedTag = "door-left-unlocked";
        public static readonly string LockDoorActionName = "LOCK_DOOR";
        public static readonly string UnlockDoorActionName = "UNLOCK_DOOR";
        public static readonly int NotificationTimeout = 5;
        public static readonly int AutoLockTimeout = 20;

        public FrontDoorLock(IHaContext context, IScheduler scheduler, IMqttEntityManager entityManager, ITextToSpeechService tts) : base(context, scheduler, entityManager, tts)
        {
        }

        public override void Init()
        {
            Entities.Lock.FrontDoor
                .StateChanges()
                .WhenStateIsFor(s => s?.State == LockState.Unlocked, TimeSpan.FromMinutes(NotificationTimeout))
                .Subscribe(s =>
                {
                    Notifications.SendPushNotification(new PushNotification()
                    {
                        Title = "Door Unlocked!",
                        Message = $"The door has been unlocked for more than {NotificationTimeout} minutes.",
                        Tag = "DoorUnlocked",
                        Data = new PushNotificationData()
                        {
                            TTL = 0,
                            Priority = PushNotificationPriority.High,
                            Tag = DoorLeftUnlockedTag,
                            Actions = new PushNotificationAction[]
                            {
                                new PushNotificationAction(LockDoorActionName, "Lock Door")
                            }
                        }
                    });
                });

            Entities.Lock.FrontDoor
                .StateChanges()
                .WhenStateIsFor(s => s?.State == LockState.Unlocked, TimeSpan.FromMinutes(AutoLockTimeout))
                .Subscribe(s =>
                {
                    Notifications.ClearPushNotification(LockDoorActionName, MobileApp.PatPhone, MobileApp.SierraPhone);

                    s.Entity.Lock();
                });

            Notifications.OnAction(UnlockDoorActionName).Subscribe(s =>
            {
                Services.Lock.Unlock(ServiceTarget.FromEntity(Entities.Lock.FrontDoor.EntityId));
            });

            Notifications.OnAction(LockDoorActionName).Subscribe(s =>
            {
                Services.Lock.Lock(ServiceTarget.FromEntity(Entities.Lock.FrontDoor.EntityId));

                Notifications.ClearPushNotification(DoorLeftUnlockedTag, MobileApp.PatPhone, MobileApp.SierraPhone);
            });
        }
    }
}
