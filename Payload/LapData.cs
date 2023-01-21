using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimHub.MQTTDriverID.Payload
{
    class LapData
    {
        public long lap { get; set; }
        public double fuelLevel { get; set; }
        public double trackTemp { get; set; }
        public double sessionTime { get; set; }
        public double laptime { get; set; }
        public string driver { get; set; }
        public long driverId { get; set; }
    }
}
