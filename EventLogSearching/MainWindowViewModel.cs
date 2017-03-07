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
using System.IO;
using System.Windows.Input;

namespace EventLogSearching
{
    class MainWindowViewModel : PropertyChangeEventBase
    {
        //Delegate
      

        private EventLogRepo m_repoEventLog;

       // public Expression<Func<EventLog, bool>> searchParseDeleg;
        public Expression<Func<EventLog, bool>> searchParseDeleg;

        private ObservableCollection<EventLog> m_ListEventLog;
        string[] StrSearchEventList1;
        string[] StrSearchEventList2;
        string[] StrSearchEventList3;
        string[] StrSearchEventList4;
        string[] StrSearchEventList5;
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

        private List<EventLog> listEventLog { get; set; }
        public string _ErrorFile { get; set; }
        public string ErrorFile
        {
            get { return _ErrorFile; }
            set
            {
                _ErrorFile = value;
                OnPropertyChanged("ErrorFile");
            }
        }
        public MessageBox myMessageBox { get; set; }
        private OpenFileDialog openFileDialog { get; set; }
        private SaveFileDialog saveFileDialog { get; set; }
        public string[] fileList { get; set; }
        public string saveFileFullPath { get; set; }
        public string[] fieldName { get; set; }

        private string m_ExpressionTree;
        public string ExpressionTree
        {
            get { return m_ExpressionTree;}
            set
            {
                m_ExpressionTree = value;
                OnPropertyChanged("ExpressionTree");
            }
        }

        private string m_fileDirectory;
        public string fileDirectory
        {
            get { return m_fileDirectory; }
            set
            {
                m_fileDirectory = value;
                OnPropertyChanged("fileDirectory");
            }
        }
        
