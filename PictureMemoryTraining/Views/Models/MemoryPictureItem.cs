using System;
using System.Collections.Generic;
using System.IO;
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
        /// <summary>
        /// 图片名称
        /// </summary>
        public string PictureName => Path.GetFileNameWithoutExtension(ImageUri);


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
                if (value)
                {
                    //图片显示时，覆盖状态取消
                    IsPictureCovered = false;
                }
                OnPropertyChanged();
            }
        }
        private bool _isPictureEnabled = true;
        /// <summary>
        /// 图片元素是否可操作
        /// </summary>
        public bool IsPictureEnabled
        {
            get => _isPictureEnabled;
            set
            {
                _isPictureEnabled = value;
                OnPropertyChanged();
            }
        }

        private bool _isPictureCovered = true;
        /// <summary>
        /// 图片元素是否可显示被覆盖
        /// </summary>
        public bool IsPictureCovered
        {
            get => _isPictureCovered;
            set
            {
                _isPictureCovered = value;
                OnPropertyChanged();
            }
        }

        #endregion
    }
}