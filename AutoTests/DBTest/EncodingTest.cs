using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using DBSelectionForm.Models;
using DBSelectionForm.Services;

namespace AutoTests.DBTest
{
    internal class EncodingTest
    {
        /// <summary>
        /// Проверка правильного декодирования динамических файлов среза 06-09.08.2022  
        /// </summary>
        /// <returns></returns>
        static public bool EncodingTestNO1()
        {
            InfoData _InfoData = new InfoData()
            {
                DayFrom = "",
                DayTo = "",
                SlicePathDB = @"",
                TimeFrom = "",
                TimeTo = "",
                PathToFolder = @"D:\Archive_LAES2\out_06-09_07_2018", // Начал заполнять пути до файлов
                SensorName = ""
            };

            string[]  filePaths = Directory.GetFiles(_InfoData.PathToFolder);

            foreach (string item in filePaths)
            {
                string filename = Path.GetFileName(item);
                using (StreamReader sr = new StreamReader($"{_InfoData.PathToFolder}/{filename}", GetData.GetEncoding($"{_InfoData.PathToFolder}/{filename}")))
                {
                    string Text = sr.ReadToEnd();
                    Console.WriteLine();
                }
            }

            return true;
        }
    }
}