        private string m_strSearchEventParse1;
        public string StrSearchEventParse1
        {
            get { return m_strSearchEventParse1; }
            set
            {
                m_strSearchEventParse1 = value;
                this.StrSearchEventList1 = m_strSearchEventParse1.Split(new string[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);

                OnPropertyChanged("StrSearchEventParse1");
            }
        }

        private string m_strSearchEventParse2;
        public string StrSearchEventParse2
        {
            get { return m_strSearchEventParse2; }
            set
            {
                m_strSearchEventParse2 = value;
                this.StrSearchEventList2 = m_strSearchEventParse2.Split(new string[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);
                OnPropertyChanged("StrSearchEventParse2");
            }
        }
        private string m_strSearchEventParse3;
        public string StrSearchEventParse3
        {
            get { return m_strSearchEventParse3; }
            set
            {
                m_strSearchEventParse3 = value;
                this.StrSearchEventList3 = m_strSearchEventParse3.Split(new string[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);
                OnPropertyChanged("StrSearchEventParse3");
            }
        }

        private string m_strSearchMessageParses;

        public string StrSearchMessageParses
        {
            get { return m_strSearchMessageParses; }
            set
            {
                m_strSearchMessageParses = value;
                this.StrSearchEventList4 = m_strSearchMessageParses.Split(new string[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);
                OnPropertyChanged("StrSearchMessageParses");
            }
        }

        private string m_strSearchExcludeParses;
        public string StrSearchExcludeParses
        {
            get { return m_strSearchExcludeParses; }
            set
            {
                m_strSearchExcludeParses = value;
                this.StrSearchEventList5 = m_strSearchExcludeParses.Split(new string[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);
                OnPropertyChanged("StrSearchExcludeParses");
            }
        }

        //private DispatcherTimer m_dispatcherTimer = new DispatcherTimer();
        public MainWindowViewModel()
        {
            this.m_repoEventLog = new EventLogRepo();
            this.m_strSearchEventParse1 = "";
            this.m_strSearchEventParse2 = "";
            this.m_strSearchEventParse3 = "";
            this.m_strSearchMessageParses = "";
            this.m_strSearchExcludeParses = "";

            this.StrSearchEventList1 = m_strSearchEventParse1.Split(new string[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);
            this.StrSearchEventList2 = m_strSearchEventParse2.Split(new string[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);
            this.StrSearchEventList3 = m_strSearchEventParse3.Split(new string[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);
            this.StrSearchEventList4 = m_strSearchMessageParses.Split(new string[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);
            this.StrSearchEventList5 = m_strSearchExcludeParses.Split(new string[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);

            this.searchParseDeleg = null;

            this.listEventLog = null;
            this.ErrorFile = null;


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
            this.fieldName = properties.Select(d => d.Name).ToArray();

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
            this.fileDirectory = Path.GetDirectoryName(fileList[0]).ToString();

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
            this.listEventLog = new List<EventLog>(this.ListEventLog);

            //Generate CSV File
            if (ExportToCSV.CreateCSVFromGenericList(listEventLog, this.saveFileFullPath))
                MessageBox.Show("Saving Complete @ " + this.saveFileFullPath.ToString(), "Saved Location", MessageBoxButton.OK, MessageBoxImage.Information); ;

            this.listEventLog = null;

        }
        public RelayCommand cmdSearching { get; private set; }
        private bool canSearchCommand()
        {
            //Can Search if some SearchEventParseX not Null or Empty && has File Name in fileList
            return (!(
                    string.IsNullOrEmpty(this.m_strSearchEventParse1) &&
                    string.IsNullOrEmpty(this.m_strSearchEventParse2) &&
                    string.IsNullOrEmpty(this.m_strSearchEventParse3) &&
                    string.IsNullOrEmpty(this.m_strSearchMessageParses) &&
                    string.IsNullOrEmpty(this.m_strSearchExcludeParses))
                    && this.fileList != null
                   );
        }

        private ICommand _showAboutMeCommand;
        public ICommand ShowAboutMeCommand
        {
            get
            {
                return _showAboutMeCommand ??
                       (_showAboutMeCommand = new RelayCommand(p => DisplayAboutMe(), p => true));
            }
        }

        private void DisplayAboutMe()
        {
            About aboutMe = new About();
            aboutMe.Show();
        }

        private async void onSearchCommand()
        {
            if (this.fileList != null)
            {
                m_repoEventLog.ErrorFile.Clear();

                this.ErrorFile = null;

                List<SearchElement> searchItems = new List<SearchElement>();
                searchItems.Add(new SearchElement(StrSearchEventList1, fieldName[(int)EventLogField.EVENT_FIELD], true));       // Event Include Keyword 1
                searchItems.Add(new SearchElement(StrSearchEventList2, fieldName[(int)EventLogField.EVENT_FIELD], true));       // Event Include Keyword 2
                searchItems.Add(new SearchElement(StrSearchEventList3, fieldName[(int)EventLogField.EVENT_FIELD], true));       // Event Include Keyword 3
                searchItems.Add(new SearchElement(StrSearchEventList4, fieldName[(int)EventLogField.MESSAGE_FIELD], true));     // Message Include Keyword
                searchItems.Add(new SearchElement(StrSearchEventList5, fieldName[(int)EventLogField.EVENT_FIELD], false));    // Event Exclude Keyword
                searchParseDeleg = SearchingExpressionBuilder.GetExpression<EventLog>(searchItems);

                if (searchParseDeleg == null)
                {

                    Console.WriteLine("Expression Building Error");
                    ExpressionTree = "Expression Building Error";
                }
                else
                {
                    Console.WriteLine(searchParseDeleg.Body);
                    ExpressionTree = searchParseDeleg.Body.ToString();

                    using (new WaitCursor())
                    {
                        StringBuilder txt = new StringBuilder();
                        await Task.Run(() => m_repoEventLog.ReadRptFileAsync(fileList, searchParseDeleg));

                        if (m_repoEventLog.ErrorFile.Count != 0)
                            foreach (var item in m_repoEventLog.ErrorFile)
                            { 
                                txt.Append(item);
                                txt.AppendLine();
                            }
                        this.ErrorFile  = txt.ToString();
                        // very long task
                    }
                }

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

