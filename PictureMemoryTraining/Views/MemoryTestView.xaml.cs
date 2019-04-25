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
using PictureMemoryTraining.Business.Excel;
using PictureMemoryTraining.Utils;
using PictureMemoryTraining.Views.Models;
using Path = System.IO.Path;

namespace PictureMemoryTraining.Views
{
    /// <summary>
    /// MemoryTestView.xaml 的交互逻辑
    /// </summary>
    public partial class MemoryTestView : UserControl
    {
        public MemoryTestView(List<MemoryPictureItem> items, GroupTestInfo groupTestInfo)
        {
            InitializeComponent();
            _memoryPictureItems = items;
            _groupTestInfo = groupTestInfo;
            Loaded += async (s, e) =>
            {
                await StartLearning(true);
            };
        }

        private GroupTestInfo _groupTestInfo;

        private List<MemoryPictureItem> _memoryPictureItems = null;

        #region 学习

        private List<int> _memoryCountOrderList = new List<int>() { 4, 5, 6 };
        private List<int> _usedmemoryCountOrderList = new List<int>();
        private UserTestRecordInfo _currentTestRecordInfo = null;
        private async Task StartLearning(bool isFirstLearning = false)
        {
            CurrentStateTextBlock.Text = isFirstLearning ? "开始记忆" : "继续记忆";
            CurrentStateDetailTextBlock.Text = "依次点击图片，记忆此图片位置及点击的顺序";
            var clickMaxLimit = GetClickMaxLimit();
            _currentTestRecordInfo = GetTestInfoByClickCount(clickMaxLimit);
            ResetMemoryPictureListStatus();

            var memoryPictureItems = _memoryPictureItems.ToList();
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
            }, _currentTestRecordInfo);
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
            //记录时间
            _currentTestRecordInfo.StartLearningTime = DateTime.Now;
        }

        private UserTestRecordInfo GetTestInfoByClickCount(int clickMaxLimit)
        {
            if (clickMaxLimit == 4)
            {
                return _groupTestInfo.FourPicturesUserTestRecordInfo;
            }
            else if (clickMaxLimit == 5)
            {
                return _groupTestInfo.FivePicturesUserTestRecordInfo;
            }
            else
            {
                return _groupTestInfo.SixPicturesUserTestRecordInfo;
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
            CurrentStateTextBlock.Text = "记忆还原-顺序";
            CurrentStateDetailTextBlock.Text = "按照顺序点击刚记忆过的最后3个图片,选择完后点击确定";
        }

        private void StartSequentialMemoryTest()
        {
            var memoryPictureItems = _memoryPictureItems.ToList();
            //打乱随机排序
            var pictureItems = memoryPictureItems.RandomSort();

            //初始化图片
            foreach (var memoryPictureItem in pictureItems)
            {
                memoryPictureItem.IsPictureVisibile = true;
            }
            var memoryPictureListControl = new MemoryPictureListControl(new TrainingStageSetting()
            {
                ClickMaxLimit = 3,
                TrainingStage = TrainingStage.SequentialTesting
            }, _currentTestRecordInfo);
            memoryPictureListControl.MemoryPictureItems = pictureItems;
            memoryPictureListControl.SequentialSelected += MemoryPictureList_OnSequentialSelected;
            MemoryPictureListContentControl.Content = memoryPictureListControl;
            //记录开始测试时间
            _currentTestRecordInfo.StartTestingTime = DateTime.Now;
        }

        private void CancedlTestingTip()
        {
            OperationGrid.Visibility = Visibility.Collapsed;
            OperationGrid.PreviewMouseDown -= StartTesting;
        }

        private void MemoryPictureList_OnSequentialSelected(object sender, List<MemoryPictureItem> selectedItems)
        {
            //记录顺序测试结果
            var memorizedPictureList = GetLastThreeMemorizedPictures();
            for (int i = 0; i < 3; i++)
            {
                var memorizedPicture = memorizedPictureList[i];
                var sequentialTestingClickInfo = _currentTestRecordInfo.SequentialTestingClickInfos[i];

                var memorizedPictureName = Path.GetFileNameWithoutExtension(memorizedPicture.PictureItem.ImageUri);
                sequentialTestingClickInfo.IsRight = sequentialTestingClickInfo.PictureName == memorizedPictureName;
            }

            ResetMemoryPictureListStatus();
            StartLocationMemoryTesting();
            CurrentStateTextBlock.Text = "记忆还原-位置";
            CurrentStateDetailTextBlock.Text = "判断该位置是否与学习阶段相同，是选勾，否选叉";
        }

        private void ResetMemoryPictureListStatus()
        {
            MemoryPictureListContentControl.Content = null;
            foreach (var memoryPictureItem in _memoryPictureItems)
            {
                memoryPictureItem.IsHighlighted = false;
                memoryPictureItem.IsPictureVisibile = false;
                memoryPictureItem.IsPictureCovered = false;
            }
        }

        #endregion

        #region 位置测试

        private void StartLocationMemoryTesting()
        {
            var memoryPictureItems = _memoryPictureItems.ToList();
            //打乱随机排序
            var pictureItems = memoryPictureItems.RandomSort();
            //保持一个图片原位置
            var memorizedPictureList = GetLastThreeMemorizedPictures();
            var random = new Random();
            var randomIndex = random.Next(memorizedPictureList.Count);
            var memoryPictureItem = memorizedPictureList[randomIndex].PictureItem;
            pictureItems.Remove(memoryPictureItem);
            pictureItems.Insert(memorizedPictureList[randomIndex].Location, memoryPictureItem);

            //设置初始状态
            pictureItems.ForEach(i => i.IsPictureCovered = true);

            //初始化图片
            var visibileRandomIndex = random.Next(memorizedPictureList.Count);
            var visibileRandomPictureItem = memorizedPictureList[visibileRandomIndex].PictureItem;
            visibileRandomPictureItem.IsPictureVisibile = true;

            //添加控件内容
            var memoryPictureListControl = new MemoryPictureListControl(new TrainingStageSetting()
            {
                ClickMaxLimit = 3,
                TrainingStage = TrainingStage.LocationTesting
            }, _currentTestRecordInfo);
            memoryPictureListControl.MemoryPictureItems = pictureItems;
            memoryPictureListControl.PictureLocationComfirmed += MemoryPictureList_OnPictureLocationComfirmed;
            MemoryPictureListContentControl.Content = memoryPictureListControl;
        }
        /// <summary>
        ///  位置测试选中的图片
        /// </summary>
        private List<LocationMemoryPictureItem> _selectedLocationTestingPictureList = new List<LocationMemoryPictureItem>();

        public event EventHandler TestingCompleted;
        private void MemoryPictureList_OnPictureLocationComfirmed(object sender, LocationMemoryPictureItem checkedPictureItem)
        {
            //记录位置测试结果
            var memorizedPictureList = GetLastThreeMemorizedPictures();
            var memorizedMemoryPictureItem = memorizedPictureList.First(i => i.PictureItem == checkedPictureItem.PictureItem);
            var isRightLocation = checkedPictureItem.Location == memorizedMemoryPictureItem.Location;
            _currentTestRecordInfo.LocationTestingClickInfos.Add(new TestingClickInfo()
            {
                PictureName = Path.GetFileNameWithoutExtension(memorizedMemoryPictureItem.PictureItem.ImageUri),
                ClickTime = DateTime.Now,
                Location = checkedPictureItem.Location,
                IsRight = isRightLocation ? checkedPictureItem.IsMatchedByUserComfirmed : !checkedPictureItem.IsMatchedByUserComfirmed
            });

            _selectedLocationTestingPictureList.Add(checkedPictureItem);
            if (sender is MemoryPictureListControl memoryPictureListControl &&
                _selectedLocationTestingPictureList.Count >= memoryPictureListControl.TrainingStageSetting.ClickMaxLimit)
            {
                ////完成一轮测试
                _selectedLocationTestingPictureList.Clear();
                CompleteOneRoundTest();
            }
            else
            {
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
            if (_usedmemoryCountOrderList.Count < _memoryCountOrderList.Count)
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
