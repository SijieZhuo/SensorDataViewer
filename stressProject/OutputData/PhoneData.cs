using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace stressProject.OutputData
{
    class PhoneData
    {
        public string Time { get; set; }
        public int Sound { get; set; }
        public double AccX { get; set; }
        public double AccY { get; set; }
        public double AccZ { get; set; }
        public double RotX { get; set; }
        public double RotY { get; set; }
        public double RotZ { get; set; }
        public double GraX { get; set; }
        public double GraY { get; set; }
        public double GraZ { get; set; }
        public string CurrentApp { get; set; }
        public string ScreenStatus { get; set; }
        public double Downloads { get; set; }
        public double Uploads { get; set; }

        public PhoneData(string time, int sound, double accX, double accY, double accZ, double rotX, double rotY, double rotZ, double graX, double graY, double graZ, string currentApp, string screenStatus, double downloads, double uploads)
        {
            Time = time;
            Sound = sound;
            AccX = accX;
            AccY = accY;
            AccZ = accZ;
            RotX = rotX;
            RotY = rotY;
            RotZ = rotZ;
            GraX = graX;
            GraY = graY;
            GraZ = graZ;
            CurrentApp = currentApp;
            ScreenStatus = screenStatus;
            Downloads = downloads;
            Uploads = uploads;
        }
    }
}
