using NetDaemonApps.Integrations.Minerstat.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace NetDaemonApps.Models
{
    public class MinerstatWorkerHardwareEntityAttributes
    {
        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("core_temp")]
        public int CoreTemp { get; set; }

        [JsonPropertyName("memory_temp")]
        public int MemoryTemp { get; set; }

        [JsonPropertyName("power")]
        public int Power { get; set; }

        [JsonPropertyName("power_min")]
        public int PowerMin { get; set; }

        [JsonPropertyName("power_max")]
        public int PowerMax { get; set; }

        [JsonPropertyName("core_clock")]
        public int CoreClock { get; set; }

        [JsonPropertyName("core_clock_max")]
        public int CoreClockMax { get; set; }

        [JsonPropertyName("memory_clock")]
        public int MemoryClock { get; set; }

        [JsonPropertyName("memory_clock_max")]
        public int MemoryClockMax { get; set; }

        [JsonPropertyName("fan_speed")]
        public int FanSpeed { get; set; }

        [JsonPropertyName("estimated_cost_usd_day")]
        public decimal EstimatedCostUsdDay { get; set; }

        [JsonPropertyName("estimated_cost_usd_week")]
        public decimal EstimatedCostUsdWeek { get; set; }

        [JsonPropertyName("estimated_cost_usd_month")]
        public decimal EstimatedCostUsdMonth { get; set; }

        public MinerstatWorkerHardwareEntityAttributes(Worker worker, WorkerHardware hardware)
        {
            Name = hardware.Name;
            CoreTemp = hardware.Temperature;
            MemoryTemp = hardware.MemoryTemperature;
            Power = hardware.Power;
            PowerMin = hardware.PowerMin;
            PowerMax = hardware.PowerMax;
            CoreClock = hardware.Core;
            CoreClockMax = hardware.CoreMax;
            MemoryClock = hardware.Memory;
            MemoryClockMax = hardware.MemoryMax;
            FanSpeed = hardware.Fan;
            EstimatedCostUsdDay = (Power / 1000m) * worker.Info.Electricity * 24;
            EstimatedCostUsdWeek = EstimatedCostUsdDay * 7;
            EstimatedCostUsdMonth = EstimatedCostUsdDay * 30;
        }
    }
}
