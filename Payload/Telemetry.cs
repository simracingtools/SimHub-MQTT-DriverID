using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IRacingReader;

namespace SimHub.MQTTDriverID.Payload
{
    public class Telemetry : PayloadRoot
    {
        public iRacingSDK.Telemetry telemetry { get; set; }

        public Telemetry(DataSampleEx irData)
        {
            telemetry = irData.Telemetry;
        }
    }
}
