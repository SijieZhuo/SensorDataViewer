using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text;

namespace MonitorApp.Helper
{
    public static class WindowHelper
    {
        public static IntPtr GetForegroundWindowPointer()
        {
            return GetForegroundWindow();
        }

        public static List<string> GetAllVisibleWindowExcludingForegroundWindow(IntPtr foregroundWindowHandle)
        {
            List<string> listOfVisibleWindows = new List<string>();

            foreach (IntPtr window in GetOpenWindows())
            {
                IntPtr windowHandle = window;
                if (windowHandle != IntPtr.Zero && windowHandle != foregroundWindowHandle && !IsOverlapped(windowHandle))
                {
                    string windowInfo = GetWindowInfo(windowHandle);
                    if (windowInfo != null)
                    {
                        listOfVisibleWindows.Add(windowInfo);
                    }
                }
            }

            return listOfVisibleWindows;
        }

        public static string GetWindowInfo(IntPtr handle)
        {
            string moduleName = GetProcessModuleName(handle);
            string windowName = GetWindowText(handle);
            if (moduleName != null) {
                string activeWindowInfo = moduleName+",";
                if (windowName != null)
                {
                    activeWindowInfo += windowName;
                }
                return activeWindowInfo;
            }
            return null;
        }

        public static string GetProcessModuleName(IntPtr handle)
        {
            uint processId;
            if (GetWindowThreadProcessId(handle, out processId) > 0)
            {
                string moduleName = Process.GetProcessById((int)processId).MainModule.ModuleName;
                moduleName = moduleName.Replace(",", string.Empty);
                return moduleName;
            }
            return null;
        }

        public static string GetWindowText(IntPtr handle)
        {
            const int charLimit = 256;
            StringBuilder buffer = new StringBuilder(charLimit);
            if (GetWindowText(handle, buffer, charLimit) > 0)
            {
                string str = buffer.ToString();
                str = str.Replace(",", string.Empty);
                return str;
            }
            return null;
        }

        // SOURCE: https://stackoverflow.com/questions/7268302/get-the-titles-of-all-open-windows
        private static List<IntPtr> GetOpenWindows()
        {
            IntPtr shellWindow = GetShellWindow();
            List<IntPtr> windows = new List<IntPtr>();

            EnumWindows(delegate (IntPtr hWnd, int lParam)
            {
                if (hWnd == shellWindow) return true;
                if (!IsWindowVisible(hWnd)) return true;

                int length = GetWindowTextLength(hWnd);
                if (length == 0) return true;

                windows.Add(hWnd);
                
                return true;

            }, 0);

            return windows;
        }

        // I modified the code from: https://social.msdn.microsoft.com/Forums/vstudio/en-US/78289886-f3c1-405b-aaa1-722a23690245/how-to-check-if-a-window-is-partially-or-completely-obscured-by-other-windows?forum=netfxbcl
        private static bool IsOverlapped(IntPtr windowHandle)
        {
            if (windowHandle == IntPtr.Zero)
                throw new InvalidOperationException("Window does not yet exist");
            if (!IsWindowVisible(windowHandle))
                return false;

            IntPtr hWnd = IntPtr.Zero;
            hWnd = windowHandle;
            HashSet<IntPtr> visited = new HashSet<IntPtr> { hWnd };

            // The set is used to make calling GetWindow in a loop stable by checking if we have already
            //  visited the window returned by GetWindow. This avoids the possibility of an infinate loop.

            RECT thisRect;
            GetWindowRect(hWnd, out thisRect);
            thisRect = MakeRectSmaller(thisRect);

            if (thisRect.top <= 10 && thisRect.right <= 10 && thisRect.bottom <= 10 && thisRect.left <= 10) return true;

            while ((hWnd = GetWindow(hWnd, GW_HWNDPREV)) != IntPtr.Zero && !visited.Contains(hWnd))
            {
                visited.Add(hWnd);
                RECT testRect, intersection;
                if (IsWindowVisible(hWnd) && GetWindowRect(hWnd, out testRect)) 
                {
                    testRect = MakeRectSmaller(testRect);
                    if (IntersectRect(out intersection, ref thisRect, ref testRect)) return true;
                   
                }
            }
            return false;
        }

        private static RECT MakeRectSmaller(RECT thisRect)
        {
            RECT smallerRect = new RECT();
            double scale = 0.2;
            double width = thisRect.right - thisRect.left;
            double height = thisRect.bottom - thisRect.top;

            smallerRect.top = (int)(thisRect.top + height*scale);
            smallerRect.right = (int)(thisRect.right - width*scale);
            smallerRect.bottom = (int)(thisRect.bottom - height*scale);
            smallerRect.left = (int)(thisRect.left + width*scale);

            return smallerRect;
        }

        private delegate bool EnumWindowsProc(IntPtr hWnd, int lParam);

        [DllImport("USER32.DLL")]
        private static extern bool EnumWindows(EnumWindowsProc enumFunc, int lParam);
        [DllImport("USER32.DLL")]
        private static extern int GetWindowTextLength(IntPtr hWnd);
        [DllImport("USER32.DLL")]
        private static extern IntPtr GetShellWindow();

        [DllImport("user32.dll")]
        private static extern IntPtr GetForegroundWindow();
        [DllImport("user32.dll")]
        private static extern int GetWindowText(IntPtr hWnd, StringBuilder text, int count);
        [DllImport("user32.dll")]
        private static extern int GetWindowModuleFileName(IntPtr hWnd, StringBuilder text, int count);
        [DllImport("user32.dll", SetLastError = true)]
        private static extern uint GetWindowThreadProcessId(IntPtr hWnd, out uint lpdwProcessId);


        [DllImport("user32.dll")]
        private static extern IntPtr GetWindow(IntPtr hWnd, int uCmd);
        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool GetWindowRect(IntPtr hWnd, [Out] out RECT lpRect);
        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool IntersectRect([Out] out RECT lprcDst, [In] ref RECT lprcSrc1, [In] ref RECT lprcSrc2);
        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool IsWindowVisible(IntPtr hWnd);

        private const int GW_HWNDPREV = 3;

        [StructLayout(LayoutKind.Sequential)]
        private struct RECT
        {
            public int left;
            public int top;
            public int right;
            public int bottom;
        }
    }
}