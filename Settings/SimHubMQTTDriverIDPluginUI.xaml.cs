using SimHub.MQTTPublisher.ViewModels;
using System.Windows.Controls;
using SimHub.MQTTDriverID.comm;

namespace SimHub.MQTTPublisher.Settings
{
    /// <summary>
    /// Logique d'interaction pour SimHubMQTTDriverIDPluginUI.xaml
    /// </summary>
    public partial class SimHubMQTTDriverIDPluginUI : UserControl
    {
        public SimHubMQTTDriverIDPluginUI(SimHubMQTTDriverIdPlugin simHubMQTTDriverIDPlugin)
        {
            InitializeComponent();
            SimHubMQTTPublisherPlugin = simHubMQTTDriverIDPlugin;

            this.Model = new SimHubMQTTDriverIDPluginUIModel()
            {
                Server = simHubMQTTDriverIDPlugin.Settings.Server,
                Topic = simHubMQTTDriverIDPlugin.Settings.Topic,
                Login = simHubMQTTDriverIDPlugin.Settings.Login,
                Password = simHubMQTTDriverIDPlugin.Settings.Password,
                UpdateTreshold = simHubMQTTDriverIDPlugin.Settings.UpdateThreshold,
                UserId = simHubMQTTDriverIDPlugin.UserSettings.UserId,
                IracingId = simHubMQTTDriverIDPlugin.UserSettings.IracingId,
            };

            this.DataContext = Model;
        }

        private SimHubMQTTDriverIDPluginUIModel Model { get; }

        private SimHubMQTTDriverIdPlugin SimHubMQTTPublisherPlugin { get; }

        private void Apply_Settings(object sender, System.Windows.RoutedEventArgs e)
        {
            SimHubMQTTPublisherPlugin.Settings.Server = Model.Server;
            SimHubMQTTPublisherPlugin.Settings.Topic = Model.Topic;
            SimHubMQTTPublisherPlugin.Settings.Login = Model.Login;
            SimHubMQTTPublisherPlugin.Settings.Password = Model.Password;
            SimHubMQTTPublisherPlugin.Settings.UpdateThreshold = Model.UpdateTreshold;
            SimHubMQTTPublisherPlugin.UserSettings.IracingId = Model.IracingId;
            SimHubMQTTPublisherPlugin.UserSettings.UserId = Model.UserId;
            SimHubMQTTPublisherPlugin.MqttRelay.Connect(Model.Login, Model.Password);
        }
    }
}