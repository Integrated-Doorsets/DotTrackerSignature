using System;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.DirectoryServices.AccountManagement;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Navigation;

namespace IdslTracker
{
    /// <summary>
    /// Interaction logic for MainMenuWindow.xaml
    /// </summary>
    public partial class MainMenuWindow : Window
    {
        //SopReportWindow sopReportWindow;
        //MainWindow TrackerWindow;
        //ManufacturingReportWindow manufacturingReportWindow;

        public MainMenuWindow()
        {
            InitializeComponent();
            Globals.IsPowerUser = GetPowerUserStatus();
            Globals.IsAdGrpTrackerUsers = IsInAdGroup("TrackerUsers");
        }

        private bool IsInAdGroup(string grpName)
        {
            using (PrincipalContext context = new PrincipalContext(ContextType.Domain))
            using (UserPrincipal user = UserPrincipal.FindByIdentity(context, Environment.UserName))
            using (PrincipalSearchResult<Principal> groups = user.GetAuthorizationGroups())
            {
                return groups.OfType<GroupPrincipal>().Any(g => g.Name.Equals(grpName, StringComparison.OrdinalIgnoreCase));
            }

        }

        private bool GetPowerUserStatus()
        {
            using (SqlConnection connection = new SqlConnection(Properties.Resources.db))
            {
                using (SqlCommand command = new SqlCommand("Tracker.dbo.GET_POWER_USERS", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    connection.Open();

                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            if (reader.GetString(0) == System.Security.Principal.WindowsIdentity.GetCurrent().Name.ToUpper())
                            {
                                return true;
                            }
                        }
                    }
                }
            }

            return false;
        }

        private void Tracker_Button_Click(object sender, RoutedEventArgs e)
        {
            if (Globals.TrackerWindow == null || Globals.TrackerWindow.IsLoaded == false)
            {
                Globals.TrackerWindow = new MainWindow();
            }
            else
            {
                Globals.TrackerWindow.Activate();
            }
            Globals.TrackerWindow.WindowState = WindowState.Maximized;
            Globals.TrackerWindow.Show();
        }

        //private void Reports_Button_Click(object sender, RoutedEventArgs e)
        //{
        //    if (Globals.reportWindow == null || Globals.reportWindow.IsLoaded == false)
        //    {
        //        Globals.reportWindow = new ReportWindow();
        //    }
        //    else
        //    {
        //        Globals.reportWindow.Activate();
        //    }
        //    Globals.reportWindow.WindowState = WindowState.Maximized;
        //    Globals.reportWindow.Show();

        //}


        

        private void Hyperlink_RequestNavigate(object sender, RequestNavigateEventArgs e)
        {
            Process.Start(new ProcessStartInfo(e.Uri.AbsoluteUri));
            e.Handled = true;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            if (Environment.UserName == "shanebonser" || Environment.UserName == "garethparkin" || Environment.UserName == "CraigWragg")
            {
                var path = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
                System.IO.Directory.CreateDirectory(path + "\\AppRefreshAnimation");
                if(!File.Exists(path + "\\AppRefreshAnimation\\loading.gif"))
                {
                    File.Copy("Images/loading.gif", path + "\\AppRefreshAnimation\\loading.gif");
                }

                Globals.LoadingAnimPath = new Uri(path + "\\AppRefreshAnimation\\loading.gif", UriKind.Absolute);
            }
            else
            {
                Globals.LoadingAnimPath = new Uri("Images/loading.gif", UriKind.Relative);
            }
        }

        private void ManageProforma_Button_Click(object sender, RoutedEventArgs e)
        {
           
        }
    }
}
