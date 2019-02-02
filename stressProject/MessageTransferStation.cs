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

        public SystemLogData systemLogdata;
        public List<SystemLogData> systemLogdataList;

        public ChromeData chromeData;
        public List<ChromeData> chromeDataList;




        public string RootDirectory = Directory.GetCurrentDirectory();
        Guid guid = new Guid("6bfc8497-b445-406e-b639-a5abaf4d9739");
        double startTime;

        private bool recording = false;

        Stopwatch stopwatch;

        RTChart chart1, chart2, chart3;



        public static MessageTransferStation instance;


        public MessageTransferStation()
        {
            _messageText = string.Empty;
            mw = (MainWindow)Application.Current.MainWindow;

            shimmerDataList = new List<ShimmerData>();
            phoneDataList = new List<PhoneData>();
            phoneTouchDataList = new List<TouchData>();
            systemLogdataList = new List<SystemLogData>();
            chromeDataList = new List<ChromeData>();

            stopwatch = new Stopwatch();
            stopwatch.Start();
            startTime = new TimeSpan(DateTime.Now.Ticks).TotalSeconds;

            chart1 = mw.GetRTChart1();
            chart2 = mw.GetRTChart2();
            chart3 = mw.GetRTChart3();
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



        //============== Data Properties ====================//

        public ShimmerData SData
        {
            get { return shimmerData; }
            set
            {
                shimmerData = value;
                OnPropertyChanged("SData");

                chart1.Read(shimmerData.GSR);

                chart2.Read(shimmerData.PPG);

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
                //mw.pChart.updateShimmerChart(GetTime(), phoneData.Sound);
                chart3.Read(phoneData.Sound);

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

        public SystemLogData SystemLogData
        {
            get { return systemLogdata; }
            set
            {
                systemLogdata = value;
                OnPropertyChanged("SystemLogData");

                mw.UpdateSystem();
                if (recording)
                {
                    systemLogdataList.Add(systemLogdata);
                    Debug.WriteLine("systemlog wrote");
                }

            }

        }

        public ChromeData ChromeData
        {
            get { return chromeData; }
            set
            {
                chromeData = value;
                OnPropertyChanged("ChromeData");

                mw.UpdateChrome();
                Debug.WriteLine("chrome data: " + chromeData);
                if (recording)
                {
                    chromeDataList.Add(chromeData);
                    Debug.WriteLine("chrome wrote");
                }
            }

        }


        //=================== Write Data =======================//

        private void Write<T>(string folderName, string name, List<T> list) {
            using (var writer = new StreamWriter("Records\\" + folderName + "\\"+ name +".csv"))
            using (var csv = new CsvWriter(writer))
            {
                csv.WriteRecords(list);
            }
        }

        public void WriteData(string name)
        {
            if (shimmerDataList.Count() > 0)
            {
                Write(name, "Shimmer", shimmerDataList);
            }
            if (phoneDataList.Count() > 0)
            {
                Write(name, "Phone", phoneDataList);
            }
            if (phoneTouchDataList.Count() > 0)
            {
                Write(name, "Touch", phoneTouchDataList);
            }
            if (systemLogdataList.Count() > 0)
            {
                Write(name, "SystemLog", systemLogdataList);
            }
            if (chromeDataList.Count() > 0)
            {
                Write(name, "Chrome", chromeDataList);
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
