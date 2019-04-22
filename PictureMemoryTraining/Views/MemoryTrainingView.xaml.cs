using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using PictureMemoryTraining.Views.Models;

namespace PictureMemoryTraining.Views
{
    /// <summary>
    /// 记忆训练界面 的交互逻辑
    /// </summary>
    public partial class MemoryTrainingView : UserControl
    {
        public MemoryTrainingView()
        {
            InitializeComponent();
        }

        public static readonly DependencyProperty CurrentProperty = DependencyProperty.Register(
            "Current", typeof(string), typeof(MemoryTrainingView), new PropertyMetadata(default(string)));

        public string Current
        {
            get { return (string)GetValue(CurrentProperty); }
            set { SetValue(CurrentProperty, value); }
        }

        #region 熟悉阶段

        private void FamiliarButton_OnClick(object sender, RoutedEventArgs e)
        {
            var memoryFamiliarView = new MemoryFamiliarView();
            memoryFamiliarView.Tag = sender;
            memoryFamiliarView.TestingCompleted += MemoryFamiliarView_TestingCompleted;

            TraingViewCotnentControl.Content = memoryFamiliarView;

            var items = MemoryPictureItemsManager.GetMemoryPictures();
            memoryFamiliarView.InitMemoryPictures(items);
        }

        private void MemoryFamiliarView_TestingCompleted(object sender, EventArgs e)
        {
            if (sender is MemoryFamiliarView memoryFamiliarView && memoryFamiliarView.Tag is Button button)
            {
                TraingViewCotnentControl.Content = null;
                button.IsEnabled = false;
            }
        }

        #endregion

        #region 测试阶段
        private void Test1Button_OnClick(object sender, RoutedEventArgs e)
        {
            //在程序运行期间，使用同一组图片
            var items = MemoryPictureItemsManager.GetTest1MemoryPictures();
            EnterTestingView(sender, items);
        }
        private void Test2Button_OnClick(object sender, RoutedEventArgs e)
        {
            var items = MemoryPictureItemsManager.GetTest2MemoryPictures();
            EnterTestingView(sender, items);
        }
        private void Test3Button_OnClick(object sender, RoutedEventArgs e)
        {
            var items = MemoryPictureItemsManager.GetTest3MemoryPictures();
            EnterTestingView(sender, items);
        }
        private void EnterTestingView(object sender, List<MemoryPictureItem> items)
        {
            var memoryTestView = new MemoryTestView(items);
            memoryTestView.Tag = sender;
            memoryTestView.TestingCompleted += MemoryTestView_TestingCompleted;
            TraingViewCotnentControl.Content = memoryTestView;
        }
        public event EventHandler TestingCompleted;
        private void MemoryTestView_TestingCompleted(object sender, EventArgs e)
        {
            if (sender is MemoryTestView memoryTestView && memoryTestView.Tag is Button button)
            {
                TraingViewCotnentControl.Content = null;
                button.IsEnabled = false;
            }

            if (Test1Button.IsEnabled == false && Test2Button.IsEnabled == false &&Test3Button.IsEnabled == false)
            {
                TestingCompleted?.Invoke(this,EventArgs.Empty);
            }
        }

        #endregion

    }
}
