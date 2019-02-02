using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace stressProject.OutputData
{
    class ChromeData
    {
        public string Time { get; set; }
        public int NumTabs { get; set; }
        public string URL { get; set; }
        public string TabName { get; set; }
        public string TabTitle { get; set; }
        public int TabID { get; set; }

        public ChromeData(string time, int numTabs, string uRL, string tabName, string tabTitle, int tabID)
        {
            Time = time;
            NumTabs = numTabs;
            URL = uRL;
            TabName = tabName;
            TabTitle = tabTitle;
            TabID = tabID;
        }
    }
}
