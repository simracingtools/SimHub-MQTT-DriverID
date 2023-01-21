using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimHub.MQTTDriverID.Payload
{
    public class Authorize : PayloadRoot
    {
        public string mqttClientID { get; set; }

        public Authorize(string clientId)
        {
            mqttClientID = clientId;
        }
    }
}
