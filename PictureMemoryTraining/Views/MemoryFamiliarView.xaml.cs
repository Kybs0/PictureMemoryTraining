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
using PictureMemoryTraining.Utils;
using PictureMemoryTraining.Views.Models;

namespace PictureMemoryTraining.Views
{
    /// <summary>
    /// MemoryFamiliarView.xaml 的交互逻辑
    /// </summary>
    public partial class MemoryFamiliarView : UserControl
    {
        public MemoryFamiliarView()
        {
            InitializeComponent();
        }

        private List<MemoryPictureItem> _memoryPictureItems = null;

        #region 学习

        public async void InitMemoryPictures(List<MemoryPictureItem> items)
        {
            _memoryPictureItems = items;
            foreach (var memoryPictureItem in items)
            {
                memoryPictureItem.IsPictureEnabled = false;
                memoryPictureItem.IsPictureVisibile = true;
            }

            var memoryPictureListControl = new MemoryPictureListControl(new TrainingStageSetting()
            {
                ClickMaxLimit = 2,
                TrainingStage = TrainingStage.Learning
            });
            memoryPictureListControl.MemoryPictureItems = items;
            memoryPictureListControl.PictureMemorized += MemoryPictureList_OnPictureMemorized;
            MemoryPictureListContentControl.Content = memoryPictureListControl;
            await Task.Delay(TimeSpan.FromSeconds(2));
            foreach (var memoryPictureItem in items)
            {
                memoryPictureItem.IsPictureEnabled = true;
                memoryPictureItem.IsPictureVisibile = false;
            }
        }

        /// <summary>
        /// 已经记忆过的图片
        /// </summary>
        private List<MemorizedMemoryPictureItem> _memorizedPictureList = new List<MemorizedMemoryPictureItem>();
        private void MemoryPictureList_OnPictureMemorized(object sender, List<MemorizedMemoryPictureItem> items)
        {
            _memorizedPictureList = items;
            SetTestingTip();
        }

        #endregion

        #region 进入顺序测试

        private void SetTestingTip()
        {
            OperationGrid.Visibility = Visibility.Visible;
            OperationTipTextBlock.Text = "Testing";
            OperationGrid.PreviewMouseDown -= StartTesting;
            OperationGrid.PreviewMouseDown += StartTesting;
            //置空当前图片列表控件
            MemoryPictureListContentControl.Content = null;
        }

        private void StartTesting(object sender, MouseButtonEventArgs e)
        {
            CancedlTestingTip();
            ResetMemoryPictureListStatus();
            StartSequentialMemoryTest();
        }

        private void StartSequentialMemoryTest()
        {
            var memoryPictureItems = _memoryPictureItems;
            //打乱随机排序
            var pictureItems = memoryPictureItems.RandomSort();
            _memoryPictureItems = pictureItems;

            //初始化图片
            foreach (var memoryPictureItem in pictureItems)
            {
                memoryPictureItem.IsPictureVisibile = true;
            }
            var memoryPictureListControl = new MemoryPictureListControl(new TrainingStageSetting()
            {
                ClickMaxLimit = 2,
                TrainingStage = TrainingStage.SequentialTesting
            });
            memoryPictureListControl.MemoryPictureItems = pictureItems;
            memoryPictureListControl.SequentialSelected += MemoryPictureList_OnSequentialSelected;
            MemoryPictureListContentControl.Content = memoryPictureListControl;
        }

        private void CancedlTestingTip()
        {
            OperationGrid.Visibility = Visibility.Collapsed;
            OperationGrid.PreviewMouseDown -= StartTesting;
        }

        private void MemoryPictureList_OnSequentialSelected(object sender, List<MemoryPictureItem> selectedItems)
        {
            //TODO 记录顺序测试结果
            ResetMemoryPictureListStatus();
            StartLocationMemoryTesting();
        }

        private void ResetMemoryPictureListStatus()
        {
            MemoryPictureListContentControl.Content = null;
            foreach (var memoryPictureItem in _memoryPictureItems)
            {
                memoryPictureItem.IsHighlighted = false;
                memoryPictureItem.IsPictureVisibile = false;
            }
        }

        #endregion

        #region 位置测试

        private void StartLocationMemoryTesting()
        {
            var memoryPictureItems = _memoryPictureItems;
            //打乱随机排序
            var pictureItems = memoryPictureItems.RandomSort();
            //保持一个图片原位置
            var memorizedPictureList = _memorizedPictureList;
            var random = new Random();
            var randomIndex = random.Next(memorizedPictureList.Count);
            var memoryPictureItem = memorizedPictureList[randomIndex].PictureItem;
            pictureItems.Remove(memoryPictureItem);
            pictureItems.Insert(memorizedPictureList[randomIndex].Location, memoryPictureItem);
            _memoryPictureItems = pictureItems;

            //初始化图片
            var visibileRandomIndex = random.Next(memorizedPictureList.Count);
            var visibileRandomPictureItem = memorizedPictureList[visibileRandomIndex].PictureItem;
            visibileRandomPictureItem.IsPictureVisibile = true;

            //添加控件内容
            var memoryPictureListControl = new MemoryPictureListControl(new TrainingStageSetting()
            {
                ClickMaxLimit = 2,
                TrainingStage = TrainingStage.LocationTesting
            });
            memoryPictureListControl.MemoryPictureItems = pictureItems;
            memoryPictureListControl.PictureLocationComfirmed += MemoryPictureList_OnPictureLocationComfirmed;
            MemoryPictureListContentControl.Content = memoryPictureListControl;
        }
        /// <summary>
        ///  位置测试选中的图片
        /// </summary>
        private List<LocationMemoryPictureItem> _selectedLocationTestingPictureList = new List<LocationMemoryPictureItem>();

        public event EventHandler TestingCompleted;
        private void MemoryPictureList_OnPictureLocationComfirmed(object sender, LocationMemoryPictureItem item)
        {
            //TODO 记录

            _selectedLocationTestingPictureList.Add(item);
            if (sender is MemoryPictureListControl memoryPictureListControl &&
                _selectedLocationTestingPictureList.Count >= memoryPictureListControl.TrainingStageSetting.ClickMaxLimit)
            {
                TestingCompleted?.Invoke(this, EventArgs.Empty);
            }
            else
            {
                var memorizedPictureList = _memorizedPictureList;
                var random = new Random();
                MemoryPictureItem visibileRandomPictureItem = null;
                while (visibileRandomPictureItem==null)
                {
                    var visibileRandomIndex = random.Next(memorizedPictureList.Count);
                    var randomPictureItem = memorizedPictureList[visibileRandomIndex].PictureItem;
                    if (_selectedLocationTestingPictureList.All(i=>i.PictureItem!=randomPictureItem))
                    {
                        visibileRandomPictureItem = randomPictureItem;
                    }
                }
                
                visibileRandomPictureItem.IsPictureVisibile = true;
            }
        }

        #endregion
    }
}
