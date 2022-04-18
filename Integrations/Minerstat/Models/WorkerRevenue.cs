using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace NetDaemonApps.Integrations.Minerstat.Models
{
    public class WorkerRevenue
    {
        [JsonPropertyName("usd_day")]
        public decimal UsdDay { get; set; }

        [JsonPropertyName("usd_day_dual")]
        public int UsdDayDual { get; set; }

        [JsonPropertyName("usd_day_cpu")]
        public int UsdDayCpu { get; set; }

        [JsonPropertyName("usd_week")]
        public decimal UsdWeek { get; set; }

        [JsonPropertyName("usd_month")]
        public decimal UsdMonth { get; set; }

        [JsonPropertyName("usd_month_dual")]
        public int UsdMonthDual { get; set; }

        [JsonPropertyName("usd_month_cpu")]
        public int UsdMonthCpu { get; set; }

        [JsonPropertyName("btc_day")]
        public decimal BtcDay { get; set; }

        [JsonPropertyName("btc_week")]
        public decimal BtcWeek { get; set; }

        [JsonPropertyName("btc_month")]
        public decimal BtcMonth { get; set; }

        [JsonPropertyName("coin")]
        public decimal Coin { get; set; }

        [JsonPropertyName("coin_dual")]
        public int CoinDual { get; set; }

        [JsonPropertyName("coin_cpu")]
        public int CoinCpu { get; set; }

        [JsonPropertyName("cprice")]
        public decimal Cprice { get; set; }

        [JsonPropertyName("cprice_dual")]
        public int CpriceDual { get; set; }

        [JsonPropertyName("cprice_cpu")]
        public int CpriceCpu { get; set; }
    }
}
