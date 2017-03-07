using EventLogSearching.Model;
using Microsoft.VisualBasic.FileIO;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace EventLogSearching.Repository
{
    class EventLogRepo
    {
        private List<EventLog> m_listEventLogs;
        public List<EventLog> ListEventLog
        {
            get
            {
                return m_listEventLogs;
            }
        }
        private List<string> _ErrorFile { get; set; }
        public List<string> ErrorFile
        {
            get
            {
                return _ErrorFile;
            }
        }
        private String m_strSearchEventParse;
        public String StrSearchEventParse
        {
            get { return m_strSearchEventParse; }
            set { m_strSearchEventParse = value; }
        }

        public EventLogRepo()
        {
            this.m_listEventLogs = new List<EventLog>();

            this.m_strSearchEventParse = "UNBALANCE";
            //this.m_strSearchEventParse = "43CCS+43RCC";
            this._ErrorFile = new List<string>();

        }
        public bool ReadRptFile()
        {

            this.m_listEventLogs.Clear();
            try
            {
                string csvFile = @"C:\Users\OP\Desktop\January 2016\January 04.rpt";


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
                this.m_listEventLogs = File.ReadLines(csvFile)
                            .Skip(4)
                            .Select(line => GetLineEventRptFile(line))
                            .Where(line => line.Event.Contains(this.m_strSearchEventParse))
                            .Where(line => line.Event.Contains(""))
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

        public void ReadRptFileAsync(string[] fileList, String StrSearchEventParse)
        {
            this.m_listEventLogs.Clear();

            foreach (var file in fileList)
            {
                 IEnumerable<EventLog> listEventLog = File.ReadLines(file)
                            .Skip(4)
                            .Select(line => GetLineEventRptFile(line))
                            .Where(line => line.Event.Contains(StrSearchEventParse))
                            
                            .ToList<EventLog>();

                this.m_listEventLogs.AddRange(listEventLog);
            }
            
        }

        public void ReadRptFileAsync(string[] fileList, String StrSearchEventParse, String StrSearchMessageParse)
        {
            this.m_listEventLogs.Clear();

            foreach (var file in fileList)
            {
                IEnumerable<EventLog> listEventLog = File.ReadLines(file)
                           .Skip(4)
                           .Select(line => GetLineEventRptFile(line))
                           .Where(line => line.Event.Contains(StrSearchEventParse))
                           .Where(line => line.Message.Contains(StrSearchMessageParse))
                           .ToList<EventLog>();

                this.m_listEventLogs.AddRange(listEventLog);
            }

        }


        public void ReadRptFileAsync(string[] fileList, string[] StrSearchEventList, String StrSearchMessageParse)
        {
            this.m_listEventLogs.Clear();

            foreach (var file in fileList)
            {
                IEnumerable<EventLog> listEventLog = File.ReadLines(file)
                           .Skip(4)
                           .Select(line => GetLineEventRptFile(line))
                           .Where(line => line.Event.Contains(StrSearchEventList[0]))
                           //.Where(line => line.Event.Contains(StrSearchEventList[1]))
                           //.Where(line => line.Event.Contains(StrSearchEventList[2]))
                           .Where(line => line.Message.Contains(StrSearchMessageParse))
                           .ToList<EventLog>();

                this.m_listEventLogs.AddRange(listEventLog);
            }

        }

        //Use Where by Linq Expression
        public void ReadRptFileAsync(string[] fileList, Expression<Func<EventLog, bool>> filterParseDeleg)
        {
            // Compiling the expression tree into a delegate.  
           
            Func<EventLog, bool> result = filterParseDeleg.Compile();
            this.m_listEventLogs.Clear();

            foreach (var file in fileList)
            {
                try
                {
                    IEnumerable<EventLog> listEventLog = File.ReadLines(file).Skip(4)
                          .Select(line => GetLineEventRptFile(line))
                          .Where(result)
                          .ToList<EventLog>();

                    this.m_listEventLogs.AddRange(listEventLog);
                }
                catch (Exception e)
                {
                    Console.WriteLine("File " + file.ToString()+ " Error");
                    string FileName = Path.GetFileName(file);
                    string FolderName = Path.GetFileName(Path.GetDirectoryName(file));
                    this._ErrorFile.Add(FolderName +"\\" + FileName);
                    continue;
                }
                
            }

        }

        public static EventLog GetLineEventRptFile(string RptLine)
        {
#if false  //เกิดปัญหากับ Points : "NCO", "51-212,51-222,51-232 CONVERTER FAIL"
            string csnLineTemp = RptLine.Replace("\"", string.Empty);
            string[] value = csnLineTemp.Split(new char[] { ',', '"' });

            try
            {
                EventLog localValue = new EventLog(value);
                Console.WriteLine("TestBreak");
                return localValue;

            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
                return null;
            }
#else
            List<string> iColumn = new List<string>();
            using (MemoryStream ms = new MemoryStream(Encoding.ASCII.GetBytes(RptLine)))
            {
                using (TextFieldParser parser = new TextFieldParser(ms))
                {

                    parser.Delimiters = new string[] { "," };
                    int iLine = 0;
                    while (true)
                    {
                        try
                        {
                            string[] parts = parser.ReadFields();
                            if (parts == null) break;
                            if (iLine++ == 0)
                            {
                                for (int iCol = 0; iCol < parts.Length; iCol++) iColumn.Add(parts[iCol]);
                                continue;
                            }
                        }
                        catch
                        {
                            continue;
                        }
                    }
                }
            }

            try
            {
                EventLog localValue = new EventLog(iColumn);
                return localValue;

            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
                return null;
            }

#endif
        }
    }
}
