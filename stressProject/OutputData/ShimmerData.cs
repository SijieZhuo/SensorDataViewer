using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace stressProject.OutputData
{
    class ShimmerData
    {
        public string Time { get; set; }
        public double GSR { get; set; }
        public double PPG { get; set; }
        public double AccX { get; set; }
        public double AccY { get; set; }
        public double AccZ { get; set; }

        public ShimmerData(string t, double d, double p, double x, double y, double z)
        {
            Time = t;
            GSR = d;
            PPG = p;
            AccX = x;
            AccY = y;
            AccZ = z;
        }
    }
}
