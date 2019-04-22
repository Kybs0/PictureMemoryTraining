using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PictureMemoryTraining.Business.MemoryPicturesData;

namespace PictureMemoryTraining.Views.Models
{
    public static class MemoryPictureItemsManager
    {
        private static List<MemoryPictureItem> _test1MemoryPictures = null;
        public static List<MemoryPictureItem> GetTest1MemoryPictures()
        {
            var memoryPictureItems = _test1MemoryPictures ?? (_test1MemoryPictures = GetMemoryPictures());
            return memoryPictureItems;
        }
        private static List<MemoryPictureItem> _test2MemoryPictures = null;
        public static List<MemoryPictureItem> GetTest2MemoryPictures()
        {
            var memoryPictureItems = _test2MemoryPictures ?? (_test2MemoryPictures = GetMemoryPictures());
            return memoryPictureItems;
        }

        private static List<MemoryPictureItem> _test3MemoryPictures = null;
        public static List<MemoryPictureItem> GetTest3MemoryPictures()
        {
            var memoryPictureItems = _test3MemoryPictures ?? (_test3MemoryPictures = GetMemoryPictures());
            return memoryPictureItems;
        }

        public static List<MemoryPictureItem> GetMemoryPictures()
        {
            var memoryPictures = MemoryPictureManager.GetMemoryPictures();
            var memoryPictureItems = memoryPictures.Select(i => new MemoryPictureItem()
            {
                ImageUri = i
            }).ToList();
            return memoryPictureItems;
        }
    }
}
