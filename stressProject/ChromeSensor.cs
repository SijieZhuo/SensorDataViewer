using stressProject.OutputData;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace stressProject
{
    class ChromeSensor
    {

        private static HttpListener _listener;

        private static MessageTransferStation mts;


        public ChromeSensor()
        {

            _listener = new HttpListener();
            _listener.Prefixes.Add("http://localhost:12000/");
            _listener.Start();
            _listener.BeginGetContext(new AsyncCallback(ChromeSensor.ProcessRequest), null);

            mts = MessageTransferStation.Instance;
        }

        static void ProcessRequest(IAsyncResult result)
        {
            HttpListenerContext context = _listener.EndGetContext(result);
            HttpListenerRequest request = context.Request;

            //Answer getCommand/get post data/do whatever

           // string postData;
            //using (var reader = new StreamReader(request.InputStream, request.ContentEncoding))
            //{
           //     postData = reader.ReadToEnd();
           //     byte[] array = Convert.FromBase64String(postData);
           //     string s = Encoding.Default.GetString(array);
//
           //     string[] chromeData = s.Split(',');
           //     Debug.WriteLine("chrome count: " + chromeData.Count());
           //     ChromeData data = new ChromeData(chromeData[0], int.Parse(chromeData[1]),
            //                       chromeData[2], chromeData[3], chromeData[4], chromeData[5]);

                //UpdateData(data);

                Debug.WriteLine("write");
           // }
        
            _listener.BeginGetContext(new AsyncCallback(ChromeSensor.ProcessRequest), null);

            


        }

        private static void UpdateData(ChromeData data)
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                mts.ChromeData = data;
            });
        }

    }
}
