using NetDaemon.Extensions.Tts;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace NetDaemonApps
{
    public abstract class HomeAssistantAutomation
    {
        public IHaContext Context { get; set; }
        public IScheduler Scheduler { get; set; }
        public IMqttEntityManager EntityManager { get; set; }
        public ITextToSpeechService TextToSpeech { get; set; }
        public Config Config { get; set; }

        public Entities Entities { get; set; }
        public Services Services { get; set; }
        public Notifications Notifications { get; set; }

        public HomeAssistantAutomation(
            IHaContext context,
            IScheduler scheduler,
            IMqttEntityManager entityManager,
            ITextToSpeechService tts)
        {
            if (File.Exists("Config.yaml"))
            {
                try
                {
                    var contents = File.ReadAllText("Config.yaml");
                    var deserializer = new DeserializerBuilder()
                        .WithNamingConvention(PascalCaseNamingConvention.Instance)
                        .Build();

                    Config = deserializer.Deserialize<Config>(contents);
                }
                catch
                {

                }
            }

            Context = context;
            Scheduler = scheduler;
            EntityManager = entityManager;
            TextToSpeech = tts;
            Entities = new Entities(context);
            Services = new Services(context);
            Notifications = new Notifications(context, Config);

            Init();
        }

        public abstract void Init();
    }
}
