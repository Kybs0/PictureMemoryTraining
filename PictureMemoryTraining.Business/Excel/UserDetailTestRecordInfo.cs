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
        public DateTime StartLearningTime { get; set; }
        public List<LearningClickInfo> LearningClickInfos = new List<LearningClickInfo>()
        {
            new LearningClickInfo(),
            new LearningClickInfo(),
            new LearningClickInfo()
        };


        public DateTime StartTestingTime { get; set; }
        public List<TestingClickInfo> SequentialTestingClickInfos = new List<TestingClickInfo>()
        {
            new TestingClickInfo(),
            new TestingClickInfo(),
            new TestingClickInfo()
        };

        public List<TestingClickInfo> LocationTestingClickInfos = new List<TestingClickInfo>()        {
            new TestingClickInfo(),
            new TestingClickInfo(),
            new TestingClickInfo()
        };
    }

    public class TestingClickInfo
    {
        public string PictureName { get; set; }
        public DateTime ClickTime { get; set; }
        public bool IsRight { get; set; }
    }

    public class LearningClickInfo
    {
        public string PictureName { get; set; }
        public DateTime ClickTime { get; set; }
    }
}
