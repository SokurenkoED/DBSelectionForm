﻿using DBSelectionForm.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Threading;
using System.Xml.Linq;

namespace DBSelectionForm.Services
{
    class TimeValueData
    {
        public DateTime DataTime;
        string _DataValue;
        public string DataValue 
        {
            get
            {
                if (_DataValue == "ДА")
                {
                    return "1";
                }
                else if (_DataValue == "НЕТ")
                {
                    return "0";
                }
                else
                {
                    return _DataValue;
                }
                
            }
            set
            {
                _DataValue = value;
            }
        }
    }
    class GetData
    {

        private static Encoding GetEncoding(string path) // получаем кодировку файла перед чтением
        {
            BinaryReader instr = new BinaryReader(File.OpenRead(path));
            byte[] data = instr.ReadBytes((int)instr.BaseStream.Length);
            instr.Close();

            // определяем BOM (EF BB BF)
            if (data.Length > 2 && data[0] == 0xef && data[1] == 0xbb && data[2] == 0xbf)
            {
                if (data.Length != 3) return Encoding.UTF8;
                else return Encoding.Default;
            }

            int i = 0;
            while (i < data.Length - 1)
            {
                if (data[i] > 0x7f)
                { // не ANSI-символ
                    if ((data[i] >> 5) == 6)
                    {
                        if ((i > data.Length - 2) || ((data[i + 1] >> 6) != 2))
                            return Encoding.GetEncoding(1251);
                        i++;
                    }
                    else if ((data[i] >> 4) == 14)
                    {
                        if ((i > data.Length - 3) || ((data[i + 1] >> 6) != 2) || ((data[i + 2] >> 6) != 2))
                            return Encoding.GetEncoding(1251);
                        i += 2;
                    }
                    else if ((data[i] >> 3) == 30)
                    {
                        if ((i > data.Length - 4) || ((data[i + 1] >> 6) != 2) || ((data[i + 2] >> 6) != 2) || ((data[i + 3] >> 6) != 2))
                            return Encoding.GetEncoding(1251);
                        i += 3;
                    }
                    else
                    {
                        return Encoding.GetEncoding(1251);
                    }
                }
                i++;
            }

            return Encoding.UTF8;
        }

