using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EventLogSearching.Model;
using EventLogSearching.Repository;
using System.Windows.Threading;
using EventLogSearching.Service;
using Microsoft.Win32;
using System.Windows;
using System.Collections.ObjectModel;
using System.Linq.Expressions;

namespace EventLogSearching
{
    class MainWindowViewModel : PropertyChangeEventBase
    {

        private EventLogRepo m_repoEventLog;

       // public Expression<Func<EventLog, bool>> searchParseDeleg;
       public Expression<Func<EventLog, bool>> searchParseDeleg;
        public static List<Item> searchList = new List<Item>();

        private ObservableCollection<EventLog> m_ListEventLog;
        string[] StrSearchEventList;
        //public static List<RestorationAlarmList> CustAlarmListDump { get; private set; }
        public ObservableCollection<EventLog> ListEventLog
        {
            get { return m_ListEventLog; }
            set
            {
                m_ListEventLog = value;
                OnPropertyChanged("ListEventLog");
            }
        }


        public MessageBox myMessageBox { get; set; }
        private OpenFileDialog openFileDialog { get; set; }
        private SaveFileDialog saveFileDialog { get; set; }
        public string[] fileList { get; set; }
        public string saveFileFullPath { get; set; }
        public string[] feildName { get; set; }
        
        private String m_strSearchEventParse1;
        public String StrSearchEventParse1
        {
            get { return m_strSearchEventParse1; }
            set
            {
                m_strSearchEventParse1 = value;
                StrSearchEventList[0] = m_strSearchEventParse1;

                OnPropertyChanged("StrSearchEventParse1");
            }
        }

        private String m_strSearchEventParse2;
        public String StrSearchEventParse2
        {
            get { return m_strSearchEventParse2; }
            set
            {
                m_strSearchEventParse2 = value;
                StrSearchEventList[1] = m_strSearchEventParse2;
                OnPropertyChanged("StrSearchEventParse2");
            }
        }
        private String m_strSearchEventParse3;
        public String StrSearchEventParse3
        {
            get { return m_strSearchEventParse3; }
            set
            {
                m_strSearchEventParse3 = value;
                StrSearchEventList[2] = m_strSearchEventParse3;
                OnPropertyChanged("StrSearchEventParse3");
            }
        }

        private String m_strSearchMessageParses;

        public String StrSearchMessageParses
        {
            get { return m_strSearchMessageParses; }
            set
            {
                m_strSearchMessageParses = value;
                OnPropertyChanged("StrSearchMessageParses");
            }
        }

        //private DispatcherTimer m_dispatcherTimer = new DispatcherTimer();
        public MainWindowViewModel()
        {
            this.m_repoEventLog = new EventLogRepo();
            this.m_strSearchEventParse1 = "";
            this.m_strSearchEventParse2 = "";
            this.m_strSearchEventParse3 = "";

            this.StrSearchEventList = new string[] { m_strSearchEventParse1, m_strSearchEventParse2, m_strSearchEventParse3 };

            this.m_strSearchMessageParses = "";


            this.ListEventLog = new ObservableCollection<EventLog>();

            //this.myMessageBox = new MessageBox();
            //Init openFile dialog
            this.openFileDialog = new OpenFileDialog();
            this.openFileDialog.Multiselect = true;
            this.openFileDialog.Filter = "Report (*.rpt)|*.rpt;|" + "All files (*.*)|*.*";

            //Init savefile dialog
            this.saveFileDialog = new SaveFileDialog();
            this.saveFileDialog.Filter = "Report (*.csv)|*.csv;|" + "All files (*.*)|*.*";


            //gets all FieldName
            var properties = typeof(EventLog).GetProperties();
            this.feildName = properties.Select(d => d.Name).ToArray();

            //Init Command
            OpenFileDlg = new RelayCommand(O => onOpenFileCommand(), O => canOpenFileCommand());
            cmdSearching = new RelayCommand(O => onSearchCommand(), O => canSearchCommand());
            cmdClearList = new RelayCommand(O => onClearListCommand(), O => canClearListCommand());
            cmdExportToCSV = new RelayCommand(O => onExportToCSVtCommand(), O => canExportToCSVtCommand());

        }


