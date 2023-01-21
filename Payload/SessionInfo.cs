
using SimHub.MQTTPublisher;

namespace SimHub.MQTTDriverID.Payload
{
    public class SessionInfo : PayloadRoot
    {
        public SessionInfo(IracingSimState simState, IRacingReader.DataSampleEx irData)
        {
            Track = irData.SessionData.WeekendInfo.TrackName;
            TrackId = irData.SessionData.WeekendInfo.TrackID;
            Car = irData.SessionData.DriverInfo.Drivers[simState.DriverIndex].CarScreenName;
            CarId = irData.SessionData.DriverInfo.Drivers[simState.DriverIndex].CarID;
            TeamName = irData.SessionData.DriverInfo.Drivers[simState.DriverIndex].TeamName;
            MaxFuel = irData.SessionData.DriverInfo.DriverCarFuelMaxLtr * irData.SessionData.DriverInfo.DriverCarMaxFuelPct;
            SessionLaps = irData.SessionData.SessionInfo.Sessions[simState.SessionNumber].SessionLaps;
            SessionType = irData.SessionData.SessionInfo.Sessions[simState.SessionNumber].SessionType;
            SessionDuration = irData.SessionData.SessionInfo.Sessions[simState.SessionNumber].SessionTime;
        }

        public string Track { get; set; }
        public long TrackId { get; set; }
        public string Car { get; set; }
        public long CarId { get; set; }
        public double MaxFuel { get; set; }
        public string SessionLaps { get; set; }
        public string SessionDuration { get; set; }
        public string SessionType { get; set; }
        public string TeamName { get; set; }
    }
}