        private static string LineInterpol(TimeValueData Values1, TimeValueData Values2, DateTime X) // Линейная интерполяция
        {
            double result;
            bool IsDouble_Values1 = Double.TryParse(Values1.DataValue, out result);
            bool IsDouble_Values2 = Double.TryParse(Values2.DataValue, out result);
            var var_dt = Values1.DataTime.Subtract(Values2.DataTime);
            var another_var_dt = X.Subtract(Values1.DataTime);
            if (IsDouble_Values1 != false && IsDouble_Values2 != false)
            {
                return (((double.Parse(Values1.DataValue) - double.Parse(Values2.DataValue)) / (var_dt.Seconds + var_dt.Minutes * 60 + var_dt.Hours * 60 * 60 + var_dt.Days * 24 * 60 * 60)) * (another_var_dt.Seconds + another_var_dt.Minutes * 60 + another_var_dt.Hours * 60 * 60 + var_dt.Days * 24 * 60 * 60) + double.Parse(Values1.DataValue)).ToString();
            }
            else
            {
                if (IsDouble_Values1 == false)
                {
                    return Values1.DataValue;
                }
                else
                {
                    return Values2.DataValue;
                }
            }
        }
        private static string ConvertDataFormat(string OldFormat, string Day, IFormatProvider formatter)
        {
            string[] SplitStr = OldFormat.Trim().Split(new string[] { ":" }, StringSplitOptions.RemoveEmptyEntries);
            return ((double.Parse(Day, formatter) - 6) * 3600 * 24 + double.Parse(SplitStr[0], formatter) * 3600 + double.Parse(SplitStr[1], formatter) * 60 + double.Parse(SplitStr[2], formatter)).ToString();
        }
        private static double ConvertDementionToDouble(string Demention) // конвертируем строкове определение единичного отрезка времени в числовое значение
        {
            if (Demention == "сек")
            {
                return 1;
            }
            if (Demention == "мин")
            {
                return 60;
            }
            return 3600;
        }
        public static void GetDataMethod(InfoData _InfoData, ref ObservableCollection<string> _TextInformation, string SlicePath, string TimeDemention)
        {

            #region Настроечные данные

            System.Threading.Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo("en-US");
            IFormatProvider formatter = new NumberFormatInfo { NumberDecimalSeparator = "." };
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

            #endregion

            // проверка на временные интервалы
            if (_InfoData.DayFrom == null || _InfoData.TimeFrom == null || _InfoData.DayTo == null || _InfoData.DayFrom == null)
            {
                MessageBox.Show("Не введены временные интервалы");
                return;
            }

            #region Получаем допустимый временной интервал

            var CI = new CultureInfo("de_DE");
            List<string> AcceptableDate = CheckAccectableTime(_InfoData.PathToFolder);
            DateTime AcceptableTimeFrom = DateTime.Parse($"{AcceptableDate[0]} {AcceptableDate[2]}", CI);
            DateTime AcceptableTimeTo = DateTime.Parse($"{AcceptableDate[1]} {AcceptableDate[3]}", CI);

            #endregion

            _TextInformation.Clear();

            #region Ввод данных из модели

            string[] SensorName = _InfoData.SensorName.Split(new string[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries);
            string TempTimeFrom = _InfoData.TimeFrom.Trim();

            DateTime DT_From = DateTime.Now;
            DateTime DT_To = DateTime.Now;

            try
            {
                DT_From = new DateTime(
                    int.Parse(_InfoData.DayFrom.Split(new string[] { "." }, StringSplitOptions.RemoveEmptyEntries)[2]) + 2000,
                    int.Parse(_InfoData.DayFrom.Split(new string[] { "." }, StringSplitOptions.RemoveEmptyEntries)[1]),
                    int.Parse(_InfoData.DayFrom.Split(new string[] { "." }, StringSplitOptions.RemoveEmptyEntries)[0]),
                    int.Parse(_InfoData.TimeFrom.Split(new string[] { ":" }, StringSplitOptions.RemoveEmptyEntries)[0]),
                    int.Parse(_InfoData.TimeFrom.Split(new string[] { ":" }, StringSplitOptions.RemoveEmptyEntries)[1]),
                    int.Parse(_InfoData.TimeFrom.Split(new string[] { ":", "," }, StringSplitOptions.RemoveEmptyEntries)[2])
                );
                DT_To = new DateTime(
                    int.Parse(_InfoData.DayTo.Split(new string[] { "." }, StringSplitOptions.RemoveEmptyEntries)[2]) + 2000,
                    int.Parse(_InfoData.DayTo.Split(new string[] { "." }, StringSplitOptions.RemoveEmptyEntries)[1]),
                    int.Parse(_InfoData.DayTo.Split(new string[] { "." }, StringSplitOptions.RemoveEmptyEntries)[0]),
                    int.Parse(_InfoData.TimeTo.Split(new string[] { ":" }, StringSplitOptions.RemoveEmptyEntries)[0]),
                    int.Parse(_InfoData.TimeTo.Split(new string[] { ":" }, StringSplitOptions.RemoveEmptyEntries)[1]),
                    int.Parse(_InfoData.TimeTo.Split(new string[] { ":", "," }, StringSplitOptions.RemoveEmptyEntries)[2])
                    );
            }
            catch (FormatException)
            {
                MessageBox.Show("Введен неверный формат временного интервала!");
                return;
            }

            #endregion

            #region Проверка на правильность ввода временного интервала

            if (DT_From < AcceptableTimeFrom || DT_To > AcceptableTimeTo)
            {
                MessageBox.Show("Указан неверный временной интервал");
                return;
            }

            #endregion

            #region Проверка на пустое значение в имени датчиков

            if (String.IsNullOrEmpty(_InfoData.SensorName))
            {
                MessageBox.Show("Введите имена датчиков!");
                return;
            }

            #endregion

            #region Setup

            List<string[]> ListData = new List<string[]>();
            List<TimeValueData> NewListData = new List<TimeValueData>();
            List<DateTime> Dates = new List<DateTime>();
            double Date;
            string[] example;
            string RuteName;
            string RelatePath = _InfoData.PathToFolder;
            string[] filePaths = null;
            try
            {
                filePaths = Directory.GetFiles(RelatePath);
            }
            catch (DirectoryNotFoundException)
            {
                MessageBox.Show($"Нет файлов по адресу {RelatePath}");
                return;
            }
            
            List<string> ColdReactor = new List<string>(); // Не обновляем
            _TextInformation.Clear();

            #endregion

            for (int k = 0; k < SensorName.Length; k++)
            {

                ListData.Clear();
                NewListData.Clear();


                try
                {
                    RuteName = SensorName[k].Substring(2, 3);
                }
                catch (ArgumentOutOfRangeException)
                {
                    MessageBox.Show($"Введено некорректное название датчика '{SensorName[k]}'");
                    return;
                }
                
                _TextInformation.Add($"{_TextInformation.Count + 1}) Началось составление файла для датчика {SensorName[k]}.");

                #region Запись данных в массив

                foreach (string item in filePaths)
                {

                    string filename = Path.GetFileName(item);
                    if (filename.IndexOf(RuteName) != -1 && !filename.Contains(".dat"))
                    {

                        using (StreamReader sr = new StreamReader($"{RelatePath}/{filename}", GetEncoding($"{RelatePath}/{filename}")))
                        {

                            string line;
                            while ((line = sr.ReadLine()) != null)
                            {

                                // если элемент - ЗАДВИЖКА
                                if (SensorName[k].Contains("AA1") && SensorName[k].Contains("_Z0"))
                                {
                                    example = line.Split(new string[] { "\t", " " }, StringSplitOptions.RemoveEmptyEntries);
                                    if (String.Compare(example[2], $"{SensorName[k]}#0") == 0)
                                    {
                                        DateTime DT = new DateTime(
                                            int.Parse(example[0].Split(new string[] { "." }, StringSplitOptions.RemoveEmptyEntries)[2]) + 2000,
                                            int.Parse(example[0].Split(new string[] { "." }, StringSplitOptions.RemoveEmptyEntries)[1]),
                                            int.Parse(example[0].Split(new string[] { "." }, StringSplitOptions.RemoveEmptyEntries)[0]),
                                            int.Parse(example[1].Split(new string[] { ":" }, StringSplitOptions.RemoveEmptyEntries)[0]),
                                            int.Parse(example[1].Split(new string[] { ":" }, StringSplitOptions.RemoveEmptyEntries)[1]),
                                            int.Parse(example[1].Split(new string[] { ":", "," }, StringSplitOptions.RemoveEmptyEntries)[2])
                                            );

                                        NewListData.Add(new TimeValueData()
                                        {
                                            DataTime = DT,
                                            DataValue = "0"
                                        });
                                    }
                                    else if (String.Compare(example[2], $"{SensorName[k]}#1") == 0)
                                    {
                                        DateTime DT = new DateTime(
                                            int.Parse(example[0].Split(new string[] { "." }, StringSplitOptions.RemoveEmptyEntries)[2]) + 2000,
                                            int.Parse(example[0].Split(new string[] { "." }, StringSplitOptions.RemoveEmptyEntries)[1]),
                                            int.Parse(example[0].Split(new string[] { "." }, StringSplitOptions.RemoveEmptyEntries)[0]),
                                            int.Parse(example[1].Split(new string[] { ":" }, StringSplitOptions.RemoveEmptyEntries)[0]),
                                            int.Parse(example[1].Split(new string[] { ":" }, StringSplitOptions.RemoveEmptyEntries)[1]),
                                            int.Parse(example[1].Split(new string[] { ":", "," }, StringSplitOptions.RemoveEmptyEntries)[2])
                                            );

                                        NewListData.Add(new TimeValueData()
                                        {
                                            DataTime = DT,
                                            DataValue = "100"
                                        });
                                    }
                                }
                                // если другой элемент
                                else
                                {
                                    if (String.Compare(line.Split(new string[] { "\t", " " }, StringSplitOptions.RemoveEmptyEntries)[2], SensorName[k]) == 0)
                                    {

                                        #region Определили время

                                        example = line.Split(new string[] { "\t", " " }, StringSplitOptions.RemoveEmptyEntries);
                                        DateTime DT = new DateTime(
                                            int.Parse(example[0].Split(new string[] { "." }, StringSplitOptions.RemoveEmptyEntries)[2]) + 2000,
                                            int.Parse(example[0].Split(new string[] { "." }, StringSplitOptions.RemoveEmptyEntries)[1]),
                                            int.Parse(example[0].Split(new string[] { "." }, StringSplitOptions.RemoveEmptyEntries)[0]),
                                            int.Parse(example[1].Split(new string[] { ":" }, StringSplitOptions.RemoveEmptyEntries)[0]),
                                            int.Parse(example[1].Split(new string[] { ":" }, StringSplitOptions.RemoveEmptyEntries)[1]),
                                            int.Parse(example[1].Split(new string[] { ":", "," }, StringSplitOptions.RemoveEmptyEntries)[2])
                                            );

                                        #endregion

                                        #region Добавили значение в массив 

                                        NewListData.Add(new TimeValueData()
                                        {
                                            DataTime = DT,
                                            DataValue = example[3]
                                        });

                                        #endregion

                                    }
                                }
                            }
                        }
                    }
                }


                using (StreamReader sr = new StreamReader($"{SlicePath}", GetEncoding(SlicePath))) // Поиск по срезу для холодного реактора
                {
                    string line;
                    int KeyVar = -1; // 0 - старый формат, 1 - новый формат
                    int m = 0;
                    while ((line = sr.ReadLine()) != null)
                    {
                        string[] split_str = line.Split(new string[] { "\t" }, StringSplitOptions.RemoveEmptyEntries);
                        if (m == 3)
                        {
                            if (split_str[1] == "Время")
                            {
                                KeyVar = 0;
                            }
                            else
                            {
                                KeyVar = 1;
                            }
                            
                        }
                        if (line.StartsWith(SensorName[k]) && KeyVar == 0)
                        {
                            if (line.StartsWith($"{SensorName[k]}#0"))
                            {
                                if (split_str[2] == "НЕТ")
                                {
                                    ColdReactor.Add("100");
                                    break;
                                }
                                else if (split_str[2] == "ДА")
                                {
                                    ColdReactor.Add("0");
                                    break;
                                }
                            }
                            else
                            {
                                ColdReactor.Add(split_str[2]);
                                break;
                            }
                            

                        } else if (line.StartsWith(SensorName[k]) && KeyVar == 1)
                        {
                            if (line.StartsWith($"{SensorName[k]}#0"))
                            {
                                if (split_str[1] == "НЕТ")
                                {
                                    ColdReactor.Add("100");
                                    break;
                                }
                                else if (split_str[1] == "ДА")
                                {
                                    ColdReactor.Add("0");
                                    break;
                                }
                            }
                            else
                            {
                                if (split_str[1] == "ДА")
                                {
                                    ColdReactor.Add("1");
                                    break;
                                }
                                if (split_str[1] == "НЕТ")
                                {
                                    ColdReactor.Add("0");
                                    break;
                                }
                                ColdReactor.Add(split_str[1]);
                                break;
                            }

                        }
                        m++;
                    }
                }

                #endregion

                #region Проверка, нашелся ли сигнал в срезе

                if (ColdReactor.Count == 0)
                {
                    _TextInformation.Add($"{_TextInformation.Count + 1}) Сигнал {SensorName[k]} не был найден в срезе. Полностью отсутствует информация!");
                    continue;
                }

                #endregion

                #region Запись массива в файл

               

                using (StreamWriter sw = new StreamWriter($"{SensorName[k]}_{TempTimeFrom.Replace(":", "-")}.dat", false, Encoding.UTF8))
                {
                    //sw.WriteLine($"Time {SensorName}");
                    DateTime LastTime = new DateTime(2000, 1, 1);
                    int i = 0;
                    int CountNods = 0;
                    double DementionValue = ConvertDementionToDouble(TimeDemention);
                    foreach (var item in NewListData) // item[0] - время, item[1] - значение датчика
                    {
                        if (item.DataTime >= DT_From && item.DataTime <= DT_To)
                        {
                            LastTime = item.DataTime;

                            // если элемент по типу задвижка
                            if (SensorName[k].Contains("AA1") && SensorName[k].Contains("_Z0"))
                            {
                                if (CountNods == 0) // Логика для крайнего первого значения
                                {
                                    if (item.DataTime == DT_From) // Если значение времени ОТ есть в массиве
                                    {
                                        TimeSpan var_dt = item.DataTime.Subtract(DT_From);
                                        sw.WriteLine($"{(var_dt.Seconds + var_dt.Minutes * 60 + var_dt.Hours * 60 * 60 + var_dt.Days * 24 * 60 * 60) / DementionValue} {item.DataValue}");
                                    }
                                    else if (item.DataTime != DT_From && item != NewListData[0])
                                    {
                                        sw.WriteLine($"{0} {LineInterpol(NewListData[i - 1], NewListData[i], DT_From)}");
                                        TimeSpan var_dt = item.DataTime.Subtract(DT_From);
                                        sw.WriteLine($"{(var_dt.Seconds + var_dt.Minutes * 60 + var_dt.Hours * 60 * 60 + var_dt.Days * 24 * 60 * 60) / DementionValue} {item.DataValue}");
                                    }
                                    else if (item.DataTime != DT_From && item == NewListData[0])
                                    {
                                        sw.WriteLine($"{0} {ColdReactor[k]}");
                                        TimeSpan var_dt = item.DataTime.Subtract(DT_From);
                                        sw.WriteLine($"{(var_dt.Seconds + var_dt.Minutes * 60 + var_dt.Hours * 60 * 60 + var_dt.Days * 24 * 60 * 60) / DementionValue} {item.DataValue}");
                                    }
                                }
                                else
                                {
                                    TimeSpan var_dt = item.DataTime.Subtract(DT_From);
                                    sw.WriteLine($"{(var_dt.Seconds + var_dt.Minutes * 60 + var_dt.Hours * 60 * 60 + var_dt.Days * 24 * 60 * 60) / DementionValue} {item.DataValue}");
                                }
                                CountNods++;
                            }
                            else
                            {
                                if (CountNods == 0) // Логика для крайнего первого значения
                                {
                                    if (item.DataTime == DT_From) // Если значение времени ОТ есть в массиве
                                    {
                                        TimeSpan var_dt = item.DataTime.Subtract(DT_From);
                                        sw.WriteLine($"{(var_dt.Seconds + var_dt.Minutes * 60 + var_dt.Hours * 60 * 60 + var_dt.Days * 24 * 60 * 60) / DementionValue} {item.DataValue}");
                                    }
                                    else if (item.DataTime != DT_From && item != NewListData[0])
                                    {
                                        sw.WriteLine($"{0} {LineInterpol(NewListData[i - 1], NewListData[i], DT_From)}");
                                        TimeSpan var_dt = item.DataTime.Subtract(DT_From);
                                        sw.WriteLine($"{(var_dt.Seconds + var_dt.Minutes * 60 + var_dt.Hours * 60 * 60 + var_dt.Days * 24 * 60 * 60) / DementionValue} {item.DataValue}");
                                    }
                                    else if (item.DataTime != DT_From && item == NewListData[0])
                                    {
                                        sw.WriteLine($"{0} {ColdReactor[k]}");
                                        TimeSpan var_dt = item.DataTime.Subtract(DT_From);
                                        sw.WriteLine($"{(var_dt.Seconds + var_dt.Minutes * 60 + var_dt.Hours * 60 * 60 + var_dt.Days * 24 * 60 * 60) / DementionValue} {item.DataValue}");
                                    }
                                }
                                else
                                {
                                    TimeSpan var_dt = item.DataTime.Subtract(DT_From);
                                    sw.WriteLine($"{(var_dt.Seconds + var_dt.Minutes * 60 + var_dt.Hours * 60 * 60 + var_dt.Days * 24 * 60 * 60) / DementionValue} {item.DataValue}");
                                }
                                CountNods++;
                            }
                        }
                        else if (item.DataTime > DT_To)
                        {
                            break; // Так как больше совпадений не будет
                        }
                        i++;
                    }
                    if (LastTime == new DateTime(2000, 1, 1))
                    { 
                        sw.WriteLine($"{0} {ColdReactor[0]}");
                        _TextInformation.Add($"{_TextInformation.Count + 1}) Значение датчика {SensorName[k]} не изменялось на заданном приоде времени.");
                        continue;
                    }

                    // если элемент по типу задвижка
                    if (SensorName[k].Contains("AA1") && SensorName[k].Contains("_Z0"))
                    {
                        if (LastTime != DT_To && LastTime != NewListData[NewListData.Count - 1].DataTime)
                        {
                            // изменил аргументы для интерполяции
                            TimeSpan var_dt = DT_To.Subtract(DT_From);
                            sw.WriteLine($"{(var_dt.Seconds + var_dt.Minutes * 60 + var_dt.Hours * 60 * 60 + var_dt.Days * 24 * 60 * 60) / DementionValue} {LineInterpol(NewListData[i - 1], NewListData[i], DT_To)}");
                        }
                    }
                    else
                    {
                        if (LastTime != DT_To && LastTime != NewListData[NewListData.Count - 1].DataTime)
                        {
                            TimeSpan var_dt = DT_To.Subtract(DT_From);
                            sw.WriteLine($"{(var_dt.Seconds + var_dt.Minutes * 60 + var_dt.Hours * 60 * 60 + var_dt.Days * 24 * 60 * 60) / DementionValue} {LineInterpol(NewListData[i - 1], NewListData[i + 1], DT_To)}");
                        }
                    }
                    
                }

                #endregion

                _TextInformation.Add($"{_TextInformation.Count + 1}) Файл для датчика {SensorName[k]} составлен!");

            }
            _TextInformation.Add($"{_TextInformation.Count + 1}) Выборка завершена!");
        }

        /// <summary>
        /// Записываю допустимое время выборки
        /// </summary>
        /// <param name="PathToFolder"></param>
        /// <returns></returns>
        public static List<string> CheckAccectableTime(string PathToFolder)
        {

            #region Настроечные данные

            System.Threading.Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo("en-US");
            IFormatProvider formatter = new NumberFormatInfo { NumberDecimalSeparator = "." };
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
            Encoding ANSI = Encoding.GetEncoding(1251);

            #endregion

            string RelatePath = PathToFolder;
            string[] filePaths = null;
            string RuteName;
            List<DateTime> DT_list_from = new List<DateTime>();
            List<DateTime> DT_list_to = new List<DateTime>();

            try
            {
                filePaths = Directory.GetFiles(RelatePath);
            }
            catch (DirectoryNotFoundException)
            {
                MessageBox.Show($"Нет файлов по адресу {RelatePath}");
                return new List<string>();
            }

            //RuteName = SensorName[0].Substring(2, 3);
            RuteName = "JKT";


            foreach (string item in filePaths)
            {
                string filename = Path.GetFileName(item);
                if (filename.IndexOf(RuteName) != -1)
                {
                    using (StreamReader sr = new StreamReader($"{RelatePath}/{filename}", ANSI))
                    {

                        string line = null;
                        for (int i = 0; i < 3; i++)
                        {
                            line = sr.ReadLine();
                        }
                        string[] MainStr = line.Split(new string[] { "\t", " " }, StringSplitOptions.RemoveEmptyEntries);
                        DT_list_from.Add(new DateTime(
                            int.Parse(MainStr[4].Split(new string[] { "." }, StringSplitOptions.RemoveEmptyEntries)[2]) + 2000,
                            int.Parse(MainStr[4].Split(new string[] { "." }, StringSplitOptions.RemoveEmptyEntries)[1]),
                            int.Parse(MainStr[4].Split(new string[] { "." }, StringSplitOptions.RemoveEmptyEntries)[0]),
                            int.Parse(MainStr[5].Split(new string[] { ":" }, StringSplitOptions.RemoveEmptyEntries)[0]),
                            int.Parse(MainStr[5].Split(new string[] { ":" }, StringSplitOptions.RemoveEmptyEntries)[1]),
                            int.Parse(MainStr[5].Split(new string[] { ":" }, StringSplitOptions.RemoveEmptyEntries)[2])
                            ));

                        DT_list_to.Add(new DateTime(
                            int.Parse(MainStr[7].Split(new string[] { "." }, StringSplitOptions.RemoveEmptyEntries)[2]) + 2000,
                            int.Parse(MainStr[7].Split(new string[] { "." }, StringSplitOptions.RemoveEmptyEntries)[1]),
                            int.Parse(MainStr[7].Split(new string[] { "." }, StringSplitOptions.RemoveEmptyEntries)[0]),
                            int.Parse(MainStr[8].Split(new string[] { ":" }, StringSplitOptions.RemoveEmptyEntries)[0]),
                            int.Parse(MainStr[8].Split(new string[] { ":" }, StringSplitOptions.RemoveEmptyEntries)[1]),
                            int.Parse(MainStr[8].Split(new string[] { ":" }, StringSplitOptions.RemoveEmptyEntries)[2])
                            ));
                    }
                }
            }
            // Определить минимальную и максимальную даты

            //Thread.CurrentThread.CurrentCulture = CultureInfo.CreateSpecificCulture("en-GB");
            string AccectableDayFrom = DT_list_from.Min().ToString("dd.MM.yy");
            string AccectableTimeFrom = DT_list_from.Min().ToString("HH:mm:ss");
            string AccectableDayTo = DT_list_to.Max().ToString("dd.MM.yy");
            string AccectableTimeTo = DT_list_to.Max().ToString("HH:mm:ss");

            List<string> Result = new List<string>();
            Result.Add(AccectableDayFrom);
            Result.Add(AccectableDayTo);
            Result.Add(AccectableTimeFrom);
            Result.Add(AccectableTimeTo);
            return Result;
        }

    }
}
