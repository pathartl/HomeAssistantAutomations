using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetDaemonApps.Extensions
{
    public static class EntityManagerExtensions
    {
        private static NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();

        public static async Task CreateAsync(this IMqttEntityManager entityManager, string entityId, string name, string icon)
        {
            Logger.Info("Creating entity {EntityName} ({EntityId})", entityId, name);

            await entityManager.CreateAsync(entityId, new EntityCreationOptions()
            {
                Name = name,
            }, new {
                Icon = icon,
            });
        }

        public static async Task CreateAsync(this IMqttEntityManager entityManager, string entityId, string name, string icon, string deviceClass)
        {
            Logger.Info("Creating entity {EntityName} ({EntityId})", entityId, name);

            await entityManager.CreateAsync(entityId, new EntityCreationOptions()
            {
                DeviceClass = deviceClass,
                Name = name,
            }, new
            {
                Icon = icon,
            });
        }
    }
}
