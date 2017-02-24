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
using System.Windows.Controls.Ribbon;

namespace EventLogSearching
{
    class MainWindowViewModel : PropertyChangeEventBase
    {

        private EventLogRepo m_repoEventLog;

        private ObservableCollection<EventLog> m_ListEventLog;
        private List<string> StrSearchEventList;
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

        private OpenFileDialog openFileDialog { get; set; }
        public string[] fileList { get; set; }

        private String m_strSearchEventParse1;
        public String StrSearchEventParse1
        {
            get { return m_strSearchEventParse1; }
            set {
                    m_strSearchEventParse1 = value;
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
                OnPropertyChanged("StrSearchEventParse2");
            }
        }
        private String m_strSearchEventParse3;
        public String StrSearchEventParse3
        {
            get { return m_strSearchEventParse3; }
            set
            {
                m_strSearchEventParse1 = value;
                OnPropertyChanged("StrSearchEventParse3");
            }
        }

        private String m_strSearchMessageParse;
        public String StrSearchMessageParse
        {
            get { return m_strSearchMessageParse; }
            set
            {
                m_strSearchMessageParse = value;
                OnPropertyChanged("StrSearchMessageParse");
            }
        }

        //private DispatcherTimer m_dispatcherTimer = new DispatcherTimer();
        public MainWindowViewModel()
        {
            this.m_repoEventLog = new EventLogRepo();
            this.m_strSearchEventParse1 = "";
            this.m_strSearchEventParse2 = "";
            this.m_strSearchEventParse3 = "";
            this.m_strSearchMessageParse = "";

            this.StrSearchEventList = new List<string>();
            this.ListEventLog = new ObservableCollection<EventLog>();

            this.openFileDialog = new OpenFileDialog();
            this.openFileDialog.Multiselect = true;
            this.openFileDialog.Filter = "Report (*.rpt)|*.rpt;|" + "All files (*.*)|*.*";

            OpenFileDlg = new RelayCommand(O => onFirstPageCommand(), O => canPrePageCommand());
            cmdSearching = new RelayCommand(O => onSearchCommand(), O => canSearchCommand());
            cmdClearList = new RelayCommand(O => onClearListCommand(), O => canSClearListCommand());


        }


        public RelayCommand OpenFileDlg { get; private set; }
        private bool canPrePageCommand()
        {
            return (true);
        }

        private void onFirstPageCommand()
        {
            this.openFileDialog.ShowDialog();
            this.fileList = openFileDialog.FileNames;

            //foreach (string y in this.fileList)
            //    MessageBox.Show(y, "Selected Item",MessageBoxButton.OK,MessageBoxImage.Information);

        }

        public RelayCommand cmdSearching { get; private set; }
        private bool canSearchCommand()
        {
            return (m_strSearchEventParse1 != "" && this.fileList != null);
        }

        private async void onSearchCommand()
        {
            StrSearchEventList.Clear();

            StrSearchEventList.Add(this.m_strSearchEventParse1);
            StrSearchEventList.Add(this.m_strSearchEventParse2);
            StrSearchEventList.Add(this.m_strSearchEventParse3);

            if (this.fileList != null)
            {
                await Task.Run(() => this.m_repoEventLog.ReadRptFileAsync(this.fileList, this.StrSearchEventList, this.m_strSearchMessageParse));

                //For only first time
                if (this.ListEventLog == null)
                {
                    this.ListEventLog = new ObservableCollection<EventLog>(this.m_repoEventLog.ListEventLog);
                }
                else //Next Time
                {
                    foreach(var EventLog in this.m_repoEventLog.ListEventLog)
                    {
                        this.ListEventLog.Add(EventLog);
                    }
                }
                //EventLogs = ListEventLog.Select(s => s.Event);
                MessageBox.Show("ค้นพบ " + this.ListEventLog.Count.ToString() + " Event(s)" , "เสร็จสิ้นการค้นหา", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        //cmdClearList = new RelayCommand(O => onClearListCommand(), O => canSClearListCommand());
        public RelayCommand cmdClearList { get; private set; }
        private bool canSClearListCommand()
        {
            return (this.ListEventLog.Count > 0);
        }

        private void onClearListCommand()
        {
            this.ListEventLog.Clear();

        }
    }
}

