using System;

namespace EventLogSearching.Model
{
    public enum PointType
    {
        Digital,
        Analog
    };

    public enum AlarmEvent
    {
        ANALOG_LOW2 = 401051,
        ANALOG_HIGH2 = 401021,
    };

    public enum AnalogTableField
    {
        RECINDEX_FIELD = 0,
        STATIONNAME_FIELD = 3,
        POINTNAME_FIELD = 7,
        SHORTNAME_FIELD = 8,
        ACTUALVALUE_FIELD = 10,
        TELEMETERFAIL_FIELD = 44,
        DATETIME_FIELD = 52
    };

    public enum AlarmListTableField
    {
        RECINDEX_FIELD = 0,
        PKALARMLIST_FIELD = 1,
        DATETIME_FIELD = 2,
        POINTTYPE_FIELD = 3,
        FKINDEX_FIELD = 4,
        STATIONNAME_FIELD = 5,
        POINTNAME_FIELD = 6,
        ALARMTYPE_FIELD = 7,
        FLASHING_FIELD = 8,
        ACTUALVALUE_FIELD = 9,
        MESSAGE_FIELD = 10,
        SOURCENAME_FIELD = 11,
        SOURCEID_FIELD = 12,
        SOURCETYPE_FIELD = 13,
        ALARMFLAG_FIELD = 14
    };
    public class AnalogPoint
        {
            private UInt32 m_nRecIndex;
            public UInt32 RecIndex
            {
                get { return m_nRecIndex; }
            }

            private String m_strStationName;
            public String StationName
            {
                get { return m_strStationName; }
            }

            private String m_strPointName;
            public String PointName
            {
                get { return m_strPointName; }
            }

            private String m_strShortName;
            public String ShortName
            {
                get { return m_strShortName; }
            }

            private float m_fPreFaultValue;
            public float PreFaultValue
            {
                get { return m_fPreFaultValue; }
            }

            private float m_fActualValue;
            public float ActualValue
            {
                get { return m_fActualValue; }
            }

            private Byte m_byTelemeterFail;

            public Byte TelemeterFail
            {
                get { return m_byTelemeterFail; }
                set { m_byTelemeterFail = value; }
            }

            private UInt32 m_nAlarmType;
            public UInt32 AlarmType
            {
                get { return m_nAlarmType; }
            }

            private DateTime m_DateTime;
            public DateTime Time
            {
                get { return m_DateTime; }
                set { m_DateTime = value; }
            }

            private bool m_bUpdate;
            public bool Update
            {
                get { return m_bUpdate; }
                set { m_bUpdate = value; }
            }


            public AnalogPoint(string[] parts)
            {
                this.m_nRecIndex = UInt32.Parse(parts[(int)AnalogTableField.RECINDEX_FIELD].ToString());
                this.m_strStationName = parts[(int)AnalogTableField.STATIONNAME_FIELD].ToString();
                this.m_strPointName = parts[(int)AnalogTableField.POINTNAME_FIELD].ToString();
                this.m_strShortName = parts[(int)AnalogTableField.SHORTNAME_FIELD].ToString();
                this.m_DateTime = DateTime.ParseExact(parts[(int)AnalogTableField.DATETIME_FIELD].ToString(), "dd/MM/yyyy HH:mm:ss.000", System.Globalization.CultureInfo.InvariantCulture);
        }

            public bool UpdateValue(string[] parts)
            {
                this.m_fPreFaultValue = this.m_fActualValue;
                this.m_fActualValue = float.Parse(parts[(int)AnalogTableField.ACTUALVALUE_FIELD].ToString());
                this.m_byTelemeterFail = Byte.Parse(parts[(int)AnalogTableField.TELEMETERFAIL_FIELD].ToString());
                return true;
            }

            //public bool UpdateValue(Alarm al)
            //{
            //    this.m_nAlarmType = al.AlarmType;
            //    this.m_DateTime = al.Time;
            //    this.Update = true;
            //    return true;
            //}
        
    }
}
