using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EventLogSearching.Model
{
    public enum EventLogField
    {
        DATETIME_FIELD = 0,
        STATIONNAME_FIELD = 1,
        EVENT_FIELD = 2,
        MESSAGE_FIELD = 3,
        VALUE_FIELD = 4,
        SOURCE_FIELD = 5,
        
    };

    public class EventLog
    {
        /*
        private DateTime m_DateTime;
        public DateTime Time
        {
            get { return m_DateTime; }
        }
        */
        private string m_DateTime;
        public string Time
        {
            get { return m_DateTime; }
        }

        private string m_strStationName;
        public string StationName
        {
            get { return m_strStationName; }
        }

        private string m_strEvent;
        public string Event
        {
            get { return m_strEvent; }
        }

        private string m_strMessage;
        public string Message
        {
            get { return m_strMessage; }
        }

        private float m_fValue;
        public float Value
        {
            get { return m_fValue; }
        }
        private string m_strSource;
        public string Source
        {
            get { return m_strSource; }
        }

        public EventLog(string[] parts)
        {
            //this.m_DateTime = DateTime.ParseExact(parts[(int)EventLogField.DATETIME_FIELD].ToString(), "dd/MM/yyyy  HH:mm:ss.000", System.Globalization.CultureInfo.InvariantCulture);
            this.m_DateTime = parts[(int)EventLogField.DATETIME_FIELD].ToString();
            this.m_strStationName = parts[(int)EventLogField.STATIONNAME_FIELD].ToString();
            this.m_strEvent = parts[(int)EventLogField.EVENT_FIELD].ToString();
            this.m_strMessage = parts[(int)EventLogField.MESSAGE_FIELD].ToString();
            this.m_fValue = float.Parse(parts[(int)EventLogField.VALUE_FIELD].ToString() == " " ? parts[(int)EventLogField.VALUE_FIELD].ToString() : "0");
            this.m_strSource = parts[(int)EventLogField.SOURCE_FIELD].ToString();
            
        }
        public EventLog(List<string> parts)
        {
            //this.m_DateTime = DateTime.ParseExact(parts[(int)EventLogField.DATETIME_FIELD].ToString(), "dd/MM/yyyy  HH:mm:ss.000", System.Globalization.CultureInfo.InvariantCulture);
            this.m_DateTime = parts[(int)EventLogField.DATETIME_FIELD].ToString();
            this.m_strStationName = parts[(int)EventLogField.STATIONNAME_FIELD].ToString();
            this.m_strEvent = parts[(int)EventLogField.EVENT_FIELD].ToString();
            this.m_strMessage = parts[(int)EventLogField.MESSAGE_FIELD].ToString();
            this.m_fValue = float.Parse( parts[(int)EventLogField.VALUE_FIELD].ToString() == " " ? parts[(int)EventLogField.VALUE_FIELD].ToString() : "0");
            this.m_strSource = parts[(int)EventLogField.SOURCE_FIELD].ToString();

        }
        
    }


}
