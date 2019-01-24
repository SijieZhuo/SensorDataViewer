using log4net;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MonitorApp.Helper
{
    public class IgnoreStringHelper
    {
        private static readonly ILog _logger = LogManager.GetLogger("IgnoreStringHelper");

        private List<string> ignoreList;
        public IgnoreStringHelper()
        {
            ignoreList = new List<string>();
            try
            {
                string path = System.IO.Directory.GetCurrentDirectory() + @"\ignoreList.txt";
                string[] FileContents = System.IO.File.ReadAllLines(path);
                ignoreList = FileContents.ToList();
            }
            catch(Exception)
            {
                // if not found log everything
            }
        }

        public bool shouldIgnoreThisString(string str)
        {
            string strLower = str.ToLower();
            foreach(string ignoreString in ignoreList)
            {
                if (strLower.Contains(ignoreString.ToLower()))
                {
                    return true;
                }
            }
            return false;
        }
    }
}
