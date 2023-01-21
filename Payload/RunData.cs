using GameReaderCommon;
using System.Collections.Generic;
using System.Linq;

namespace SimHub.MQTTDriverID.Payload
{
    public class RunData : PayloadRoot
    {
        public RunData(GameData data, IRacingReader.DataSampleEx irData)
        {
            Speed = data.NewData.SpeedKmh;
            Throttle = data.NewData.Throttle;
            Brake = data.NewData.Brake;
            FuelLevel = data.NewData.Fuel;

            CarCoordinates = data.NewData.CarCoordinates.ToList();
            LapDistPct = data.NewData.TrackPositionPercent;
            //SessionFlags flags = irData.Telemetry.SessionFlags;
            Flags = (long)irData.Telemetry.SessionFlags;

            TimeInLap = data.NewData.CurrentLapTime.TotalMilliseconds;
            SessionTime = irData.Telemetry.SessionTime;
            SessionTimeRemaining = irData.Telemetry.SessionTimeRemain;
            SessionLapsRemaining = irData.Telemetry.SessionLapsRemain;

            Position = data.NewData.Position;
            PositionInClass = irData.Telemetry.PlayerCarClassPosition;
        }

        public double FuelLevel { get; set; }
        public long Flags { get; set; }
        public double SessionTime { get; set; }
        public double SessionTimeRemaining { get; set; }
        public long SessionLapsRemaining { get; set; }
        public long SessionState { get; set; }
        public double SessionToD { get; set; }
        public double EstLaptime { get; set; }
        public long LapNo { get; set; }
        public double TimeInLap  { get; set; }
        public double Brake { get; set; }
        public double Throttle { get; set; }
        public double Speed { get; set; }
        public double LapDistPct { get; set; }
        public double[] FieldLapDistPct { get; set; }
        public List<double> CarCoordinates { get; set; }
        public long Position { get; set; }
        public long PositionInClass { get; set; }
    }
}
