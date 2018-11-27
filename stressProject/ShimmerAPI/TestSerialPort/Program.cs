﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static ShimmerAPI.ShimmerBluetooth;

namespace TestSerialPort
{
    class Program
    {
        protected static double LastReceivedTimeStamp = 0;
        protected static double CurrentTimeStampCycle = 0;
        protected static double LastReceivedCalibratedTimeStamp = -1;
        protected static double TimeStampPacketRawMaxValue = 16777216;// 16777216 or 65536 
        protected static bool FirstTimeCalTime = true;
        protected static double CalTimeStart = 0;
        protected static double SamplingRate = 512;
        protected static double lastKnownTS =0;
        static void Main(string[] args)
        {


            System.IO.Ports.SerialPort SerialPort = new System.IO.Ports.SerialPort();
            int packetSize = 23; //LN Accel + EXG TEST SIGNAL Using L&S0.11
            SerialPort.BaudRate = 115200;
            SerialPort.PortName = "COM11";
            SerialPort.ReadTimeout = 2000;
            SerialPort.WriteTimeout = 2000;
            SerialPort.ReadBufferSize = 2147483647;
            System.Console.WriteLine(SerialPort.ReadBufferSize);
            try
            {
                SerialPort.Open();
            }
            catch
            {

            }

            SerialPort.Write(new byte[1] { (byte)PacketTypeShimmer2.START_STREAMING_COMMAND }, 0, 1);
            SerialPort.ReadByte();

            for (int i = 0; i < 1000000; i++)
            {
                int[] dataTS = new int[3];
                for (int j = 0; j < (packetSize + 1); j++) {
                    try
                    {
                        int b = SerialPort.ReadByte();
                        
                        if (j == 1)
                        {
                            dataTS[0] = b;
                        }
                        if (j == 2)
                        {
                            dataTS[1] = b;
                        }
                        if (j == 3)
                        {
                            dataTS[2] = b;
                        }
                        if (i%SamplingRate==0)
                        System.Console.Write(j + "," + b + ",");
                    } catch 
                    {
                        
                        System.Console.WriteLine(j + " TimeoutException");
                        break;
                    }
                }
                double parsedts = parseTimeStamps(dataTS);
                double calibratedts = CalibrateTimeStamp(parsedts);
                if (i % SamplingRate == 0)
                    System.Console.Write(calibratedts + "," + (calibratedts-lastKnownTS) +"," + SerialPort.BytesToRead);
                lastKnownTS = calibratedts;
                if (i % SamplingRate == 0)
                    System.Console.WriteLine();
            }
        }

        public static double parseTimeStamps(int[] data)
        {
            long xmsb = ((long)(data[0 + 2] & 0xFF) << 16);
            long msb = ((long)(data[0 + 1] & 0xFF) << 8);
            long lsb = ((long)(data[0 + 0] & 0xFF));
            double parsedts = xmsb + msb + lsb;
            return parsedts;
        }

        protected static double CalibrateTimeStamp(double timeStamp)
        {
            //first convert to continuous time stamp
            double calibratedTimeStamp = 0;
            if (LastReceivedTimeStamp > (timeStamp + (TimeStampPacketRawMaxValue * CurrentTimeStampCycle)))
            {
                CurrentTimeStampCycle = CurrentTimeStampCycle + 1;
            }

            LastReceivedTimeStamp = (timeStamp + (TimeStampPacketRawMaxValue * CurrentTimeStampCycle));
            calibratedTimeStamp = LastReceivedTimeStamp / 32768 * 1000;   // to convert into mS
            if (FirstTimeCalTime)
            {
                FirstTimeCalTime = false;
                CalTimeStart = calibratedTimeStamp;
            }
            if (LastReceivedCalibratedTimeStamp != -1)
            {
                double timeDifference = calibratedTimeStamp - LastReceivedCalibratedTimeStamp;
                double clockConstant = 32768;
              
                double expectedTimeDifference = (1 / SamplingRate) * 1000; //in ms
                double adjustedETD = expectedTimeDifference + (expectedTimeDifference * 0.1);
                
            }
                        
            LastReceivedCalibratedTimeStamp = calibratedTimeStamp;
            return calibratedTimeStamp - CalTimeStart; // make it start at zero
        }
    }
}
