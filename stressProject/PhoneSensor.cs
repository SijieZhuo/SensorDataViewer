﻿using InTheHand.Net.Sockets;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace stressProject
{
    class PhoneSensor
    {

        BluetoothListener BTListener;
        Guid guid;
        private MessageTransferStation mts;
        bool connected;

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
            BluetoothClient client = BTListener.AcceptBluetoothClient();

            connected = true;
            Stream mStream = client.GetStream();
            Debug.WriteLine(client.Connected + "1");
            while (connected)
            {
                ReceivingData(mStream);               
            }
        }

        private void ReceivingData(Stream mStream) {
            try
            {
                byte[] received = new byte[1024];
                mStream.Read(received, 0, received.Length);
                char[] charArray = Encoding.UTF8.GetString(received).ToCharArray();
                string s = Encoding.UTF8.GetString(received, 0, received.Length);

                string[] phoneData = s.Split(',');


                Debug.WriteLine("received : " + s);
                Debug.WriteLine("");
            }
            catch (IOException e)
            {
                connected = false;

            }
        }


    }
}
