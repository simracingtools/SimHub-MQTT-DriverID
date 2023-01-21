using System;

namespace SimHub.MQTTPublisher
{
    /// <summary>
    /// Settings class, make sure it can be correctly serialized using JSON.net
    /// </summary>
    public class SimHubMQTTDriverIDPluginSettings
    {
        public string Server { get; set; } = "localhost";

        public string Topic { get; set; } = "racing/driver_name";

        public string Login { get; set; } = "admin";

        public string Password { get; set; }

        public int UpdateThreshold { get; set; } = 50;
    }

    public class SimHubMQTTPublisherPluginUserSettings
    {
        public long IracingId { get; set; }
        public string UserId { get; set; }

    }
}