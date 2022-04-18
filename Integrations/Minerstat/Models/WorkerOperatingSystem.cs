using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace NetDaemonApps.Integrations.Minerstat.Models
{
    public class WorkerOperatingSystem
    {
        [JsonPropertyName("status")]
        public string Status { get; set; }

        [JsonPropertyName("sync")]
        public int Sync { get; set; }

        [JsonPropertyName("uptime")]
        public string Uptime { get; set; }

        [JsonPropertyName("cpu_temp")]
        public int CpuTemperature { get; set; }

        [JsonPropertyName("cpu_load")]
        public int CpuLoad { get; set; }

        [JsonPropertyName("freespace")]
        public double Freespace { get; set; }

        [JsonPropertyName("freemem")]
        public int FreeMemory { get; set; }

        [JsonPropertyName("localip")]
        public string LocalIp { get; set; }
    }
}
