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
                    Logger.Info($"Front door has been unlocked for {NotificationTimeout} minutes, sending notification...");
                    Notifications.SendPushNotification(new PushNotification()
                    {
                        Title = "Door Unlocked!",
                        Message = $"The door has been unlocked for more than {NotificationTimeout} minutes.",
                        Tag = "DoorUnlocked",
                        Data = new PushNotificationData()
                        {
                            TTL = 0,
                            Priority = PushNotificationPriority.High,
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
                    Logger.Info("Front door has been unlocked too long, automatically locking...");

                    Notifications.ClearPushNotification(LockDoorActionName);

                    s.Entity.Lock();
                });

            Entities.Lock.FrontDoor
                .StateChanges()
                .Subscribe(s =>
                {
                    if (s.New?.State == LockState.Unlocked)
                    {
                        Logger.Info("Turning on the entry lights, door has been locked");
                        Entities.Light.EntryLights.TurnOn();
                    }
                });

            Notifications.OnAction(UnlockDoorActionName).Subscribe(s =>
            {
                Logger.Info("Front door notification unlock action pressed, unlocking the door...");
                Services.Lock.Unlock(ServiceTarget.FromEntity(Entities.Lock.FrontDoor.EntityId));
            });

            Notifications.OnAction(LockDoorActionName).Subscribe(s =>
            {
                Logger.Info("Front door notification lock action pressed, locking the door...");
                Services.Lock.Lock(ServiceTarget.FromEntity(Entities.Lock.FrontDoor.EntityId));

                Notifications.ClearPushNotification("DoorUnlocked");
            });
        }
    }
}
