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
            #region Инициализация

            InfoData _InfoData = new InfoData()
            {
                DayFrom = "",
                DayTo = "",
                SlicePathDB = @"D:\Archive_LAES2\out_06-09_07_2018\срез_06_07_2018.txt",
                TimeFrom = "",
                TimeTo = "",
                PathToFolder = @"D:\Archive_LAES2\out_06-09_07_2018", // Начал заполнять пути до файлов
                SensorName = ""
            };

            #endregion

            #region Запись эталонного массива

            List<string> ResultData = new List<string>()
            {   "АРМ админ. СВБУ (20CKD01)",
                "Имя пользователя: ?, Имя отчета: 113011_181021 ",
                "Создано: 18.10.2021 11:32:51, Выборка: 06.07.18 00:00:00 - 09.07.18 23:59:59 @20CKM01",
                $"Время\tИмя\tЗначение\tКачество\tСобытие\tСтатус\tПодпись\t",
                "06.07.18 00:00:00,000\t10JAA10FP911_XQ01\t-0.0\tвых за диап\tД > НДдиап\t\tСКОР ИЗМ Р НАД АКТ ЗОН 0-25 МПa/ч\t" };

            #endregion

            #region Получение реальных данных

            string filePath = Directory.GetFiles(_InfoData.PathToFolder)[0];
            string filename = Path.GetFileName(filePath);
            List<string> ListData = new List<string>();
            using (StreamReader sr = new StreamReader($"{_InfoData.PathToFolder}/{filename}", GetData.GetEncoding($"{_InfoData.PathToFolder}/{filename}")))
            {
                string line;
                int iter = 0;
                while ((line = sr.ReadLine()) != null && iter < 5)
                {
                    ListData.Add(line);
                    iter++;
                }
            }

            #endregion

            #region Сравнение результатов

            if (ResultData.Count != ListData.Count)
            {
                Console.WriteLine(@"Тест 'Проверка кодировки файла' => 'Архив 06-07-2018' => 'Тест NO1' => Не пройден! Ошибка!");
                return false;
            }
            for (int i = 0; i < ResultData.Count; i++)
            {
                if (ResultData[i] != ListData[i])
                {
                    Console.WriteLine(@"Тест 'Проверка кодировки файла' => 'Архив 06-07-2018' => 'Тест NO1' => Не пройден! Ошибка!");
                    return false;
                }
            }
            Console.WriteLine(@"Тест 'Проверка кодировки файла' => 'Архив 06-07-2018' => 'Тест NO1' => Пройден!");
            return true;

            #endregion

        }

        /// <summary>
        /// Проверка правильного декодирования динамических файлов среза 29_10_2021  
        /// </summary>
        /// <returns></returns>
        static public bool EncodingTestNO2()
        {
            #region Инициализация

            InfoData _InfoData = new InfoData()
            {
                DayFrom = "",
                DayTo = "",
                SlicePathDB = @"D:\Archive_LAES2\29_10_2021\20220222_srez_29.10.txt",
                TimeFrom = "",
                TimeTo = "",
                PathToFolder = @"D:\Archive_LAES2\29_10_2021", // Начал заполнять пути до файлов
                SensorName = ""
            };

            #endregion

            #region Запись эталонного массива

            List<string> ResultData = new List<string>()
            {   "Устройство передачи данных-1",
                "Имя пользователя: PLS, Имя протокола: 094931_281221 ",
                "Создано: 28.12.2021 10:18:54, Выборка: 01.11.21 00:00:00 - 03.11.21 23:59:59",
                $"Время\tKKS\tЗнач\tЕд.изм\tДост\tОписание\t",
                "01.11.21 00:00:00,075\t10JKS06FX077_XQ01\t1.16\t\tдост\tОтносительные энерговыделения ТВС\t"
            };

            #endregion

            #region Получение реальных данных

            string filePath = Directory.GetFiles(_InfoData.PathToFolder)[0];
            string filename = Path.GetFileName(filePath);
            List<string> ListData = new List<string>();
            using (StreamReader sr = new StreamReader($"{_InfoData.PathToFolder}/{filename}", GetData.GetEncoding($"{_InfoData.PathToFolder}/{filename}")))
            {
                string line;
                int iter = 0;
                while ((line = sr.ReadLine()) != null && iter < 5)
                {
                    ListData.Add(line);
                    iter++;
                }
            }

            #endregion

            #region Сравнение результатов

            if (ResultData.Count != ListData.Count)
            {
                Console.WriteLine(@"Тест 'Проверка кодировки файла' => 'Архив 29_10_2021' => 'Тест NO1' => Не пройден! Ошибка!");
                return false;
            }
            for (int i = 0; i < ResultData.Count; i++)
            {
                if (ResultData[i] != ListData[i])
                {
                    Console.WriteLine(@"Тест 'Проверка кодировки файла' => 'Архив 29_10_2021' => 'Тест NO1' => Не пройден! Ошибка!");
                    return false;
                }
            }
            Console.WriteLine(@"Тест 'Проверка кодировки файла' => 'Архив 29_10_2021' => 'Тест NO1' => Пройден!");
            return true;

            #endregion

        }
    }
}
