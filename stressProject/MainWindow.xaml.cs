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
using InTheHand.Net;
using System.IO;

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


        bool shimmerOn = false;
        ShimmerSensor sensor;
        RealTimeChart shimmerChart;

        BluetoothListener BTListener;

        Guid guid = new Guid("6bfc8497-b445-406e-b639-a5abaf4d9739");

        Series series1 = null;


        public MainWindow()
        {
            InitializeComponent();

            shimmerBtn.IsEnabled = false;

            mts = MessageTransferStation.Instance;
            shimmerChart = new RealTimeChart();
            RealTimechartSetup(shimmerChart);

            sp.Children.Add(shimmerChart);








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

        public void updateTextBox()
        {
            Debug.WriteLine(mts.MessageText);

            textBox.Text = mts.MessageText;

        }


        private void RealTimechartSetup(RealTimeChart chart)
        {
            chart.Width = 500;
            chart.Height = 300;
            chart.FastScrollMode = true;
            chart.TitleText = "Shimmer";

            chart.TooltipVisibility = Visibility.Visible;
            chart.ShowFallbackTooltip = true;

            chart.XAxis.Title = "Time";
            chart.XAxis.PinLabels = false;
            chart.XAxis.Length = 10;
            chart.XAxis.Interval = 1;


            Axis yAxis = new Axis();
            yAxis.Origin = 0;
            yAxis.Length = 2000;
            yAxis.Interval = 200;
            yAxis.Title = "GSR";
            yAxis.TitleRotationAngle = -90.0;
            yAxis.TitleFontFamily = new FontFamily("Verdana");
            yAxis.LabelFontFamily = new FontFamily("Verdana");
            yAxis.TickLength = 5;
            yAxis.TitleOffset = 10;
            chart.YAxisCollection.Add(yAxis);


            series1 = new Series(chart.YAxisCollection[0])
            {
                Stroke = new SolidColorBrush(Color.FromRgb(166, 46, 68)),
                Title = "GSR",
                ScatterType = ScatterType.None,
                TitleFontFamily = new FontFamily("Verdana"),
                TitleFontSize = 12

            };

            chart.SeriesCollection.Add(series1);

            chart.TooltipAxis = chart.YAxisCollection[0];

        }

        private void DisposeOldData(Series series)
        {


            if (series.Data.Count > 500 &&
                series.Data[499].X < shimmerChart.XAxis.Origin)
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

            if (!shimmerOn)
            {
                shimmerOn = true;
                shimmerBtn.IsEnabled = false;
                if (checkShimmer(comboBox.SelectedItem.ToString()))
                {
                    mts.MessageText = "pairing device";
                    Debug.WriteLine("mainwindow" + Thread.CurrentThread.ManagedThreadId);

                    sensor = new ShimmerSensor(BTmap[comboBox.SelectedItem.ToString()]);
                    Debug.WriteLine("object created");
                    new Thread(sensor.setup).Start();
                    shimmerBtn.IsEnabled = true;
                    shimmerBtn.Content = "Dicconnect";

                }
                else
                {
                    mts.MessageText = "the device you trying to connect is not a shimmer sensor";
                }
            }
            else
            {
                shimmerOn = false;
                if (sensor != null)
                {
                    sensor.disconnect();
                    shimmerBtn.Content = "Connect";
                    sp.Children.Remove(shimmerChart);
                    shimmerChart = new RealTimeChart();
                    RealTimechartSetup(shimmerChart);
                    sp.Children.Add(shimmerChart);
                }
            }
        }

        public void updateShimmerChart(Tuple<SensorData, TimeSpan> data)
        {
            shimmerChart.FastScrollMode = true;
            Point[] points1 = new Point[clusterSize];

            double minNewX = currCount - 2000;

            points1[0] = new Point(data.Item2.TotalSeconds, data.Item1.Data);
            currCount++;
            Debug.WriteLine(currCount + "  " + data.Item2.TotalSeconds);
            series1.Data.AddRange(points1);

            shimmerChart.Commit();

            DisposeOldData(series1);



        }



        private void recordBtn_Click(object sender, RoutedEventArgs e)
        {
            mts.writeShimmerData();
        }

        private void phoneBTBtn_Click(object sender, RoutedEventArgs e)
        {
            /*Debug.WriteLine(BTmap[comboBox.SelectedItem.ToString()]);
            BluetoothAddress address = BluetoothAddress.Parse("786256C7CF9E");
            //uuid : 6bfc8497-b445-406e-b639-a5abaf4d9739
            var client = new BluetoothClient();
            var btEndPoint = new BluetoothEndPoint(address, guid);
            client.Connect(btEndPoint);
            Debug.WriteLine(client.Connected);*/

            new Thread(ListeningBT).Start();

        }


        private void ListeningBT()
        {
            BTListener = new BluetoothListener(guid);
            BTListener.Start();
            BluetoothClient client = BTListener.AcceptBluetoothClient();

            bool connected = true;
            Stream mStream = client.GetStream();
            Debug.WriteLine(client.Connected + "1");
            while (connected)
            {
                //Debug.WriteLine(client.Connected);
                try
                {
                    //Debug.WriteLine("waiting");
                    byte[] received = new byte[1024];
                    mStream.Read(received, 0, received.Length);
                    char[] charArray = Encoding.UTF8.GetString(received).ToCharArray();
                    string s = Encoding.UTF8.GetString(received, 0, received.Length);
                    StringBuilder receivedString = new StringBuilder();
                    foreach (byte b in received) {
                        receivedString.AppendFormat("{0}",received);
                    }
                    Debug.WriteLine("received : " + s);

                    //updateUI("Received: " + receivedString);
                    //handleBluetoothInput(receivedString);
                    //byte[] send = Encoding.ASCII.GetBytes("Hello world");
                    //mStream.Write(send, 0, send.Length);
                }
                catch (IOException e)
                {
                    connected = false;
                    //updateUI("Client disconnected");
                    //disconnectBluetooth();
                }
            }
        }


    }

}
