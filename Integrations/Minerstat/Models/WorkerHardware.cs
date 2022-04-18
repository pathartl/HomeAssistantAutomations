using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace NetDaemonApps.Integrations.Minerstat.Models
{
    public class WorkerHardware
    {
        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("temp")]
        public int Temperature { get; set; }

        [JsonPropertyName("accepted")]
        public int Accepted { get; set; }

        [JsonPropertyName("memTemp")]
        public int MemoryTemperature { get; set; }

        [JsonPropertyName("fan")]
        public int Fan { get; set; }

        [JsonPropertyName("power")]
        public int Power { get; set; }

        [JsonPropertyName("powerMin")]
        public int PowerMin { get; set; }

        [JsonPropertyName("powerMax")]
        public int PowerMax { get; set; }

        [JsonPropertyName("powerStock")]
        public int PowerStock { get; set; }

        [JsonPropertyName("speed")]
        public double Speed { get; set; }

        [JsonPropertyName("bus")]
        public string Bus { get; set; }

        [JsonPropertyName("type")]
        public string Type { get; set; }

        [JsonPropertyName("bios")]
        public string Bios { get; set; }

        [JsonPropertyName("pstate")]
        public string PState { get; set; }

        [JsonPropertyName("core")]
        public int Core { get; set; }

        [JsonPropertyName("coreMax")]
        public int CoreMax { get; set; }

        [JsonPropertyName("memory")]
        public int Memory { get; set; }

        [JsonPropertyName("memoryMax")]
        public int MemoryMax { get; set; }

        [JsonPropertyName("load")]
        public int Load { get; set; }

        [JsonPropertyName("alertTemp")]
        public string AlertTemperature { get; set; }
    }
}
