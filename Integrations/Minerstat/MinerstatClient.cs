using NetDaemonApps.Integrations.Minerstat.Models;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetDaemonApps.Integrations.Minerstat
{
    public class MinerstatClient
    {
        private RestClient Client;
        private string AccessKey;

        public MinerstatClient(string accessKey)
        {
            AccessKey = accessKey;

            Client = new RestClient($"https://api.minerstat.com/v2/stats/{AccessKey}");
        }

        public async Task<Worker> GetWorker(string worker)
        {
            var request = new RestRequest(worker);

            var response = await Client.GetAsync<Dictionary<string, Worker>>(request);

            return response[worker];
        }

        public async Task<IEnumerable<Worker>> GetWorkers()
        {
            var request = new RestRequest();

            var response = await Client.GetAsync<Dictionary<string, Worker>>(request);

            return response.Values;
        }
    }
}
