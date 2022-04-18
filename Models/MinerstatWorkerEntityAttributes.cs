using NetDaemonApps.Integrations.Minerstat.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace NetDaemonApps.Models
{
    public class MinerstatWorkerEntityAttributes
    {
        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("hashrate")]
        public decimal Hashrate { get; set; }

        [JsonPropertyName("hashrate_unit")]
        public string HashrateUnit { get; set; }

        [JsonPropertyName("client")]
        public string Client { get; set; }

        [JsonPropertyName("crypto")]
        public string Crypto { get; set; }

        [JsonPropertyName("pool")]
        public string Pool { get; set; }

        [JsonPropertyName("revenue_usd_day")]
        public decimal RevenueUsdDay { get; set; }

        [JsonPropertyName("revenue_usd_week")]
        public decimal RevenueUsdWeek { get; set; }

        [JsonPropertyName("revenue_usd_month")]
        public decimal RevenueUsdMonth { get; set; }

        [JsonPropertyName("revenue_btc_day")]
        public decimal RevenueBtcDay { get; set; }

        [JsonPropertyName("revenue_btc_week")]
        public decimal RevenueBtcWeek { get; set; }

        [JsonPropertyName("revenue_btc_month")]
        public decimal RevenueBtcMonth { get; set; }

        [JsonPropertyName("accepted_shares")]
        public int AcceptedShares { get; set; }

        public MinerstatWorkerEntityAttributes(Worker worker)
        {
            Name = worker.Info.Name;
            Hashrate = worker.MiningInfo.Hashrate.Hashrate;
            HashrateUnit = worker.MiningInfo.Hashrate.HashrateUnit;
            Client = worker.MiningInfo.Client;
            Crypto = worker.MiningInfo.Crypto;
            Pool = worker.MiningInfo.Pool;
            RevenueUsdDay = worker.Revenue.UsdDay;
            RevenueUsdWeek = worker.Revenue.UsdWeek;
            RevenueUsdMonth = worker.Revenue.UsdMonth;
            RevenueBtcDay = worker.Revenue.BtcDay;
            RevenueBtcWeek = worker.Revenue.BtcWeek;
            RevenueBtcMonth = worker.Revenue.BtcMonth;
            AcceptedShares = worker.MiningInfo.Shares.AcceptedShare;
        }
    }
}
