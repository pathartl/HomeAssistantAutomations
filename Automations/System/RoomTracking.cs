using CaseExtensions;
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
    public class RoomTracking : HomeAssistantAutomation
    {
        public RoomTracking(IHaContext context, IScheduler scheduler, IMqttEntityManager entityManager, ITextToSpeechService tts) : base(context, scheduler, entityManager, tts)
        {
        }

        public override async void Init()
        {
            /*foreach (var device in Config.BluetoothDeviceTracking.Devices)
            {
                await EntityManager.CreateAsync($"sensor.tracker_{device.EntityId}", device.Name, device.Icon);
            }

            Scheduler.RunEvery(TimeSpan.FromSeconds(Config.BluetoothDeviceTracking.ScanInterval), DateTimeOffset.Now, async () =>
            {
                foreach (var device in Config.BluetoothDeviceTracking.Devices)
                {
                    int strength = -9999;
                    string state = "Unknown";

                    var deviceEntity = new Entity(Context, $"sensor.tracker_{device.EntityId}");

                    try
                    {
                        foreach (var room in Config.BluetoothDeviceTracking.Rooms)
                        {
                            int roomStrength = 0;
                            var roomDevice = new Entity(Context, $"sensor.{device.EntityId}_{room.ToSnakeCase()}_rssi");

                            int.TryParse(roomDevice.State, out roomStrength);

                            if (roomStrength != 0 && roomStrength > strength)
                            {
                                state = room;
                                strength = roomStrength;
                            }
                        }

                        if (deviceEntity.State != state)
                            await EntityManager.SetStateAsync(deviceEntity.EntityId, state);
                    }
                    catch { }
                }
            });*/
        }
    }
}
