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

namespace stressProject
{
    class MessageTransferStation : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private string _messageText;
        private MainWindow mw;
        public ObservableCollection<Tuple<SensorData, TimeSpan>> shimmerData { get; set; }
        public Tuple<SensorData, TimeSpan> _data;


        public static MessageTransferStation instance;


        public MessageTransferStation()
        {
            _messageText = string.Empty;
            mw = (MainWindow)Application.Current.MainWindow;
            shimmerData = new ObservableCollection<Tuple<SensorData, TimeSpan>>();
            shimmerData.CollectionChanged += OnListChanged;
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

        public Tuple<SensorData,TimeSpan> Data
        {
            get { return _data; }
            set
            {
                _data = value;
                OnPropertyChanged("Data");

                mw.updateShimmerChart(_data);

            }

        }



        // Create the OnPropertyChanged method to raise the event
        protected void OnPropertyChanged(string name)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));

        }


        private void OnListChanged(object sender, NotifyCollectionChangedEventArgs args)
        {
            //Debug.WriteLine(sender.GetType());
            ObservableCollection<Tuple<SensorData, TimeSpan>> datalist = sender as ObservableCollection<Tuple<SensorData, TimeSpan>>;
            //Debug.WriteLine(datalist.Last().Item1.Data);
            mw.updateShimmerChart(datalist.Last());
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
    }
}
