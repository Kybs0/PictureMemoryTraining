using System;
using System.Collections.Generic;
using System.IO;
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
using PictureMemoryTraining.Business.MemoryPicturesData;
using PictureMemoryTraining.Views.Models;
using Path = System.IO.Path;

namespace PictureMemoryTraining.Views
{
    /// <summary>
    /// 风格图片列表控件
    /// </summary>
    public partial class MemoryPictureListControl : UserControl
    {
        private UserTestRecordInfo _testRecordInfo = null;
        public MemoryPictureListControl(TrainingStageSetting trainingStageSetting, UserTestRecordInfo testRecordInfo)
        {
            InitializeComponent();
            _testRecordInfo = testRecordInfo;
            SetTrainingStageSetting(trainingStageSetting);
        }

        private void SetTrainingStageSetting(TrainingStageSetting trainingStageSetting)
        {
            TrainingStageSetting = trainingStageSetting;
            if (trainingStageSetting.TrainingStage == TrainingStage.SequentialTesting)
            {
                ComfirmButton.Visibility = Visibility.Visible;
            }
            else if (trainingStageSetting.TrainingStage == TrainingStage.LocationTesting)
            {
                YesButton.Visibility = Visibility.Visible;
                NoButton.Visibility = Visibility.Visible;
            }
        }

        #region 属性

        public static readonly DependencyProperty MemoryPictureItemsProperty = DependencyProperty.Register(
            "MemoryPictureItems", typeof(List<MemoryPictureItem>), typeof(MemoryPictureListControl), new PropertyMetadata(default(List<MemoryPictureItem>)));

        public List<MemoryPictureItem> MemoryPictureItems
        {
            get { return (List<MemoryPictureItem>)GetValue(MemoryPictureItemsProperty); }
            set { SetValue(MemoryPictureItemsProperty, value); }
        }

        public static readonly DependencyProperty TrainingStageSettingProperty = DependencyProperty.Register(
            "TrainingStageSetting", typeof(TrainingStageSetting), typeof(MemoryPictureListControl), new PropertyMetadata(default(TrainingStageSetting)));

        public TrainingStageSetting TrainingStageSetting
        {
            get { return (TrainingStageSetting)GetValue(TrainingStageSettingProperty); }
            set { SetValue(TrainingStageSettingProperty, value); }
        }

        #endregion

        #region ListBoxItem事件

        private bool _pictureClicking = false;
        private async void ListBoxItem_OnPreviewMouseDownHandler(object sender, MouseButtonEventArgs e)
        {
            if (sender is ListBoxItem listBoxItem && listBoxItem.DataContext is MemoryPictureItem memoryPictureItem
                && !_pictureClicking)
            {
                _pictureClicking = true;
                var trainingStage = TrainingStageSetting.TrainingStage;
                if (trainingStage == TrainingStage.Learning)
                {
                    await HandleLearningPictureClick(listBoxItem, memoryPictureItem);
                }
                else if (trainingStage == TrainingStage.SequentialTesting)
                {
                    await HandleSequentialTestingPictureClick(memoryPictureItem);
                }
                else if (trainingStage == TrainingStage.LocationTesting)
                {
                    await HandleLocationTestingPictureClick(memoryPictureItem);
                }
                _pictureClicking = false;
            }
        }

        #region 位置测试

        /// <summary>
        ///  位置测试选中的图片
        /// </summary>
        private List<MemoryPictureItem> _selectedLocationTestingPictureList = new List<MemoryPictureItem>();
        private async Task HandleLocationTestingPictureClick(MemoryPictureItem memoryPictureItem)
        {
            ////如果选中的图片，超过限制，则直接返回
            //if (_selectedLocationTestingPictureList.Count >= TrainingStageSetting.ClickMaxLimit ||
            //    _selectedLocationTestingPictureList.Any(i => i == memoryPictureItem) ||
            //    _selectedLocationTestingPictureList.Any(i => i.IsHighlighted))
            //{
            //    return;
            //}
            //await SetMemoryPictureMarkedStatus(memoryPictureItem);
            ////添加序列
            //if (memoryPictureItem.IsHighlighted)
            //{
            //    if (_selectedLocationTestingPictureList.All(i => i != memoryPictureItem))
            //    {
            //        _selectedLocationTestingPictureList.Add(memoryPictureItem);
            //    }
            //}
            //else
            //{
            //    if (_selectedLocationTestingPictureList.Any(i => i == memoryPictureItem))
            //    {
            //        _selectedLocationTestingPictureList.Remove(memoryPictureItem);
            //    }
            //}
            //YesButton.IsEnabled = memoryPictureItem.IsHighlighted;
        }

        public event EventHandler<LocationMemoryPictureItem> PictureLocationComfirmed;
        private async void YesButton_OnClick(object sender, RoutedEventArgs e)
        {
            await ComfirmLocation(sender, true);
        }

        private async Task ComfirmLocation(object sender, bool comfirmed)
        {
            this.IsEnabled = false;
            if (sender is Button button)
            {
                button.IsEnabled = false;
                await Task.Delay(TimeSpan.FromMilliseconds(400));
                var memoryPictureItem = MemoryPictureItems.First(i => i.IsPictureVisibile);
                memoryPictureItem.IsPictureVisibile = false;
                memoryPictureItem.IsPictureCovered = true;
                PictureLocationComfirmed?.Invoke(this, new LocationMemoryPictureItem()
                {
                    PictureItem = memoryPictureItem,
                    Location = MemoryPictureItems.IndexOf(memoryPictureItem),
                    IsMatchedByUserComfirmed = comfirmed
                });
                button.IsEnabled = true;
            }
            this.IsEnabled = true;
        }

