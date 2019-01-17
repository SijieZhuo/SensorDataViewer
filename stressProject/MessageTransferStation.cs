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
        private MainWindow mw;
        public Tuple<double, SensorData[]> shimmerData;
        public Tuple<double, string[]> phoneData;
        public List<Tuple<double, SensorData[]>> shimmerDataList;
        public List<string[]> phoneDataList;
        public string RootDirectory = Directory.GetCurrentDirectory();

        private readonly BackgroundWorker worker = new BackgroundWorker();

        Stopwatch stopwatch;


        Guid guid = new Guid("6bfc8497-b445-406e-b639-a5abaf4d9739");


        public static MessageTransferStation instance;


        public MessageTransferStation()
        {
            _messageText = string.Empty;
            mw = (MainWindow)Application.Current.MainWindow;
            shimmerDataList = new List<Tuple<double, SensorData[]>>();

            stopwatch = new Stopwatch();
            stopwatch.Start();
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

        public Tuple<double, SensorData[]> SData
        {
            get { return shimmerData; }
            set
            {
                shimmerData = value;
                OnPropertyChanged("SData");
                SensorData[] data = shimmerData.Item2;
                //mw.updateShimmerChart(_data);
                mw.timeChart.updateShimmerChart(shimmerData.Item1,data[0].Data);
                shimmerDataList.Add(shimmerData);

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
                mw.pChart.updateShimmerChart(phoneData.Item1, Int32.Parse(data[0]));
                mw.AccX.Content = "Acc X: " + data[1];
                mw.AccX.Content = "Acc Y: " + data[2];
                mw.AccX.Content = "Acc Z: " + data[3];
                mw.AccX.Content = "Rot X: " + data[4];
                mw.AccX.Content = "Rot Y: " + data[5];
                mw.AccX.Content = "Rot Z: " + data[6];
                mw.AccX.Content = "Gra X: " + data[7];
                mw.AccX.Content = "Gra Y: " + data[8];
                mw.AccX.Content = "Gra Z: " + data[9];
                //shimmerDataList.Add(shimmerData);

            }

        }



        // Create the OnPropertyChanged method to raise the event
        protected void OnPropertyChanged(string name)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));

        }


        public void writeShimmerData(string fileName)
        {
            var records = new List<object>();
            StreamWriter sw = new StreamWriter("Records\\"+fileName+".csv");
            var csv = new CsvWriter(sw);

            worker.DoWork += worker_DoWork;
            worker.RunWorkerCompleted += worker_RunWorkerCompleted;

            worker.RunWorkerAsync();

            foreach (Tuple<double, SensorData[]> tuple in shimmerDataList)
            {
                dynamic record = new ExpandoObject();
                 record.Time = tuple.Item1;
                record.GSR = tuple.Item2[0].Data;
                //records.Clear();
                //records.Add(record);
                OutputSdata s = new OutputSdata(tuple.Item1, tuple.Item2[0].Data);
                //Tuple<double, double> d = new Tuple<double, double>(tuple.Item2.TotalSeconds,tuple.Item1.Data);
                
                records.Add(s);
                //Debug.WriteLine(d);
                //csv.WriteRecords(records);
                


            }
            Debug.WriteLine(records.Count());
            //csv.WriteRecords(records);

            Thread thread = new Thread(() => Write(csv, records));
            thread.Start();

            Debug.WriteLine("done");
        }

        private void Write(CsvWriter csv, dynamic records) {
            csv.WriteRecords(records);
        }


        private void worker_DoWork(object sender, DoWorkEventArgs e)
        {
            // run all background tasks here
        }

        private void worker_RunWorkerCompleted(object sender,
                                               RunWorkerCompletedEventArgs e)
        {
            //update ui once worker complete his work
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




        private class OutputSdata {
            public double Time { get; set; }
            public double Data { get; set; }

            public OutputSdata(double t, double d) {
                Time = t;
                Data = d;
            }
        }










        public Guid GetGuid()
        {
            return guid;
        }


        public double getTime()
        {
            return stopwatch.Elapsed.TotalSeconds;
        }

    }
}
