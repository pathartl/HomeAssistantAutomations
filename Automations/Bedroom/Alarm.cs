using NetDaemon.HassModel.Entities;
using NetDaemonApps.Extensions;
using NetDaemonApps.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace NetDaemonApps.Automations.Bedroom
{
    [NetDaemonApp]
    public class Alarm : HomeAssistantAutomation
    {
        public Alarm(IHaContext context, IScheduler scheduler, IMqttEntityManager entityManager, ITextToSpeechService tts) : base(context, scheduler, entityManager, tts)
        {
        }

        public override async void Init()
        {
            Scheduler.ScheduleCron("* * * * *", async () =>
            {
                try
                {
                    DateTime pat;
                    DateTime sierra;

                    DateTime.TryParse(Entities.Sensor.PatSPhoneNextAlarm.State, out pat);
                    DateTime.TryParse(Entities.Sensor.SierraSPhoneNextAlarm.State, out sierra);

                    var now = DateTime.Now;

                    if (pat.Hour == now.Hour && pat.Minute - Config.Alarm.MinutesBeforeAlarm == now.Minute)
                    {
                        if (sierra.Date != now.Date || (pat > sierra && pat.Date == now.Date))
                        {
                            StartPlaylist();

                            await Task.Delay(TimeSpan.FromMinutes(Config.Alarm.MinutesBeforeLightsOn));

                            await Entities.Light.MasterBedroomLights.TransitionBrightness(255, 128, TimeSpan.FromMinutes(Config.Alarm.LightsTransitionDuration));
                        }
                    }
                }
                catch (Exception ex)
                { }
            });
        }

        private void StartPlaylist()
        {
            Services.Spotcast.Start(new SpotcastStartParameters()
            {
                DeviceName = Config.Alarm.SpotcastDeviceName,
                Uri = Config.Alarm.Playlist,
                RandomSong = true,
                Shuffle = true,
                StartVolume = Config.Alarm.Volume
            });
        }
    }
}
