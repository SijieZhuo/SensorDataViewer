using System;
using System.Collections.Generic;
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
        public static AutoResetEvent autoResetEvent = new AutoResetEvent(false);

        private string _messageText;
        private MainWindow mw;

        public static MessageTransferStation instance;


        public MessageTransferStation()
        {
            _messageText = string.Empty;
            mw = (MainWindow)Application.Current.MainWindow;
        }

        public string MessageText
        {
            get { return _messageText; }
            set
            {
                _messageText = value;
                Debug.WriteLine("changing value");
                OnPropertyChanged("MessageText");

                mw.updateTextBox();

            }

        }

        // Create the OnPropertyChanged method to raise the event
        protected void OnPropertyChanged(string name)
        {
            Debug.WriteLine("onpropertychanged called");
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
