using DBSelectionForm.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;

namespace DBSelectionForm.Services
{
    class GetListFromDB
    {
        private static void IsFoundName(ref List<string> DBArray, ref List<string> ICArray)
        {
            var result = ICArray.Except(DBArray);
            foreach (var item in result)
            {
                throw new Exception($"Не был найден элемент {item}!");
            }
        }
        private static string GetCategory(string str)
        {
            if (str.IndexOf("CP") != -1 || str.IndexOf("CT") != -1 || str.IndexOf("CL") != -1 || str.IndexOf("CF") != -1 || str.IndexOf("FX") != -1) // категория 0
            {
                return "0";
            }
            else if (str.IndexOf("AA2") != -1) // Категория 2
            {
                return "2";
            }
            else if (str.IndexOf("AA1") != -1) // Категория 3
            {
                return "3";
            }
            else if (str.IndexOf("AA6") != -1 || str.IndexOf("AA4") != -1) // Категория 4
            {
                return "4";
            }
            else if (str.IndexOf("AP") != -1) // Категория 5
            {
                return "5";
            }
            else if (str.IndexOf("DL") != -1 || str.IndexOf("EE") != -1 || str.IndexOf("EZ") != -1 || str.IndexOf("ED") != -1 || str.IndexOf("ER") != -1) // категория 6
            {
                return "6";
            }
            else if (str.IndexOf("KIN1.OR") != -1) // Категория 7
            {
                return "7";
            }
            else
            {
                return "-100000";
            }
        }
        /// <summary> Считываем все данные с файла !List_IC.txt </summary>
        private static void ReadDataFromIC(string Path, ref List<string> StringArray)
        {
            try
            {
                using (StreamReader sr = new StreamReader(Path))
                {
                    string Line;
                    while ((Line = sr.ReadLine()) != null)
                    {
                        if (string.IsNullOrEmpty(Line))
                        {
                            continue;
                        }
                        string[] ArrOfStr = Line.Split(new string[] { ";" }, StringSplitOptions.RemoveEmptyEntries);
                        if (ArrOfStr.Length > 1)
                        {
                            if (StringArray.Any(w => w == ArrOfStr[1])) // Проверяю на повтор элемента
                            {
                                MessageBox.Show($"{"Элемент"} {ArrOfStr[1]} {"повторяется"}");
                            }
                            if (ArrOfStr[1] == "Count")
                            {
                                continue;
                            }
                            StringArray.Add(ArrOfStr[1]);
                        }
                        else
                        {
                            if (StringArray.Any(w => w == Line)) // Проверяю на повтор элемента
                            {
                                MessageBox.Show($"{"Элемент"} {Line} {"повторяется"}");
                                //throw new Exception($"{"Элемент"} {Line} {"повторяется"}");
                            }
                            StringArray.Add(Line);
                        }
                    }
                }
            }
            catch (FileNotFoundException)
            {
                MessageBox.Show("Файл " + Path + " не был найден");
            }
        }

