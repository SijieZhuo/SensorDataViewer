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
using System.Windows.Navigation;
using System.Windows.Shapes;
using InTheHand.Net.Sockets;
using ShimmerAPI;
using Windows.Devices.Bluetooth;
using Windows.Devices.Enumeration;
using System.Text.RegularExpressions;
using System.Threading;
using System.ComponentModel;
using System.Diagnostics;



using MindFusion.RealTimeCharting.Wpf;
using System.Windows.Threading;

namespace stressProject
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        Dictionary<string, string> BTmap = new Dictionary<string, string>();

        private MessageTransferStation mts;



        int clusterSize = 1;
        int currCount = 0;


        Series series1 = null;




        public MainWindow()
        {
            InitializeComponent();

            shimmerBtn.IsEnabled = false;

            mts = MessageTransferStation.Instance;
            chartSetup();

        }

        private void BTSearchBtn_Click(object sender, RoutedEventArgs e)
        {
            comboBox.Items.Clear();

            Button button = sender as Button;

            Debug.WriteLine("searching for bt device");
            mts.MessageText = "searching for bluetooth device";

            Debug.WriteLine(mts.MessageText.LongCount());
            BackgroundWorker bw = new BackgroundWorker();
            bw.DoWork += new DoWorkEventHandler(bw_BT_DoWork);
            bw.RunWorkerCompleted += new RunWorkerCompletedEventHandler(bw_BT_RunWorkerCompleted);

            bw.RunWorkerAsync();

        }

        public void checkBTConnection()
        {
            BTmap = new Dictionary<string, string>();

            BluetoothClient client = new BluetoothClient();

            BluetoothDeviceInfo[] devices = client.DiscoverDevicesInRange();

            foreach (BluetoothDeviceInfo d in devices)
            {
                string address = d.DeviceAddress.ToString();
                address = Regex.Replace(address, ".{2}", "$0:");
                address = address.Remove(address.Length - 1);

                BTmap.Add(d.DeviceName, address);

            }

        }

        private void bw_BT_DoWork(object sender, DoWorkEventArgs e)
        {
            checkBTConnection();
        }

        private void bw_BT_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            mts.MessageText = "searching completed";

            foreach (KeyValuePair<string, string> entry in BTmap)
            {
                comboBox.Items.Add(entry.Key);
            }
            comboBox.SelectedIndex = 0;
            shimmerBtn.IsEnabled = true;
        }




        private void chartSetup()
        {
            chart1.FastScrollMode = true;

            chart1.TooltipVisibility = Visibility.Visible;
            chart1.ShowFallbackTooltip = true;

            chart1.XAxis.Title = "Time";
            chart1.XAxis.PinLabels = false;
            chart1.XAxis.Length = 10;
            chart1.XAxis.Interval = 1;


            Axis yAxis = new Axis();
            yAxis.Origin = 0;
            yAxis.Length = 2000;
            yAxis.Interval = 200;
            yAxis.Title = "Server1";
            yAxis.TitleRotationAngle = -90.0;
            yAxis.TitleFontFamily = new FontFamily("Verdana");
            yAxis.LabelFontFamily = new FontFamily("Verdana");
            yAxis.TickLength = 5;
            yAxis.TitleOffset = 10;
            chart1.YAxisCollection.Add(yAxis);


            series1 = new Series(chart1.YAxisCollection[0])
            {
                Stroke = new SolidColorBrush(Color.FromRgb(166, 46, 68)),
                Title = "Server 1",
                ScatterType = ScatterType.None,
                TitleFontFamily = new FontFamily("Verdana"),
                TitleFontSize = 12

            };

            chart1.SeriesCollection.Add(series1);

            chart1.TooltipAxis = chart1.YAxisCollection[0];

        }

        private void DisposeOldData(Series series)
        {
            if (series.Data.Count > 500 &&
                series.Data[499].X < chart1.XAxis.Origin)
                series.Data.RemoveRange(0, 500);
        }





        private bool checkShimmer(string name)
        {
            bool shimmerOrNot = name.Contains("Shimmer");
            return shimmerOrNot;
        }

        private void Shimmer_Click(object sender, RoutedEventArgs e)
        {
            Debug.WriteLine(comboBox.SelectedItem.ToString());

            if (checkShimmer(comboBox.SelectedItem.ToString()))
            {
                mts.MessageText = "pairing device";
                Debug.WriteLine("mainwindow" + Thread.CurrentThread.ManagedThreadId);

                ShimmerSensor sensor = new ShimmerSensor(BTmap[comboBox.SelectedItem.ToString()]);
                Debug.WriteLine("object created");
                new Thread(sensor.setup).Start();

            }
            else
            {
                mts.MessageText = "the device you trying to connect is not a shimmer sensor";
            }
        }

        public void updateShimmerChart(Tuple<SensorData, TimeSpan> data)
        {
            chart1.FastScrollMode = true;
            Point[] points1 = new Point[clusterSize];

            double minNewX = currCount - 2000;

            points1[0] = new Point(data.Item2.TotalSeconds, data.Item1.Data);
            currCount++;
            Debug.WriteLine(currCount + "  " + data.Item2.TotalSeconds);
            series1.Data.AddRange(points1);

            chart1.Commit();

            DisposeOldData(series1);



        }

        public void updateTextBox()
        {
            Debug.WriteLine(mts.MessageText);

            textBox.Text = mts.MessageText;

        }


    }

}
