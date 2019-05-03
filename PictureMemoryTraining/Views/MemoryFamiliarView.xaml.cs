using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
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

            CurrentStateTextBlock.Text = "开始记忆";
            CurrentStateDetailTextBlock.Text = "依次点击图片，记忆此图片位置及点击的顺序";

            foreach (var memoryPictureItem in items)
            {
                memoryPictureItem.IsPictureEnabled = false;
                memoryPictureItem.IsPictureVisibile = true;
            }

            var memoryPictureListControl = new MemoryPictureListControl(new TrainingStageSetting()
            {
                ClickMaxLimit = 2,
                TrainingStage = TrainingStage.Learning
            }, new UserTestRecordInfo());
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
            OperationTipTextBlock.Text = "突击测试";
            //置空当前图片列表控件
            MemoryPictureListContentControl.Content = null;

            var timer = new Timer();
            timer.Interval = TimeSpan.FromSeconds(2).TotalMilliseconds;
            timer.Elapsed += Timer_Elapsed;
            timer.Start();

            OperationGrid.PreviewMouseDown -= OperationGridOnPreviewMouseDown;
            OperationGrid.PreviewMouseDown += OperationGridOnPreviewMouseDown;

            void Timer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
            {
                EnterTestingStatus();
            }

            void OperationGridOnPreviewMouseDown(object sender, MouseButtonEventArgs e)
            {
                EnterTestingStatus();
            }
            void EnterTestingStatus()
            {
                timer.Stop();
                timer.Close();
                Application.Current.Dispatcher.Invoke(() =>
                {
                    OperationTipTextBlock.Text = string.Empty;
                    OperationGrid.Visibility = Visibility.Collapsed;
                    OperationGrid.PreviewMouseDown -= OperationGridOnPreviewMouseDown;
                    StartOrderTesting();
                });
            }
        }

        private void StartOrderTesting()
        {
            ResetMemoryPictureListStatus();
            StartSequentialMemoryTest();
            CurrentStateTextBlock.Text = "顺序测试";
            CurrentStateDetailTextBlock.Text = "按照顺序点击刚学过的物体,选择完后点击确定";
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
                ClickMaxLimit = 2,
                TrainingStage = TrainingStage.SequentialTesting
            }, new UserTestRecordInfo());
            memoryPictureListControl.MemoryPictureItems = pictureItems;
            memoryPictureListControl.SequentialSelected += MemoryPictureList_OnSequentialSelected;
            MemoryPictureListContentControl.Content = memoryPictureListControl;
        }

        private void MemoryPictureList_OnSequentialSelected(object sender, List<MemoryPictureItem> selectedItems)
        {
            ShowSequentialTestResult(selectedItems);

            ResetMemoryPictureListStatus();
            StartLocationMemoryTesting();
            CurrentStateTextBlock.Text = "位置测试";
            CurrentStateDetailTextBlock.Text = "判断该位置是否与学习阶段相同，是选勾，否选叉";
        }

        private void ShowSequentialTestResult(List<MemoryPictureItem> selectedItems)
        {
            var memoryPictureItems = _memorizedPictureList.ToList();
            bool isSequentialAllRight = true;
            for (int i = 0; i < memoryPictureItems.Count; i++)
            {
                if (memoryPictureItems[i].PictureItem != selectedItems[i])
                {
                    isSequentialAllRight = false;
                    break;
                }
            }
            var resultTipText = isSequentialAllRight ? "正确" : "错误";
            SetResultTip(resultTipText);
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
            var memoryPictureItems = _memoryPictureItems.ToList();
            //打乱随机排序
            var pictureItems = memoryPictureItems.RandomSort();
            //保持一个图片原位置
            var memorizedPictureList = _memorizedPictureList;
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
                ClickMaxLimit = 2,
                TrainingStage = TrainingStage.LocationTesting
            }, new UserTestRecordInfo());
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
            //提示结果
            var resultTipText = _memorizedPictureList.Any(i => i.PictureItem == item.PictureItem && i.Location == item.Location) ? "正确" : "错误";
            SetResultTip(resultTipText);

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

        #region 公共

        public event EventHandler<string> ResultTipShowing;

        /// <summary>
        /// 设置结果提示
        /// </summary>
        /// <param name="resultTipText"></param>
        private void SetResultTip(string resultTipText)
        {
            ResultTipShowing?.Invoke(this,resultTipText);
        }

        #endregion
    }
}
