using System;
using System.Threading;
using System.Threading.Tasks;
using MQTTnet;
using MQTTnet.Client;
using SimHub.MQTTDriverID.PluginProperties;
using Newtonsoft.Json;
using SimHub.MQTTDriverID.Payload;
using SimHub.MQTTPublisher;

namespace SimHub.MQTTDriverID.comm
{
    public class MqttRelay
    {
        public bool Authorized { get; private set; }

        private MqttFactory mqttFactory;
        private IMqttClient mqttClient;

        private PropertyHandler propertyHandler;
        private string server;
        private string defaultTopic;
        private SimHubMQTTPublisherPluginUserSettings userSettings;

        public MqttRelay(SimHubMQTTDriverIDPluginSettings settings, SimHubMQTTPublisherPluginUserSettings userSettings, PropertyHandler propertyHandler)
        {
            Logging.Current.Info("MqttRelay("+ server + ", " + userSettings.UserId + ", " + propertyHandler.ToString());

            this.propertyHandler = propertyHandler;
            this.server = settings.Server;
            this.defaultTopic = settings.Topic;
            this.userSettings = userSettings;

            Authorized = false;
            propertyHandler.SetMqttAuthorized(Authorized);

            mqttFactory = new MqttFactory();
        }

        public void sendMessage(MQTTMessage message, string topic = null)
        {
            var applicationMessage = new MqttApplicationMessageBuilder()
                    .WithTopic(topic == null || topic.Equals("") ? defaultTopic : topic)
                    .WithPayload(JsonConvert.SerializeObject(message))
                    .Build();

            Task.Run(async () => await mqttClient.PublishAsync(applicationMessage, CancellationToken.None)).Wait();

        }

        public void Connect(string login, string password)
        {
            var newmqttClient = mqttFactory.CreateMqttClient();

            var mqttClientOptions = new MqttClientOptionsBuilder()
                .WithClientId(userSettings.UserId)
                .WithTcpServer(server)
                .WithCredentials(login, password)
                .Build();

            var mqttSubscribeOptions = mqttFactory.CreateSubscribeOptionsBuilder()
                .WithTopicFilter(
                    f =>
                    {
                        f.WithTopic("/echoes");
                    })
                .WithTopicFilter(
                    f =>
                    {
                        f.WithTopic("/racecontrol/#");
                    })
                .WithTopicFilter(
                    f =>
                    {
                        f.WithTopic(defaultTopic + "/" + userSettings.UserId);
                    })
                .Build();

            // Setup message handling before connecting so that queued messages
            // are also handled properly. When there is no event handler attached all
            // received messages get lost.
            newmqttClient.ApplicationMessageReceivedAsync += e =>
            {
                string topic = e.ApplicationMessage.Topic != null ? e.ApplicationMessage.Topic : "";
                string payload = System.Text.Encoding.UTF8.GetString(e.ApplicationMessage.Payload);
                propertyHandler.SetRacecontrolMessage(topic + ": " + payload);

                if (topic.Equals(defaultTopic + "/" + userSettings.UserId))
                {
                    if (payload.Equals("Auth" +
                        "orized"))
                    {
                        Authorized = true;
                        propertyHandler.SetMqttAuthorized(Authorized);
                        Logging.Current.Info("DriverID authorized");
                    }
                }
                
                return Task.CompletedTask;
            };

            try
            {
                newmqttClient.ConnectAsync(mqttClientOptions, CancellationToken.None).Wait();
                Logging.Current.Info("Connected");
                newmqttClient.SubscribeAsync(mqttSubscribeOptions, CancellationToken.None).Wait();
                Logging.Current.Info("Subscribed");
            }
            catch (System.Exception e)
            {
                Logging.Current.Error("MQTT connection error: " + e.Message);
                propertyHandler.SetMqttError(e.Message);
            }

            var oldMqttClient = this.mqttClient;

            mqttClient = newmqttClient;

            if (oldMqttClient != null)
            {
                oldMqttClient.Dispose();
            }

            if (mqttClient.IsConnected)
            {
                Logging.Current.Info("DriverID connected to " + server);
                propertyHandler.SetMqttConnected(true);
                propertyHandler.SetMqttError("");
                sendMessage(new MQTTMessage(userSettings, new Authorize(userSettings.UserId)));
            }
            else
            {
                Logging.Current.Warn("DriverID not connected to " + server);
                propertyHandler.SetMqttConnected(false);
            }
        }

        public void Disconnect()
        {
            mqttClient.DisconnectAsync(MqttClientDisconnectReason.NormalDisconnection).Wait();
            mqttClient.Dispose();
        }

        public void ReconnectIfNeeded()
        {
            if (!mqttClient.IsConnected)
            {
                Logging.Current.Info("MQTT reconnect to " + server);
                mqttClient.ReconnectAsync();
            }
        }
    }
}
