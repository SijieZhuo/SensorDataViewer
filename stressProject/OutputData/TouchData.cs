using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace stressProject.OutputData
{
    class TouchData
    {
        public string StartTime { get; set; }
        public string StartX { get; set; }
        public string StartY { get; set; }
        public string EndTime { get; set; }
        public string EndX { get; set; }
        public string EndY { get; set; }
        public string Type { get; set; }

        public TouchData( string startTime, string startX, string startY, string endTime, string endX, string endY, string type)
        {
            StartTime = startTime;
            StartX = startX;
            StartY = startY;
            EndTime = endTime;
            EndX = endX;
            EndY = endY;
            Type = type;
        }
    }
}
