﻿using DBSelectionForm.Models;
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
        private static double ConvertDataFormat(string OldFormat, string Day, IFormatProvider formatter)
        {
            string[] SplitStr = OldFormat.Trim().Split(new string[] { ":" }, StringSplitOptions.RemoveEmptyEntries);
            return ((double.Parse(Day, formatter) - 6) * 3600 * 24 + double.Parse(SplitStr[0], formatter) * 3600 + double.Parse(SplitStr[1], formatter) * 60 + double.Parse(SplitStr[2], formatter));
        }
        /// <summary> Проверяем левую и правую букву, если это цифра, то возвращает - 1, это нужно для метода IsFoundName</summary>
        private static bool CheckForNums(string CheckStr, string KeyStr)
        {
            var asf = CheckStr[CheckStr.IndexOf(KeyStr) - 1];
            bool IsLeftNum = int.TryParse(CheckStr[CheckStr.IndexOf(KeyStr) - 1].ToString(), out _);
            bool IsRightNum = int.TryParse(CheckStr[CheckStr.IndexOf(KeyStr) + KeyStr.Length + 1].ToString(), out _);

            if (IsLeftNum == true && IsRightNum == true)
            {
                return true;
            }
            else
            {
                return false;
            }
            
        }
        private static void IsFoundName(ref List<string> DBArray, ref List<string> ICArray)
        {
            var result = ICArray.Except(DBArray);
            foreach (var item in result)
            {
                MessageBox.Show($"Не был найден элемент {item}!");
            }
        }
        private static string GetCategory(string str)
        {
            if ((str.IndexOf("CP") != -1 && CheckForNums(str, "CP")) || (str.IndexOf("CT") != -1 && CheckForNums(str, "CT")) || (str.IndexOf("CL") != -1 && CheckForNums(str, "CL")) || (str.IndexOf("CF") != -1 && CheckForNums(str, "CF")) || (str.IndexOf("FX") != -1 && CheckForNums(str, "FX")) || (str.IndexOf("FF") != -1 && CheckForNums(str, "FF")) || (str.IndexOf("FP") != -1 && CheckForNums(str, "FP")) || (str.IndexOf("FT") != -1 && CheckForNums(str, "FT")) || (str.IndexOf("FL") != -1 && CheckForNums(str, "FL"))) // категория 0
            {
                return "0";
            }
            else if (str.IndexOf("AA2") != -1 && CheckForNums(str, "AA")) // Категория 2
            {
                return "2";
            }
            else if (str.IndexOf("AA1") != -1 && CheckForNums(str, "AA")) // Категория 3
            {
                return "3";
            }
            else if ((str.IndexOf("AA6") != -1 && CheckForNums(str, "AA")) || (str.IndexOf("AA4") != -1 && CheckForNums(str, "AA"))) // Категория 4
            {
                return "4";
            }
            else if (str.IndexOf("AP") != -1 && CheckForNums(str, "AP")) // Категория 5
            {
                return "5";
            }
            else if ((str.IndexOf("DL") != -1 && CheckForNums(str, "DL")) || (str.IndexOf("EE") != -1 && CheckForNums(str, "EE"))|| (str.IndexOf("EZ") != -1 && CheckForNums(str, "EZ")) || (str.IndexOf("ED") != -1 && CheckForNums(str, "ED")) || (str.IndexOf("ER") != -1 && CheckForNums(str, "ER"))) // категория 6
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
                int index = 0;
                using (StreamReader sr = new StreamReader(Path))
                {
                    string Line;
                    while ((Line = sr.ReadLine()) != null)
                    {
                        if (string.IsNullOrEmpty(Line))
                        {
                            continue;
                        }
                        if (index == 0 && Line.IndexOf("Count") == -1)
                        {
                            index++;
                            MessageBox.Show($"Ошибка. В первой строчке отсутствует слово: Count");
                            
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
                                index++;
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
                        index++;
                    }
                }
                index = 0;
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



                    List<string> VarArr = new List<string>();
                    string Line;
                    int k = 0;
                    foreach (var IC in ICArray)
                    {
                    using (StreamReader sr = new StreamReader(Path, ANSI))
                    {
                        while ((Line = sr.ReadLine()) != null)
                        {
                            if (k > 3)
                            {
                                string[] StrArr = Line.Split(new char[] { '\t' }, StringSplitOptions.RemoveEmptyEntries);
                                
                                //<------------------------------------------------------------------------------------------------------------>

                                if (IC.IndexOf("_Z0") != -1)// Если встречается датчик со значением Z0
                                {
                                    if ($"{IC}#1".Contains(StrArr[0]))
                                    {
                                        string str = GetCategory(StrArr[0]);
                                        if (str == "-100000")
                                        {
                                            IsReliable = false;
                                        }
                                        if (StrArr[2] == "ДА")
                                        {
                                            DBArray.Add($"{IC}{"\t"}1{"\t"}{StrArr[3]}\t{str}\t{StrArr[1]}");
                                            DBName.Add(IC);
                                        }
                                        else if (StrArr[2] == "НЕТ")
                                        {
                                            DBArray.Add($"{IC}{"\t"}0{"\t"}{StrArr[3]}\t{str}\t{StrArr[1]}");
                                            DBName.Add(IC);
                                        }
                                        k = 0;
                                        break;
                                    }
                                }
                                //<------------------------------------------------------------------------------------------------------------>
                                else // Если встречается обычный датчик
                                {
                                    if (IC.Contains(StrArr[0]))
                                    {
                                        string str = GetCategory(StrArr[0]);
                                        if (str == "-100000")
                                        {
                                            IsReliable = false;
                                        }
                                        DBArray.Add($"{IC}{"\t"}{StrArr[2]}{"\t"}{StrArr[3]}\t{str}\t{StrArr[1]}");
                                        DBName.Add(IC);

                                        k = 0;
                                        break;
                                    }
                                }
                            }
                            k++;
                        }
                    }
                    
                }
            }
            catch (FileNotFoundException)
            {
                Console.WriteLine("Файл " + Path + " не был найден");
            }
        }

        /// <summary> Нужно найти свежую информацию о датчике, если она есть в большом массиве, где данные не всегда записываются</summary>
        private static void FindFreshDataInDB(ref List<string> DBArray, ref List<string> DBNameFresh, string RelatePathToFolder, double EndTime)
        {
            System.Threading.Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo("en-US");
            IFormatProvider formatter = new NumberFormatInfo { NumberDecimalSeparator = "." };
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
            Encoding ANSI = Encoding.GetEncoding(1251);
            string[] filePaths = Directory.GetFiles(RelatePathToFolder);
            string LastValueOfSensor = null;
            string LastDateOfSensor = null;
            double ConvertedDate; // конвертируем дату
            string[] ConvertedTimeArr; // конвертируем время
            double ConvertedTimeDouble; // конечное время, переведенное
            string VarStr; // присаиваем сюда значение item из цикла foreach
            bool IsEnd = false;


            foreach (var SensorName in DBArray)
            {
                VarStr = SensorName;
                string[] StrArr = SensorName.Split(new char[] { '\t' }, StringSplitOptions.RemoveEmptyEntries);
                foreach (var path in filePaths)
                {
                    string filename = Path.GetFileName(path);
                    string RuteName = StrArr[0].Substring(2, 3);
                    if (filename.IndexOf(RuteName) != -1)
                    {
                        using (StreamReader sr = new StreamReader($"{RelatePathToFolder}/{filename}", ANSI))
                        {
                            string line;
                            string[] lineSplit;
                            while ((line = sr.ReadLine()) != null)
                            {
                                lineSplit = line.Split(new string[] { "\t", " " }, StringSplitOptions.RemoveEmptyEntries);
                                if (lineSplit[2].IndexOf(StrArr[0]) != -1) //String.Compare(lineSplit[2], StrArr[0]) == 0
                                {

                                    #region Обработка данных

                                    ConvertedDate = double.Parse(lineSplit[0].Split(new string[] { "." }, StringSplitOptions.RemoveEmptyEntries)[0], formatter);
                                    ConvertedTimeArr = lineSplit[1].Split(new string[] { ":" }, StringSplitOptions.RemoveEmptyEntries);
                                    ConvertedTimeDouble = (ConvertedDate - 6) * 24 * 60 * 60 + double.Parse(ConvertedTimeArr[0], formatter) * 3600 + double.Parse(ConvertedTimeArr[1], formatter) * 60 + double.Parse(ConvertedTimeArr[2], formatter)/1000;

                                    try
                                    {
                                        if (EndTime >= ConvertedTimeDouble && lineSplit[4] == "дост")
                                        {
                                            LastValueOfSensor = lineSplit[3];
                                            LastDateOfSensor = $"{lineSplit[0]} {lineSplit[1]}";
                                        }
                                        else if (EndTime < ConvertedTimeDouble && lineSplit[4] == "дост")
                                        {
                                            IsEnd = true;
                                            VarStr = $"{StrArr[0]}\t{LastValueOfSensor}\t{StrArr[2]}\t{StrArr[3]}\t{LastDateOfSensor}";
                                            break;
                                        }
                                        else
                                        {
                                            IsEnd = true;
                                            break;
                                        }
                                    }
                                    catch (NullReferenceException)
                                    {
                                        MessageBox.Show("Вылетел NULL в методе GetDataListBD");
                                    }

                                    #endregion

                                }
                            }

                            
                        }

                    }
                    LastDateOfSensor = null;
                    LastValueOfSensor = null;
                    if (IsEnd)
                    {
                        IsEnd = false;
                        break;
                    }
                }
                DBNameFresh.Add(VarStr);
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
                            sw.WriteLine($"{ArrOfStr[1]};{ArrOfStr[0]};{ArrOfStr[3]};_{ArrOfStr[4].Replace(" ","_")}");
                        }
                        else if (ArrOfStr[1] == "ДА")
                        {
                            sw.WriteLine($"{1};{ArrOfStr[0]};{ArrOfStr[3]};_{ArrOfStr[4].Replace(" ", "_")}");
                        }
                        else if (ArrOfStr[1] == "НЕТ")
                        {
                            sw.WriteLine($"{0};{ArrOfStr[0]};{ArrOfStr[3]};_{ArrOfStr[4].Replace(" ", "_")}");
                        }
                        else
                        {
                            sw.WriteLine($"{9999997};{ArrOfStr[0]};{ArrOfStr[3]};_{ArrOfStr[4].Replace(" ", "_")}");
                        }
                    }
                    else
                    {
                        if (double.TryParse(ArrOfStr[1], NumberStyles.Number, formatter, out d))
                        {
                            if (d >= 0)
                            {
                                sw.WriteLine($"{9999999};{ArrOfStr[0]};{ArrOfStr[3]};_{ArrOfStr[4].Replace(" ", "_")}");
                            }
                            else
                            {
                                sw.WriteLine($"{-9999999};{ArrOfStr[0]};{ArrOfStr[3]};_{ArrOfStr[4].Replace(" ", "_")}");
                            }
                        }
                        else
                        {
                            sw.WriteLine($"{9999998};{ArrOfStr[0]};{ArrOfStr[3]};_{ArrOfStr[4].Replace(" ", "_")}");
                        }
                    }
                    i++;
                }

            }
        }

        public static void GetListMethod(InfoData _InfoData, string EndTimeFormat, string EndDay)
        {
            IFormatProvider formatter = new NumberFormatInfo { NumberDecimalSeparator = "." };
            bool IsReliable = true;
            string ReadPathFromDB = _InfoData.PathToDataFile;
            string WorkPath = _InfoData.PathToListFile;
            string RelatePathToFolder = _InfoData.PathToFolder;
            //double EndTime = ConvertDataFormat(EndTimeFormat, EndDay, formatter);

            List<string> DBName = new List<string>(); 
            List<string> StringArrayFromDBFresh = new List<string>(); // конечный массив
            List<string> StringArrayFromDB = new List<string>();// только со среза, старый массив
            List<string> StringArrayFromIC = new List<string>();


            ReadDataFromIC(WorkPath, ref StringArrayFromIC);

            FindDataInDB(ReadPathFromDB, ref StringArrayFromDB, ref StringArrayFromIC, ref IsReliable, ref DBName);

            //FindFreshDataInDB(ref StringArrayFromDB,ref StringArrayFromDBFresh, RelatePathToFolder, EndTime);

            WriteDataToIC(WorkPath, ref StringArrayFromDB, IsReliable);

            IsFoundName(ref DBName, ref StringArrayFromIC);

            

            MessageBox.Show($"Список создан");
        }
    }
}