        /// <summary> Находим все данные в БД, которые нас интересуют </summary>
        private static void FindDataInDB(string Path, ref List<string> DBArray, ref List<string> ICArray, ref bool IsReliable, ref List<string> DBName)
        {
            try
            {
                System.Threading.Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo("en-US");
                IFormatProvider formatter = new NumberFormatInfo { NumberDecimalSeparator = "." };
                Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
                Encoding ANSI = Encoding.GetEncoding(1251);
                using (StreamReader sr = new StreamReader(Path, ANSI))
                {
                    List<string> VarArr = new List<string>();
                    string Line;
                    int k = 0;
                    while ((Line = sr.ReadLine()) != null)
                    {
                        if (k > 3)
                        {

                            string[] StrArr = Line.Split(new char[] { '\t' }, StringSplitOptions.RemoveEmptyEntries);
                            if (ICArray.Contains(StrArr[0]))
                            {
                                string str = GetCategory(StrArr[0]);
                                DBArray.Add($"{StrArr[0]}{"\t"}{StrArr[2]}{"\t"}{StrArr[3]}\t{str}");
                                DBName.Add(StrArr[0]);
                                if (str == "-100000") // Если не нашли категорию
                                {
                                    IsReliable = false;
                                }
                            }
                            else if (ICArray.IndexOf($"{StrArr[0].Replace("#0", "")}") != -1)
                            {
                                VarArr.Add($"{StrArr[2]}");

                            }
                            else if (ICArray.IndexOf($"{StrArr[0].Replace("#1", "")}") != -1)
                            {
                                if (VarArr[0] == "НЕТ" && StrArr[2] == "ДА")
                                {
                                    DBArray.Add($"{StrArr[0].Replace("#1", "")}{"\t"}1{"\t"}{StrArr[3]}\t-10000");
                                }
                                else if (VarArr[0] == "ДА" && StrArr[2] == "НЕТ")
                                {
                                    DBArray.Add($"{StrArr[0].Replace("#1", "")}{"\t"}0{"\t"}{StrArr[3]}\t-10000");
                                }
                                else
                                {
                                    DBArray.Add($"{StrArr[0].Replace("#1", "")}{"\t"}99995{"\t"}{StrArr[3]}\t-10000");
                                }
                                DBName.Add(StrArr[0].Replace("#1", ""));
                                VarArr = new List<string>();
                            }
                        }
                        k++;
                    }
                }
            }
            catch (FileNotFoundException)
            {
                Console.WriteLine("Файл " + Path + " не был найден");
            }
        }

        ///<summary> Записываем информацию в файл !List_IC.txt </summary>
        private static void WriteDataToIC(string WorkPath, ref List<string> StringArrayFromDB, bool IsReliable)
        {
            IFormatProvider formatter = new NumberFormatInfo { NumberDecimalSeparator = "." };
            int i = 0;
            using (StreamWriter sw = new StreamWriter(WorkPath, false, Encoding.Default))
            {
                if (IsReliable == true)
                {
                    sw.WriteLine($"{StringArrayFromDB.Count};Count;0");
                }
                else
                {
                    sw.WriteLine($"{StringArrayFromDB.Count};Count;-1");
                }

                foreach (var item in StringArrayFromDB)
                {
                    string[] ArrOfStr = item.Split(new string[] { "\t" }, StringSplitOptions.RemoveEmptyEntries);
                    double d;
                    if (ArrOfStr[2] == "дост")
                    {
                        if (double.TryParse(ArrOfStr[1], NumberStyles.Number, formatter, out d))
                        {
                            sw.WriteLine($"{ArrOfStr[1]};{ArrOfStr[0]};{ArrOfStr[3]}");
                        }
                        else
                        {
                            sw.WriteLine($"{9999997};{ArrOfStr[0]};{ArrOfStr[3]}");
                        }
                    }
                    else
                    {
                        if (double.TryParse(ArrOfStr[1], NumberStyles.Number, formatter, out d))
                        {
                            if (d >= 0)
                            {
                                sw.WriteLine($"{9999999};{ArrOfStr[0]};{ArrOfStr[3]}");
                            }
                            else
                            {
                                sw.WriteLine($"{-9999999};{ArrOfStr[0]};{ArrOfStr[3]}");
                            }
                        }
                        else
                        {
                            sw.WriteLine($"{9999998};{ArrOfStr[0]};{ArrOfStr[3]}");
                        }
                    }
                    i++;
                }

            }
        }

        public static void GetListMethod(InfoData _InfoData)
        {

            bool IsReliable = true;
            string ReadPathFromDB = _InfoData.PathToDataFile;
            string WorkPath = _InfoData.PathToListFile;

            List<string> DBName = new List<string>();
            List<string> StringArrayFromDB = new List<string>();
            List<string> StringArrayFromIC = new List<string>();


            ReadDataFromIC(WorkPath, ref StringArrayFromIC);

            FindDataInDB(ReadPathFromDB, ref StringArrayFromDB, ref StringArrayFromIC, ref IsReliable, ref DBName);

            WriteDataToIC(WorkPath, ref StringArrayFromDB, IsReliable);

            IsFoundName(ref DBName, ref StringArrayFromIC);

            MessageBox.Show($"Список создан");
        }
    }
}
