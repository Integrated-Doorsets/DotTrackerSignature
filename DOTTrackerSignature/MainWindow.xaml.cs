using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using Org.BouncyCastle.Asn1.Ocsp;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace IdslTracker
{
    public partial class MainWindow : Window
    {
        const double TIMER_TIMEOUT = 60000 * 2;
        private Timer REFRESH_TIMER;
        public MainWindow()
        {
            InitializeComponent();
            loadingGif.Visibility = Visibility.Hidden;

            REFRESH_TIMER = new Timer(TIMER_TIMEOUT);
            REFRESH_TIMER.Elapsed += DisplayRefreshPrompt;
            REFRESH_TIMER.AutoReset = false;
            REFRESH_TIMER.Enabled = true;
            REFRESH_TIMER.Start();


            Globals.ManfSites = GetManfSites();
            Globals.PrintedByNames = GetPrintedByNames();
            Globals.ManufactureReps = GetManufactureReps();






            RefreshWarning.Visibility = Visibility.Hidden;
        }

        private void DisplayRefreshPrompt(object sender, ElapsedEventArgs e)
        {
            this.Dispatcher.Invoke(() =>
            {
                //RefreshWarning.Visibility = Visibility.Visible;
                PopulateTrackerDataGrids();
            });
        }

        private List<string> GetManufactureReps()
        {
            List<string> manufactureReps = new List<string>();
            using (SqlConnection connection = new SqlConnection(Properties.Resources.db))
            {
                using (SqlCommand command = new SqlCommand("Tracker.dbo.GET_MANUFACTURE_REPS", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    connection.Open();

                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            manufactureReps.Add(reader.GetString(0));
                        }
                    }
                }
            }
            return manufactureReps;
        }

        private List<string> GetPrintedByNames()
        {
            List<string> printedByNames = new List<string>();
            using (SqlConnection connection = new SqlConnection(Properties.Resources.db))
            {
                using (SqlCommand command = new SqlCommand("Tracker.dbo.GET_PRINTED_BY_NAMES", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    connection.Open();

                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            printedByNames.Add(reader.GetString(0));
                        }
                    }
                }
            }
            return printedByNames;
        }

        private List<string> GetManfSites()
        {
            List<string> manfSiteNames = new List<string>();
            using (SqlConnection connection = new SqlConnection(Properties.Resources.db))
            {
                using (SqlCommand command = new SqlCommand("Tracker.dbo.GET_MANF_SITES", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    connection.Open();

                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            manfSiteNames.Add(reader.GetString(0));
                        }
                    }
                }
            }
            return manfSiteNames;
        }

        private void LoadingGif_MediaEnded(object sender, RoutedEventArgs e)
        {
            loadingGif.Position = new TimeSpan(0, 0, 1);
            loadingGif.Play();
        }

        private void Refresh_Button_Click(object sender, RoutedEventArgs e)
        {
            //RefreshPrompt.Visibility = Visibility.Hidden;

            DataGrid dg = GetVisibleDataGrid();
            RefreshDataGrid(dg);

        }

        private void RefreshDataGrid(DataGrid dg)
        {
            string dgName = dg.Name;
            var bgw = new BackgroundWorker();
            bgw.DoWork += (_, __) =>
            {
                this.Dispatcher.Invoke(() =>
                {
                    MainTabControl.Visibility = Visibility.Hidden;
                    HeaderControlsGrid.Visibility = Visibility.Hidden;
                    loadingGif.Visibility = Visibility.Visible;
                });

                if (dgName == "MaterialReviewDataGrid")
                {
                 
                    this.Dispatcher.Invoke(() =>
                    {
                        MessageBox.Show(this, "Only the material review tab has been refreshed.");

                    });
                }
                else if (dgName == "CreditStopReviewDataGrid")
                {
           
                    this.Dispatcher.Invoke(() =>
                    {
                        MessageBox.Show(this, "Only the Credit Stop review tab has been refreshed.");

                    });

                }
                else
                {
                    PopulateTrackerDataGrids();
                }
            };



            bgw.RunWorkerCompleted += (_, __) =>
            {
                this.Dispatcher.Invoke(() =>
                {
                    loadingGif.Visibility = Visibility.Hidden;
                    MainTabControl.Visibility = Visibility.Visible;
                    HeaderControlsGrid.Visibility = Visibility.Visible;
                    ApplyFilter();
                   
                    //if (Globals.manufacturingReportWindow != null)
                    //{
                    //    Globals.manufacturingReportWindow.RefreshMainReportDataGrid();
                    //}


                    REFRESH_TIMER.Interval = TIMER_TIMEOUT;
                    RefreshWarning.Visibility = Visibility.Hidden;
                    REFRESH_TIMER.Start();
                });
            };
            bgw.RunWorkerAsync();
        }

     


        private void PopulateTrackerDataGrids()
        {

            

           

        


            List<IdslTrackerLine> mainTrackerLines = new List<IdslTrackerLine>();
          
            List<DateTime> deliveryMonthList = new List<DateTime>();
            List<int> deliveryWeekList = new List<int>();

            List<string> commentColourHexs = new List<string>();

            using (SqlConnection connection = new SqlConnection(Properties.Resources.db))
            {
                using (SqlCommand command = new SqlCommand("SELECT * FROM Tracker.dbo.TrackingData where Processed = 1 AND Signed = 0 AND Active = 1 ", connection))

                {
                    command.CommandTimeout = 180;
                  

                    command.CommandType = CommandType.Text;
                    SqlDataAdapter da = new SqlDataAdapter(command);
                    DataTable dt = new DataTable();
                    da.Fill(dt);

                     if(dt.Rows.Count > 0)
                    { int i;
                        for(i=0;i<=dt.Rows.Count - 1; i++)
                        {
                           
                             IdslTrackerLine trackerLine = new IdslTrackerLine();
                            trackerLine.id = Convert.ToInt32(dt.Rows[i]["id"]);
                            if (DateTime.TryParse(Convert.ToString(dt.Rows[i]["DeliveryDate"]), out DateTime Temp) == true)
                            {
                                trackerLine.DeliveryDate = Convert.ToDateTime(dt.Rows[i]["DeliveryDate"]);

                            }
                            else { }
                          
                            trackerLine.ContractNumber = Convert.ToString(dt.Rows[i]["ContractNumber"]);
                            trackerLine.ContractName = Convert.ToString(dt.Rows[i]["ContractName"] + "");
                            trackerLine.PostCode = Convert.ToString(dt.Rows[i]["PostCode"]);
                            trackerLine.ContactName = Convert.ToString(dt.Rows[i]["ContactName"] + "");
                            trackerLine.ContactTelephoneNumber = Convert.ToString(dt.Rows[i]["ContactTelephoneNumber"] + "");
                            trackerLine.VehicleReg = Convert.ToString(dt.Rows[i]["VehicleReg"]);
                            trackerLine.VehicleSizeUsed = Convert.ToString(dt.Rows[i]["VehicleSizeUsed"] + "");
                            trackerLine.EtaOnSite = Convert.ToString(dt.Rows[i]["EtaOnSite"]);
                            trackerLine.ActualOnSite = Convert.ToString(dt.Rows[i]["ActualOnSite"] + "");
                            trackerLine.Comments = Convert.ToString(dt.Rows[i]["Comments"] + "");
                            trackerLine.CustomerSiteEmailAddress = Convert.ToString(dt.Rows[i]["CustomerSiteEmailAddress"]);
                            trackerLine.CustomerAccountEmailAddress = Convert.ToString(dt.Rows[i]["CustomerAccountEmailAddress"] + "");
                            trackerLine.OtherEmailAddress = Convert.ToString(dt.Rows[i]["OtherEmailAddress"]);
                            trackerLine.FTBDeliveryVehicle = Convert.ToString(dt.Rows[i]["FTBDeliveryVehicle"] + "");
                            trackerLine.DriverName = Convert.ToString(dt.Rows[i]["DriverName"] + "");
                            if (Convert.ToBoolean(dt.Rows[i]["NotesEmailed"]) == true)
                            { trackerLine.NotesEmailed = true;}else {trackerLine.NotesEmailed = false;}
                            if (Convert.ToBoolean(dt.Rows[i]["OnTracker"]) == true)
                            { trackerLine.OnTracker = true; }
                            else { trackerLine.OnTracker = false; }
                            if (Convert.ToBoolean(dt.Rows[i]["OnTime"]) == true)
                            { trackerLine.OnTime = true; }
                            else { trackerLine.OnTime = false; }
                            if (Convert.ToBoolean(dt.Rows[i]["Active"]) == true)
                            { trackerLine.Active = true; }
                            else { trackerLine.Active = false; }
                            if (Convert.ToBoolean(dt.Rows[i]["Processed"]) == true)
                            { trackerLine.Processed = true; }
                            else { trackerLine.Processed = false; }
                            if (Convert.ToBoolean(dt.Rows[i]["Completed"]) == true)
                            { trackerLine.Completed = true; }
                            else { trackerLine.Completed = false; }
                            if (Convert.ToBoolean(dt.Rows[i]["Signed"]) == true)
                            { trackerLine.Signed = true; }
                            else { trackerLine.Signed = false; }

                            try { trackerLine.SignatureCopy = StrToByteArray( Convert.ToString(dt.Rows[i]["SignatureCopy"])); } catch { }
                            mainTrackerLines.Add(trackerLine);

                                                        
                        }
                    }
                    else { return; }

                   
                }

              

                this.Dispatcher.Invoke(() =>
                {
                    MainTrackerDataGrid.ItemsSource = mainTrackerLines;
                  

                    deliveryMonthList.Sort((x, y) => DateTime.Compare(x, y));

                    
                    deliveryWeekList.Sort();
                   

                   

                    //universalDeliveryWeekComboBox.Items.SortDescriptions = SortDescription;

                });
            }
        }






        static byte[] StrToByteArray(string str)
        {
            Dictionary<string, byte> hexindex = new Dictionary<string, byte>();
            for (int i = 0; i <= 255; i++)
                hexindex.Add(i.ToString("X2"), (byte)i);

            List<byte> hexres = new List<byte>();
            for (int i = 0; i < str.Length; i += 2)
                hexres.Add(hexindex[str.Substring(i, 2)]);

            return hexres.ToArray();
        }

       


        private int GetIso8601WeekNumber(DateTime date)
        {
            if (date == DateTime.MinValue)
            {
                return 0;
            }

            var thursday = date.AddDays(3 - ((int)date.DayOfWeek + 6) % 7);
            return 1 + (thursday.DayOfYear - 1) / 7;
        }

        

        public static DateTime ConvertToLastDayOfMonth(DateTime date)
        {
            return new DateTime(date.Year, date.Month, DateTime.DaysInMonth(date.Year, date.Month));
        }
        public static DateTime ConvertToFirstDayOfMonth(DateTime date)
        {
            return new DateTime(date.Year, date.Month, 1);
            //return new DateTime(date.Year, date.Month, DateTime.DaysInMonth(date.Year, date.Month));
        }

        private DataGrid GetVisibleDataGrid()
        {
            DataGrid dg = null;

            switch ((MainTabControl.SelectedItem as TabItem).Header.ToString())
            {
                case "DOT Tracker":

                    dg = MainTrackerDataGrid;
                    break;
               
                default:
                    //MessageBox.Show(this, "Unexcpected error... Aborting operation");
                    break;

            }

            return dg;
        }




        System.Windows.Threading.DispatcherTimer _typingTimer;
        private void SearchBox_TextChanged(object sender, TextChangedEventArgs e)
        {

            if (_typingTimer == null)
            {
                _typingTimer = new DispatcherTimer();
                _typingTimer.Interval = TimeSpan.FromMilliseconds(500);

                _typingTimer.Tick += new EventHandler(this.HandleTypingTimerTimeout);
            }
            _typingTimer.Stop(); // Resets the timer
            _typingTimer.Tag = (sender as TextBox).Text; // This should be done with EventArgs
            _typingTimer.Start();
        }

        private void HandleTypingTimerTimeout(object sender, EventArgs e)
        {
            var timer = sender as DispatcherTimer;
            if (timer == null)
            {
                return;
            }

            ApplyFilter();
            //MessageBox.Show(this,"Not Implimented Yet");
            timer.Stop();
        }

        private void TrackerItemDoubleClick(object sender, MouseButtonEventArgs e)
        {
            DataGrid dg = GetVisibleDataGrid();
            IdslTrackerLine selectedTrackerLine = dg.SelectedItem as IdslTrackerLine;

            SignatureWindow signWindow = new SignatureWindow(selectedTrackerLine, dg.Name);
            signWindow.Owner = this;
            signWindow.ShowDialog();
            if (signWindow.DialogResult == true)
            {
                //RefreshDataGrid(dg);

                var bgw = new BackgroundWorker();
                bgw.DoWork += (_, __) =>
                {
                    this.Dispatcher.Invoke(() =>
                    {
                        MainTabControl.Visibility = Visibility.Hidden;
                        HeaderControlsGrid.Visibility = Visibility.Hidden;
                        loadingGif.Visibility = Visibility.Visible;
                    });

                    PopulateTrackerDataGrids();
                    //  RefreshDataGrid(MainTrackerDataGrid);

                    //RefreshSelectedLine(ref selectedTrackerLine);
                };

                bgw.RunWorkerCompleted += (_, __) =>
                {
                    this.Dispatcher.Invoke(() =>
                    {
                        dg.Items.Refresh();
                        loadingGif.Visibility = Visibility.Hidden;
                        MainTabControl.Visibility = Visibility.Visible;
                        HeaderControlsGrid.Visibility = Visibility.Visible;
                    });
                };
                bgw.RunWorkerAsync();

            }

        }

        private void RefreshSelectedLine(ref IdslTrackerLine selectedTrackerLine)
        {
            using (SqlConnection connection = new SqlConnection(Properties.Resources.db))
            {
                using (SqlCommand command = new SqlCommand("select * from Tracker.dbo.TrackingData where id=@id", connection))
                {
                    command.CommandTimeout = 180;
                    command.CommandType = CommandType.Text;
                   command.Parameters.AddWithValue("@id", selectedTrackerLine.id);

                    SqlDataAdapter da = new SqlDataAdapter(command);
                    DataTable dt = new DataTable();
                    da.Fill(dt);

                    if (dt.Rows.Count > 0)
                    {
                        int i;
                        for (i = 0; i <= dt.Rows.Count - 1; i++)
                        {
                            selectedTrackerLine.id = Convert.ToInt32(dt.Rows[i]["id"]);
                            if (DateTime.TryParse(Convert.ToString(dt.Rows[i]["DeliveryDate"]), out DateTime Temp) == true)
                            {
                                selectedTrackerLine.DeliveryDate = Convert.ToDateTime(dt.Rows[i]["DeliveryDate"]);

                            }
                            else { }

                            selectedTrackerLine.ContractNumber = Convert.ToString(dt.Rows[i]["ContractNumber"]);
                            selectedTrackerLine.ContractName = Convert.ToString(dt.Rows[i]["ContractName"] + "");
                            selectedTrackerLine.PostCode = Convert.ToString(dt.Rows[i]["PostCode"]);
                            selectedTrackerLine.ContactName = Convert.ToString(dt.Rows[i]["ContactName"] + "");
                            selectedTrackerLine.ContactTelephoneNumber = Convert.ToString(dt.Rows[i]["ContactTelephoneNumber"] + "");
                            selectedTrackerLine.VehicleReg = Convert.ToString(dt.Rows[i]["VehicleReg"]);
                            selectedTrackerLine.VehicleSizeUsed = Convert.ToString(dt.Rows[i]["VehicleSizeUsed"] + "");
                            selectedTrackerLine.EtaOnSite = Convert.ToString(dt.Rows[i]["EtaOnSite"]);
                            selectedTrackerLine.ActualOnSite = Convert.ToString(dt.Rows[i]["ActualOnSite"] + "");
                            selectedTrackerLine.Comments = Convert.ToString(dt.Rows[i]["Comments"] + "");
                            selectedTrackerLine.CustomerSiteEmailAddress = Convert.ToString(dt.Rows[i]["CustomerSiteEmailAddress"]);
                            selectedTrackerLine.CustomerAccountEmailAddress = Convert.ToString(dt.Rows[i]["CustomerAccountEmailAddress"] + "");
                            selectedTrackerLine.OtherEmailAddress = Convert.ToString(dt.Rows[i]["OtherEmailAddress"]);
                            selectedTrackerLine.FTBDeliveryVehicle = Convert.ToString(dt.Rows[i]["FTBDeliveryVehicle"] + "");
                            selectedTrackerLine.DriverName = Convert.ToString(dt.Rows[i]["DriverName"] + "");
                            if (Convert.ToBoolean(dt.Rows[i]["NotesEmailed"]) == true)
                            { selectedTrackerLine.NotesEmailed = true; }
                            else { selectedTrackerLine.NotesEmailed = false; }
                            if (Convert.ToBoolean(dt.Rows[i]["OnTracker"]) == true)
                            { selectedTrackerLine.OnTracker = true; }
                            else { selectedTrackerLine.OnTracker = false; }
                            if (Convert.ToBoolean(dt.Rows[i]["OnTime"]) == true)
                            { selectedTrackerLine.OnTime = true; }
                            else { selectedTrackerLine.OnTime = false; }
                            if (Convert.ToBoolean(dt.Rows[i]["Active"]) == true)
                            { selectedTrackerLine.Active = true; }
                            else { selectedTrackerLine.Active = false; }
                            if (Convert.ToBoolean(dt.Rows[i]["Processed"]) == true)
                            { selectedTrackerLine.Processed = true; }
                            else { selectedTrackerLine.Processed = false; }
                            if (Convert.ToBoolean(dt.Rows[i]["Completed"]) == true)
                            { selectedTrackerLine.Completed = true; }
                            else { selectedTrackerLine.Completed = false; }
                            if (Convert.ToBoolean(dt.Rows[i]["Signed"]) == true)
                            { selectedTrackerLine.Signed = true; }
                            else { selectedTrackerLine.Signed = false; }

                            try { selectedTrackerLine.SignatureCopy = StrToByteArray(Convert.ToString(dt.Rows[i]["SignatureCopy"])); } catch { }


                        }
                    }
                           

                 
                }
            }
        }

        private void ApplyFilter()
        {
            Decimal filteredTotal = 0;
            DataGrid dg = GetVisibleDataGrid();

            if (dg.Name == "CreditStopReviewDataGrid")
            {

             
            }
            else if (dg.Name == "MaterialReviewDataGrid")
            {
               
            }
            else
            {
             
            }


            string filterText = SearchBox.Text.ToLower();

            dg.Items.Refresh();

            if (dg.ItemsSource != null)
            {
                ICollectionView view = CollectionViewSource.GetDefaultView(dg.ItemsSource);
                Predicate<object> yourCostumFilter = null;

                if (dg.Name == "MaterialReviewDataGrid")
                {
                   

                }
                else if (dg.Name == "CreditStopReviewDataGrid")
                {
                   

                }
                else
                {

                    var query1 = new Predicate<object>(x => ((IdslTrackerLine)x).ContractNumber.ToLower().Contains(filterText));
                    var query2 = new Predicate<object>(x => ((IdslTrackerLine)x).ContractName.ToLower().Contains(filterText));

                   
                    var query3 = new Predicate<object>(x => ((IdslTrackerLine)x).PostCode.ToLower().Contains(filterText));
                    var query4 = new Predicate<object>(x => ((IdslTrackerLine)x).ContactName.ToLower().Contains(filterText));
                    var query5 = new Predicate<object>(x => ((IdslTrackerLine)x).CustomerSiteEmailAddress.ToLower().Contains(filterText));
                    


                  





                    yourCostumFilter = new Predicate<object>(x =>
                        (query1(x) || query2(x) || query3(x) || query4(x) || query5(x)));

                    view.Filter = yourCostumFilter;
                    dg.ItemsSource = view;



                    List<IdslTrackerLine> lines = new List<IdslTrackerLine>();
                    try
                    {
                        lines = ((List<IdslTrackerLine>)dg.ItemsSource) as List<IdslTrackerLine>;

                    }
                    catch
                    {
                        lines = CollectionViewSource.GetDefaultView(dg.ItemsSource).Cast<IdslTrackerLine>().ToList();
                    }

                    //foreach (IdslTrackerLine line in lines)
                    //{
                    //    filteredTotal = filteredTotal + line.Sales;
                    //}
                }


             //   ProductionTrackerFilteredTotalTextBox.Text = string.Format(("{0:C}"), filteredTotal);
                

            }
            ////////////////////////////////////////

        }


        private void MainTabControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.Source is TabControl)
            {
                ApplyFilter();
            }
        }

        

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {

            DataGrid dg = GetVisibleDataGrid();
            RefreshDataGrid(dg);
        }





        private void BulkUpdateManufactureInfo_Click(object sender, RoutedEventArgs e)
        {
           
           
        }


        private void CopyContract_MenuItem_Click(object sender, RoutedEventArgs e)
        {
           
        }


        private void CopyBatchRef_MenuItem_Click(object sender, RoutedEventArgs e)
        {
           
        }

        private void CopyCustomer_MenuItem_Click(object sender, RoutedEventArgs e)
        {
         
        }

        private void CopyDocNumber_MenuItem_Click(object sender, RoutedEventArgs e)
        {
           
        }
        private void CopyMiscComments_MenuItem_Click(object sender, RoutedEventArgs e)
        {
         
        }
        private void CopyThirdPartyComments_MenuItem_Click(object sender, RoutedEventArgs e)
        {
          
        }
        private void CopyShopfloorComments_MenuItem_Click(object sender, RoutedEventArgs e)
        {
          
        }
        private void CopyIronmongeryComments_MenuItem_Click(object sender, RoutedEventArgs e)
        {
        
        }


        private void CopyJobNo_MenuItem_Click(object sender, RoutedEventArgs e)
        {
           
        }

        private void CopyProductionCommentsDoor_MenuItem_Click(object sender, RoutedEventArgs e)
        {
         
        }

        private void CopyProductionCommentsFrame_MenuItem_Click(object sender, RoutedEventArgs e)
        {
           
        }

        private void BulkUpdatePrinted_MenuItem_Click(object sender, RoutedEventArgs e)
        {
           



        }

       


        private void BulkUpdateMaterialComment_MenuItem_Click(object sender, RoutedEventArgs e)
        {
           
        }

        private void BulkUpdateManufactureComplete_MenuItem_Click(object sender, RoutedEventArgs e)
        {
           
        }

     


        private void BulkAddToAccruals_MenuItem_Click(object sender, RoutedEventArgs e)
        {
           
        }

        private void CustomerStatusComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ApplyFilter();
        }

      

    

        //private void CreaitStopReviewClearFiltersButton_Click(object sender, RoutedEventArgs e)
        //{
        //    DeliveryMonthComboBox.SelectedIndex = -1;
        //    CustomerStatusComboBox.SelectedIndex = -1;
        //}


        System.Windows.Threading.DispatcherTimer _selectingCreditStopReviewDataGridTimer;

        private void CreditStopReviewDataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
           
        }

        private void handleCreditStopReviewSelectingTimerTimeout(object sender, EventArgs e)
        {
            var timer = sender as System.Windows.Threading.DispatcherTimer;
            if (timer == null)
            {
                return;
            }

            decimal highlightedTotal = 0;
           



            timer.Stop();
        }

        private void MaterialRiskComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ApplyFilter();
        }

        System.Windows.Threading.DispatcherTimer _selectingMainTrackerDataGridTimer;

        private void MainTrackerDataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (_selectingMainTrackerDataGridTimer == null)
            {
                _selectingMainTrackerDataGridTimer = new System.Windows.Threading.DispatcherTimer();
                _selectingMainTrackerDataGridTimer.Interval = TimeSpan.FromMilliseconds(100);

                _selectingMainTrackerDataGridTimer.Tick += new EventHandler(this.handleSelectingMainTrackerDataGridTimerTimeout);
            }
            _selectingMainTrackerDataGridTimer.Stop();
            //_selectingTimer.Tag = (sender as TextBox).Text; 
            _selectingMainTrackerDataGridTimer.Start();
        }

        private void handleSelectingMainTrackerDataGridTimerTimeout(object sender, EventArgs e)
        {
            var timer = sender as System.Windows.Threading.DispatcherTimer;
            if (timer == null)
            {
                return;
            }

            decimal highlightedTotal = 0;

            foreach (IdslTrackerLine line in MainTrackerDataGrid.SelectedItems)
            {
               // highlightedTotal = highlightedTotal + line.Sales;
            }

           // ProductionTrackerHighlightedTotalTextBox.Text = string.Format(("{0:C}"), highlightedTotal);



            timer.Stop();
        }


        System.Windows.Threading.DispatcherTimer _selectingThirdPartyTrackerDataGridTimer;

        private void ThirdPartyTrackerDataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (_selectingThirdPartyTrackerDataGridTimer == null)
            {
                _selectingThirdPartyTrackerDataGridTimer = new System.Windows.Threading.DispatcherTimer();
                _selectingThirdPartyTrackerDataGridTimer.Interval = TimeSpan.FromMilliseconds(100);

                _selectingThirdPartyTrackerDataGridTimer.Tick += new EventHandler(this.handleSelectingThirdPartyTrackerDataGridTimerTimeout);
            }
            _selectingThirdPartyTrackerDataGridTimer.Stop();
            //_selectingTimer.Tag = (sender as TextBox).Text; 
            _selectingThirdPartyTrackerDataGridTimer.Start();
        }

        private void handleSelectingThirdPartyTrackerDataGridTimerTimeout(object sender, EventArgs e)
        {
            var timer = sender as System.Windows.Threading.DispatcherTimer;
            if (timer == null)
            {
                return;
            }

            decimal highlightedTotal = 0;

          



            timer.Stop();
        }

        System.Windows.Threading.DispatcherTimer _selectingMiscTrackerDataGridTimer;

        private void MiscTrackerDataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (_selectingMiscTrackerDataGridTimer == null)
            {
                _selectingMiscTrackerDataGridTimer = new System.Windows.Threading.DispatcherTimer();
                _selectingMiscTrackerDataGridTimer.Interval = TimeSpan.FromMilliseconds(100);

                _selectingMiscTrackerDataGridTimer.Tick += new EventHandler(this.handleSelectingMiskTrackerDataGridTimerTimeout);
            }
            _selectingMiscTrackerDataGridTimer.Stop();
            //_selectingTimer.Tag = (sender as TextBox).Text; 
            _selectingMiscTrackerDataGridTimer.Start();
        }

        private void handleSelectingMiskTrackerDataGridTimerTimeout(object sender, EventArgs e)
        {
            var timer = sender as System.Windows.Threading.DispatcherTimer;
            if (timer == null)
            {
                return;
            }

            decimal highlightedTotal = 0;

          



            timer.Stop();
        }

        System.Windows.Threading.DispatcherTimer _selectingIronTrackerDataGridTimer;

        private void IronTrackerDataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (_selectingIronTrackerDataGridTimer == null)
            {
                _selectingIronTrackerDataGridTimer = new System.Windows.Threading.DispatcherTimer();
                _selectingIronTrackerDataGridTimer.Interval = TimeSpan.FromMilliseconds(100);

                _selectingIronTrackerDataGridTimer.Tick += new EventHandler(this.handleSelectingIronTrackerDataGridTimerTimeout);
            }
            _selectingIronTrackerDataGridTimer.Stop();
            //_selectingTimer.Tag = (sender as TextBox).Text; 
            _selectingIronTrackerDataGridTimer.Start();
        }

        private void handleSelectingIronTrackerDataGridTimerTimeout(object sender, EventArgs e)
        {
            var timer = sender as System.Windows.Threading.DispatcherTimer;
            if (timer == null)
            {
                return;
            }

            decimal highlightedTotal = 0;

          


            timer.Stop();
        }

        System.Windows.Threading.DispatcherTimer _selectingVestingTrackerDataGridTimer;

        private void VestingTrackerDataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (_selectingVestingTrackerDataGridTimer == null)
            {
                _selectingVestingTrackerDataGridTimer = new System.Windows.Threading.DispatcherTimer();
                _selectingVestingTrackerDataGridTimer.Interval = TimeSpan.FromMilliseconds(100);

                _selectingVestingTrackerDataGridTimer.Tick += new EventHandler(this.handleSelectingVestingTrackerDataGridTimerTimeout);
            }
            _selectingVestingTrackerDataGridTimer.Stop();
            //_selectingTimer.Tag = (sender as TextBox).Text; 
            _selectingVestingTrackerDataGridTimer.Start();
        }

        private void handleSelectingVestingTrackerDataGridTimerTimeout(object sender, EventArgs e)
        {
            var timer = sender as System.Windows.Threading.DispatcherTimer;
            if (timer == null)
            {
                return;
            }

            decimal highlightedTotal = 0;

           



            timer.Stop();
        }

        private void UniversalDeliveryWeekComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ApplyFilter();
        }

        private void NinthCharacterComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ApplyFilter();

        }

        private void ManfRepComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ApplyFilter();
        }

        private void ManfCompleteComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ApplyFilter();
        }

        private void MainTrackerDataGrid_Sorting(object sender, DataGridSortingEventArgs e)
        {
            var dgSender = (DataGrid)sender;
            var cView = CollectionViewSource.GetDefaultView(dgSender.ItemsSource);

            //Alternate between ascending/descending if the same column is clicked 
            ListSortDirection direction = ListSortDirection.Ascending;
            if (cView.SortDescriptions.FirstOrDefault().PropertyName == e.Column.SortMemberPath)
                direction = cView.SortDescriptions.FirstOrDefault().Direction == ListSortDirection.Descending ? ListSortDirection.Ascending : ListSortDirection.Descending;

            cView.SortDescriptions.Clear();
            AddSortColumn((DataGrid)sender, e.Column.SortMemberPath, direction);
            //To this point the default sort functionality is implemented

            //Now check the wanted columns and add multiple sort 
            //if (e.Column.SortMemberPath == "WantedColumn")
            //{
            AddSortColumn((DataGrid)sender, "JobNo", direction);
            //}
            e.Handled = true;
        }

        private void AddSortColumn(DataGrid sender, string sortColumn, ListSortDirection direction)
        {
            var cView = CollectionViewSource.GetDefaultView(sender.ItemsSource);
            cView.SortDescriptions.Add(new SortDescription(sortColumn, direction));
            //Add the sort arrow on the DataGridColumn
            foreach (var col in sender.Columns.Where(x => x.SortMemberPath == sortColumn))
            {
                col.SortDirection = direction;
            }
        }

        private void CreditStopReviewDataGrid_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
          
        }

        private void BulkUpdateMaterialReveiwComments_MenuItem_Click(object sender, RoutedEventArgs e)
        {
           



        }

        private void SearchColourComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ApplyFilter();
        }

        private void ExportStockAndWip_Button_Click(object sender, RoutedEventArgs e)
        {
         
        }

        private void CreateCell(IRow CurrentRow, int CellIndex, string Value, XSSFCellStyle Style)
        {
            ICell Cell = CurrentRow.CreateCell(CellIndex);
            Cell.SetCellValue(Value);
            Cell.CellStyle = Style;
        }
    }

    internal class HexStyle
    {
        public HexStyle(XSSFCellStyle customCellStyle, string styleHex)
        {
            CustomCellStyle = customCellStyle;
            StyleHex = styleHex;
        }

        public XSSFCellStyle CustomCellStyle { get; }
        public string StyleHex { get; }
    }

    internal class ColourComboBoxItem
    {
        public string cbItemText { get; internal set; }
        public Brush cbItemColour { get; internal set; }
    }
}

internal class TrackerWipDetail
{
    public string Doc { get; internal set; }
    public string DoorWip { get; internal set; }
    public string FrameWip { get; internal set; }
}