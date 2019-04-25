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
        private static List<MemoryPictureItem> _learning1MemoryPictures = null;
        public static List<MemoryPictureItem> GetLearning1MemoryPictures()
        {
            var memoryPictureItems = _learning1MemoryPictures ?? (_learning1MemoryPictures = GetMemoryPictures()[0]);
            return memoryPictureItems;
        }
        private static List<MemoryPictureItem> _learning2MemoryPictures = null;
        public static List<MemoryPictureItem> GetLearning2MemoryPictures()
        {
            var memoryPictureItems = _learning2MemoryPictures ?? (_learning2MemoryPictures = GetMemoryPictures()[1]);
            return memoryPictureItems;
        }

        private static List<MemoryPictureItem> _test1MemoryPictures = null;
        public static List<MemoryPictureItem> GetTest1MemoryPictures()
        {
            var memoryPictureItems = _test1MemoryPictures ?? (_test1MemoryPictures = GetMemoryPictures()[2]);
            return memoryPictureItems;
        }
        private static List<MemoryPictureItem> _test2MemoryPictures = null;
        public static List<MemoryPictureItem> GetTest2MemoryPictures()
        {
            var memoryPictureItems = _test2MemoryPictures ?? (_test2MemoryPictures = GetMemoryPictures()[3]);
            return memoryPictureItems;
        }

        private static List<MemoryPictureItem> _test3MemoryPictures = null;
        public static List<MemoryPictureItem> GetTest3MemoryPictures()
        {
            var memoryPictureItems = _test3MemoryPictures ?? (_test3MemoryPictures = GetMemoryPictures()[4]);
            return memoryPictureItems;
        }


        private static List<List<MemoryPictureItem>> _allMemoryPictures = null;
        private static List<List<MemoryPictureItem>> GetMemoryPictures()
        {
            if (_allMemoryPictures == null)
            {
                var memoryPictures = MemoryPictureManager.GetMemoryPictureGroups();
                _allMemoryPictures = memoryPictures.Select(i => i.Select(j => new MemoryPictureItem()
                {
                    ImageUri = j
                }).ToList()).ToList();
            }

            return _allMemoryPictures;
        }
    }
}
