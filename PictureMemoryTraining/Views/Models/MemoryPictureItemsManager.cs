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
