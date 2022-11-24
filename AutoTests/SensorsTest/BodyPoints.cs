using DBSelectionForm.Models;
using DBSelectionForm.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Text;

namespace AutoTests.SensorsTest
{
    internal class BodyPoints
    {
        /// <summary>
        /// Тест #1. Проверка тела графика.
        /// </summary>
        /// <param name="_TextInformation"></param>
        /// <param name="TimeDemention"></param>
        /// <returns></returns>
        static public bool FirstPointNO1(ref ObservableCollection<string> _TextInformation, List<string> TimeDemention)
        {

            #region Указываем начальные данные

            InfoData _InfoData = new InfoData()
            {
                DayFrom = "29.10.21",
                DayTo = "31.10.21",
                SlicePathDB = @"D:\\Archive_LAES2\\29_10_2021\\20220222_srez_29.10.txt",
                TimeFrom = "00:00:00",
                TimeTo = "09:00:00",
                PathToFolder = @"D:\\Archive_LAES2\\29_10_2021", // Начал заполнять пути до файлов
                SensorName = "10KBA20CF001_XQ01"
            };

            #endregion

            #region Указываем  то что должно получиться

            List<string> CorrectData = new List<string>()
            {
                "",
                ""
            };
            //Перезаписать эталонные значения для элементов изза изменения работы 

            #endregion

            #region Запускаем расчетное ядро

            GetData.GetDataMethod(_InfoData, ref _TextInformation, _InfoData.SlicePathDB, TimeDemention[0]);

            #endregion

            #region Считываем полученный файл

            List<string> ResultData = new List<string>();
            using (StreamReader sr = new StreamReader($"{_InfoData.SensorName}_{_InfoData.TimeFrom.Trim().Replace(":", "-")}.dat"))
            {
                string line;
                while ((line = sr.ReadLine()) != null)
                {
                    ResultData.Add(line);
                }
            }

            #endregion

            #region Сравниваем результаты

            if (CorrectData[0] != ResultData[0])
            {
                Console.WriteLine(@"Тест 'Правильное заполнение датчика' => 'Проверка тела графика' => 'Тест NO1' => Не пройден! Ошибка!");
                return false;
            }
            Console.WriteLine(@"Тест 'Правильное заполнение датчика' => 'Проверка тела графика' => 'Тест NO1' => Пройден!");
            return true;

            #endregion

        }
    }
}
