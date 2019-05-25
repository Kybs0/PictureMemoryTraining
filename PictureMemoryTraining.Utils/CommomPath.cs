using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PictureMemoryTraining.Utils
{
    public static class CommomPath
    {
        public static string LogPath()
        {
            string appdataPath = OutputDataFolder();
            string logFilePath = Path.Combine(appdataPath, "log.txt");
            if (!File.Exists(logFilePath))
            {
                var aaa = File.Create(logFilePath);
                aaa.Dispose();
            }

            return logFilePath;

        }
        public static string OutputPath()
        {
            string appdataPath = OutputDataFolder();
            string logFilePath = Path.Combine(appdataPath, "output.txt");
            if (!File.Exists(logFilePath))
            {
                var aaa = File.Create(logFilePath);
                aaa.Dispose();
            }

            return logFilePath;

        }
        public static string OutputDataFolder()
        {
            string appdataPath = Path.Combine(@"C:\Users\" + Environment.UserName + @"\AppData\Roaming\PictureMemoryTraining");
            if (!Directory.Exists(appdataPath))
            {
                Directory.CreateDirectory(appdataPath);
            }
            return appdataPath;

        }
    }
}
