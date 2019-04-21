using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PictureMemoryTraining.Views.Models
{
    public class MemorizedMemoryPictureItem
    {
        public MemoryPictureItem PictureItem { get; set; }
        public int Location { get; set; }
    }

    public class SequentialMemoryPictureItem
    {
        public MemoryPictureItem PictureItem { get; set; }
        public bool Selected { get; set; }
    }
    public class LocationMemoryPictureItem
    {
        public MemoryPictureItem PictureItem { get; set; }
        public int Location { get; set; }
        /// <summary>
        /// 是否与原有位置一致（此为用户提交）
        /// </summary>
        public bool IsMatchedByUserComfirmed { get; set; }
    }
}
