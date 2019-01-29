using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace stressProject.OutputData
{
       class SystemLogData
    {
        public string Time { get; set; }
        public string ModuleName { get; set; }
        public string WindowName { get; set; }
        public string Status { get; set; }

        public SystemLogData(string time, string moduleName, string windowName, string status)
        {
            Time = time;
            ModuleName = moduleName;
            WindowName = windowName;
            Status = status;
        }
    }
}