        public RelayCommand OpenFileDlg { get; private set; }
        private bool canOpenFileCommand()
        {
            return (true);
        }

        private void onOpenFileCommand()
        {
            this.openFileDialog.ShowDialog();
            this.fileList = openFileDialog.FileNames;

        }

        public RelayCommand cmdClearList { get; private set; }
        private bool canClearListCommand()
        {
            return (this.ListEventLog.Count > 0);
        }

        private void onClearListCommand()
        {
            this.ListEventLog.Clear();

        }

        public RelayCommand cmdExportToCSV { get; private set; }
        private bool canExportToCSVtCommand()
        {
            return (this.ListEventLog.Count > 0);
        }

        private void onExportToCSVtCommand()
        {
            //Get Save File Location
            this.saveFileDialog.ShowDialog();
            this.saveFileFullPath = saveFileDialog.FileName;

            //Change ObservableCollection to list
            var listEventLog = new List<EventLog>(this.ListEventLog);

            //Generate CSV File
            if (ExportToCSV.CreateCSVFromGenericList(listEventLog, this.saveFileFullPath))
                MessageBox.Show("Complete Saving @ " + this.saveFileFullPath.ToString(), "Saved Location", MessageBoxButton.OK, MessageBoxImage.Information); ;
            

        }

        public RelayCommand cmdSearching { get; private set; }
        private bool canSearchCommand()
        {
            //Can Search if some SearchEventParseX not Null or Empty && has File Name in fileList
            return (!(
                    (string.IsNullOrEmpty(this.m_strSearchEventParse1)) &&
                    (string.IsNullOrEmpty(this.m_strSearchEventParse2)) &&
                    (string.IsNullOrEmpty(this.m_strSearchEventParse3)))
                    && this.fileList != null
                   );
        }

        private async void onSearchCommand()
        {

            if (this.fileList != null)
            {

                //searchParseDeleg = xxxx;
                searchList.Add(new Item(feildName[(int)EventLogField.EVENT_FIELD], "FieldName"));

                string[] search_Event_Parse_List1 = StrSearchEventParse1.Split(new string[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);
                string[] search_Event_Parse_List2 = StrSearchEventParse2.Split(new string[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);
                string[] search_Event_Parse_List3 = StrSearchEventParse3.Split(new string[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);
                searchParseDeleg = SearchingExpressionBuilder.GetExpression<EventLog>(feildName[(int)EventLogField.EVENT_FIELD],search_Event_Parse_List1);
                //searchParseDeleg = SearchingExpressionBuilder.GetExpression<EventLog>(groupFields, search_Event_Parse_List1, search_Event_Parse_List2,search_Event_Parse_List3);


                if (searchParseDeleg == null)
                {
                    Console.WriteLine("Expression Building Error");
                }
                else
                {
                    //RestAlarmsRepo.filterParseDeleg = searchParseDeleg;
                    //await RestAlarmsRepo.GetCustAlarmAct();
                   Console.WriteLine(searchParseDeleg.Body);
                }


                await Task.Run(() => m_repoEventLog.ReadRptFileAsync(fileList, search_Event_Parse_List1, m_strSearchMessageParses));


                //For only first time
                if (this.ListEventLog == null)
                {
                    this.ListEventLog = new ObservableCollection<EventLog>(this.m_repoEventLog.ListEventLog);
                }
                else //Next Time
                {
                    foreach (var EventLog in this.m_repoEventLog.ListEventLog)
                    {
                        this.ListEventLog.Add(EventLog);
                    }
                }
                //EventLogs = ListEventLog.Select(s => s.Event);
                MessageBox.Show("ค้นพบ " + this.ListEventLog.Count.ToString() + " Event(s)", "เสร็จสิ้นการค้นหา", MessageBoxButton.OK, MessageBoxImage.Information);

            }
            else
            {
                MessageBox.Show("คุณไม่ได้เลือก file ที่ต้องการค้นหา ", "File Path Error", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

    }
}

