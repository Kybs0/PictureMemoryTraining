using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using PictureMemoryTraining.Business;

namespace PictureMemoryTraining.Views.Models
{
    /// <summary>
    /// 记忆图片Item
    /// </summary>
    public class MemoryPictureItem : NotifyPropertyChanged
    {
        private string _imageUri = string.Empty;
        public string ImageUri
        {
            get => _imageUri;
            set
            {
                _imageUri = value;
                OnPropertyChanged();
            }
        }

        public BitmapImage Picture
        {
            get
            {
                if (string.IsNullOrEmpty(ImageUri))
                {
                    return null;
                }
                var imageNonReadOnly = ImageHelper.CreateImageNonReadOnly(ImageUri);
                return imageNonReadOnly;
            }
        }

        #region 图片显示状态

        private bool _isHighlighted = false;
        /// <summary>
        /// 是否高亮显示
        /// </summary>
        public bool IsHighlighted
        {
            get => _isHighlighted;
            set
            {
                _isHighlighted = value;
                OnPropertyChanged();
            }
        }

        private bool _isPictureVisibile = false;
        /// <summary>
        /// 是否显示图片
        /// </summary>
        public bool IsPictureVisibile
        {
            get => _isPictureVisibile;
            set
            {
                _isPictureVisibile = value;
                OnPropertyChanged();
            }
        }

        private bool _isSettingMemoryPictureVisibileStatus = false;

        public async void SetMemoryPictureVisibileStatus()
        {
            if (!_isSettingMemoryPictureVisibileStatus)
            {
                _isSettingMemoryPictureVisibileStatus = true;

                if (IsHighlighted)
                {
                    IsPictureVisibile = false;
                    await Task.Delay(TimeSpan.FromMilliseconds(500));
                    IsHighlighted = false;
                }
                else
                {
                    IsHighlighted = true;
                    await Task.Delay(TimeSpan.FromMilliseconds(500));
                    IsPictureVisibile = true;
                }

                _isSettingMemoryPictureVisibileStatus = false;
            }
        }

        #endregion
    }
}