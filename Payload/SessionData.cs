using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IRacingReader;

namespace SimHub.MQTTDriverID.Payload
{
    public class SessionData : PayloadRoot
    {
        public iRacingSDK.SessionData sessionData { get; set; }

        public SessionData(DataSampleEx irData)
        {
            sessionData = new iRacingSDK.SessionData();
            sessionData.WeekendInfo = irData.SessionData.WeekendInfo;
            sessionData.SessionInfo = irData.SessionData.SessionInfo;
            sessionData.DriverInfo = irData.SessionData.DriverInfo;
            sessionData.SplitTimeInfo = irData.SessionData.SplitTimeInfo;
            sessionData.CarSetup = irData.SessionData.CarSetup;
        }
    }
}
