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

        public Tuple<SensorData,TimeSpan> Data
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
