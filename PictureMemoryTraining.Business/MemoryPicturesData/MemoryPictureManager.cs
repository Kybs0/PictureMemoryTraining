using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using PictureMemoryTraining.Utils;

namespace PictureMemoryTraining.Business.MemoryPicturesData
{
    public class MemoryPictureManager
    {
        /// <summary>
        /// 获取指定数量的记忆图片
        /// </summary>
        /// <param name="count"></param>
        /// <returns></returns>
        public static List<string> GetMemoryPictures(int count = 16)
        {
            var list = new List<string>();

            var allPictures = GetAllPictures();
            if (allPictures.Count < count)
            {
                throw new InvalidOperationException($"图片配置数量小于{count}张！");
            }

            while (list.Count < 16)
            {
                var random = new Random();
                var randomIndex = random.Next(allPictures.Count);
                if (list.All(i => i != allPictures[randomIndex]))
                {
                    list.Add(allPictures[randomIndex]);
                }
            }

            return list;
        }

        /// <summary>
        /// 获取指定数量的记忆图片
        /// </summary>
        /// <param name="count"></param>
        /// <returns></returns>
        public static List<List<string>> GetMemoryPictureGroups(int count = 80)
        {
            var allGroups = new List<List<string>>();

            var allPictures = GetAllPictures();
            if (allPictures.Count < count)
            {
                throw new InvalidOperationException($"图片配置数量小于{count}张！");
            }
            
            var random = new Random();
            for (int gropIndex = 0; gropIndex < 5; gropIndex++)
            {
                List<string> list = new List<string>();
                while (list.Count < 16)
                {
                    var randomIndex = random.Next(allPictures.Count);
                    var picture = allPictures[randomIndex];
                    if (list.All(i => i != picture))
                    {
                        list.Add(picture);
                        allPictures.Remove(picture);
                    }
                }
                allGroups.Add(list);
            }

            return allGroups;
        }
        private static List<string> GetAllPictures()
        {
            var list = new List<string>();
            var baseDirectory = AppDomain.CurrentDomain.SetupInformation.ApplicationBase;
            var memoryPicturesFolder = Path.Combine(baseDirectory, @"Resources\MemoryPictures");
            if (Directory.Exists(memoryPicturesFolder))
            {
                var allFiles = FolderUtil.GetAllFiles(memoryPicturesFolder);
                list.AddRange(allFiles);
            }

            return list;
        }
    }
}
