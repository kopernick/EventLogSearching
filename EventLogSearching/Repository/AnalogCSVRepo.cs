using EventLogSearching.Model;
using Microsoft.VisualBasic.FileIO;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EventLogSearching.Repository
{
    public class AnalogCSVRepo
    {
            private String m_strCsvPath;
            public String CsvPath
            {
                get { return m_strCsvPath; }
                set { m_strCsvPath = value; }
            }

            private String m_str60CAParse;
            public String Str60CAParse
            {
                get { return m_str60CAParse; }
                set { m_str60CAParse = value; }
            }
        private String m_strDPParse;
            public String DPParse
            {
                get { return m_strDPParse; }
                set { m_strDPParse = value; }
            }

            private String m_strMWParse;
            public String MWParse
            {
                get { return m_strMWParse; }
                set { m_strMWParse = value; }
            }

            private String m_strKVParse;
            public String KVParse
            {
                get { return m_strKVParse; }
                set { m_strKVParse = value; }
            }

            private UInt16 m_nInterval;
            public UInt16 Interval
            {
                get { return m_nInterval; }
                set { m_nInterval = value; }
            }

            private String m_strLineAPI;
            public String LineAPI
            {
                get { return m_strLineAPI; }
                set { m_strLineAPI = value; }
            }

            private bool m_bMWLoss;
            public bool MWLoss
            {
                get { return m_bMWLoss; }
                set { m_bMWLoss = value; }
            }

            private bool m_bVoltageSwing;
            public bool VoltageSwing
            {
                get { return m_bVoltageSwing; }
                set { m_bVoltageSwing = value; }
            }

            private float m_fMWThreshold;
            public float MWThreshold
            {
                get { return m_fMWThreshold; }
                set { m_fMWThreshold = value; }
            }

            private Byte m_byVoltageLimit;
            public Byte VoltageLimit
            {
                get { return m_byVoltageLimit; }
                set { m_byVoltageLimit = value; }
            }

            private List<AnalogPoint> m_listAnalogPoint;
            public List<AnalogPoint> ListAnalogPoint
            {
                get
                {
                    return m_listAnalogPoint;
                }
            }

        private List<AnalogPoint> m_listMWPoint;
            public List<AnalogPoint> ListMWPoint
            {
                get { return m_listMWPoint; }
            }

            private List<AnalogPoint> m_listKVPoint;
            public List<AnalogPoint> ListKVPoint
            {
                get { return m_listKVPoint; }
            }


            private List<string> m_listGenerateEvent;
            public List<string> ListGenerateEvent
            {
                get { return m_listGenerateEvent; }
            }

            private bool m_bMWLossEnable;
            public bool MWLossEnable
            {
                get { return m_bMWLossEnable; }
                set { m_bMWLossEnable = value; }
            }

            private bool m_bVoltageSwingEnable;
            public bool VoltageSwingEnable
            {
                get { return m_bVoltageSwingEnable; }
                set { m_bVoltageSwingEnable = value; }
            }

            private Int32 m_nLastAlarmRecIndex;
            public Int32 LastAlarmRecIndex
            {
                get { return m_nLastAlarmRecIndex; }
            }

            public AnalogCSVRepo()
            {
                this.m_listMWPoint = new List<AnalogPoint>();
                this.m_listAnalogPoint =  new List<AnalogPoint>();
                this.m_listKVPoint = new List<AnalogPoint>();
                this.m_listGenerateEvent = new List<string>();
                this.m_strDPParse = "DP";
                this.m_str60CAParse = "DP";
                this.m_strMWParse = "MW";
                this.m_strKVParse = "KV";
                this.m_nInterval = 1000;
                this.m_fMWThreshold = 0;
                this.m_nLastAlarmRecIndex = -1;

                Microsoft.Win32.Registry.CurrentUser.CreateSubKey("A2LConfiguration");
            }

            public void SaveConfiguration()
            {
                Microsoft.Win32.RegistryKey reg = Microsoft.Win32.Registry.CurrentUser.OpenSubKey("A2LConfiguration", true);
                reg.SetValue("CSVPath", this.m_strCsvPath);
                reg.SetValue("DP", this.m_strDPParse);
                reg.SetValue("KV", this.m_strKVParse);
                reg.SetValue("MW", this.m_strMWParse);
                reg.SetValue("LineAPI", this.m_strLineAPI);
                reg.SetValue("Interval", this.m_nInterval.ToString());
                reg.SetValue("MWThreshold", this.m_fMWThreshold.ToString());
                reg.SetValue("VoltageLimit", this.m_byVoltageLimit.ToString());
                reg.SetValue("MWLossEnable", this.m_bMWLossEnable.ToString());
                reg.SetValue("VoltageSwingEnable", this.m_bVoltageSwingEnable.ToString());
                reg.Close();
            }

            public void LoadConfiguration()
            {
                try
                {
                    Microsoft.Win32.RegistryKey reg = Microsoft.Win32.Registry.CurrentUser.OpenSubKey("A2LConfiguration");
                    this.m_strCsvPath = reg.GetValue("CSVPath").ToString();
                    this.m_strDPParse = reg.GetValue("DP").ToString();
                    this.m_strKVParse = reg.GetValue("KV").ToString();
                    this.m_strMWParse = reg.GetValue("MW").ToString();
                    this.m_strLineAPI = reg.GetValue("LineAPI").ToString();
                    this.m_nInterval = UInt16.Parse(reg.GetValue("Interval").ToString());
                    this.m_fMWThreshold = float.Parse(reg.GetValue("MWThreshold").ToString());
                    this.m_byVoltageLimit = Byte.Parse(reg.GetValue("VoltageLimit").ToString());
                    this.m_bMWLossEnable = bool.Parse(reg.GetValue("MWLossEnable").ToString());
                    this.m_bVoltageSwingEnable = bool.Parse(reg.GetValue("VoltageSwingEnable").ToString());
                    reg.Close();
                }
                catch (Exception err)
                {
                    System.Diagnostics.Debug.WriteLine(err.Message);
                }
            }

            public AnalogPoint GetAnalogByFkIndex(UInt32 nFkIndex, string strStationName, string strPointName)
            {
                for (int iPoint = 0; iPoint < this.m_listMWPoint.Count; iPoint++)
                {
                    AnalogPoint point = this.m_listMWPoint[iPoint];
                    if (point.RecIndex == nFkIndex)
                    {
                        if (point.StationName == strStationName)
                        {
                            if (point.PointName == strPointName) return point;
                        }
                        return null;
                    }
                }

                for (int iPoint = 0; iPoint < this.m_listKVPoint.Count; iPoint++)
                {
                    AnalogPoint point = this.m_listKVPoint[iPoint];
                    if (point.RecIndex == nFkIndex)
                    {
                        if (point.StationName == strStationName)
                        {
                            if (point.PointName == strPointName) return point;
                        }
                        return null;
                    }
                }
                return null;
            }

            public AnalogPoint GetMWPoint(string strStationName, string strPointName)
            {
                for (int iPoint = 0; iPoint < this.m_listMWPoint.Count; iPoint++)
                {
                    AnalogPoint point = this.m_listMWPoint[iPoint];
                    if (point.StationName == strStationName)
                    {
                        if (point.PointName == strPointName) return point;
                    }
                }
                return null;
            }

            public AnalogPoint GetKVPoint(string strStationName, string strPointName)
            {
                for (int iPoint = 0; iPoint < this.m_listKVPoint.Count; iPoint++)
                {
                    AnalogPoint point = this.m_listKVPoint[iPoint];
                    if (point.StationName == strStationName)
                    {
                        if (point.PointName == strPointName) return point;
                    }
                }
                return null;
            }

            public bool ReadAnalogCSV(bool bConfig)
            {
                string strFilePath = this.m_strCsvPath + @"\\10.20.86.210\ExportDB\\Analog.csv";

                if (!System.IO.File.Exists(strFilePath)) return false;

                int iLoop = 0;
                using (TextFieldParser parser = new TextFieldParser(strFilePath))
                {
                    parser.Delimiters = new string[] { "," };
                    while (true)
                    {
                        string[] parts = parser.ReadFields();
                        if (parts == null) break;
                        if (iLoop++ == 0) continue;

                        if (bConfig)
                        {
                            string strShortName = parts[(int)AnalogTableField.SHORTNAME_FIELD].ToString();
                            if (strShortName.IndexOf(this.DPParse) >= 0)
                            {
                                if (strShortName.IndexOf(this.MWParse) >= 0)
                                {
                                    AnalogPoint point = new AnalogPoint(parts);
                                    AnalogPoint pointMW = GetMWPoint(point.StationName, point.PointName);
                                    if (pointMW != null) throw (new Exception());
                                    this.ListMWPoint.Add(point);
                                }
                                else if (strShortName.IndexOf(this.KVParse) >= 0)
                                {
                                    AnalogPoint point = new AnalogPoint(parts);
                                    AnalogPoint pointKV = GetKVPoint(point.StationName, point.PointName);
                                    if (pointKV != null) throw (new Exception());
                                    this.ListKVPoint.Add(point);
                                }
                            }
                        }
                        else
                        {
                            string strStationName = parts[(int)AnalogTableField.STATIONNAME_FIELD].ToString();
                            string strPointName = parts[(int)AnalogTableField.POINTNAME_FIELD].ToString();
                            AnalogPoint pointMW = GetMWPoint(strStationName, strPointName);
                            if (pointMW != null)
                            {
                                pointMW.UpdateValue(parts);
                            }
                            AnalogPoint pointKV = GetKVPoint(strStationName, strPointName);
                            if (pointKV != null)
                            {
                                pointKV.UpdateValue(parts);
                            }
                        }
                    }
                    return false;
                }
            }

        public bool ReadAnalogCSV()
        {

            this.m_listAnalogPoint.Clear();
            try
            {
                string csvFile = @"\\10.20.86.210\ExportDB\Analog.csv";


#if false
                //Oldcode
                DataTable dt = new DataTable("AlarmList");
                using (TextFieldParser parser = new TextFieldParser(csvFile))
                    {
                        
                            parser.Delimiters = new string[] { "," };
                            int iLine = 0;
                            while (true)
                            {
                                string[] parts = parser.ReadFields();
                                if (parts == null) break;
                                if (iLine++ == 0)
                                {
                                    for (int iCol = 0; iCol < parts.Length; iCol++) dt.Columns.Add(parts[iCol]);
                                    continue;
                                }
                                dt.Rows.Add(parts);
                            }

                            //dt.DefaultView.Sort = "DATETIME";
                            dt = dt.DefaultView.ToTable();

                            for (int iRow = 0; iRow < dt.Rows.Count; iRow++)
                            {
                                DataRow row = dt.Rows[iRow];
                                DigitalAlarm al = new DigitalAlarm(row);
                                this.m_listAlarm.Add(al);
                            }

                            this.m_dLastReadCSV = DateTime.Now;
                            return true;
                    }
#else
                m_listAnalogPoint = File.ReadLines(csvFile)
                            .Skip(1)
                            .Select(line => GetLineAnalogCsv(line))
                            .Where(line =>line.ShortName.Contains(this.Str60CAParse))
                            .ToList();
                //this.m_dLastReadCSV = DateTime.Now;
                    return true;


#endif
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
                return false;
            }
        }

        public static AnalogPoint GetLineAnalogCsv(string csvLine)
        {
#if true  //เกิดปัญหากับ Points : "NCO", "51-212,51-222,51-232 CONVERTER FAIL"
            string csnLineTemp = csvLine.Replace("\"", string.Empty);
            string[] value = csnLineTemp.Split(new char[] { ',', '"' });
#else
            List<string> iColumn = new List<string>();
            using (MemoryStream ms = new MemoryStream(Encoding.ASCII.GetBytes(csvLine)))
            {
                using (TextFieldParser parser = new TextFieldParser(ms))
                {

                    parser.Delimiters = new string[] { "," };
                    int iLine = 0;
                    while (true)
                    {
                        string[] parts = parser.ReadFields();
                        if (parts == null) break;
                        if (iLine++ == 0)
                        {
                            for (int iCol = 0; iCol < parts.Length; iCol++) iColumn.Add(parts[iCol]);
                            continue;
                        }
                    }
                }
            }
#endif
            try
            {
                AnalogPoint localValue = new AnalogPoint(value);
                return localValue;

            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
                return null;
            }
        }

            public bool CheckDPZero()
            {
                bool bResult = false;

                if (this.m_bMWLossEnable)
                {
                    for (int iPoint = 0; iPoint < this.m_listMWPoint.Count; iPoint++)
                    {
                        AnalogPoint point = this.m_listMWPoint[iPoint];
                        if (point.ActualValue != point.PreFaultValue)
                        {
                            if (point.ActualValue < this.m_fMWThreshold) bResult = true;
                        }
                    }
                }
                return bResult;
            }

            public bool CheckVoltageSwing()
            {
                bool bResult = false;

                if (this.m_bVoltageSwingEnable)
                {
                    for (int iPoint = 0; iPoint < this.m_listKVPoint.Count; iPoint++)
                    {
                        AnalogPoint point = this.m_listKVPoint[iPoint];
                        if (point.Update)
                        {
                            if ((point.AlarmType == (UInt32)AlarmEvent.ANALOG_LOW2) ||
                                (point.AlarmType == (UInt32)AlarmEvent.ANALOG_HIGH2))
                            {
                                bResult = true;
                            }
                        }
                    }
                }
                return bResult;
            }

            public void GenerateMWLossEvent()
            {
                float fPreFaultMW = 0;
                float fPostFaultMW = 0;
                for (int iPoint = 0; iPoint < this.m_listMWPoint.Count; iPoint++)
                {
                    AnalogPoint point = this.m_listMWPoint[iPoint];
                    fPreFaultMW += point.PreFaultValue;
                    fPostFaultMW += point.ActualValue;
                }

                StringBuilder strBuilder = new StringBuilder();

                string strEvent = string.Format("\\nTotal MW Loss {0}", fPreFaultMW - fPostFaultMW);
                strBuilder.Append(strEvent);

                for (int iPoint = 0; iPoint < this.m_listMWPoint.Count; iPoint++)
                {
                    AnalogPoint point = this.m_listMWPoint[iPoint];
                    if (point.ActualValue != point.PreFaultValue)
                    {
                        if (point.ActualValue > this.m_fMWThreshold) continue;
                        strEvent = string.Format("\\n - {0} {1}", point.StationName, point.PointName, point.PreFaultValue);
                        strBuilder.Append(strEvent);
                    }
                }
                this.m_listGenerateEvent.Add(strBuilder.ToString());
            }

            public void GenerateVoltageSwingEvent()
            {
                float fPreFaultMW = 0;
                float fPostFaultMW = 0;
                for (int iPoint = 0; iPoint < this.m_listMWPoint.Count; iPoint++)
                {
                    AnalogPoint point = this.m_listMWPoint[iPoint];
                    fPreFaultMW += point.PreFaultValue;
                    fPostFaultMW += point.ActualValue;
                }

                StringBuilder strBuilder = new StringBuilder();

                string strEvent = string.Format("\\nVoltage Swing", fPreFaultMW - fPostFaultMW);
                strBuilder.Append(strEvent);

                for (int iPoint = 0; iPoint < this.m_listKVPoint.Count; iPoint++)
                {
                    AnalogPoint point = this.m_listKVPoint[iPoint];
                    if (point.Update)
                    {
                        if ((point.AlarmType == (UInt32)AlarmEvent.ANALOG_LOW2) ||
                            (point.AlarmType == (UInt32)AlarmEvent.ANALOG_HIGH2))
                        {
                            strEvent = string.Format("\\n - {0} {1} {2}", point.Time, point.StationName, point.PointName);
                            strBuilder.Append(strEvent);
                        }
                        point.Update = false;
                    }
                }
                this.m_listGenerateEvent.Add(strBuilder.ToString());
            }
        
    }
}
