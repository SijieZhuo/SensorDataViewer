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
        public Tuple<SensorData, TimeSpan> _data;
        public List<Tuple<SensorData, TimeSpan>> shimmerData;


        public static MessageTransferStation instance;


        public MessageTransferStation()
        {
            _messageText = string.Empty;
            mw = (MainWindow)Application.Current.MainWindow;
            shimmerData = new List<Tuple<SensorData, TimeSpan>>();
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

        public Tuple<SensorData, TimeSpan> Data
        {
            get { return _data; }
            set
            {
                _data = value;
                OnPropertyChanged("Data");

                mw.updateShimmerChart(_data);
                shimmerData.Add(_data);

            }

        }



        // Create the OnPropertyChanged method to raise the event
        protected void OnPropertyChanged(string name)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));

        }


        public void writeShimmerData()
        {
            var records = new List<object>();
            StreamWriter sw = new StreamWriter("output.csv");
            var csv = new CsvWriter(sw);
            foreach (Tuple<SensorData, TimeSpan> tuple in shimmerData)
            {
                dynamic record = new ExpandoObject();
                 record.Time = tuple.Item2.TotalSeconds;
                record.GSR = tuple.Item1.Data;
                //records.Clear();
                //records.Add(record);
                Sdata s = new Sdata(tuple.Item2.TotalSeconds, tuple.Item1.Data);
                //Tuple<double, double> d = new Tuple<double, double>(tuple.Item2.TotalSeconds,tuple.Item1.Data);
                
                records.Add(s);
                //Debug.WriteLine(d);
                //csv.WriteRecords(records);


            }
            Debug.WriteLine(records.Count());
            csv.WriteRecords(records);



            Debug.WriteLine("done");
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




        private class Sdata {
            public double Time { get; set; }
            public double Data { get; set; }

            public Sdata(double t, double d) {
                Time = t;
                Data = d;
            }
        }
    }
}
