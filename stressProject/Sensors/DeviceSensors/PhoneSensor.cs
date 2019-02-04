using InTheHand.Net.Sockets;
using stressProject.OutputData;
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
        public BluetoothClient client;

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
            updateConnection();
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
                string s = Encoding.UTF8.GetString(received, 0, received.Length);

                string[] phoneData = s.Split(',');
                string time = mts.DoubleToTimeString(mts.GetTime() - startTime);
                if (phoneData.Count() == 14)
                {
                    PhoneData pData = new PhoneData(time, int.Parse(phoneData[0]), double.Parse(phoneData[1]),
                        double.Parse(phoneData[2]), double.Parse(phoneData[3]), double.Parse(phoneData[4]), 
                        double.Parse(phoneData[5]), double.Parse(phoneData[6]), double.Parse(phoneData[7]),
                        double.Parse(phoneData[8]), double.Parse(phoneData[9]), phoneData[10],
                        phoneData[11], double.Parse(phoneData[12]), double.Parse(phoneData[13]));
                    updateData(pData);
                }
                else if (phoneData.Count() == 7)
                {
                    //updateTouchData(new Tuple<double, string[]>(mts.GetTime() - startTime, phoneData));

                    TouchData data = new TouchData(phoneData[0], phoneData[1], phoneData[2], phoneData[3], phoneData[4], phoneData[5], phoneData[6]);
                    UpdateTouchData(data);
                    Debug.WriteLine("touch: " + s);
                }




                //Debug.WriteLine("received : " + s);
            }
            catch (IOException e)
            {
                connected = false;

            }
        }


        private void updateData(PhoneData data)
        {
            Application.Current.Dispatcher.Invoke(() =>
             {
                 mts.PData = data;
             });
        }


        private void UpdateTouchData(TouchData data)
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                mts.PTData = data;
            });
        }



        public void disconnect()
        {
            connected = false;
            client.Close();
        }


        private void updateConnection() {
            Application.Current.Dispatcher.Invoke(() =>
            {
                mts.mw.phoneBTBtn.IsEnabled = true;
            });
        }

    }
}
