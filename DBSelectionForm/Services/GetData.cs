using DBSelectionForm.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Text;
using System.Windows;
using System.Windows.Threading;

namespace DBSelectionForm.Services
{
    class GetData
    {
        private static string LineInterpol(string[] Values1, string[] Values2, string X) // Линейная интерполяция
        {
            double result;
            bool IsDouble_Values1 = Double.TryParse(Values1[1], out result);
            bool IsDouble_Values2 = Double.TryParse(Values2[1], out result);

            if (IsDouble_Values1 != false && IsDouble_Values2 != false)
            {
                return (((double.Parse(Values1[1]) - double.Parse(Values2[1])) / (double.Parse(Values1[0]) - double.Parse(Values2[0]))) * (double.Parse(X) - double.Parse(Values1[0])) + double.Parse(Values1[1])).ToString();
            }
            else
            {
                if (IsDouble_Values1 == false)
                {
                    return Values1[1];
                }
                else
                {
                    return Values2[1];
                }
            }
        }
        private static string ConvertDataFormat(string OldFormat, string Day, IFormatProvider formatter)
        {
            string[] SplitStr = OldFormat.Trim().Split(new string[] { ":" }, StringSplitOptions.RemoveEmptyEntries);
            return ((double.Parse(Day, formatter) - 6) * 3600 * 24 + double.Parse(SplitStr[0], formatter) * 3600 + double.Parse(SplitStr[1], formatter) * 60 + double.Parse(SplitStr[2], formatter)).ToString();
        }
        public static void GetDataMethod(InfoData _InfoData, ref ObservableCollection<string> _TextInformation)
        {
            
            #region Настроечные данные

            System.Threading.Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo("en-US");
            IFormatProvider formatter = new NumberFormatInfo { NumberDecimalSeparator = "." };
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
            Encoding ANSI = Encoding.GetEncoding(1251);

            #endregion

            #region Ввод данных из модели

            string DayFrom = _InfoData.DayFrom;
            string DayTo = _InfoData.DayTo;
            string[] SensorName = _InfoData.SensorName.Split(new string[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries);
            string TempTimeFrom = _InfoData.TimeFrom.Trim();
            string TimeFrom = ConvertDataFormat(_InfoData.TimeFrom, DayFrom, formatter);
            string TimeTo = ConvertDataFormat(_InfoData.TimeTo, DayTo, formatter);

            #endregion

            #region Setup

            List<string[]> ListData = new List<string[]>();
            double Date;
            string DateStr;
            string[] example;
            string[] DateArr;
            string[] DateDayArr;
            string RuteName;
            string RelatePath = _InfoData.PathToFolder;
            string[] filePaths = Directory.GetFiles(RelatePath);
            List<string> ColdReactor = new List<string>();
            _TextInformation.Clear();

            #endregion

            for (int k = 0; k < SensorName.Length; k++)
            {

                ListData.Clear();

                RuteName = SensorName[k].Substring(2, 3);

                _TextInformation.Add($"{_TextInformation.Count + 1}) Началось составление файла для датчика {SensorName[k]}.");

                #region Запись данных в массив

                foreach (string item in filePaths)
                {
                    string filename = Path.GetFileName(item);
                    if (filename.IndexOf(RuteName) != -1)
                    {
                        using (StreamReader sr = new StreamReader($"{RelatePath}/{filename}", ANSI))
                        {

                            string line;
                            while ((line = sr.ReadLine()) != null)
                            {
                                if (String.Compare(line.Split(new string[] { "\t", " " }, StringSplitOptions.RemoveEmptyEntries)[2], SensorName[k]) == 0)
                                {

                                    #region Обработка данных

                                    example = line.Split(new string[] { "\t", " " }, StringSplitOptions.RemoveEmptyEntries);
                                    DateArr = example[1].Trim().Replace(",", ".").Split(new string[] { ":" }, StringSplitOptions.RemoveEmptyEntries);
                                    DateDayArr = example[0].Trim().Split(new string[] { "." }, StringSplitOptions.RemoveEmptyEntries);
                                    Date = double.Parse(DateArr[0], formatter) * 3600 + double.Parse(DateArr[1], formatter) * 60 + double.Parse(DateArr[2], formatter) + (double.Parse(DateDayArr[0], formatter) - 6) * 24 * 3600;
                                    DateStr = Date.ToString();
                                    #endregion

                                    
                                    ListData.Add(new string[] { DateStr, example[3] });
                                }
                            }
                        }

                    }
                }
                using (StreamReader sr = new StreamReader($"{RelatePath}/срез_06_07_2018.txt", ANSI)) // Поиск по срезу для холодного реактора
                {
                    string line;
                    while ((line = sr.ReadLine()) != null)
                    {
                        if (line.StartsWith(SensorName[k]))
                        {
                            ColdReactor.Add(line.Split(new string[] { "\t" }, StringSplitOptions.RemoveEmptyEntries)[2]);
                            break;
                        }
                    }
                }

                #endregion

                #region Проверка на предмет существования датчика в БД

                try
                {
                    if (ListData.Count == 0)
                    {
                        throw new Exception($"\nОшибка! Датчик {SensorName[k]} не был найден!");
                    }
                }
                catch (Exception e)
                {
                    MessageBox.Show(e.Message);
                    return;
                }

                #endregion

                #region Запись массива в файл

                using (StreamWriter sw = new StreamWriter($"{SensorName[k]}_{TempTimeFrom.Replace(":", "-")}.dat", false, System.Text.Encoding.Default))
                {
                    //sw.WriteLine($"Time {SensorName}");
                    string LastTime = null;
                    int i = 0;
                    int CountNods = 0;
                    foreach (var item in ListData)
                    {
                        if (double.Parse(item[0], formatter) >= double.Parse(TimeFrom, formatter) && double.Parse(item[0], formatter) <= double.Parse(TimeTo, formatter))
                        {
                            LastTime = item[0];

                            if (CountNods == 0) // Логика для крайнего первого значения
                            {
                                if (double.Parse(item[0], formatter) == double.Parse(TimeFrom, formatter)) // Если значение времени ОТ есть в массиве
                                {
                                    sw.WriteLine($"{double.Parse(item[0], formatter) - double.Parse(TimeFrom, formatter)} {item[1]}");
                                }
                                else if (double.Parse(item[0], formatter) != double.Parse(TimeFrom, formatter) && item != ListData[0])
                                {
                                    sw.WriteLine($"{double.Parse(TimeFrom, formatter) - double.Parse(TimeFrom, formatter)} {LineInterpol(ListData[i - 1], ListData[i], TimeFrom)}");
                                    sw.WriteLine($"{double.Parse(item[0], formatter) - double.Parse(TimeFrom, formatter)} {item[1]}");
                                }
                                else if (double.Parse(item[0], formatter) != double.Parse(TimeFrom, formatter) && item == ListData[0])
                                {
                                    sw.WriteLine($"{double.Parse(TimeFrom, formatter) - double.Parse(TimeFrom, formatter)} {ColdReactor[0]}");
                                    sw.WriteLine($"{double.Parse(item[0], formatter) - double.Parse(TimeFrom, formatter)} {item[1]}");
                                }
                            }
                            else
                            {
                                sw.WriteLine($"{double.Parse(item[0], formatter) - double.Parse(TimeFrom, formatter)} {item[1]}");
                            }
                            CountNods++;
                        }
                        else if (double.Parse(item[0], formatter) > double.Parse(TimeTo, formatter))
                        {
                            break; // Так как больше совпадений не будет
                        }
                        i++;
                    }
                    if (LastTime == null)
                    {
                        sw.WriteLine($"{double.Parse(TimeFrom, formatter) - double.Parse(TimeFrom, formatter)} {ColdReactor[0]}");
                        MessageBox.Show($"\nЗначение датчика {SensorName[k]} не изменялось на заданном приоде времени.");
                        return;
                    }
                    if (double.Parse(LastTime, formatter) != double.Parse(TimeTo, formatter) && LastTime != ListData[ListData.Count - 1][0])
                    {
                        sw.WriteLine($"{double.Parse(TimeTo, formatter) - double.Parse(TimeFrom, formatter)} {LineInterpol(ListData[i - 1], ListData[i + 1], TimeTo)}");
                    }
                }

                #endregion

                _TextInformation.Add($"{_TextInformation.Count + 1}) Файл для датчика {SensorName[k]} составлен!");

                //MessageBox.Show($"\nЗапись датчика {SensorName[k]} завершена.");
            }
            _TextInformation.Add($"{_TextInformation.Count + 1}) Выборка завершена!");
        }

    }
}
