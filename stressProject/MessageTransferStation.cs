﻿using ShimmerAPI;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using CsvHelper;
using System.Dynamic;
using System.IO;
using stressProject.OutputData;

namespace stressProject
{
    class MessageTransferStation : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private string _messageText;

        private MainWindow mw;

        public ShimmerData shimmerData;
        public List<ShimmerData> shimmerDataList;

        public PhoneData phoneData;
        public List<PhoneData> phoneDataList;

        public TouchData phoneTouchData;
        public List<TouchData> phoneTouchDataList;

        private SystemLogData systemLogdata;
        public List<SystemLogData> systemLogdataList;



        public string RootDirectory = Directory.GetCurrentDirectory();
        Guid guid = new Guid("6bfc8497-b445-406e-b639-a5abaf4d9739");
        double startTime;

        private bool recording = false;

        Stopwatch stopwatch;

        RTChart chart;



        public static MessageTransferStation instance;


        public MessageTransferStation()
        {
            _messageText = string.Empty;
            systemLogdata = null;
            mw = (MainWindow)Application.Current.MainWindow;
            shimmerDataList = new List<ShimmerData>();
            phoneDataList = new List<PhoneData>();
            phoneTouchDataList = new List<TouchData>();
            systemLogdataList = new List<SystemLogData>();

            stopwatch = new Stopwatch();
            stopwatch.Start();
            startTime = new TimeSpan(DateTime.Now.Ticks).TotalSeconds;

            chart = mw.GetRTChart();
        }

        public string MessageText
        {
            get { return _messageText; }
            set
            {
                _messageText = value;
                OnPropertyChanged("MessageText");

                mw.updateTextBox();

            }

        }

        public SystemLogData SystemLogData
        {
            get { return systemLogdata; }
            set
            {
                systemLogdata = value;
                OnPropertyChanged("SystemLogData");

                systemLogdataList.Add(systemLogdata);
                Debug.WriteLine(value.ToString());
                mw.updateSystem();

            }

        }

        //============== Data Properties ====================//

        public ShimmerData SData
        {
            get { return shimmerData; }
            set
            {
                shimmerData = value;
                OnPropertyChanged("SData");

                chart.Read(shimmerData.GSR);
                if (recording)
                {
                    shimmerDataList.Add(shimmerData);
                }


            }

        }

        public PhoneData PData
        {
            get { return phoneData; }
            set
            {
                phoneData = value;
                OnPropertyChanged("PData");
                //mw.updateShimmerChart(_data);
                mw.pChart.updateShimmerChart(GetTime(), phoneData.Sound);
                mw.AccX.Content = "Acc X: " + phoneData.AccX;
                mw.AccY.Content = "Acc Y: " + phoneData.AccY;
                mw.AccZ.Content = "Acc Z: " + phoneData.AccZ;
                mw.RotX.Content = "Rot X: " + phoneData.RotX;
                mw.RotY.Content = "Rot Y: " + phoneData.RotY;
                mw.RotZ.Content = "Rot Z: " + phoneData.RotZ;
                mw.GraX.Content = "Gra X: " + phoneData.GraX;
                mw.GraY.Content = "Gra Y: " + phoneData.GraY;
                mw.GraZ.Content = "Gra Z: " + phoneData.GraZ;
                mw.currentApp.Content = "Current App: " + phoneData.CurrentApp;
                mw.ScreenStatus.Content = "Screen Status: " + phoneData.ScreenStatus;
                mw.Data_download.Content = "Data Download: " + phoneData.Downloads;
                mw.Data_upload.Content = "Data Upload: " + phoneData.Uploads;

                if (recording)
                {
                    phoneDataList.Add(phoneData);
                }
            }

        }

        public TouchData PTData
        {
            get { return phoneTouchData; }
            set
            {
                phoneTouchData = value;
                OnPropertyChanged("PTData");
                //string[] data = phoneData.Item2;
                //mw.updateShimmerChart(_data);

                if (recording)
                {
                    phoneTouchDataList.Add(phoneTouchData);
                }
            }

        }



        //=================== Write Data =======================//

        public void WriteShimmerData(string folderName)
        {
            var records = new List<object>();
            StreamWriter sw = new StreamWriter("Records\\" + folderName + "\\Shimmer.csv");
            var csv = new CsvWriter(sw);

            foreach (ShimmerData data in shimmerDataList)
            {
                //Debug.WriteLine(data.Time);
                records.Add(data);
            }
            Debug.WriteLine(records.Count());

            Debug.WriteLine(((ShimmerData)records.ElementAt(0)).Time);
            Thread thread = new Thread(() => Write(csv, records));
            thread.Start();
        }

