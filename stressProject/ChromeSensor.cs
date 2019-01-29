using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace stressProject
{
    class ChromeSensor
    {

        private static HttpListener _listener;

        public ChromeSensor() {

            _listener = new HttpListener();
            _listener.Prefixes.Add("http://localhost:60024/");
            _listener.Start();
            _listener.BeginGetContext(new AsyncCallback(ChromeSensor.ProcessRequest), null);

        }

        static void ProcessRequest(IAsyncResult result)
        {
            HttpListenerContext context = _listener.EndGetContext(result);
            HttpListenerRequest request = context.Request;

            //Answer getCommand/get post data/do whatever

            _listener.BeginGetContext(new AsyncCallback(ChromeSensor.ProcessRequest), null);

            string postData;
            using (var reader = new StreamReader(request.InputStream, request.ContentEncoding))
            {
                postData = reader.ReadToEnd();
                byte[] array = Convert.FromBase64String(postData);
                string s = Encoding.Default.GetString(array);

                string[] chromeData = s.Split(',');


                Debug.WriteLine(s);
            }

        }

    }
}
