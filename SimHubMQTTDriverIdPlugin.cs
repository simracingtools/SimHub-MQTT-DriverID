using GameReaderCommon;
using MQTTnet;
using MQTTnet.Client;
using Newtonsoft.Json;
using SimHub.MQTTPublisher.Settings;
using SimHub.Plugins;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Media;

namespace SimHub.MQTTPublisher
{
    [PluginDescription("MQTT DriverID")]
    [PluginAuthor("robbyb67")]
    [PluginName("MQTT DriverID")]
    public class SimHubMQTTDriverIdPlugin : IPlugin, IDataPlugin, IWPFSettingsV2
    {
        public SimHubMQTTDriverIDPluginSettings Settings;

        public SimHubMQTTPublisherPluginUserSettings UserSettings { get; private set; }

        private MqttFactory mqttFactory;
        private IMqttClient mqttClient;

        /// <summary>
        /// Instance of the current plugin manager
        /// </summary>
        public PluginManager PluginManager { get; set; }

        /// <summary>
        /// Gets the left menu icon. Icon must be 24x24 and compatible with black and white display.
        /// </summary>
        public ImageSource PictureIcon => this.ToIcon(MQTTDriverID.Properties.Resources.sdkmenuicon);

        /// <summary>
        /// Gets a short plugin title to show in left menu. Return null if you want to use the title as defined in PluginName attribute.
        /// </summary>
        public string LeftMenuTitle => "MQTT DriverID";

        /// <summary>
        /// Called one time per game data update, contains all normalized game data,
        /// raw data are intentionnally "hidden" under a generic object type (A plugin SHOULD NOT USE IT)
        ///
        /// This method is on the critical path, it must execute as fast as possible and avoid throwing any error
        ///
        /// </summary>
        /// <param name="pluginManager"></param>
        /// <param name="data">Current game data, including current and previous data frame.</param>
        public void DataUpdate(PluginManager pluginManager, ref GameData data)
        {
            if (data.GameRunning)
            {
                if (!mqttClient.IsConnected)
                {
                    Logging.Current.Info("MQTT reconnect to " + Settings.Server);
                    mqttClient.ReconnectAsync();
                }
                var applicationMessage = new MqttApplicationMessageBuilder()
               .WithTopic(Settings.Topic)
               .WithPayload(JsonConvert.SerializeObject(new Payload.PayloadRoot(data, UserSettings)))
               .Build();

                Task.Run(async () => await mqttClient.PublishAsync(applicationMessage, CancellationToken.None)).Wait();
            }
        }

        /// <summary>
        /// Called at plugin manager stop, close/dispose anything needed here !
        /// Plugins are rebuilt at game change
        /// </summary>
        /// <param name="pluginManager"></param>
        public void End(PluginManager pluginManager)
        {
            // Save settings
            this.SaveCommonSettings("GeneralSettings", Settings);
            this.SaveCommonSettings("UserSettings", UserSettings);
            mqttClient.Dispose();
        }

        /// <summary>
        /// Returns the settings control, return null if no settings control is required
        /// </summary>
        /// <param name="pluginManager"></param>
        /// <returns></returns>
        public System.Windows.Controls.Control GetWPFSettingsControl(PluginManager pluginManager)
        {
            return new SimHubMQTTPublisherPluginUI(this);
        }

        /// <summary>
        /// Called once after plugins startup
        /// Plugins are rebuilt at game change
        /// </summary>
        /// <param name="pluginManager"></param>
        public void Init(PluginManager pluginManager)
        {
            SimHub.Logging.Current.Info("Starting MQTT DriverID plugin");

            // Load settings
            Settings = this.ReadCommonSettings<SimHubMQTTDriverIDPluginSettings>("GeneralSettings", () => new SimHubMQTTDriverIDPluginSettings());

            UserSettings = this.ReadCommonSettings<SimHubMQTTPublisherPluginUserSettings>("UserSettings", () => new SimHubMQTTPublisherPluginUserSettings());

            this.mqttFactory = new MqttFactory();

            CreateMQTTClient();
        }

        internal void CreateMQTTClient()
        {
            var newmqttClient = mqttFactory.CreateMqttClient();

            var mqttClientOptions = new MqttClientOptionsBuilder()
                .WithClientId(UserSettings.UserId.ToString())
                .WithTcpServer(Settings.Server)
                .WithCredentials(Settings.Login, Settings.Password)
                .Build();

            try
            {
                newmqttClient.ConnectAsync(mqttClientOptions, CancellationToken.None).Wait();
            }
            catch(System.Exception e)
            {
                Logging.Current.Error("MQTT connection error: " + e.Message);
            }

            var oldMqttClient = this.mqttClient;

            mqttClient = newmqttClient;

            if (oldMqttClient != null)
            {
                oldMqttClient.Dispose();
            }

            if (mqttClient.IsConnected)
            {
                Logging.Current.Info("DriverID connected to " + Settings.Server);
            } 
            else
            {
                Logging.Current.Warn("DriverID not connected to " + Settings.Server);
            }
        }
    }
}