        public void WritePhoneData(string fileName)
        {
            var records = new List<object>();
            StreamWriter sw = new StreamWriter("Records\\" + fileName + "\\Phone.csv");
            var csv = new CsvWriter(sw);


            foreach (PhoneData data in phoneDataList)
            {


                records.Add(data);


            }
            Debug.WriteLine(records.Count());

            Thread thread = new Thread(() => Write(csv, records));
            thread.Start();
        }

        public void WriteTouchData(string folderName)
        {
            var records = new List<object>();
            StreamWriter sw = new StreamWriter("Records\\" + folderName + "\\Touch.csv");
            var csv = new CsvWriter(sw);

            foreach (TouchData data in phoneTouchDataList)
            {
                //Debug.WriteLine(data.Time);
                records.Add(data);
            }
            Debug.WriteLine(records.Count());

            Thread thread = new Thread(() => Write(csv, records));
            thread.Start();
        }

        private void Write(CsvWriter csv, dynamic records)
        {
            csv.WriteRecords(records);
            _messageText = "record completed";
            Debug.WriteLine("record completed");
        }

        public void WriteData(string name)
        {
            if (shimmerDataList.Count() > 0)
            {
                WriteShimmerData(name);
            }
            if (phoneDataList.Count() > 0)
            {
                WritePhoneData(name);
            }
            if (phoneTouchDataList.Count() > 0)
            {
                WriteTouchData(name);
            }
        }




        //====================== Output Data Type ====================//

        private class OutputSdata
        {
            public string Time { get; set; }
            public double GSR { get; set; }
            public double PPG { get; set; }
            public double AccX { get; set; }
            public double AccY { get; set; }
            public double AccZ { get; set; }

            public OutputSdata(string t, double d, double p, double x, double y, double z)
            {
                Time = t;
                GSR = d;
                PPG = p;
                AccX = x;
                AccY = y;
                AccZ = z;
            }
        }

        private class OutputPdata
        {

            public string Time { get; set; }
            public int Sound { get; set; }
            public double AccX { get; set; }
            public double AccY { get; set; }
            public double AccZ { get; set; }
            public double RotX { get; set; }
            public double RotY { get; set; }
            public double RotZ { get; set; }
            public double GraX { get; set; }
            public double GraY { get; set; }
            public double GraZ { get; set; }
            public string CurrentApp { get; set; }
            public string ScreenStatus { get; set; }
            public double Downloads { get; set; }
            public double Uploads { get; set; }

            public OutputPdata(string time, int sound, double accX, double accY, double accZ, double rotX, double rotY, double rotZ, double graX, double graY, double graZ, string currentApp, string screenStatus, double downloads, double uploads)
            {
                Time = time;
                Sound = sound;
                AccX = accX;
                AccY = accY;
                AccZ = accZ;
                RotX = rotX;
                RotY = rotY;
                RotZ = rotZ;
                GraX = graX;
                GraY = graY;
                GraZ = graZ;
                CurrentApp = currentApp;
                ScreenStatus = screenStatus;
                Downloads = downloads;
                Uploads = uploads;
            }
        }






        //================== Getters =================//

        public Guid GetGuid()
        {
            return guid;
        }

        public double GetTime()
        {
            return stopwatch.Elapsed.TotalSeconds;
        }

        public double GetStartTime()
        {
            return startTime;
        }

        public List<SystemLogData> GetSystemLogDatas()
        {
            return systemLogdataList;
        }

        //====================== Recording ====================//

        public void startRecording()
        {
            recording = true;
        }

        public void stopRecording()
        {
            recording = false;
        }


        /// <summary>
        /// this method would convert a time in seconds into a string with the format
        /// "yyyy//MM//dd HH:mm:ss.fff"
        /// the input value should be the time in seconds form the process start
        /// </summary>
        /// <param name="time"></param>
        /// <returns></returns>
        public string DoubleToTimeString(double time)
        {
            double totalTime = time + GetStartTime();
            return new DateTime(0001, 1, 1, 0, 0, 0).AddSeconds(totalTime).ToString("yyyy/MM/dd_HH:mm:ss.fff");
        }


        public static MessageTransferStation Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new MessageTransferStation();
                }
                return instance;
            }
        }

        // Create the OnPropertyChanged method to raise the event
        protected void OnPropertyChanged(string name)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));

        }


    }
}
