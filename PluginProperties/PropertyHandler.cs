using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SimHub.Plugins;

namespace SimHub.MQTTDriverID.PluginProperties
{
    public class PropertyHandler
    {
        private readonly PluginManager PluginManager;

        public PropertyHandler(PluginManager pluginManager)
        {
            PluginManager = pluginManager;

            AddProp("DriverInCar", false);
            AddProp("MqttConnected", false);
            AddProp("RacecontrolMessage", "");
            AddProp("MqttError", "");
            AddProp("MqttAuthorized", false);
        }

        public void SetDriverInCar(bool flag)
        {
            SetProp("DriverInCar", flag);
        }

        public void SetMqttConnected(bool flag)
        {
            SetProp("MqttConnected", flag);
        }

        public void SetRacecontrolMessage(string message)
        {
            SetProp("RacecontrolMessage", message);
        }

        public void SetMqttError(string message)
        {
            SetProp("MqttError", message);
        }

        public void SetMqttAuthorized(bool auth)
        {
            SetProp("MqttAuthorized", auth);
        }

        public void AddProp(string PropertyName, dynamic defaultValue) => PluginManager.AddProperty(PropertyName, GetType(), defaultValue);
        public void SetProp(string PropertyName, dynamic value) => PluginManager.SetPropertyValue(PropertyName, GetType(), value);
        public dynamic GetProp(string PropertyName) => PluginManager.GetPropertyValue(PropertyName);
        public bool HasProp(string PropertyName) => PluginManager.GetAllPropertiesNames().Contains(PropertyName);
    }
}
