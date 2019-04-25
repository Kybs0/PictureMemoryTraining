using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Aspose.Cells;

namespace PictureMemoryTraining.Business.Excel
{
    public class MemoryPicturesExcelHelper
    {
        public static void SaveMemoryTestData(UserDetailTestRecordInfo recordInfo)
        {
            if (GetExcelPath(out var excelPath))
            {
                try
                {
                    Workbook workbook = new Workbook(excelPath);
                    var workbookWorksheet = workbook.Worksheets[0];
                    Cells cells = workbookWorksheet.Cells;
                    var startRow = cells.MaxDataRow;
                    SaveOneGroupTestInfo(recordInfo.UserInfo, recordInfo.Group1TestInfo, cells, startRow++);
                    SaveOneGroupTestInfo(recordInfo.UserInfo, recordInfo.Group2TestInfo, cells, startRow++);
                    SaveOneGroupTestInfo(recordInfo.UserInfo, recordInfo.Group3TestInfo, cells, startRow++);
                    workbookWorksheet.AutoFitColumns(); //自适应宽
                    workbook.Save(excelPath, SaveFormat.Auto);
                }
                catch (Exception e)
                {
                    MessageBox.Show(e.Message);
                }
            }
        }

        private static void SaveOneGroupTestInfo(UserInfoMode userInfo, GroupTestInfo groupTestInfo, Cells cells,
            int startRow)
        {
            int columnIndex = 0;
            //记录用户信息
            cells[startRow, columnIndex++].Value = userInfo.TestCode;
            cells[startRow, columnIndex++].Value = userInfo.UserName;
            cells[startRow, columnIndex++].Value = userInfo.Age;
            cells[startRow, columnIndex++].Value = userInfo.TestDate.ToString("yyyy-MM-dd");

            //记录记忆信息
            SavePicturesUserTestInfoByStep(cells, startRow, ref columnIndex, groupTestInfo.FourPicturesUserTestRecordInfo);
            SavePicturesUserTestInfoByStep(cells, startRow, ref columnIndex, groupTestInfo.FivePicturesUserTestRecordInfo);
            SavePicturesUserTestInfoByStep(cells, startRow, ref columnIndex, groupTestInfo.SixPicturesUserTestRecordInfo);
        }

        private static void SavePicturesUserTestInfoByStep(Cells cells, int startRow, ref int columnIndex,
            UserTestRecordInfo fourPicturesTestInfo)
        {
            cells[startRow, columnIndex++].Value = fourPicturesTestInfo.StartLearningTime.ToString("yyyy-MM-dd HH:mm:ss");
            for (int i = 0; i < fourPicturesTestInfo.LearningClickInfos.Count; i++)
            {
                var learningClickInfo = fourPicturesTestInfo.LearningClickInfos[i];
                cells[startRow, columnIndex++].Value = learningClickInfo.PictureName;
                cells[startRow, columnIndex++].Value = learningClickInfo.Location;
                cells[startRow, columnIndex++].Value = learningClickInfo.ClickTime.ToString("yyyy-MM-dd HH:mm:ss");
            }

            //记录测试数据
            cells[startRow, columnIndex++].Value = fourPicturesTestInfo.StartTestingTime.ToString("yyyy-MM-dd HH:mm:ss");
            for (int i = 0; i < fourPicturesTestInfo.SequentialTestingClickInfos.Count; i++)
            {
                var clickInfo = fourPicturesTestInfo.SequentialTestingClickInfos[i];
                cells[startRow, columnIndex++].Value = clickInfo.PictureName;
                cells[startRow, columnIndex++].Value = clickInfo.Location;
                cells[startRow, columnIndex++].Value = clickInfo.ClickTime.ToString("yyyy-MM-dd HH:mm:ss");
                cells[startRow, columnIndex++].Value = clickInfo.IsRight;
            }

            for (int i = 0; i < fourPicturesTestInfo.LocationTestingClickInfos.Count; i++)
            {
                var clickInfo = fourPicturesTestInfo.LocationTestingClickInfos[i];
                cells[startRow, columnIndex++].Value = clickInfo.PictureName;
                cells[startRow, columnIndex++].Value = clickInfo.Location;
                cells[startRow, columnIndex++].Value = clickInfo.ClickTime.ToString("yyyy-MM-dd HH:mm:ss");
                cells[startRow, columnIndex++].Value = clickInfo.IsRight;
            }
        }

        private static bool GetExcelPath(out string excelPath)
        {
            var baseDirectory = AppDomain.CurrentDomain.SetupInformation.ApplicationBase;
            excelPath = Path.Combine(baseDirectory, @"Output\ZL实验结果呈现.xlsx");
            if (!File.Exists(excelPath))
            {
                MessageBox.Show("文件“ZL实验结果呈现.xlsx”未找到！");
                return false;
            }

            return true;
        }
    }
}
