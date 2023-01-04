using SimHub.MQTTPublisher.ViewModels;
using System.Windows.Controls;

namespace SimHub.MQTTPublisher.Settings
{
    /// <summary>
    /// Logique d'interaction pour SimHubMQTTPublisherPluginUI.xaml
    /// </summary>
    public partial class SimHubMQTTPublisherPluginUI : UserControl
    {
        public SimHubMQTTPublisherPluginUI(SimHubMQTTDriverIdPlugin simHubMQTTPublisherPlugin)
        {
            InitializeComponent();
            SimHubMQTTPublisherPlugin = simHubMQTTPublisherPlugin;

            this.Model = new SimHubMQTTDriverIDPluginUIModel()
            {
                Server = simHubMQTTPublisherPlugin.Settings.Server,
                Topic = simHubMQTTPublisherPlugin.Settings.Topic,
                Login = simHubMQTTPublisherPlugin.Settings.Login,
                Password = simHubMQTTPublisherPlugin.Settings.Password,
                UserId = simHubMQTTPublisherPlugin.UserSettings.UserId,
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

            SimHubMQTTPublisherPlugin.CreateMQTTClient();
        }
    }
}