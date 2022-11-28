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

namespace AutoTests.AutoTest.ValvesTest
{
    class FirstPoints
    {

        /// <summary>
        /// Тест #1. Проверка первого значения сигнала задвижки. Если timefrom совпадает со временем первого элемента выборки.
        /// </summary>
        /// <param name="_TextInformation"></param>
        /// <param name="TimeDemention"></param>
        /// <returns></returns>
        static public bool FirstPointNO1(ref ObservableCollection<string> _TextInformation, List<string> TimeDemention)
        {

            #region Указываем начальные данные

            InfoData _InfoData = new InfoData()
            {
                DayFrom = "02.11.21",
                DayTo = "08.11.21",
                SlicePathDB = @"D:\\Archive_LAES2\\29_10_2021\\20220222_srez_29.10.txt",
                TimeFrom = "07:22:38",
                TimeTo = "23:59:59",
                PathToFolder = @"D:\\Archive_LAES2\\29_10_2021", // Начал заполнять пути до файлов
                SensorName = "10KBA15AA101_Z0"
            };

            #endregion

            #region Указываем  то что должно получиться

            List<string> CorrectData = new List<string>();
            CorrectData.Add("0 100");

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

            if (CorrectData[0] != ResultData[0])
            {
                Console.WriteLine(@"Тест 'Правильное заполнение задвижки' => 'Проверка первого значения' => 'Тест NO1' => Не пройден! Ошибка!");
                return false;
            }
            Console.WriteLine(@"Тест 'Правильное заполнение задвижки' => 'Проверка первого значения' => 'Тест NO1' => Пройден!");
            return true;

            #endregion

        }


        /// <summary>
        /// Тест #2. Проверка первого значения сигнала задвижки. Если timefrom не совпадает со временем первого элемента выборки. Проводим линейную интерполяцию для определения значения.
        /// </summary>
        /// <param name="_TextInformation"></param>
        /// <param name="TimeDemention"></param>
        /// <returns></returns>
        static public bool FirstPointNO2(ref ObservableCollection<string> _TextInformation, List<string> TimeDemention)
        {

            #region Указываем начальные данные

            InfoData _InfoData = new InfoData()
            {
                DayFrom = "30.10.21",
                DayTo = "08.11.21",
                SlicePathDB = @"D:\\Archive_LAES2\\29_10_2021\\20220222_srez_29.10.txt",
                TimeFrom = "08:09:53",
                TimeTo = "23:59:59",
                PathToFolder = @"D:\\Archive_LAES2\\29_10_2021",
                SensorName = "10KBA15AA101_Z0"
            };

            #endregion

            #region Указываем  то что должно получиться

            List<string> CorrectData = new List<string>();
            CorrectData.Add("0 не определено");

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

            if (CorrectData[0] != ResultData[0])
            {
                Console.WriteLine(@"Тест 'Правильное заполнение задвижки' => 'Проверка первого значения' => 'Тест NO2' => Не пройден! Ошибка!");
                return false;
            }
            Console.WriteLine(@"Тест 'Правильное заполнение задвижки' => 'Проверка первого значения' => 'Тест NO2' => Пройден!");
            return true;

            #endregion

        }

        /// <summary>
        /// Тест #3. Проверка первого значения сигнала. Если первое значение берем со временем timefrom которое до первого значения в динамическом срезе.
        /// </summary>
        /// <param name="_TextInformation"></param>
        /// <param name="TimeDemention"></param>
        /// <returns></returns>
        static public bool FirstPointNO3(ref ObservableCollection<string> _TextInformation, List<string> TimeDemention)
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
                SensorName = "10KBA15AA101_Z0"
            };

            #endregion

            #region Указываем  то что должно получиться

            List<string> CorrectData = new List<string>();
            CorrectData.Add("0 0");

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

            if (CorrectData[0] != ResultData[0])
            {
                Console.WriteLine(@"Тест 'Правильное заполнение задвижки' => 'Проверка первого значения' => 'Тест NO3' => Не пройден! Ошибка!");
                return false;
            }
            Console.WriteLine(@"Тест 'Правильное заполнение задвижки' => 'Проверка первого значения' => 'Тест NO3' => Пройден!");
            return true;

            #endregion

        }


    }
}
