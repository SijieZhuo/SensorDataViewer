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

        private string _messageQueue;
        private MainWindow mw;

        public Queue<string> testingQ = new Queue<string>();

        public static MessageTransferStation instance;


        public MessageTransferStation()
        {
            _messageQueue = string.Empty;
            mw = (MainWindow)Application.Current.MainWindow;
        }

        public string MessageQueue
        {
            get { return _messageQueue; }
            set
            {
                _messageQueue = value;
                Debug.WriteLine("changing value");
                OnPropertyChanged("MessageQueue");
                
                mw.updateTextBox();
               // autoResetEvent.WaitOne();
                //updateTextBox();
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

        public void eventSet() {
            autoResetEvent.Set();
        }


    }
}
