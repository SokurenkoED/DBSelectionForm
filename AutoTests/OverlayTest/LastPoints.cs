using DBSelectionForm.Models;
using DBSelectionForm.Services;
using Microsoft.Win32;
using Microsoft.WindowsAPICodePack.Dialogs;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Text;
using System.Windows;

namespace AutoTests.AutoTest.OverlayTest
{
    class LastPoints
    {

        /// <summary>
        /// Тест #1. Проверка последнего значения накладки. Последний элемент не является последним элементом в динамических данных.
        /// Время последнего элемента не совпадает с заданным временем пользователя.
        /// </summary>
        /// <param name="_TextInformation"></param>
        /// <param name="TimeDemention"></param>
        /// <returns></returns>
        static public bool LastPointNO1(ref ObservableCollection<string> _TextInformation, List<string> TimeDemention)
        {

            #region Указываем начальные данные

            InfoData _InfoData = new InfoData()
            {
                DayFrom = "29.10.21",
                DayTo = "30.10.21",
                SlicePathDB = @"D:\\Archive_LAES2\\29_10_2021\\20220222_srez_29.10.txt",
                TimeFrom = "00:00:00",
                TimeTo = "12:00:00",
                PathToFolder = @"D:\\Archive_LAES2\\29_10_2021", // Начал заполнять пути до файлов
                SensorName = "10KBA20CF001_XQ01"
            };

            #endregion

            #region Указываем  то что должно получиться

            List<string> CorrectData = new List<string>();
            CorrectData.Add("129600 0.43535858855877185");

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

            if (CorrectData[0] != ResultData[ResultData.Count-1])
            {
                Console.WriteLine(@"Тест 'Правильное заполнение датчика' => 'Проверка последнего значения' => 'Тест NO1' => Не пройден! Ошибка!");
                return false;
            }
            Console.WriteLine(@"Тест 'Правильное заполнение датчика' => 'Проверка последнего значения' => 'Тест NO1' => Пройден!");
            return true;

            #endregion

        }

        /// <summary>
        /// Тест #2. Проверка последнего значения накладки. После заданного времени нет изменений для заданного сигнала.
        /// Время последнего элемента не совпадает с заданным временем пользователя.
        /// </summary>
        /// <param name="_TextInformation"></param>
        /// <param name="TimeDemention"></param>
        /// <returns></returns>
        static public bool LastPointNO2(ref ObservableCollection<string> _TextInformation, List<string> TimeDemention)
        {

            #region Указываем начальные данные

            InfoData _InfoData = new InfoData()
            {
                DayFrom = "29.10.21",
                DayTo = "08.11.21",
                SlicePathDB = @"D:\\Archive_LAES2\\29_10_2021\\20220222_srez_29.10.txt",
                TimeFrom = "00:00:00",
                TimeTo = "08:15:00",
                PathToFolder = @"D:\\Archive_LAES2\\29_10_2021", // Начал заполнять пути до файлов
                SensorName = "10KBA20CF001_XQ01"
            };

            #endregion

            #region Указываем  то что должно получиться

            List<string> CorrectData = new List<string>();
            CorrectData.Add("893700 0.4");

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

            if (CorrectData[0] != ResultData[ResultData.Count - 1])
            {
                Console.WriteLine(@"Тест 'Правильное заполнение датчика' => 'Проверка последнего значения' => 'Тест NO2' => Не пройден! Ошибка!");
                return false;
            }
            Console.WriteLine(@"Тест 'Правильное заполнение датчика' => 'Проверка последнего значения' => 'Тест NO2' => Пройден!");
            return true;

            #endregion

        }

        /// <summary>
        /// Тест #3. Проверка последнего значения накладки. Если встретился сигнал со временем равным времени timeto, заданным порльзователем
        /// </summary>
        /// <param name="_TextInformation"></param>
        /// <param name="TimeDemention"></param>
        /// <returns></returns>
        static public bool LastPointNO3(ref ObservableCollection<string> _TextInformation, List<string> TimeDemention)
        {

            #region Указываем начальные данные

            InfoData _InfoData = new InfoData()
            {
                DayFrom = "29.10.21",
                DayTo = "08.11.21",
                SlicePathDB = @"D:\\Archive_LAES2\\29_10_2021\\20220222_srez_29.10.txt",
                TimeFrom = "00:00:00",
                TimeTo = "08:13:33",
                PathToFolder = @"D:\\Archive_LAES2\\29_10_2021", // Начал заполнять пути до файлов
                SensorName = "10KBA20CF001_XQ01"
            };

            #endregion

            #region Указываем  то что должно получиться

            List<string> CorrectData = new List<string>();
            CorrectData.Add("893613 0.5");

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

            if (CorrectData[0] != ResultData[ResultData.Count - 1])
            {
                Console.WriteLine(@"Тест 'Правильное заполнение датчика' => 'Проверка последнего значения' => 'Тест NO3' => Не пройден! Ошибка!");
                return false;
            }
            Console.WriteLine(@"Тест 'Правильное заполнение датчика' => 'Проверка последнего значения' => 'Тест NO3' => Пройден!");
            return true;

            #endregion

        }


    }
}
