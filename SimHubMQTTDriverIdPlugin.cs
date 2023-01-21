using GameReaderCommon;
using SimHub.MQTTPublisher.Settings;
using SimHub.Plugins;
using System.Windows.Media;
using SimHub.MQTTDriverID.comm;
using SimHub.MQTTDriverID.PluginProperties;
using SimHub.MQTTDriverID.Payload;

namespace SimHub.MQTTPublisher
{
    [PluginDescription("MQTT DriverID")]
    [PluginAuthor("robbyb67")]
    [PluginName("MQTT DriverID")]
    public class SimHubMQTTDriverIdPlugin : IPlugin, IDataPlugin, IWPFSettingsV2
    {
        public IRacingReader.DataSampleEx IRData { get; set; }

        public SimHubMQTTDriverIDPluginSettings Settings;

        public SimHubMQTTPublisherPluginUserSettings UserSettings { get; private set; }

        private int eventCounter = 0;
        private IracingSimState irSimState = new IracingSimState();
        private PropertyHandler PropertyHandler;
        public MqttRelay MqttRelay { get; private set; }

        /// <summary>
        /// Instance of the current plugin manager
        /// </summary>
        public PluginManager PluginManager { get; set; }

        /// <summary>
        /// Gets the left menu icon. Icon must be 24x24 and compatible with black and white display.
        /// </summary>
        public ImageSource PictureIcon => this.ToIcon(MQTTDriverID.Properties.Resources.mqttidicon);

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
            if (!MqttRelay.Authorized)
            {
                return;
            }

            if (data.GameRunning && data.GameName == "IRacing")
            {
                IRData = data.NewData.GetRawDataObject() as IRacingReader.DataSampleEx;

                MqttRelay.ReconnectIfNeeded();

                if (data.OldData.CompletedLaps != data.NewData.CompletedLaps && irSimState.IsInCar(UserSettings.IracingId))
                {
                    MqttRelay.sendMessage(new MQTTMessage(data, UserSettings, IRData, new SessionData(IRData)));
                }

                if (eventCounter < Settings.UpdateThreshold)
                {
                    eventCounter++;
                    return;
                }
                eventCounter = 0;
                if (irSimState.updateSimState(IRData) && irSimState.IsInCar(UserSettings.IracingId))
                {
                    Logging.Current.Info("MQTT DriverID session change");
                    MqttRelay.sendMessage(new MQTTMessage(data, UserSettings, IRData, new SessionData(IRData)));

                    if (irSimState.DriverChange)
                    {
                        Logging.Current.Info("MQTT DriverID driver change from " + irSimState.PreviousDriverId + " to " + irSimState.DriverId);
                    }
                }
                if (irSimState.IsInCar(UserSettings.IracingId))
                {
                    MqttRelay.sendMessage(new MQTTMessage(data, UserSettings, IRData, new Telemetry(IRData)));
                }
                PropertyHandler.SetDriverInCar(irSimState.IsInCar(UserSettings.IracingId));
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

            MqttRelay.Disconnect();
        }

        /// <summary>
        /// Returns the settings control, return null if no settings control is required
        /// </summary>
        /// <param name="pluginManager"></param>
        /// <returns></returns>
        public System.Windows.Controls.Control GetWPFSettingsControl(PluginManager pluginManager)
        {
            return new SimHubMQTTDriverIDPluginUI(this);
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

            PropertyHandler = new PropertyHandler(pluginManager);
            MqttRelay = new MqttRelay(Settings, UserSettings, PropertyHandler);

            MqttRelay.Connect(Settings.Login, Settings.Password);
        }
    }
}