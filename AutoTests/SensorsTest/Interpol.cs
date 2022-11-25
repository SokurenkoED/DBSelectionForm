using DBSelectionForm.Services;
using System;
using System.Collections.Generic;
using System.Text;

namespace AutoTests.SensorsTest
{
    internal class Interpol
    {
        /// <summary>
        /// Проверка работы интерполяции для датчиков
        /// </summary>
        /// <returns></returns>
        static public bool InterpolTestNO1()
        {
            TimeValueData TVD1 = new TimeValueData();
            TVD1.DataValue = "0";
            TVD1.DataTime = new DateTime(2022, 10, 1);

            TimeValueData TVD2 = new TimeValueData();
            TVD2.DataValue = "1000";
            TVD2.DataTime = new DateTime(2022, 10, 3);

            DateTime XDateTime = new DateTime(2022, 10, 2);

            string ResultValue = GetData.LineInterpol(TVD1, TVD2, XDateTime);
            if (ResultValue != "500")
            {
                Console.WriteLine(@"Тест 'Правильное заполнение датчика' => 'Проверка интерполирования' => 'Тест NO1' => Не пройден! Ошибка!");
                return false;
            }

            Console.WriteLine(@"Тест 'Правильное заполнение датчика' => 'Проверка интерполирования' => 'Тест NO1' => Пройден!");
            return true;

        }
    }
}
