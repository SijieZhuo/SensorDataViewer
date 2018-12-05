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
        private MessageTransferStation mts;
        Stopwatch stopwatch;



        public ShimmerSensor(string BTaddress)
        {
            address = BTaddress;
            mts = MessageTransferStation.Instance;
            stopwatch = new Stopwatch();

        }


        public void setup()
        {

            int enabledSensors = ((int)ShimmerBluetooth.SensorBitmapShimmer3.SENSOR_GSR) | (int)ShimmerBluetooth.SensorBitmapShimmer3.SENSOR_A_ACCEL;

            //shimmer = new Shimmer32Feet("ShimmerID1", "00:06:66:66:96:86");
            //devName,bluetoothAddress, samplingRate, accelRange, gsrRange, setEnabledSensors, enableLowPowerAccel, enableLowPowerGyro, enableLowPowerMag, gyroRange, magRange, exg1configuration, exg2configuration, internalexppower
            shimmer = new ShimmerLogAndStream32Feet("ShimmerID1", address, 51.2, 0, ShimmerBluetooth.GSR_RANGE_AUTO, enabledSensors, true, false, false, 1, 0, Shimmer3Configuration.EXG_EMG_CONFIGURATION_CHIP1, Shimmer3Configuration.EXG_EMG_CONFIGURATION_CHIP2, true);

            shimmer.UICallback += this.HandleEvent;

            shimmer.Connect();

            if (shimmer.GetState() == ShimmerBluetooth.SHIMMER_STATE_CONNECTED)
            {

                shimmer.WriteSensors(enabledSensors);
                updateMessage("Shimmer device is connected");
               
                shimmer.StartStreaming();
                stopwatch.Start();


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
                        updateMessage("device connected");
                    }
                    else if (state == (int)ShimmerBluetooth.SHIMMER_STATE_CONNECTING)
                    {
                        updateMessage("connecting shimmer device");
                    }
                    else if (state == (int)ShimmerBluetooth.SHIMMER_STATE_NONE)
                    {
                        updateMessage("device disconnected");
                    }
                    else if (state == (int)ShimmerBluetooth.SHIMMER_STATE_STREAMING)
                    {
                        updateMessage("Streaming");
                    }
                    break;
                case (int)ShimmerBluetooth.ShimmerIdentifier.MSG_IDENTIFIER_NOTIFICATION_MESSAGE:
                    break;
                case (int)ShimmerBluetooth.ShimmerIdentifier.MSG_IDENTIFIER_DATA_PACKET:
                    ObjectCluster objectCluster = (ObjectCluster)eventArgs.getObject();
                    SensorData data = objectCluster.GetData("GSR", "CAL");
                   // SensorData time = objectCluster.GetData("Timestamp","CAL");
                    TimeSpan time = stopwatch.Elapsed;
                    //Debug.WriteLine(time.GetType());

                    Tuple<SensorData, TimeSpan> dataTuple = new Tuple<SensorData, TimeSpan>(data,time);
                    //objectCluster.GetNames

                    

                    //updateMessage("AccelX: " + data.Data);
                    updateData(dataTuple);

                    break;
            }
        }

        private void updateMessage(string message)
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                mts.MessageText = message;

            });
        }


        private void updateData(Tuple<SensorData,TimeSpan> data)
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                mts.Data = data;
            });
        }

        public void disconnect() {
            shimmer.Disconnect();
        }


    }
}
