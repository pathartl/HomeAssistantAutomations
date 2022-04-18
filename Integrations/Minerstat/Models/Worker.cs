using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace NetDaemonApps.Integrations.Minerstat.Models
{
    public class Worker
    {
        [JsonPropertyName("info")]
        public WorkerInfo Info { get; set; }

        [JsonPropertyName("hardware")]
        public IEnumerable<WorkerHardware> Hardware { get; set; }

        [JsonPropertyName("revenue")]
        public WorkerRevenue Revenue { get; set; }

        [JsonPropertyName("mining")]
        public WorkerMiningInfo MiningInfo { get; set; }
    }
}
