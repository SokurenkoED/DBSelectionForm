using DBSelectionForm.Models;
using DBSelectionForm.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Text;

namespace AutoTests.AutoTest.SensorsTest
{
    class FirstPoints
    {
        static public bool FirstPointNO1(InfoData _InfoData, ref ObservableCollection<string> _TextInformation, List<string> TimeDemention)
        {

            #region Путь до дирректории теста

            string PathForTest = @"FirstPointNO1\\";

            #endregion

            #region Указываем название сигналов, время выборки, путь до БД

            _InfoData.DayFrom = "29.10.21";
            _InfoData.DayTo= "08.11.21";

            _InfoData.TimeFrom = "00:00:00";
            _InfoData.TimeTo = "23:59:59";

            _InfoData.PathToFolder = ""; // Начал заполнять пути до файлов

            _InfoData.SensorName = "10KBA20CF001_XQ01";
            string[] SensorName = _InfoData.SensorName.Split(new string[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries);

            #endregion


            #region Создаем файлы с корректными данными для сравнения

            List<string> CorrectData = new List<string>();

            #endregion

            #region Запускаем расчетное ядро

            GetData.GetDataMethod(_InfoData, ref _TextInformation, _InfoData.SlicePathDB, TimeDemention[0], PathForTest);

            #endregion

            for (int i = 0; i < SensorName.Length; i++)
            {

                #region Считываем полученный файл

                List<string> ResultData = new List<string>();
                using (StreamReader sr = new StreamReader($"{PathForTest}{_InfoData.SensorName}_{_InfoData.TimeFrom.Trim().Replace(":", "-")}.dat"))
                {
                    string line;
                    while ((line = sr.ReadLine()) != null)
                    {
                        ResultData.Add(line);
                    }
                }

                #endregion

                #region Сравниваем результаты с правильными данными

                if (CorrectData[0] != ResultData[0])
                {
                    Console.WriteLine(@"Тест 'Правильное заполнение датчика' => 'Проверка первого значения' => 'Тест NO1' => Не пройден! Ошибка!");
                    return false;
                }

                #endregion

            }

            #region Возвращаем результат теста

            Console.WriteLine(@"Тест 'Правильное заполнение датчика' => 'Проверка первого значения' => 'Тест NO1' => Пройден!");
            return true;

            #endregion

        }
    }
}
