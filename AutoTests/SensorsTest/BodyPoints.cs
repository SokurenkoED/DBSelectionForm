using DBSelectionForm.Models;
using DBSelectionForm.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Text;

namespace AutoTests.AutoTest.SensorsTest
{
    internal class BodyPoints
    {
        /// <summary>
        /// Тест #1. Проверка тела графика.
        /// </summary>
        /// <param name="_TextInformation"></param>
        /// <param name="TimeDemention"></param>
        /// <returns></returns>
        static public bool BodyTestNO1(ref ObservableCollection<string> _TextInformation, List<string> TimeDemention)
        {

            #region Указываем начальные данные

            InfoData _InfoData = new InfoData()
            {
                DayFrom = "29.10.21",
                DayTo = "08.11.21",
                SlicePathDB = @"D:\\Archive_LAES2\\29_10_2021\\20220222_srez_29.10.txt",
                TimeFrom = "00:00:00",
                TimeTo = "23:59:59",
                PathToFolder = @"D:\\Archive_LAES2\\29_10_2021", // Начал заполнять пути до файлов
                SensorName = "10KBA20CF001_XQ01"
            };

            #endregion

            #region Указываем  то что должно получиться

            List<string> CorrectData = new List<string>()
            {
                "0 0.4",
                "2650 0.5",
                "2654 0.4",
                "21290 0.5",
                "21291 0.4",
                "120340 0.5",
                "120341 0.4",
                "146527 0.5",
                "146530 0.4",
                "203261 0.5",
                "203262 0.4",
                "630640 0.5",
                "630641 0.4",
                "637070 0.5",
                "637077 0.4",
                "803236 0.5",
                "803237 0.4",
                "840102 0.5",
                "840104 0.4",
                "842569 0.5",
                "842570 0.4",
                "854133 0.5",
                "854499 0.4",
                "855668 0.5",
                "855671 0.4",
                "859694 0.5",
                "859729 0.4",
                "861185 0.5",
                "861187 0.4",
                "881993 0.5",
                "881994 0.4",
                "893613 0.5",
                "893614 0.4",
                "950399 0.4"
            };

            #endregion

            #region Запускаем расчетное ядро

            GetData.GetDataMethod(_InfoData, ref _TextInformation, _InfoData.SlicePathDB, TimeDemention[0]);

            #endregion

            #region Считываем полученный файл

            string[] TempDayFromSplit = _InfoData.DayFrom.Trim().Split(new string[] { "." }, StringSplitOptions.RemoveEmptyEntries);
            string TempDayFrom = $"20{TempDayFromSplit[2]}.{TempDayFromSplit[1]}.{TempDayFromSplit[0]}";
            List<string> ResultData = new List<string>();
            using (StreamReader sr = new StreamReader($"{_InfoData.SensorName}_{TempDayFrom.Replace(".", "-")}_{_InfoData.TimeFrom.Trim().Replace(":", "-")}.dat"))
            {
                string line;
                while ((line = sr.ReadLine()) != null)
                {
                    ResultData.Add(line);
                }
            }

            #endregion

            #region Сравниваем результаты

            if (ResultData.Count != CorrectData.Count)
            {
                Console.WriteLine(@"Тест 'Правильное заполнение датчика' => 'Проверка тела графика' => 'Тест NO1' => Не пройден! Ошибка!");
                return false;
            }

            for (int i = 0; i < ResultData.Count; i++)
            {
                if (CorrectData[i] != ResultData[i])
                {
                    Console.WriteLine(@"Тест 'Правильное заполнение датчика' => 'Проверка тела графика' => 'Тест NO1' => Не пройден! Ошибка!");
                    return false;
                }
            }

            Console.WriteLine(@"Тест 'Правильное заполнение датчика' => 'Проверка тела графика' => 'Тест NO1' => Пройден!");
            return true;

            #endregion

        }
    }
}
