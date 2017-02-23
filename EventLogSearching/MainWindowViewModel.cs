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

namespace EventLogSearching
{
    class MainWindowViewModel : PropertyChangeEventBase
    {

        private EventLogRepo m_repoEventLog;

        private ObservableCollection<EventLog> m_ListEventLog;
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

        private String m_strSearchEventParse;
        public String StrSearchEventParse
        {
            get { return m_strSearchEventParse; }
            set {
                    m_strSearchEventParse = value;
                    OnPropertyChanged("StrSearchEventParse");
            }
        }

        //private DispatcherTimer m_dispatcherTimer = new DispatcherTimer();
        public MainWindowViewModel()
        {
            this.m_repoEventLog = new EventLogRepo();
            this.m_strSearchEventParse = "";
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
            return (m_strSearchEventParse != "");
        }

        private async void onSearchCommand()
        {
            //this.m_repoEventLog.ReadRptFile();

            if (this.fileList != null)
            {
                await Task.Run(() => this.m_repoEventLog.ReadRptFileAsync(this.fileList, this.m_strSearchEventParse));

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
            return (this.ListEventLog != null);
        }

        private void onClearListCommand()
        {
            this.ListEventLog.Clear();

        }
    }
}

