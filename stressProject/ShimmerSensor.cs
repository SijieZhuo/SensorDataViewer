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
        private double startTime;


        int IndexAccelX;
        int IndexAccelY;
        int IndexAccelZ;
        int IndexGSR;
        int IndexPPG;
        bool FirstTime;



        public ShimmerSensor(string BTaddress)
        {
            address = BTaddress;
            mts = MessageTransferStation.Instance;

        }


        public void setup()
        {
            int enabledSensors = ((int)ShimmerBluetooth.SensorBitmapShimmer3.SENSOR_GSR) | (int)ShimmerBluetooth.SensorBitmapShimmer3.SENSOR_A_ACCEL | (int)ShimmerBluetooth.SensorBitmapShimmer3.SENSOR_INT_A13;

            //devName,bluetoothAddress, samplingRate, accelRange, gsrRange, setEnabledSensors, enableLowPowerAccel, enableLowPowerGyro, enableLowPowerMag, gyroRange, magRange, exg1configuration, exg2configuration, internalexppower
            shimmer = new ShimmerLogAndStream32Feet("ShimmerID1", address, 10, 0, ShimmerBluetooth.GSR_RANGE_AUTO, enabledSensors, true, false, false, 1, 0, Shimmer3Configuration.EXG_EMG_CONFIGURATION_CHIP1, Shimmer3Configuration.EXG_EMG_CONFIGURATION_CHIP2, true);
            shimmer.UICallback += this.HandleEvent;
            shimmer.Connect();
            FirstTime = true;
            if (shimmer.GetState() == ShimmerBluetooth.SHIMMER_STATE_CONNECTED)
            {
                shimmer.WriteSensors(enabledSensors);
                updateMessage("Shimmer device is connected");

                shimmer.StartStreaming();
                startTime = mts.GetTime();
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

                    if (FirstTime)
                    {
                        IndexAccelX = objectCluster.GetIndex(Shimmer3Configuration.SignalNames.LOW_NOISE_ACCELEROMETER_X, ShimmerConfiguration.SignalFormats.CAL);
                        IndexAccelY = objectCluster.GetIndex(Shimmer3Configuration.SignalNames.LOW_NOISE_ACCELEROMETER_Y, ShimmerConfiguration.SignalFormats.CAL);
                        IndexAccelZ = objectCluster.GetIndex(Shimmer3Configuration.SignalNames.LOW_NOISE_ACCELEROMETER_Z, ShimmerConfiguration.SignalFormats.CAL);
                        IndexGSR = objectCluster.GetIndex(Shimmer3Configuration.SignalNames.GSR, ShimmerConfiguration.SignalFormats.CAL);
                        IndexPPG = objectCluster.GetIndex(Shimmer3Configuration.SignalNames.INTERNAL_ADC_A13, ShimmerConfiguration.SignalFormats.CAL);
                        FirstTime = false;
                    }

                   // List<string> s= objectCluster.GetNames();
                   // foreach (string st in s) {
                  //      Debug.WriteLine(st);
                  //  }
                    SensorData datax = objectCluster.GetData(IndexAccelX);
                    SensorData datay = objectCluster.GetData(IndexAccelY);
                    SensorData dataz = objectCluster.GetData(IndexAccelZ);
                    SensorData dataGSR = objectCluster.GetData(IndexGSR);
                    SensorData dataPPG = objectCluster.GetData(IndexPPG);
                    //TimeSpan time = stopwatch.Elapsed;

                    //double time = DateTime.Now.ToOADate();
                    double time = mts.GetTime() - startTime;

                    SensorData[] data = { dataGSR, dataPPG, datax, datay, dataz };
                    //Debug.WriteLine(time.GetType());

                    Tuple<double, SensorData[]> dataTuple = new Tuple<double, SensorData[]>(time, data);
                    //objectCluster.GetNames



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


        private void updateData(Tuple<double, SensorData[]> data)
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                mts.SData = data;
            });
        }

        public void disconnect()
        {
            shimmer.Disconnect();
        }


    }
}
