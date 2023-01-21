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
        public iRacingSDK._CarSetup carSetup { get; set; }
        public iRacingSDK.SessionData._WeekendInfo weekendInfo { get; set; }
        public iRacingSDK.SessionData._DriverInfo driverInfo { get; set; }
        public iRacingSDK.SessionData._SplitTimeInfo splitTimeInfo { get; set; }
        public iRacingSDK.SessionData._SessionInfo sessionInfo { get; set; }

        public SessionData(DataSampleEx irData)
        {
            weekendInfo = irData.SessionData.WeekendInfo;
            sessionInfo = irData.SessionData.SessionInfo;
            driverInfo = irData.SessionData.DriverInfo;
            splitTimeInfo = irData.SessionData.SplitTimeInfo;
            carSetup = irData.SessionData.CarSetup;
        }
    }
}
