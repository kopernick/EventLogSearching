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
        string[] StrSearchEventList1;
        string[] StrSearchEventList2;
        string[] StrSearchEventList3;
        string[] StrSearchEventList4;
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

        //private DispatcherTimer m_dispatcherTimer = new DispatcherTimer();
        public MainWindowViewModel()
        {
            this.m_repoEventLog = new EventLogRepo();
            this.m_strSearchEventParse1 = "";
            this.m_strSearchEventParse2 = "";
            this.m_strSearchEventParse3 = "";
            this.m_strSearchMessageParses = "";

            this.StrSearchEventList1 = m_strSearchEventParse1.Split(new string[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);
            this.StrSearchEventList2 = m_strSearchEventParse2.Split(new string[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);
            this.StrSearchEventList3 = m_strSearchEventParse3.Split(new string[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);
            this.StrSearchEventList4 = m_strSearchMessageParses.Split(new string[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);

            this.searchParseDeleg = null;


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
                MessageBox.Show("Saving Complete @ " + this.saveFileFullPath.ToString(), "Saved Location", MessageBoxButton.OK, MessageBoxImage.Information); ;

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
               // searchList.Add(new Item(feildName[(int)EventLogField.EVENT_FIELD], "FieldName"))

                List<SearchElement> searchItems = new List<SearchElement>();

                searchItems.Add(new SearchElement(StrSearchEventList1, fieldName[(int)EventLogField.EVENT_FIELD]));
                searchItems.Add(new SearchElement(StrSearchEventList2, fieldName[(int)EventLogField.EVENT_FIELD]));
                searchItems.Add(new SearchElement(StrSearchEventList3, fieldName[(int)EventLogField.EVENT_FIELD]));
                searchItems.Add(new SearchElement(StrSearchEventList4, fieldName[(int)EventLogField.MESSAGE_FIELD]));


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
                    await Task.Run(() => m_repoEventLog.ReadRptFileAsync(fileList, searchParseDeleg));
                    
                }


               // await Task.Run(() => m_repoEventLog.ReadRptFileAsync(fileList, search_Event_Parse_List1, m_strSearchMessageParses));


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

