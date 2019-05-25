using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PictureMemoryTraining.Utils
{
    public static class LogHelper
    {
        public static void LogInfo(string text)
        {
            LogInfo(new List<string>() { text });
        }
        public static void LogInfo(List<string> lines)
        {
            string logFilePath = CommomPath.LogPath();
            File.AppendAllLines(logFilePath, lines);
        }
        public static void LogOutput(string text)
        {
            LogOutput(new List<string>() { text });
        }
        public static void LogOutput(List<string> lines)
        {
            string logFilePath = CommomPath.OutputPath();
            File.AppendAllLines(logFilePath, lines);
        }
    }
}
