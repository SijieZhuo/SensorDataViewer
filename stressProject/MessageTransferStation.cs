using ShimmerAPI;
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

namespace stressProject
{
    class MessageTransferStation : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private string _messageText;
        private string _systemText;
        private MainWindow mw;

        public Tuple<double, SensorData[]> shimmerData;
        public List<Tuple<double, SensorData[]>> shimmerDataList;

        public Tuple<double, string[]> phoneData;
        public List<Tuple<double, string[]>> phoneDataList;

        public Tuple<double, string[]> phoneTouchData;
        public List<Tuple<double, string[]>> phoneTouchDataList;



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
            _systemText = string.Empty;
            mw = (MainWindow)Application.Current.MainWindow;
            shimmerDataList = new List<Tuple<double, SensorData[]>>();
            phoneDataList = new List<Tuple<double, string[]>>();
            phoneTouchDataList = new List<Tuple<double, string[]>>();

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

        public string SystemText
        {
            get { return _systemText; }
            set
            {
                _systemText = value;
                OnPropertyChanged("SystemText");

                mw.updateSystem();

            }

        }

        public Tuple<double, SensorData[]> SData
        {
            get { return shimmerData; }
            set
            {
                shimmerData = value;
                OnPropertyChanged("SData");
                SensorData[] data = shimmerData.Item2;
                //mw.updateShimmerChart(_data);
                //mw.timeChart.updateShimmerChart(shimmerData.Item1, data[0].Data);
                chart.Read(data[0].Data);
                if (recording)
                {
                    shimmerDataList.Add(shimmerData);
                }


            }

        }

        public Tuple<double, string[]> PData
        {
            get { return phoneData; }
            set
            {
                phoneData = value;
                OnPropertyChanged("PData");
                string[] data = phoneData.Item2;
                //mw.updateShimmerChart(_data);
                mw.pChart.updateShimmerChart(phoneData.Item1, int.Parse(data[0]));
                mw.AccX.Content = "Acc X: " + data[1];
                mw.AccY.Content = "Acc Y: " + data[2];
                mw.AccZ.Content = "Acc Z: " + data[3];
                mw.RotX.Content = "Rot X: " + data[4];
                mw.RotY.Content = "Rot Y: " + data[5];
                mw.RotZ.Content = "Rot Z: " + data[6];
                mw.GraX.Content = "Gra X: " + data[7];
                mw.GraY.Content = "Gra Y: " + data[8];
                mw.GraZ.Content = "Gra Z: " + data[9];
                mw.currentApp.Content = "Current App: " + data[10];
                mw.ScreenStatus.Content = "Screen Status: " + data[11];
                mw.Data_download.Content = "Data Download: " + data[12];
                mw.Data_upload.Content = "Data Upload: " + data[13];

                if (recording)
                {
                    phoneDataList.Add(phoneData);
                }
            }

        }

        public Tuple<double, string[]> PTData
        {
            get { return phoneTouchData; }
            set
            {
                phoneTouchData = value;
                OnPropertyChanged("PTData");
                string[] data = phoneData.Item2;
                //mw.updateShimmerChart(_data);

                if (recording)
                {
                    phoneTouchDataList.Add(phoneTouchData);
                }
            }

        }




        // Create the OnPropertyChanged method to raise the event
        protected void OnPropertyChanged(string name)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));

        }


        public void WriteShimmerData(string folderName)
        {
            var records = new List<object>();
            StreamWriter sw = new StreamWriter("Records\\" + folderName + "Shimmer.csv");
            var csv = new CsvWriter(sw);

            foreach (Tuple<double, SensorData[]> tuple in shimmerDataList)
            {

                double time = tuple.Item1 + GetStartTime();
                string recordedTime = new DateTime(0001, 1, 1, 0, 0, 0).AddSeconds(time).ToString("yyyy MM dd HH:mm:ss.fff");

                OutputSdata s = new OutputSdata(recordedTime, tuple.Item2[0].Data,
                    tuple.Item2[1].Data, tuple.Item2[2].Data, tuple.Item2[3].Data, tuple.Item2[4].Data);

                records.Add(s);


            }
            Debug.WriteLine(records.Count());

            Thread thread = new Thread(() => Write(csv, records));
            thread.Start();
        }


        public void WritePhoneData(string fileName)
        {
            var records = new List<object>();
            StreamWriter sw = new StreamWriter("Records\\" + fileName + "\\Phone.csv");
            var csv = new CsvWriter(sw);


            foreach (Tuple<double, string[]> tuple in phoneDataList)
            {

                double time = tuple.Item1 + GetStartTime();
                string recordedTime = new DateTime(0001, 1, 1, 0, 0, 0).AddSeconds(time).ToString("yyyy MM dd HH:mm:ss.fff");

                OutputPdata p = new OutputPdata(recordedTime, int.Parse(tuple.Item2[0]),
                    double.Parse(tuple.Item2[1]), double.Parse(tuple.Item2[2])
                    , double.Parse(tuple.Item2[3]), double.Parse(tuple.Item2[4])
                    , double.Parse(tuple.Item2[5]), double.Parse(tuple.Item2[6])
                    , double.Parse(tuple.Item2[7]), double.Parse(tuple.Item2[8])
                    , double.Parse(tuple.Item2[9]), tuple.Item2[10]
                    , tuple.Item2[11], double.Parse(tuple.Item2[12])
                    , double.Parse(tuple.Item2[13]));

                records.Add(p);


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

        public void WriteData(string name) {
            if (shimmerDataList.Count()>0) {
                WriteShimmerData(name);
            }
            if (phoneDataList.Count()>0) {
                WritePhoneData(name);
            }
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


        public void startRecording()
        {
            recording = true;
        }

        public void stopRecording()
        {
            recording = false;
        }

    }
}
