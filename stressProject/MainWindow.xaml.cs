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
using Microsoft.VisualBasic;



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


        bool shimmerOn = false;
        bool phoneOn = false;

        ShimmerSensor sensor;
        PhoneSensor phoneSensor;
        RealTimeChart shimmerChart;
        public TimeChart timeChart;
        public TimeChart pChart;





        public TimeChart GetTimeChart()
        {
            return timeChart;
        }





        public MainWindow()
        {
            InitializeComponent();

            shimmerBtn.IsEnabled = false;

            mts = MessageTransferStation.Instance;
            //shimmerChart = new RealTimeChart();
            // RealTimechartSetup(shimmerChart);

            //sp.Children.Add(shimmerChart);

            timeChart = new TimeChart(500, 300, "Shimmer", "GSR", 0, 2000, 200);
            sp.Children.Add(timeChart.GetTimeChart());

            pChart = new TimeChart(500, 300, "sound", "db", 0, 150, 15);
            sp2.Children.Add(pChart.GetTimeChart());




            Directory.CreateDirectory(mts.RootDirectory + "\\Records");




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
                    shimmerBtn.Content = "Diconnect";

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
                    //sp.Children.Remove(shimmerChart);
                    timeChart = new TimeChart(500, 300, "Shimmer", "GSR", 0, 2000, 200);
                    //shimmerChart = new RealTimeChart();
                    //RealTimechartSetup(shimmerChart);
                    sp.Children.Add(timeChart.GetTimeChart());
                }
            }
        }



        private void recordBtn_Click(object sender, RoutedEventArgs e)
        {
            //string s = Interaction.InputBox("Question?", "Title", "Default Text");
            //Debug.WriteLine(s);
            //mts.writeShimmerData();
            TabItem selected = (TabItem)tabControl.SelectedItem;
            Debug.WriteLine(selected.Header);

            RecordPopup popup = new RecordPopup(selected.Header.ToString());
            popup.Show();




        }

        

        private void phoneBTBtn_Click(object sender, RoutedEventArgs e)
        {
            if (phoneOn)
            {
                phoneSensor.disconnect();
                phoneBTBtn.Content = "Connect";
               
            }
            else
            {
                phoneSensor = new PhoneSensor();
                new Thread(phoneSensor.PhoneConnection).Start();
                phoneBTBtn.Content = "Disconnect";
            }
            phoneOn = !phoneOn;

        }




    }

}
