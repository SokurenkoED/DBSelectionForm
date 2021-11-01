using DBSelectionForm.Infastructure.Commands.Base;
using DBSelectionForm.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace DBSelectionForm.Infastructure.Commands
{
    internal class GetDataCommand : Command
    {
        private static string LineInterpol(string[] Values1, string[] Values2, string X) // Линейная интерполяция
        {
            return (((double.Parse(Values1[1]) - double.Parse(Values2[1])) / (double.Parse(Values1[0]) - double.Parse(Values2[0]))) * (double.Parse(X) - double.Parse(Values1[0])) + double.Parse(Values1[1])).ToString();
        }
        private static string ConvertDataFormat(string OldFormat, IFormatProvider formatter)
        {
            string[] SplitStr = OldFormat.Trim().Split(new string[] { ":" }, StringSplitOptions.RemoveEmptyEntries);
            return (double.Parse(SplitStr[0], formatter) * 3600 + double.Parse(SplitStr[1], formatter) * 60 + double.Parse(SplitStr[2], formatter)).ToString();
        }

        public override bool CanExecute(object parameter) => true;

        public async override void Execute(object parameter)
        {
            Stopwatch stopWatch = new Stopwatch();
            stopWatch.Start();

            #region Настроечные данные

            System.Threading.Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo("en-US");
            IFormatProvider formatter = new NumberFormatInfo { NumberDecimalSeparator = "." };
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
            Encoding ANSI = Encoding.GetEncoding(1251);

            #endregion

            #region Ввод данных из модели

            string SensorName = InfoData.SensorName;
            string TempTimeFrom = InfoData.TimeFrom.Trim();
            string TimeFrom = ConvertDataFormat(InfoData.TimeFrom, formatter);
            string TimeTo = ConvertDataFormat(InfoData.TimeTo, formatter);

            #endregion

            #region Setup

            List<string[]> ListData = new List<string[]>();
            double Date;
            string DateStr;
            string[] example;
            string[] DateArr;
            string[] DateDayArr;
            string RuteName = SensorName.Substring(2, 3);
            string RelatePath = InfoData.PathToFolder;
            string[] filePaths = Directory.GetFiles(RelatePath);
            List<string> ColdReactor = new List<string>();

            #endregion

            #region Запись данных в массив

            foreach (string item in filePaths)
            {
                string filename = Path.GetFileName(item);
                if (filename.IndexOf(RuteName) != -1)
                {
                    using (StreamReader sr = new StreamReader($"{RelatePath}/{filename}", ANSI))
                    {
                        //Console.WriteLine($"\nИдет поиск в файле {filename}.");

                        string line;
                        while ((line = await sr.ReadLineAsync()) != null)
                        {
                            if (String.Compare(line.Split(new string[] { "\t", " " }, StringSplitOptions.RemoveEmptyEntries)[2], SensorName) == 0)
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
                while ((line = await sr.ReadLineAsync()) != null)
                {
                    if (line.StartsWith(SensorName))
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
                    throw new Exception($"\nОшибка! Датчик {SensorName} не был найден!");
                }
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
                Environment.Exit(0);
            }

            #endregion

            #region Запись массива в файл

            using (StreamWriter sw = new StreamWriter($"{SensorName}_{TempTimeFrom.Replace(":", "-")}.dat", false, System.Text.Encoding.Default))
            {
                await sw.WriteLineAsync($"Time {SensorName}");
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
                                await sw.WriteLineAsync($"{double.Parse(item[0], formatter) - double.Parse(TimeFrom, formatter)} {item[1]}");
                            }
                            else if (double.Parse(item[0], formatter) != double.Parse(TimeFrom, formatter) && item != ListData[0])
                            {
                                await sw.WriteLineAsync($"{double.Parse(TimeFrom, formatter) - double.Parse(TimeFrom, formatter)} {LineInterpol(ListData[i - 1], ListData[i + 1], TimeFrom)}");
                                await sw.WriteLineAsync($"{double.Parse(item[0], formatter) - double.Parse(TimeFrom, formatter)} {item[1]}");
                            }
                            else if (double.Parse(item[0], formatter) != double.Parse(TimeFrom, formatter) && item == ListData[0])
                            {
                                await sw.WriteLineAsync($"{double.Parse(TimeFrom, formatter) - double.Parse(TimeFrom, formatter)} {ColdReactor[0]}");
                                await sw.WriteLineAsync($"{double.Parse(item[0], formatter) - double.Parse(TimeFrom, formatter)} {item[1]}");
                            }
                        }
                        else
                        {
                            await sw.WriteLineAsync($"{double.Parse(item[0], formatter) - double.Parse(TimeFrom, formatter)} {item[1]}");
                        }
                        CountNods++;
                    }
                    else if (double.Parse(item[0], formatter) > double.Parse(TimeTo, formatter))
                    {
                        break; // Так как больше совпадений не будет
                    }
                    i++;
                }
                if (double.Parse(LastTime, formatter) != double.Parse(TimeTo, formatter) && LastTime != ListData[ListData.Count - 1][0])
                {
                    await sw.WriteLineAsync($"{double.Parse(TimeTo, formatter) - double.Parse(TimeFrom, formatter)} {LineInterpol(ListData[i - 1], ListData[i + 1], TimeTo)}");
                }
            }

            #endregion

            stopWatch.Stop();
            // Get the elapsed time as a TimeSpan value.
            TimeSpan ts = stopWatch.Elapsed;

            // Format and display the TimeSpan value.
            string elapsedTime = String.Format("{0:00}:{1:00}:{2:00}.{3:00}",
                ts.Hours, ts.Minutes, ts.Seconds,
                ts.Milliseconds / 10);
            MessageBox.Show("RunTime " + elapsedTime);

        }
    }
}
