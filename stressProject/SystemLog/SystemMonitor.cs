//using log4net;
//using log4net.Config;
using MonitorApp.Helper;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace stressProject.SystemLog
{
    class SystemMonitor
    {

        private const uint WINEVENT_OUTOFCONTEXT = 0;
        // private const uint EVENT_SYSTEM_FOREGROUND = 3;
        private const uint EVENT_OBJECT_NAMECHANGE = 0x800C;
        private const int KEYBOARD_TIMER_INTERVAL = 2000;

       // private NotifyIcon _trayIcon;
        private WinEventDelegate _dele;
        private LowLevelKeyboardHook _keyHook;
        private IgnoreStringHelper _ignoreStringHelper;
        private string _previousWindowTitle;
        private Boolean _isEclipseFocused;
        private Timer _keyboardTimer;

        private MessageTransferStation mts;

       // private static ILog _logger;


        public SystemMonitor() {

            //XmlConfigurator.Configure();

            mts = MessageTransferStation.Instance;

            //_logger = logger;
            _ignoreStringHelper = new IgnoreStringHelper();
            _previousWindowTitle = "";
            _isEclipseFocused = false;



            _dele = new WinEventDelegate(WinEventProc);
            IntPtr m_hhook = SetWinEventHook(EVENT_OBJECT_NAMECHANGE, EVENT_OBJECT_NAMECHANGE, IntPtr.Zero, _dele, 0, 0, WINEVENT_OUTOFCONTEXT);


        }

        public void WinEventProc(IntPtr hWinEventHook, uint eventType, IntPtr hwnd, int idObject, int idChild, uint dwEventThread, uint dwmsEventTime)
        {
            try
            {
                string currentTimpstamp = GetCurrentTimestamp();
                // Focused
                IntPtr foregroundWindowHandle = WindowHelper.GetForegroundWindowPointer();
                string moduleName = WindowHelper.GetProcessModuleName(foregroundWindowHandle);
                string windowName = WindowHelper.GetWindowText(foregroundWindowHandle);

                string foregroundWindowLogInfo = FormatWindowInfo(moduleName, windowName);
                if (foregroundWindowLogInfo == null
                    || foregroundWindowLogInfo.Length == 0
                    || foregroundWindowLogInfo.Equals(_previousWindowTitle)
                    || _ignoreStringHelper.shouldIgnoreThisString(foregroundWindowLogInfo)) return;

                _isEclipseFocused = moduleName.ToLower().Contains("eclipse.exe");
                // if (!_isEclipseFocused && _keyboardTimer.Enabled)
                //    {
                //         this.kbh_noActivity(null, null);
                //     }
                _previousWindowTitle = foregroundWindowLogInfo;

                string foregroundWindowLog = currentTimpstamp + "," + foregroundWindowLogInfo + ",focused";
                //_logger.Info(foregroundWindowLog);
                updateMessage(foregroundWindowLog);

                // visible
                List<string> listOfVisibleWindow = WindowHelper.GetAllVisibleWindowExcludingForegroundWindow(foregroundWindowHandle);

                foreach (string windowInfo in listOfVisibleWindow)
                {
                    if (!_ignoreStringHelper.shouldIgnoreThisString(windowInfo))
                    {
                        string log = currentTimpstamp + "," + windowInfo + ",visible";
                        //_logger.Info(log);
                        updateMessage(log);
                    }
                }
            }
            catch (Exception e)
            {
                // _logger.Warn(e.Message);
                Debug.WriteLine(e);

                updateMessage(e.Message);
            }
        }

        public static string FormatWindowInfo(string moduleName, string windowName)
        {
            if (moduleName != null)
            {
                string activeWindowInfo = moduleName + ",";
                if (windowName != null)
                {
                    activeWindowInfo += windowName;
                }
                return activeWindowInfo;
            }
            return null;
        }

        private string GetCurrentTimestamp()
        {
            return DateTime.UtcNow.ToString("o");
        }

        private void updateMessage(string message)
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                //mts.MessageText = message;
                mts.SystemText = message;
            });
        }



        delegate void WinEventDelegate(IntPtr hWinEventHook, uint eventType, IntPtr hwnd, int idObject, int idChild, uint dwEventThread, uint dwmsEventTime);
        [DllImport("user32.dll")]
        static extern IntPtr SetWinEventHook(uint eventMin, uint eventMax, IntPtr hmodWinEventProc, WinEventDelegate lpfnWinEventProc, uint idProcess, uint idThread, uint dwFlags);

    }
}
