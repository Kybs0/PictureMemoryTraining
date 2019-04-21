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
    /// MemoryTestView.xaml 的交互逻辑
    /// </summary>
    public partial class MemoryTestView : UserControl
    {
        public MemoryTestView(List<MemoryPictureItem> items)
        {
            InitializeComponent();
            _memoryPictureItems = items;
            Loaded += async (s, e) =>
            {
                await StartLearning(true);
            };
        }

        private List<MemoryPictureItem> _memoryPictureItems = null;

        #region 学习

        private List<int> _memoryCountOrderList = new List<int>() { 4, 5, 6 };
        private List<int> _usedmemoryCountOrderList = new List<int>();

        private async Task StartLearning(bool isFirstLearning=false)
        {
            var clickMaxLimit = GetClickMaxLimit();

            ResetMemoryPictureListStatus();

            var memoryPictureItems = _memoryPictureItems;
            if (isFirstLearning)
            {
                //首次显示所有图片
                foreach (var memoryPictureItem in memoryPictureItems)
                {
                    memoryPictureItem.IsPictureEnabled = false;
                    memoryPictureItem.IsPictureVisibile = true;
                }
            }

            var memoryPictureListControl = new MemoryPictureListControl(new TrainingStageSetting()
            {
                ClickMaxLimit = clickMaxLimit,
                TrainingStage = TrainingStage.Learning
            });
            memoryPictureListControl.MemoryPictureItems = memoryPictureItems;
            memoryPictureListControl.PictureMemorized += MemoryPictureList_OnPictureMemorized;
            MemoryPictureListContentControl.Content = memoryPictureListControl;

            //首次显示所有图片后，恢复隐藏
            if (isFirstLearning)
            {
                await Task.Delay(TimeSpan.FromSeconds(2));
                foreach (var memoryPictureItem in memoryPictureItems)
                {
                    memoryPictureItem.IsPictureEnabled = true;
                    memoryPictureItem.IsPictureVisibile = false;
                }
            }
        }

        private int GetClickMaxLimit()
        {
            var random = new Random();

            int memoryCount = 0;
            while (memoryCount == 0)
            {
                var randomIndex = random.Next(_memoryCountOrderList.Count);
                var randomMemoryCountValue = _memoryCountOrderList[randomIndex];
                if (_usedmemoryCountOrderList.All(i => i != randomMemoryCountValue))
                {
                    _usedmemoryCountOrderList.Add(randomMemoryCountValue);
                    memoryCount = randomMemoryCountValue;
                }
            }

            return memoryCount;
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

        private List<MemorizedMemoryPictureItem> GetLastThreeMemorizedPictures()
        {
            var memorizedMemoryPictureItems = _memorizedPictureList.ToList();
            memorizedMemoryPictureItems.Reverse();
            var memoryPictureItems = memorizedMemoryPictureItems.Take(3);
            memoryPictureItems.Reverse();
            return memoryPictureItems.ToList();
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
                ClickMaxLimit = 3,
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
            var memorizedPictureList = GetLastThreeMemorizedPictures();
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
                ClickMaxLimit = 3,
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
                ////完成一轮测试
                _selectedLocationTestingPictureList.Clear();
                CompleteOneRoundTest();
            }
            else
            {
                var memorizedPictureList = GetLastThreeMemorizedPictures();
                var random = new Random();
                MemoryPictureItem visibileRandomPictureItem = null;
                while (visibileRandomPictureItem == null)
                {
                    var visibileRandomIndex = random.Next(memorizedPictureList.Count);
                    var randomPictureItem = memorizedPictureList[visibileRandomIndex].PictureItem;
                    if (_selectedLocationTestingPictureList.All(i => i.PictureItem != randomPictureItem))
                    {
                        visibileRandomPictureItem = randomPictureItem;
                    }
                }

                visibileRandomPictureItem.IsPictureVisibile = true;
            }
        }

        #endregion

        #region MyRegion

        private async void CompleteOneRoundTest()
        {
            if (_usedmemoryCountOrderList.Count< _memoryCountOrderList.Count)
            {
               await StartLearning();
            }
            else
            {
                TestingCompleted?.Invoke(this, EventArgs.Empty);
            }
        }

        #endregion
    }
}
