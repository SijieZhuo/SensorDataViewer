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

namespace stressProject
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        Dictionary<string, string> BTmap = new Dictionary<string, string>();

        private MessageTransferStation mts;





        public MainWindow()
        {
            InitializeComponent();

            mts = MessageTransferStation.Instance; 

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

        protected void BTbtn_Click(object sender, RoutedEventArgs e)
        {
            Button button = sender as Button;
            textBox.Text = BTmap[button.Content.ToString()];
            if (checkShimmer(button.Content.ToString()))
            {
                textBox.Text = "pairing device";
                Debug.WriteLine("pairing device");
                Debug.WriteLine("mainwindow"+Thread.CurrentThread.ManagedThreadId);

                ShimmerSensor sensor = new ShimmerSensor(BTmap[button.Content.ToString()]);
                Debug.WriteLine("object created");
                //textBox.Text = "setting up " + name;
                new Thread(sensor.setup).Start();
                
               

            }
            else
            {
                textBox.Text = "the device you trying to connect is not a shimmer sensor";
            }

        }

        private void BTSearchBtn_Click(object sender, RoutedEventArgs e)
        {
            sp.Children.Clear();

            Button button = sender as Button;

            //textBox.Text = "searching for bluetooth device";
            Debug.WriteLine("searching for bt device");
            mts.MessageQueue = "searching for bluetooth device";
            
            Debug.WriteLine(mts.MessageQueue.LongCount());
            BackgroundWorker bw = new BackgroundWorker();
            bw.DoWork += new DoWorkEventHandler(bw_BT_DoWork);
            bw.RunWorkerCompleted += new RunWorkerCompletedEventHandler(bw_BT_RunWorkerCompleted);

            bw.RunWorkerAsync();

        }


        private bool checkShimmer(string name)
        {
            bool shimmerOrNot = name.Contains("Shimmer");
            return shimmerOrNot;
        }


        public void updateTextBox()
        {
            Debug.WriteLine(mts.MessageQueue);
            
            textBox.Text = mts.MessageQueue;

        }


        private void bw_BT_DoWork(object sender, DoWorkEventArgs e)
        {
            checkBTConnection();
        }

        private void bw_BT_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {

            textBox.Text = "searching completed";

            int i = 0;
            foreach (KeyValuePair<string, string> entry in BTmap)
            {
                Button newBtn = new Button();

                newBtn.Content = entry.Key;
                newBtn.Name = "Button" + i.ToString();
                i++;
                sp.Children.Add(newBtn);
                newBtn.Click += new RoutedEventHandler(BTbtn_Click);
            }
        }

 

        


    }

}
