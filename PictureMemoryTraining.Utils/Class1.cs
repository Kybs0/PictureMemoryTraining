using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Interop;

namespace PictureMemoryTraining.Utils
{
    public static class ScreenSizeHelper
    {
        const double DpiPercent = 96;
        public static ScreenSizeInfo GetScreenSizeInfo(this Window window)
        {
            var intPtr = new WindowInteropHelper(window).Handle;//获取当前窗口的句柄
            var screen = Screen.FromHandle(intPtr);//获取当前屏幕

            using (Graphics currentGraphics = Graphics.FromHwnd(intPtr))
            {
                double dpiXRatio = currentGraphics.DpiX / DpiPercent;
                double dpiYRatio = currentGraphics.DpiY / DpiPercent;
                var height = screen.WorkingArea.Height / dpiYRatio;
                var width = screen.WorkingArea.Width / dpiXRatio;
                var left = screen.WorkingArea.Left / dpiXRatio;
                var top = screen.WorkingArea.Top / dpiYRatio;
                return new ScreenSizeInfo()
                {
                    Height = height,
                    Width = width,
                    Left = left,
                    Top = top
                };
            }
        }
       
    }

    public class ScreenSizeInfo
    {
        public double Height{ get; set; }
        public double Width { get; set; }
        public double Left { get; set; }
        public double Top { get; set; }
    }
}
