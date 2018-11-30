using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ShimmerAPI;
using System.Windows;
using System.Threading;
using System.Diagnostics;

namespace stressProject
{
    class ShimmerSensor
    {
        ShimmerLogAndStream32Feet shimmer;

        private string address;
        string message;
        private MessageTransferStation mts;

        Queue<string> testing;

        readonly object listLock = new object();


        public ShimmerSensor(string BTaddress)
        {

            address = BTaddress;
            Debug.WriteLine("created");
            message = string.Empty;
            mts = MessageTransferStation.Instance;
            testing = new Queue<string>();

        }


        public void setup()
        {
            Debug.WriteLine("created");

            int enabledSensors = ((int)ShimmerBluetooth.SensorBitmapShimmer3.SENSOR_A_ACCEL);

            //shimmer = new Shimmer32Feet("ShimmerID1", "00:06:66:66:96:86");
            shimmer = new ShimmerLogAndStream32Feet("ShimmerID1", address, 102.4, 0, ShimmerBluetooth.GSR_RANGE_AUTO, enabledSensors, false, false, false, 1, 0, Shimmer3Configuration.EXG_EMG_CONFIGURATION_CHIP1, Shimmer3Configuration.EXG_EMG_CONFIGURATION_CHIP2, true);

            shimmer.UICallback += this.HandleEvent;
            Debug.WriteLine("sensor1 "+Thread.CurrentThread.ManagedThreadId);

            shimmer.Connect();
            Debug.WriteLine("sensor2 " + Thread.CurrentThread.ManagedThreadId);
            Debug.WriteLine("shimmer.Connect() complete");
            if (shimmer.GetState() == ShimmerBluetooth.SHIMMER_STATE_CONNECTED)
            {

                // mw.updateTextBox("shimmer connected");
                Debug.WriteLine("shimmer connected");
                //TransferMassage("shimmer connected");
                shimmer.WriteSensors(enabledSensors);
                shimmer.StartStreaming();

            }
        }

        public void HandleEvent(object sender, EventArgs args)
        {
            CustomEventArgs eventArgs = (CustomEventArgs)args;
            int indicator = eventArgs.getIndicator();

            switch (indicator)
            {
                case (int)ShimmerBluetooth.ShimmerIdentifier.MSG_IDENTIFIER_STATE_CHANGE:
                    int state = (int)eventArgs.getObject();
                    if (state == (int)ShimmerBluetooth.SHIMMER_STATE_CONNECTED)
                    {
                        //TransferMassage("the shimmer is connected");
                        message = "the shimmer is connected";
                        Debug.WriteLine(message);
                        Debug.WriteLine("sensor message" + Thread.CurrentThread.ManagedThreadId);
                        //mts.MessageQueue = "the shimmer is connected";

                    }
                    else if (state == (int)ShimmerBluetooth.SHIMMER_STATE_CONNECTING)
                    {
                        //TransferMassage("Connecting");
                        message = "Connecting";
                        Debug.WriteLine(message);

                        //mts.MessageQueue = "the shimmer is Connecting";

                    }
                    else if (state == (int)ShimmerBluetooth.SHIMMER_STATE_NONE)
                    {
                        //TransferMassage("Disconnected");
                        message = "disconnected";
                        Debug.WriteLine(message);

                        // mts.MessageQueue = "the shimmer is disconnected";

                    }
                    else if (state == (int)ShimmerBluetooth.SHIMMER_STATE_STREAMING)
                    {
                        //TransferMassage("Streaming");
                        message = "Streaming";
                        Debug.WriteLine(message);

                        //mts.MessageQueue = "the shimmer is Streaming";


                    }
                    break;
                case (int)ShimmerBluetooth.ShimmerIdentifier.MSG_IDENTIFIER_NOTIFICATION_MESSAGE:
                    break;
                case (int)ShimmerBluetooth.ShimmerIdentifier.MSG_IDENTIFIER_DATA_PACKET:
                    ObjectCluster objectCluster = (ObjectCluster)eventArgs.getObject();
                    SensorData data = objectCluster.GetData("Low Noise Accelerometer X", "CAL");
                    //mw.updateTextBox("AccelX: " + data.Data);
                    Debug.WriteLine("AccelX: " + data.Data);
                    //  mts.MessageQueue = "AccelX: " + data.Data;
                    Debug.WriteLine("sensor data" + Thread.CurrentThread.ManagedThreadId);
                    Application.Current.Dispatcher.Invoke(() =>
                    {
                        mts.MessageQueue = "AccelX: " + data.Data;

                    });


                  


                    break;
            }
        }



      
    }
}
