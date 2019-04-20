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
using PictureMemoryTraining.Business.MemoryPicturesData;
using PictureMemoryTraining.Views.Models;

namespace PictureMemoryTraining.Views
{
    /// <summary>
    /// 风格图片列表控件
    /// </summary>
    public partial class MemoryPictureListControl : UserControl
    {
        public MemoryPictureListControl()
        {
            InitializeComponent();
            var items = MemoryPictureItemsManager.GetMemoryPictures();

            MemoryPictureItems = items;
        }

        #region 属性

        public static readonly DependencyProperty MemoryPictureItemsProperty = DependencyProperty.Register(
            "MemoryPictureItems", typeof(List<MemoryPictureItem>), typeof(MemoryPictureListControl), new PropertyMetadata(default(List<MemoryPictureItem>)));

        public List<MemoryPictureItem> MemoryPictureItems
        {
            get { return (List<MemoryPictureItem>)GetValue(MemoryPictureItemsProperty); }
            set { SetValue(MemoryPictureItemsProperty, value); }
        }


        #endregion

        #region ListBoxItem事件

        private void ListBoxItem_OnPreviewMouseDownHandler(object sender, MouseButtonEventArgs e)
        {
            if (sender is ListBoxItem listBoxItem && listBoxItem.DataContext is MemoryPictureItem memoryPictureItem)
            {
                memoryPictureItem.SetMemoryPictureVisibileStatus();
            }

        }

        #endregion
    }
}
