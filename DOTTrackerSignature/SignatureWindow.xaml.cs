using IdslTracker.Classes;
using NPOI.SS.Formula.Functions;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Ink;
using System.Windows.Input;
using static NPOI.SS.Formula.Functions.LinearRegressionFunction;

namespace IdslTracker
{
    /// <summary>
    /// Interaction logic for DetailViewWindow.xaml
    /// </summary>
    public partial class SignatureWindow : Window
    {

        IdslTrackerLine trackerLine;
        public string ConnectionString = Properties.Resources.db;

        public SignatureWindow(IdslTrackerLine selectedTrackerLine, string dataGridName)
        {
            InitializeComponent();

            this.trackerLine = selectedTrackerLine;
            this.DataContext = this.trackerLine;
            Title = String.Format("IDSL | DOT Tracker | Edit | {0} | {1} | {2}", trackerLine.ContractName, trackerLine.ContractNumber, trackerLine.VehicleSizeUsed);
                    
          

         

        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            string currentUser = System.Security.Principal.WindowsIdentity.GetCurrent().Name;
            //if (Globals.IsPowerUser == true)
            //{
            //    ClearPrintedDateBtn.Visibility = Visibility.Visible;
            //}

            PopulateExistingSignature();
      
         

        }

        void PopulateExistingSignature()
        {
           
            try
            {
                byte[] decryptedBytes = SignatureClass.DownloadImage(ConnectionString, Convert.ToString(trackerLine.id));

                BinaryFormatter bf = new BinaryFormatter();
                MemoryStream ms = new MemoryStream(decryptedBytes);
                MyCustomStrokes customStrokes = bf.Deserialize(ms) as MyCustomStrokes;
                for (int i = 0; i < customStrokes.StrokeCollection.Length; i++)
                {
                    if (customStrokes.StrokeCollection[i] != null)
                    {
                        StylusPointCollection stylusCollection = new
                          StylusPointCollection(customStrokes.StrokeCollection[i]);
                        Stroke stroke = new Stroke(stylusCollection);
                        StrokeCollection strokes = new StrokeCollection();
                        strokes.Add(stroke);
                        this.MyInkCanvas.Strokes.Add(strokes);
                    }
                }

            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show(ex.Message);
            }
        }

        void UploadButton_Click(object sender, RoutedEventArgs e)
        {
            if(MyInkCanvas.Strokes.Count == 0)
            {
                MessageBox.Show("Signature Window is Empty.");
            }
            else
            {
                this.UploadStrokes(this.MyInkCanvas.Strokes);
                this.DialogResult = true;
                this.Close();
            }

           
        }



        void ClearButton_Click(object sender, RoutedEventArgs e)
        {
            this.MyInkCanvas.Strokes.Clear();
        }

        void UploadStrokes(StrokeCollection strokes)
        {
            if (strokes.Count > 0)
            {
                MyCustomStrokes customStrokes = new MyCustomStrokes();

                customStrokes.StrokeCollection = new Point[strokes.Count][];

                for (int i = 0; i < strokes.Count; i++)
                {
                    customStrokes.StrokeCollection[i] = new Point[this.MyInkCanvas.Strokes[i].StylusPoints.Count];

                    for (int j = 0; j < strokes[i].StylusPoints.Count; j++)
                    {
                        customStrokes.StrokeCollection[i][j] = new Point();
                        customStrokes.StrokeCollection[i][j].X = strokes[i].StylusPoints[j].X;
                        customStrokes.StrokeCollection[i][j].Y = strokes[i].StylusPoints[j].Y;
                    }
                }

                //Serialize
                MemoryStream ms = new MemoryStream();
                BinaryFormatter bf = new BinaryFormatter();
                bf.Serialize(ms, customStrokes);

                try
                {
                    SignatureClass.UploadImage(ConnectionString, ms.GetBuffer(),Convert.ToString( trackerLine.id));
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }

            }
        }


    }
    [Serializable]
    public sealed class MyCustomStrokes
    {
        public MyCustomStrokes() { }

        public Point[][] StrokeCollection;
    }

}
