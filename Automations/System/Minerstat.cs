using NetDaemonApps.Integrations.Minerstat;
using NetDaemonApps.Integrations.Minerstat.Models;
using NetDaemonApps.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetDaemonApps.Automations.System
{
    [NetDaemonApp]
    public class Minerstat : HomeAssistantAutomation
    {
        public Minerstat(IHaContext context, IScheduler scheduler, IMqttEntityManager entityManager, ITextToSpeechService tts) : base(context, scheduler, entityManager, tts)
        {
        }

        MinerstatClient MinerstatClient;

        public override async void Init()
        {
            MinerstatClient = new MinerstatClient(Config.Minerstat.AccessKey);
            IEnumerable<Worker> workers;

            try
            {
                await EntityManager.CreateAsync("sensor.minerstat_revenue_daily", "Minerstat Daily Revenue", "mdi:calendar-today", "monetary");
                await EntityManager.CreateAsync("sensor.minerstat_revenue_weekly", "Minerstat Weekly Revenue", "mdi:calendar-week", "monetary");
                await EntityManager.CreateAsync("sensor.minerstat_revenue_monthly", "Minerstat Monthly Revenue", "mdi:calendar-month", "monetary");

                workers = await GetWorkersAsync();

                foreach (var worker in workers)
                {
                    await EntityManager.CreateAsync(GetWorkerEntityId(worker), new EntityCreationOptions()
                    {
                        Name = $"Minerstat Worker {worker.Info.Name}",
                        DeviceClass = "running",
                        PayloadOn = "on",
                        PayloadOff = "off",
                    });

                    foreach (var hardware in worker.Hardware)
                    {
                        await EntityManager.CreateAsync(GetWorkerHardwareEntityId(worker, hardware), new EntityCreationOptions()
                        {
                            Name = $"Minerstat Worker {worker.Info.Name} {hardware.Name}",
                            DeviceClass = "running",
                            PayloadOn = "on",
                            PayloadOff = "off",
                        });
                    }
                }
            }
            catch (Exception ex) { }

            Scheduler.SchedulePeriodic(TimeSpan.FromSeconds(30), async () =>
            {
                workers = await GetWorkersAsync();

                try
                {
                    UpdateRevenueEntities(workers);

                    foreach (var worker in workers)
                    {
                        UpdateWorkerEntity(worker);

                        foreach (var hardware in worker.Hardware)
                        {
                            UpdateWorkerHardwareEntity(worker, hardware);
                        }
                    }
                }
                catch { }
            });
        }

        private async Task<IEnumerable<Worker>> GetWorkersAsync()
        {
            List<Worker> workers = new List<Worker>();

            foreach (var worker in Config.Minerstat.Workers)
            {
                try
                {
                    workers.Add(await MinerstatClient.GetWorker(worker));
                }
                catch (Exception e)
                {

                }
            }

            return workers;
        }

        private async void UpdateWorkerEntity(Worker worker)
        {
            await EntityManager.SetStateAsync(GetWorkerEntityId(worker), worker.Info.Uptime == "0s" ? "off" : "on");
            await EntityManager.SetAttributesAsync(GetWorkerEntityId(worker), new MinerstatWorkerEntityAttributes(worker));
        }

        private async void UpdateWorkerHardwareEntity(Worker worker, WorkerHardware hardware)
        {
            await EntityManager.SetStateAsync(GetWorkerHardwareEntityId(worker, hardware), worker.Info.Uptime == "0s" ? "off" : "on");
            await EntityManager.SetAttributesAsync(GetWorkerHardwareEntityId(worker, hardware), new MinerstatWorkerHardwareEntityAttributes(worker, hardware));
        }

        private async void UpdateRevenueEntities(IEnumerable<Worker> workers)
        {
            decimal daily = 0m;
            decimal weekly = 0m;
            decimal monthly = 0m;

            foreach (Worker worker in workers)
            {
                daily += worker.Revenue.UsdDay;
                weekly += worker.Revenue.UsdWeek;
                monthly += worker.Revenue.UsdMonth;
            }

            await EntityManager.SetStateAsync(Entities.Sensor.MinerstatRevenueDaily.EntityId, daily.ToString());
            await EntityManager.SetStateAsync(Entities.Sensor.MinerstatRevenueWeekly.EntityId, weekly.ToString());
            await EntityManager.SetStateAsync(Entities.Sensor.MinerstatRevenueMonthly.EntityId, monthly.ToString());
        }

        private string GetWorkerEntityId(Worker worker)
        {
            return $"binary_sensor.minerstat_worker_{worker.Info.Name.ToLower()}";
        }

        private string GetWorkerHardwareEntityId(Worker worker, WorkerHardware hardware)
        {
            var i = worker.Hardware.ToList().IndexOf(hardware);

            return $"{GetWorkerEntityId(worker)}_hardware_{i}";
        }
    }
}
