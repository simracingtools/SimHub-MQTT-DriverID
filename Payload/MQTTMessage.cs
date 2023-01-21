using System;
using GameReaderCommon;
using SimHub.MQTTPublisher;

namespace SimHub.MQTTDriverID.Payload
{
    public class MQTTMessage
    {
        public MQTTMessage(GameData data, SimHubMQTTPublisherPluginUserSettings userSettings, IRacingReader.DataSampleEx irData, PayloadRoot messageData)
        {
            type = messageData.GetType().Name;
            payload = messageData;
            version = "2.00";
            sessionId = irData.SessionData.WeekendInfo.SessionID.ToString();
            subsessionId = irData.SessionData.WeekendInfo.SubSessionID;
            irData.Telemetry.TryGetValue("SessionNum", out object rawSessionNum);
            sessionNumber = Convert.ToInt32(rawSessionNum);
            clientId = irData.SessionData.DriverInfo.Drivers[irData.SessionData.DriverInfo.DriverCarIdx].UserID.ToString();
            teamId = irData.SessionData.DriverInfo.Drivers[irData.SessionData.DriverInfo.DriverCarIdx].TeamID.ToString();
            legacySessionName = buildLegacySessionName(irData);
            accessToken = userSettings.UserId.ToString();
        }
        public MQTTMessage(SimHubMQTTPublisherPluginUserSettings userSettings, PayloadRoot messageData)
        {
            type = messageData.GetType().Name;
            payload = messageData;
            version = "2.00";
            sessionId = "0";
            subsessionId = 0;
            sessionNumber = 0;
            clientId = userSettings.IracingId.ToString();
            accessToken = userSettings.UserId.ToString();
            teamId = "0";
            legacySessionName = "";
        }

        public string type { get; set; }
        public string version { get; set; }
        public string sessionId { get; set; }
        public long subsessionId { get; set; }
        public long sessionNumber { get; set; }
        public string legacySessionName { get; set; }
        public string clientId { get; set; }
        public string teamId { get; set; }
        public string accessToken { get; set; }
        public PayloadRoot payload { get; set; }

        private string buildLegacySessionName(IRacingReader.DataSampleEx irData)
        {
            var driverIdx = irData.SessionData.DriverInfo.DriverCarIdx;
            var teamName = irData.SessionData.DriverInfo.Drivers[driverIdx].TeamName;

            if (sessionId.Equals("0") || teamId.Equals("0"))
            {
                return teamName + "@" + irData.SessionData.DriverInfo.Drivers[driverIdx].CarPath
                    + "#" + irData.SessionData.WeekendInfo.TrackName
                    + "#" + sessionNumber;
            }
            return teamName + "@" + sessionId + "#" + subsessionId + "#" + sessionNumber;
        }
    }
}
