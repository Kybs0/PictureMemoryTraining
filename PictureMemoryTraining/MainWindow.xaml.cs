using System.Windows;
using System.Windows.Input;
using PictureMemoryTraining.Utils;

namespace PictureMemoryTraining
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            //if ((DateTime.Now - new DateTime(2019, 4, 29, 0, 0, 0)).Days > 2)
            //{
            //    MessageBox.Show("图片记忆项目，已到试用期！请联系开发处理！");
            //    Environment.Exit(0);
            //}
            InitializeComponent();
            Loaded += InitWindowActualHeight_OnLoaded;
        }

        #region 窗口

        private void MinimizeButton_OnClick(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState.Minimized;
        }

        private void CloseButton_OnClick(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void HeaderGrid_OnMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ButtonState == MouseButtonState.Pressed)
            {
                this.DragMove();
            }
        }

        #endregion


        #region 设置窗口对屏幕高度的自适应

        private void InitWindowActualHeight_OnLoaded(object sender, RoutedEventArgs e)
        {
            Loaded -= InitWindowActualHeight_OnLoaded;
            InitWindowActualSize();
        }

        private void InitWindowActualSize()
        {
            //获取窗体所在屏幕的高度
            var screenSizeInfo = this.GetScreenSizeInfo();
            //获取任务栏高度
            //var taskbarHeight = SystemParameters.PrimaryScreenHeight - SystemParameters.WorkArea.Height;
            if (screenSizeInfo.Width <= this.Width || screenSizeInfo.Height <= this.Height)
            {
                //全屏。解决直接设置全屏模式，margin显示的问题
                this.Width = screenSizeInfo.Width;
                this.Height = screenSizeInfo.Height;
                this.Top = screenSizeInfo.Top;
                this.Left = screenSizeInfo.Left;
            }
            else
            {
                HeaderGrid.MouseLeftButtonDown += HeaderGrid_OnMouseLeftButtonDown;
            }
        }
        #endregion
    }
}
