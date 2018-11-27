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




namespace stressProject
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        Dictionary<string, string> BTmap = new Dictionary<string, string>();

        public MainWindow()
        {
            InitializeComponent();

            //BTmap = checkBTConnection();




        }

        /*for (int i = 0; i < 5; i++)
        {
            System.Windows.Controls.Button newBtn = new Button();

            newBtn.Content = i.ToString();
            newBtn.Name = "Button" + i.ToString();

            sp.Children.Add(newBtn);
        }*/

        public Dictionary<string, string> checkBTConnection()
        {
            BluetoothClient client = new BluetoothClient();


            BluetoothDeviceInfo[] devices = client.DiscoverDevicesInRange();

            foreach (BluetoothDeviceInfo d in devices)
            {
                string address = d.DeviceAddress.ToString();
                address = Regex.Replace(address, ".{2}", "$0:");
                address = address.Remove(address.Length - 1);

                BTmap.Add(d.DeviceName, address);


                //textBox.AppendText(d.DeviceAddress + "");
                //textBox.Text.(d.DeviceAddress + "");

                //


            }

            return BTmap;
        }




        protected void button_Click(object sender, RoutedEventArgs e)
        {
            Button button = sender as Button;
            textBox.Text = BTmap[button.Content.ToString()];
        }

        private void BTSearchBtn_Click(object sender, RoutedEventArgs e)
        {

            BTmap = checkBTConnection();


            int i = 0;
            foreach (KeyValuePair<string, string> entry in BTmap)
            {
                Button newBtn = new Button();

                newBtn.Content = entry.Key;
                newBtn.Name = "Button" + i.ToString();
                i++;
                sp.Children.Add(newBtn);
                newBtn.Click += new RoutedEventHandler(button_Click);
            }

        }
    }

}