        private async void NoButton_OnClick(object sender, RoutedEventArgs e)
        {
            await ComfirmLocation(sender, false);
        }

        #endregion

        #region 顺序测试

        /// <summary>
        ///  顺序测试选中的图片
        /// </summary>
        private List<MemoryPictureItem> _selectedSequentialPictureList = new List<MemoryPictureItem>();
        /// <summary>
        /// 顺序测试选中变更
        /// </summary>
        public event EventHandler<List<MemoryPictureItem>> SequentialSelected;
        private async Task HandleSequentialTestingPictureClick(MemoryPictureItem memoryPictureItem)
        {
            //如果选中的图片，超过限制，则直接返回
            if (_selectedSequentialPictureList.Count >= TrainingStageSetting.ClickMaxLimit &&
                _selectedSequentialPictureList.Any(i => i != memoryPictureItem) &&
                !memoryPictureItem.IsHighlighted)
            {
                return;
            }
            //设置高亮状态
            memoryPictureItem.IsHighlighted = !memoryPictureItem.IsHighlighted;
            //添加序列
            if (memoryPictureItem.IsHighlighted)
            {
                if (_selectedSequentialPictureList.All(i => i != memoryPictureItem))
                {
                    _selectedSequentialPictureList.Add(memoryPictureItem);
                    //记录顺序点击信息
                    _testRecordInfo.SequentialTestingClickInfos.Add(new TestingClickInfo()
                    {
                        PictureName = Path.GetFileNameWithoutExtension(memoryPictureItem.ImageUri),
                        Location = MemoryPictureItems.IndexOf(memoryPictureItem)
                    });
                }
            }
            else
            {
                if (_selectedSequentialPictureList.Any(i => i == memoryPictureItem))
                {
                    _selectedSequentialPictureList.Remove(memoryPictureItem);
                    var testingClickInfo = _testRecordInfo.SequentialTestingClickInfos.First(i => i.PictureName == Path.GetFileNameWithoutExtension(memoryPictureItem.ImageUri));
                    _testRecordInfo.SequentialTestingClickInfos.Remove(testingClickInfo);
                }
            }

            ComfirmButton.IsEnabled = _selectedSequentialPictureList.Count >= TrainingStageSetting.ClickMaxLimit;
        }

        private void ComfirmButton_OnClick(object sender, RoutedEventArgs e)
        {
            SequentialSelected?.Invoke(this, _selectedSequentialPictureList);
        }

        #endregion

        #region 完成图片的记忆

        /// <summary>
        /// 完成图片的记忆
        /// </summary>
        public event EventHandler<List<MemorizedMemoryPictureItem>> PictureMemorized;

        /// <summary>
        /// 已经记忆过的图片
        /// </summary>
        private List<MemorizedMemoryPictureItem> _memorizedPictureList = new List<MemorizedMemoryPictureItem>();

        /// <summary>
        /// 学习时，处理图片的点击
        /// </summary>
        /// <param name="listBoxItem"></param>
        /// <param name="memoryPictureItem"></param>
        /// <returns></returns>
        private async Task HandleLearningPictureClick(ListBoxItem listBoxItem, MemoryPictureItem memoryPictureItem)
        {
            //禁用多图片翻开;同一图片，禁止再次翻开
            if (_memorizedPictureList.Count >= TrainingStageSetting.ClickMaxLimit ||
                _memorizedPictureList.Any(i => i.PictureItem == memoryPictureItem) ||
                MemoryPictureItems.Any(i => i != memoryPictureItem && i.IsHighlighted))
            {
                return;
            }

            var isPreviousPictureVisibile = memoryPictureItem.IsPictureVisibile;
            await SetMemoryPictureMarkedStatus(memoryPictureItem);
            //第一次翻开
            if (!isPreviousPictureVisibile && memoryPictureItem.IsPictureVisibile)
            {
                _testRecordInfo.LearningClickInfos.Add(new LearningClickInfo()
                {
                    ClickToVisibleTime = DateTime.Now,
                    PictureName = Path.GetFileNameWithoutExtension(memoryPictureItem.ImageUri),
                    Location = MemoryPictureItems.IndexOf(memoryPictureItem)
                });
            }
            else
            {
                var learningClickInfo = _testRecordInfo.LearningClickInfos.First(i => i.PictureName == memoryPictureItem.PictureName);
                learningClickInfo.ClickToCollapsedTime = DateTime.Now;
            }
            //如果由显示改为关闭，则说明已经记忆过此图片
            if (isPreviousPictureVisibile && !memoryPictureItem.IsPictureVisibile)
            {
                //添加到已记忆列表
                _memorizedPictureList.Add(new MemorizedMemoryPictureItem()
                {
                    PictureItem = memoryPictureItem,
                    Location = MemoryPictureItems.IndexOf(memoryPictureItem)
                });
                //图片记忆完成
                if (_memorizedPictureList.Count >= TrainingStageSetting.ClickMaxLimit)
                {
                    PictureMemorized?.Invoke(this, _memorizedPictureList);
                }
            }
        }
        public async Task SetMemoryPictureMarkedStatus(MemoryPictureItem item, int delayedMilliseconds = 500)
        {
            if (item.IsHighlighted)
            {
                item.IsPictureVisibile = false;
                await Task.Delay(TimeSpan.FromMilliseconds(delayedMilliseconds));
                item.IsHighlighted = false;
            }
            else
            {
                item.IsHighlighted = true;
                await Task.Delay(TimeSpan.FromMilliseconds(delayedMilliseconds));
                item.IsPictureVisibile = true;
            }
        }
        #endregion

        #endregion


    }
}
