using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace NetDaemonApps.Integrations.Minerstat.Models
{
    public class WorkerMiningInfo
    {
        [JsonPropertyName("client")]
        public string Client { get; set; }

        [JsonPropertyName("client_version")]
        public string ClientVersion { get; set; }

        [JsonPropertyName("client_cpu")]
        public string ClientCpu { get; set; }

        [JsonPropertyName("client_cpu_version")]
        public string ClientCpuVersion { get; set; }

        [JsonPropertyName("crypto")]
        public string Crypto { get; set; }

        [JsonPropertyName("crypto_dual")]
        public string CryptoDual { get; set; }

        [JsonPropertyName("crypto_cpu")]
        public string CryptoCpu { get; set; }

        [JsonPropertyName("pool")]
        public string Pool { get; set; }

        [JsonPropertyName("pool_dual")]
        public string PoolDual { get; set; }

        [JsonPropertyName("pool_cpu")]
        public string PoolCpu { get; set; }

        [JsonPropertyName("hashrate")]
        public WorkerHashrate Hashrate { get; set; }

        [JsonPropertyName("shares")]
        public WorkerShares Shares { get; set; }
    }
}
