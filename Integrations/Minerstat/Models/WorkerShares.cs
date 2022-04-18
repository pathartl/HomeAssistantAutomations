using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace NetDaemonApps.Integrations.Minerstat.Models
{
    public class WorkerShares
    {
        [JsonPropertyName("accepted_share")]
        public int AcceptedShare { get; set; }

        [JsonPropertyName("accepted_share_dual")]
        public int AcceptedShareDual { get; set; }

        [JsonPropertyName("rejected_share")]
        public int RejectedShare { get; set; }

        [JsonPropertyName("rejected_share_dual")]
        public int RejectedShareDual { get; set; }

        [JsonPropertyName("accepted_share_cpu")]
        public int AcceptedShareCpu { get; set; }

        [JsonPropertyName("rejected_share_cpu")]
        public int RejectedShareCpu { get; set; }
    }
}
