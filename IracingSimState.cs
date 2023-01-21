using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimHub.MQTTPublisher
{
    public class IracingSimState
    {
        public bool updateSimState(IRacingReader.DataSampleEx irData)
        {
            bool changed = false;
            if (SessionId != irData.SessionData.WeekendInfo.SessionID)
            {
                SessionId = irData.SessionData.WeekendInfo.SessionID;
                changed = true;
            }

            if (SubsessionId != irData.SessionData.WeekendInfo.SubSessionID)
            {
                SubsessionId = irData.SessionData.WeekendInfo.SubSessionID;
                changed = true;
            }

            irData.Telemetry.TryGetValue("SessionNum", out object rawSessionNum);
            int newSessionNum = Convert.ToInt32(rawSessionNum);
            if (SessionNumber != newSessionNum)
            {
                SessionNumber = newSessionNum;
                changed = true;
            }

            if (DriverCount != irData.SessionData.DriverInfo.Drivers.Length)
            {
                DriverCount = irData.SessionData.DriverInfo.Drivers.Length;
                changed = true;
            }

            DriverIndex = irData.SessionData.DriverInfo.DriverCarIdx;
            if (TeamId != irData.SessionData.DriverInfo.Drivers[DriverIndex].TeamID)
            {
                TeamId = irData.SessionData.DriverInfo.Drivers[DriverIndex].TeamID;
                changed = true;
            }
            if (DriverId != irData.SessionData.DriverInfo.Drivers[DriverIndex].UserID)
            {
                PreviousDriverId = DriverId;
                DriverId = irData.SessionData.DriverInfo.Drivers[DriverIndex].UserID;
                changed = true;
                DriverChange = true;
            }
            else
            {
                DriverChange = false;
            }
            return changed;
        }

        public bool IsInCar(long iRacingId)
        {
            return DriverId == iRacingId;
        }
        public long SessionId { get; set; }
        public long SubsessionId { get; set; }
        public long SessionNumber { get; set; }
        public long DriverId { get; set; }
        public long PreviousDriverId { get; set; }
        public long TeamId { get; set; }
        public bool DriverChange { get; set; }
        public long DriverIndex { get; set; }
        public long DriverCount { get; set; }
    }
}
