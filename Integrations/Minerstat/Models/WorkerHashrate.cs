using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace NetDaemonApps.Integrations.Minerstat.Models
{
    public class WorkerHashrate
    {
        [JsonPropertyName("hashrate")]
        public decimal Hashrate { get; set; }

        [JsonPropertyName("hashrate_unit")]
        public string HashrateUnit { get; set; }

        [JsonPropertyName("hashrate_dual")]
        public int HashrateDual { get; set; }

        [JsonPropertyName("hashrate_unit_dual")]
        public string HashrateUnitDual { get; set; }

        [JsonPropertyName("hashrate_cpu")]
        public int HashrateCpu { get; set; }

        [JsonPropertyName("hashrate_unit_cpu")]
        public string HashrateUnitCpu { get; set; }
    }
}
