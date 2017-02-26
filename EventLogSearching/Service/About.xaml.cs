using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Deployment.Application;

namespace EventLogSearching.Service
{
    /// <summary>
    /// Interaction logic for About.xaml
    /// </summary>
    public partial class About : Window
    {
        public string version { get; set; }
        public About()
        {
            InitializeComponent();
            DataContext = this;
            version = null;
            try
            {
                //// get deployment version
                version = "Version" + ApplicationDeployment.CurrentDeployment.CurrentVersion.ToString();
                txtVersion.Content = version;
            }
            catch (InvalidDeploymentException)
            {
                //// you cannot read publish version when app isn't installed 
                //// (e.g. during debug)
                version = "not installed";
                txtVersion.Content = version;
            }
        }
    }
}
