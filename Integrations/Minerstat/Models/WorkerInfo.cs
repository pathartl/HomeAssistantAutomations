using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace NetDaemonApps.Integrations.Minerstat.Models
{
    public class WorkerInfo
    {
        [JsonPropertyName("type")]
        public string Type { get; set; }

        [JsonPropertyName("system")]
        public string System { get; set; }

        [JsonPropertyName("status")]
        public string Status { get; set; }

        [JsonPropertyName("inactive")]
        public int Inactive { get; set; }

        [JsonPropertyName("status_reason")]
        public string StatusReason { get; set; }

        [JsonPropertyName("status_cpu")]
        public string StatusCpu { get; set; }

        [JsonPropertyName("uptime")]
        public string Uptime { get; set; }

        [JsonPropertyName("sync")]
        public int Sync { get; set; }

        [JsonPropertyName("time")]
        public string Time { get; set; }

        [JsonPropertyName("note")]
        public int Note { get; set; }

        [JsonPropertyName("profit_switch")]
        public int ProfitSwitch { get; set; }

        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("version")]
        public string Version { get; set; }

        [JsonPropertyName("groups")]
        public string Groups { get; set; }

        [JsonPropertyName("cmd")]
        public string Cmd { get; set; }

        [JsonPropertyName("electricity")]
        public decimal Electricity { get; set; }

        [JsonPropertyName("hot")]
        public int Hot { get; set; }

        [JsonPropertyName("veryHot")]
        public int VeryHot { get; set; }

        [JsonPropertyName("devices")]
        public int Devices { get; set; }

        [JsonPropertyName("consumption")]
        public int Consumption { get; set; }

        [JsonPropertyName("pauseAlerts")]
        public string PauseAlerts { get; set; }

        [JsonPropertyName("pauseTriggers")]
        public string PauseTriggers { get; set; }

        [JsonPropertyName("os")]
        public WorkerOperatingSystem OperatingSystem { get; set; }
    }
}
