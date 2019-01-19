using InTheHand.Net.Sockets;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace stressProject
{
    class PhoneSensor
    {

        BluetoothListener BTListener;
        Guid guid;
        private MessageTransferStation mts;
        bool connected;
        BluetoothClient client;

        private double startTime;

        public PhoneSensor()
        {
            mts = MessageTransferStation.Instance;

            guid = mts.GetGuid();
            connected = false;

        }


        public void PhoneConnection()
        {
            BTListener = new BluetoothListener(guid);
            BTListener.Start();
             client = BTListener.AcceptBluetoothClient();

            connected = true;
            Stream mStream = client.GetStream();
            startTime = mts.GetTime();
            while (connected)
            {
                ReceivingData(mStream);
            }
        }

        private void ReceivingData(Stream mStream)
        {
            try
            {
                byte[] received = new byte[1024];
                mStream.Read(received, 0, received.Length);
                char[] charArray = Encoding.UTF8.GetString(received).ToCharArray();
                string s = Encoding.UTF8.GetString(received, 0, received.Length);

                string[] phoneData = s.Split(',');
                if (phoneData.Count() == 14)
                {
                    updateData(new Tuple<double, string[]>(mts.GetTime()- startTime, phoneData));
                }
                else if (phoneData.Count() == 7)
                {
                    updateTouchData(new Tuple<double, string[]>(mts.GetTime()-startTime, phoneData));
                    Debug.WriteLine("touch: " + s);
                }




                //Debug.WriteLine("received : " + s);
                Debug.WriteLine("");
            }
            catch (IOException e)
            {
                connected = false;

            }
        }


        private void updateData(Tuple<double, string[]> data)
        {
            Application.Current.Dispatcher.Invoke(() =>
             {
                 mts.PData = data;
             });
        }


        private void updateTouchData(Tuple<double, string[]> data)
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                mts.PTData = data;
            });
        }



        public void disconnect() {
            connected = false;
            client.Close();
        }


    }
}
