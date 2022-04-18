using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetDaemonApps.Extensions
{
    public static class EntityManagerExtensions
    {
        public static async Task CreateAsync(this IMqttEntityManager entityManager, string entityId, string name, string icon, string deviceClass)
        {
            await entityManager.CreateAsync(entityId, new EntityCreationOptions()
            {
                DeviceClass = deviceClass,
                Name = name,
            }, new {
                Icon = icon,
            });
        }
    }
}
