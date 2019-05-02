using System;
using System.Collections.Generic;

namespace PictureMemoryTraining.Business.Excel
{
    public class UserDetailTestRecordInfo
    {
        public UserInfoMode UserInfo { get; set; }

        public GroupTestInfo Group1TestInfo = new GroupTestInfo();
        public GroupTestInfo Group2TestInfo = new GroupTestInfo();
        public GroupTestInfo Group3TestInfo = new GroupTestInfo();
    }

    public class GroupTestInfo
    {
        public UserTestRecordInfo FourPicturesUserTestRecordInfo = new UserTestRecordInfo();
        public UserTestRecordInfo FivePicturesUserTestRecordInfo = new UserTestRecordInfo();
        public UserTestRecordInfo SixPicturesUserTestRecordInfo = new UserTestRecordInfo();
    }

    public class UserTestRecordInfo
    {
        public List<LearningClickInfo> LearningClickInfos = new List<LearningClickInfo>();
        public DateTime StartTestingTime { get; set; }
        public List<TestingClickInfo> SequentialTestingClickInfos = new List<TestingClickInfo>();
        public List<TestingClickInfo> LocationTestingClickInfos = new List<TestingClickInfo>();

        //public List<LearningClickInfo> LearningClickInfos = new List<LearningClickInfo>()
        //{
        //    new LearningClickInfo(),
        //    new LearningClickInfo(),
        //    new LearningClickInfo()
        //};
        //public DateTime StartTestingTime { get; set; }
        //public List<TestingClickInfo> SequentialTestingClickInfos = new List<TestingClickInfo>()
        //{
        //    new TestingClickInfo(),
        //    new TestingClickInfo(),
        //    new TestingClickInfo()
        //};
        //public List<TestingClickInfo> LocationTestingClickInfos = new List<TestingClickInfo>()        {
        //    new TestingClickInfo(),
        //    new TestingClickInfo(),
        //    new TestingClickInfo()
        //};
    }
    public class TestingClickInfo
    {
        /// <summary>
        /// 图片名称
        /// </summary>
        public string PictureName { get; set; }
        /// <summary>
        /// 点击时图片所在的位置
        /// </summary>
        public int Location { get; set; }
        public DateTime ClickToVisibleTime { get; set; }
        public DateTime ClickToCollapsedTime { get; set; }
        /// <summary>
        /// 用户确认-是否
        /// </summary>
        public bool IsMatchedByUserComfirmed { get; set; }

        /// <summary>
        /// 是否正确
        /// </summary>
        public bool IsRight { get; set; }
    }

    public class LearningClickInfo
    {
        public string PictureName { get; set; }
        /// <summary>
        /// 点击时图片所在的位置
        /// </summary>
        public int Location { get; set; }
        /// <summary>
        /// 显示时间
        /// </summary>
        public DateTime ClickToVisibleTime { get; set; }
        /// <summary>
        /// 隐藏时间
        /// </summary>
        public DateTime ClickToCollapsedTime { get; set; }
    }
}
