﻿using DBSelectionForm.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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

        private static void IsRegulator_XQ08(SignalModel Signal)
        {
            IFormatProvider formatter = new NumberFormatInfo { NumberDecimalSeparator = "." };
            if (Signal.Name.Contains("_XQ08"))
            {
                double IsParse;
                if ((Signal.Status == "ДОСТ" || Signal.Status == "ПОВТ.ДОСТ") && double.TryParse(Signal.NewValue.ToString(), NumberStyles.Any, formatter, out IsParse) && IsParse <= 0)
                {
                    Signal.NewValue = 0;
                }
            }
        }
        /// <summary>
        /// Тест на проверку формы записи: если у элемента не указывается сигнал, содержащий "_", то вылетает ошибка.
        /// </summary>
        /// <param name="ArrOfStr"></param>
        /// <param name="index">Если записано только имя элемента, то ставится "0", если перед элементом записано число, то ставится "1"</param>
        private static void TestIsContainsSimbol(string[] ArrOfStr, int index)
        {
            // Проверка на наличие "_" у элементов
            if (!ArrOfStr[index].Contains("_") && !ArrOfStr[index].Contains("Обнаружено "))
            {
                MessageBox.Show($"Ошибка. Неверный формат записи для элемента {ArrOfStr[index]}");
            }
        }
        private static int ConvertBoolStringToInt(string BoolStr)
        {
            switch (BoolStr)
            {
                case "ДА":
                    return 1;
                case "НЕТ":
                    return 0;
                default:
                    return -10000;
            }
        }
        private static double ConvertDataFormat(string OldFormat, string Day, IFormatProvider formatter)
        {
            string[] SplitStr = OldFormat.Trim().Split(new string[] { ":" }, StringSplitOptions.RemoveEmptyEntries);
            return ((double.Parse(Day, formatter) - 6) * 3600 * 24 + double.Parse(SplitStr[0], formatter) * 3600 + double.Parse(SplitStr[1], formatter) * 60 + double.Parse(SplitStr[2], formatter));
        }
        /// <summary> Проверяем левую и правую букву, если это цифра, то возвращает - 1, это нужно для метода IsFoundName</summary>
        private static bool CheckForNums(string CheckStr, string KeyStr)
        {
            CheckStr = CheckStr.Substring(5);
            if (CheckStr.IndexOf(KeyStr) == -1)
            {
                return false;
            }
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
        private static void IsFoundName(ref List<SignalModel> ICArray, ref List<SignalModel> NotFoundSignals)
        {
            //NotFoundSignals = ICArray.Where(x => !DBArray.Any(y => y.Name == x.Name)).ToList();
            foreach (var item in ICArray)
            {
                if (item.NewValue == null)
                {
                    NotFoundSignals.Add(item);
                }
            }
        }
        private static string GetCategory(string str)
        {
            if ((str.IndexOf("CE") != -1 && CheckForNums(str, "CE")) || (str.IndexOf("CS") != -1 && CheckForNums(str, "CS")) || (str.IndexOf("CQ") != -1 && CheckForNums(str, "CQ")) || (str.IndexOf("CP") != -1 && CheckForNums(str, "CP")) || (str.IndexOf("CT") != -1 && CheckForNums(str, "CT")) || (str.IndexOf("CL") != -1 && CheckForNums(str, "CL")) || (str.IndexOf("CF") != -1 && CheckForNums(str, "CF")) || (str.IndexOf("FX") != -1 && CheckForNums(str, "FX")) || (str.IndexOf("FF") != -1 && CheckForNums(str, "FF")) || (str.IndexOf("FP") != -1 && CheckForNums(str, "FP")) || (str.IndexOf("FT") != -1 && CheckForNums(str, "FT")) || (str.IndexOf("FL") != -1 && CheckForNums(str, "FL"))) // категория 0 (Каналы измерения)
            {
                return "0";
            }
            else if ((str.IndexOf("AA2") != -1 && CheckForNums(str, "AA")) || (str.IndexOf("CG2") != -1 && CheckForNums(str, "CG"))) // Категория 2 (Регуляторы)
            {
                return "2";
            }
            else if ((str.IndexOf("AA1") != -1 && CheckForNums(str, "AA")) || (str.IndexOf("AA8") != -1 && CheckForNums(str, "AA")) || (str.IndexOf("CG1") != -1 && CheckForNums(str, "CG"))) // Категория 3 (Задвижки)
            {
                return "3";
            }
            else if ((str.IndexOf("AA6") != -1 && CheckForNums(str, "AA")) || (str.IndexOf("AA4") != -1 && CheckForNums(str, "AA"))) // Категория 4 (Клапона)
            {
                return "4";
            }
            else if (str.IndexOf("AP") != -1 && CheckForNums(str, "AP")) // Категория 5 (Насосы)
            {
                return "5";
            }
            else if ((str.IndexOf("DE001_") != -1) || (str.IndexOf("CH") != -1 && CheckForNums(str, "CH")) || (str.IndexOf("DF2") != -1 && CheckForNums(str, "DF")) || (str.IndexOf("CG6") != -1 && CheckForNums(str, "CG")) || str.IndexOf("10JEF10DP911") != -1 || str.IndexOf("10LBG01DP001") != -1 || str.IndexOf("10LBG02DP001") != -1 || str.IndexOf("10LBG03DP001") != -1 || str.IndexOf("10LBA00DP002") != -1 || str.IndexOf("10MAA10DP001") != -1 || str.IndexOf("10LBA00DP001") != -1 || (str.IndexOf("EB") != -1 && CheckForNums(str, "EB")) || (str.IndexOf("EW") != -1 && CheckForNums(str, "EW")) || (str.IndexOf("EY") != -1 && CheckForNums(str, "EY")) || (str.IndexOf("DU") != -1 && CheckForNums(str, "DU")) || (str.IndexOf("CG9") != -1 && CheckForNums(str, "CG")) || (str.IndexOf("EU") != -1 && CheckForNums(str, "EU")) || (str.IndexOf("ER") != -1 && CheckForNums(str, "ER")) || (str.IndexOf("EG") != -1 && CheckForNums(str, "EG")) || (str.IndexOf("DL") != -1 && CheckForNums(str, "DL")) || (str.IndexOf("EE") != -1 && CheckForNums(str, "EE"))|| (str.IndexOf("EZ") != -1 && CheckForNums(str, "EZ")) || (str.IndexOf("ED") != -1 && CheckForNums(str, "ED")) || (str.IndexOf("ER") != -1 && CheckForNums(str, "ER"))) // категория 6 (Алгоритмы)
            {
                return "6";
            }
            else if (str.IndexOf("KIN1.OR") != -1) // Категория 7 (Группы ОР СУЗ)
            {
                return "7";
            }
            else if ((str.IndexOf("CG2") != -1 && CheckForNums(str, "CG"))) // Категория 8
            {
                return "8";
            }
            else if (str.IndexOf("GX") != -1 && CheckForNums(str, "GX")) // Категория 11
            {
                return "11";
            }
            else if (str.IndexOf("DE001A") != -1 || str.IndexOf("DE001T") != -1 || str.IndexOf("FU") != -1 && CheckForNums(str, "FU") || str.IndexOf("10LBA00DP001K") != -1) // Категория 12
            {
                return "12";
            }
            else if (str.IndexOf("EC") != -1 && CheckForNums(str, "EC") && str.IndexOf("TE0") == -1) // Категория 14
            {
                return "14";
            }
            else if (str.IndexOf("TE0") != -1) // Категория 16
            {
                return "16";
            }
            else if (str.IndexOf("VL0") != -1) // Категория 17
            {
                return "17";
            }
            else if (str.IndexOf("DF") != -1 && CheckForNums(str, "DF") || str.IndexOf("10MAN00DP001") != -1 || str.IndexOf("10LAH00DP001") != -1) // Категория 18
            {
                return "18";
            }
            else
            {
                return "-1";
            }
        }
        /// <summary> Считываем все данные с файла !List_IC.txt </summary>
        private static void ReadDataFromIC(string Path, ref List<SignalModel> ReadSignals)
        {
            try
            {
                int index = 0;
                using (StreamReader sr = new StreamReader(Path, Encoding.Default))
                {
                    string Line;
                    while ((Line = sr.ReadLine()) != null)
                    {
                        Line = Line.Trim().ToUpper();

                        if (Line.Contains("Обнаружено "))
                        {
                            continue;
                        }

                        if (string.IsNullOrEmpty(Line))
                        {
                            continue;
                        }
                        if (index == 0 && Line.IndexOf("Count") == -1)
                        {
                            index++;
                            
                        }

                        string[] ArrOfStr = Line.ToUpper().Split(new string[] { ";" }, StringSplitOptions.RemoveEmptyEntries);

                        if (ArrOfStr.Length > 1)
                        {
                            if (ReadSignals.Any(w => w.Name == ArrOfStr[1])) // Проверяю на повтор элемента (Если сигнал повторяется)
                            {
                                continue;
                            }
                            if (ArrOfStr[1] == "Count")
                            {
                                index++;
                                continue;
                            }
                            TestIsContainsSimbol(ArrOfStr, 1);
                            if (ArrOfStr.Length > 2)
                            {
                                ReadSignals.Add(new SignalModel { Name = ArrOfStr[1], OldValue = ArrOfStr[0], Category = ArrOfStr[2] });
                                if (ArrOfStr.Length == 6)
                                {
                                    var IsInvariableFromStr = Convert.ToBoolean(ArrOfStr[5]);
                                    ReadSignals[^1].IsInvariable = IsInvariableFromStr;
                                    //ReadSignals.Add(new SignalModel { Name = ArrOfStr[1], OldValue = ArrOfStr[0], IsInvariable = IsInvariableFromStr, Category = ArrOfStr[2] });
                                }
                            }
                            else
                            {
                                ReadSignals.Add(new SignalModel { Name = ArrOfStr[1], OldValue = ArrOfStr[0], Category = "-1" });
                            }
                            
                        }
                        else
                        {
                            if (ReadSignals.Any(w => w.Name == Line)) // Проверяю на повтор элемента
                            {
                                MessageBox.Show($"{"Элемент"} {Line} {"повторяется"}");
                                //throw new Exception($"{"Элемент"} {Line} {"повторяется"}");
                            }
                            TestIsContainsSimbol(ArrOfStr, 0);

                            ReadSignals.Add(new SignalModel { Name = Line, OldValue = "-", Category = "-1" });
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

        /// <summary> Находим все данные в срезе, которые нас интересуют (06.07.18 slice format)</summary>
        private static void FindDataInDB_06_07_18(string Path, ref List<SignalModel> FoundSignalsInDB, ref List<SignalModel> ReadSignals, ref bool IsReliable, ref List<SignalModel> CheckFoundSignals)
        {
            try
            {
                System.Threading.Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo("en-US");
                IFormatProvider formatter = new NumberFormatInfo { NumberDecimalSeparator = "." };
                Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
                Encoding ANSI = Encoding.GetEncoding(1251);



                List<string> VarArr = new List<string>();
                string[] StrArr;
                string Line;


                foreach (var IC in ReadSignals)
                {
                    int k = 0;
                    string TimeSansWithTag = null;
                    //List<string> GridList = new List<string>();
                    List<SignalModel> BoleanSignals = new List<SignalModel>();
                    bool IsNameWithTag = false;
                    string IsDost = null;
                    using (StreamReader sr = new StreamReader(Path, ANSI))
                    {
                        while ((Line = sr.ReadLine()) != null)
                        {
                            if (k < 4)
                            {
                                k++;
                                continue;
                            }

                            Line = Line.ToUpper();

                            StrArr = Line.ToUpper().Split(new char[] { '\t' }, StringSplitOptions.RemoveEmptyEntries);


                            //<------------------------------------------------------------------------------------------------------------>

                            if (IsNameWithTag && StrArr[0].IndexOf(IC.Name) == -1) // Если пошел следующий датчик, нам нужно добавить предыдущий с #
                            {
                                int result = 0;
                                string status = null;
                                foreach (var item in BoleanSignals)
                                {
                                    switch (status = item.Status)
                                    {
                                        case "ДОСТ":
                                            result += (int)Math.Pow(2, int.Parse(item.Name.Replace($"{IC.Name}#", ""))) * ConvertBoolStringToInt((string)item.NewValue);
                                            break;
                                        case "ПОВТ.ДОСТ":
                                            result += (int)Math.Pow(2, int.Parse(item.Name.Replace($"{IC.Name}#", ""))) * ConvertBoolStringToInt((string)item.NewValue);
                                            break;
                                        default:
                                            break;
                                    }
                                }
                                string str = GetCategory(IC.Name);
                                if (str == "-1")
                                {
                                    IsReliable = false;
                                }
                                IC.SetPropOnFindDataInDB(result, status, str, TimeSansWithTag);
                                FoundSignalsInDB.Add(IC);
                                CheckFoundSignals.Add(IC);
                                break;
                            }

                            //<------------------------------------------------------------------------------------------------------------>

                            if (IC.Name.IndexOf("_Z0") != -1)// Если встречается датчик со значением Z0
                            {
                                if (StrArr[2] == "---")
                                {
                                    StrArr = Line.ToUpper().Replace("---", "-\t-").Split(new char[] { '\t' }, StringSplitOptions.RemoveEmptyEntries);
                                }

                                if (StrArr[0].IndexOf(IC.Name) != -1)
                                {
                                    IsNameWithTag = true;
                                    TimeSansWithTag = StrArr[1].Replace("<", "");
                                    IsDost = StrArr[3];
                                    //GridList.Add($"{StrArr[0].Replace($"{IC}#", "")}\t{StrArr[2]}\t{StrArr[3]}");
                                    var CloneIC = (SignalModel)IC.Clone();
                                    CloneIC.Name = StrArr[0];
                                    CloneIC.SetPropOnFindDataInDB(StrArr[2], StrArr[3], null, null);
                                    BoleanSignals.Add(CloneIC);
                                }
                            }

                            //<------------------------------------------------------------------------------------------------------------>

                            else // Если встречается обычный датчик
                            {

                                //Запишем 

                                if (IC.Name.Contains(StrArr[0]))
                                {
                                    string str = GetCategory(StrArr[0]);
                                    if (str == "-1")
                                    {
                                        IsReliable = false;
                                    }
                                    //DBArray.Add($"{IC}{"\t"}{StrArr[2]}{"\t"}{StrArr[3]}\t{str}\t{StrArr[1].Replace("<", "")}");
                                    //CheckFoundSignals.Add(IC);

                                    if (StrArr[2] == "-")
                                    {
                                        continue;
                                    }

                                    if (StrArr[2] == "---")
                                    {
                                        StrArr = Line.ToUpper().Replace("---", "-\t-").Split(new char[] { '\t' }, StringSplitOptions.RemoveEmptyEntries);
                                    }

                                    // Если встречается сигнал с давлением (*CP* + XQ01)
                                    if (IC.Name.Substring(7, 2) == "CP" && IC.Name.Substring(IC.Name.Length - 4, 4) == "XQ01")
                                    {
                                        if (StrArr[2] == "-")
                                        {
                                            break;
                                        }

                                        string[] caption_array = StrArr[4].ToUpper().Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                                        string preasure_dimention = null;
                                        foreach (var caption in caption_array)
                                        {
                                            if (caption == "МПА" || caption == "КПА")
                                            {
                                                preasure_dimention = caption;
                                            }
                                        }

                                        if (preasure_dimention == null)
                                        {
                                            new Exception($"Неверный формат записи в БД для сигнала {IC.Name}");
                                        }

                                        double new_value = -1;
                                        if (preasure_dimention == "МПА")
                                        {
                                            new_value = double.Parse(StrArr[2], formatter) + 0.101325;
                                        }
                                        else if (true)
                                        {
                                            new_value = double.Parse(StrArr[2], formatter) + 101.325;
                                        }
                                        
                                        IC.SetPropOnFindDataInDB(new_value.ToString(), StrArr[3], str, StrArr[1].Replace("<", ""));
                                        FoundSignalsInDB.Add(IC);
                                        CheckFoundSignals.Add(IC);

                                        break;
                                    }

                                    IC.SetPropOnFindDataInDB(StrArr[2], StrArr[3], str, StrArr[1].Replace("<", ""));
                                    FoundSignalsInDB.Add(IC);
                                    CheckFoundSignals.Add(IC);

                                    break;
                                }
                            }
                            k++;
                        }
                    }
                }

                // Нужно проверить массив всех элементов на сигналы _XQ08, найти для регулятора сигналы xc01 и xc02 в срезе
                foreach (var item in FoundSignalsInDB)
                {
                    if (item.Name.Contains("_XQ08"))
                    {
                        using (StreamReader sr = new StreamReader(Path, ANSI))
                        {
                            string ReplacedName = null;
                            int k = 0;
                            while ((Line = sr.ReadLine()) != null)
                            {
                                if (k < 4)
                                {
                                    k++;
                                    continue;
                                }

                                Line = Line.ToUpper();

                                StrArr = Line.ToUpper().Split(new char[] { '\t' }, StringSplitOptions.RemoveEmptyEntries);
                                ReplacedName = item.Name.Replace("_XQ08", "");

                                if (StrArr[0].Contains(ReplacedName + "_XC01") && StrArr[2] == "ДА")
                                {
                                    item.NewValue = 100;
                                }
                                if (StrArr[0].Contains(ReplacedName + "_XC02") && StrArr[2] == "ДА")
                                {
                                    item.NewValue = 0;
                                }
                            }
                        }
                    }
                }


            }
            catch (FileNotFoundException)
            {
                Console.WriteLine("Файл " + Path + " не был найден");
            }
        }

        /// <summary> Находим все данные в срезе, которые нас интересуют (28.04.22 slice format)</summary>
        private static void FindDataInDB_28_04_22(string Path, ref List<SignalModel> FoundSignalsInDB, ref List<SignalModel> ReadSignals, ref bool IsReliable, ref List<SignalModel> CheckFoundSignals)
        {
            try
            {
                System.Threading.Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo("en-US");
                IFormatProvider formatter = new NumberFormatInfo { NumberDecimalSeparator = "." };
                Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
                Encoding ANSI = Encoding.GetEncoding(1251);



                List<string> VarArr = new List<string>();
                string[] StrArr;
                string Line;


                foreach (var IC in ReadSignals)
                {
                    int k = 0;
                    string TimeSansWithTag = null;
                    //List<string> GridList = new List<string>();
                    List<SignalModel> BoleanSignals = new List<SignalModel>();
                    bool IsNameWithTag = false;
                    string IsDost = null;
                    using (StreamReader sr = new StreamReader(Path, Encoding.Default))
                    {
                        while ((Line = sr.ReadLine()) != null)
                        {
                            if (k < 4)
                            {
                                k++;
                                continue;
                            }
                            Line = Line.ToUpper();

                            StrArr = Line.ToUpper().Split(new char[] { '\t' });

                            //<------------------------------------------------------------------------------------------------------------>

                            if (IsNameWithTag && StrArr[0].IndexOf(IC.Name) == -1) // Если пошел следующий датчик, нам нужно добавить предыдущий с #
                            {
                                int result = 0;
                                string status = null;
                                foreach (var item in BoleanSignals)
                                {
                                    switch (status = item.Status)
                                    {
                                        case "ДОСТ":
                                            result += (int)Math.Pow(2, int.Parse(item.Name.Replace($"{IC.Name}#", ""))) * ConvertBoolStringToInt((string)item.NewValue);
                                            break;
                                        case "ПОВТ.ДОСТ":
                                            result += (int)Math.Pow(2, int.Parse(item.Name.Replace($"{IC.Name}#", ""))) * ConvertBoolStringToInt((string)item.NewValue);
                                            break;
                                        default:
                                            break;
                                    }
                                }
                                string str = GetCategory(IC.Name);
                                if (str == "-1")
                                {
                                    IsReliable = false;
                                }
                                IC.SetPropOnFindDataInDB(result, status, str, TimeSansWithTag);
                                FoundSignalsInDB.Add(IC);
                                CheckFoundSignals.Add(IC);
                                break;
                            }

                            //<------------------------------------------------------------------------------------------------------------>

                            if (IC.Name.IndexOf("_Z0") != -1)// Если встречается датчик со значением Z0
                            {
                                if (StrArr[0].IndexOf(IC.Name) != -1)
                                {
                                    IsNameWithTag = true;
                                    TimeSansWithTag = StrArr[4].Replace("<", "");
                                    IsDost = StrArr[3];
                                    //GridList.Add($"{StrArr[0].Replace($"{IC}#", "")}\t{StrArr[2]}\t{StrArr[3]}");
                                    var CloneIC = (SignalModel)IC.Clone();
                                    CloneIC.Name = StrArr[0];
                                    CloneIC.SetPropOnFindDataInDB(StrArr[1], StrArr[3], null, null);
                                    BoleanSignals.Add(CloneIC);
                                }
                            }

                            //<------------------------------------------------------------------------------------------------------------>

                            else // Если встречается обычный датчик
                            {

                                //Запишем 

                                if (IC.Name.Contains(StrArr[0]))
                                {
                                    string str = GetCategory(StrArr[0]);
                                    if (str == "-1")
                                    {
                                        IsReliable = false;
                                    }
                                    //DBArray.Add($"{IC}{"\t"}{StrArr[2]}{"\t"}{StrArr[3]}\t{str}\t{StrArr[1].Replace("<", "")}");
                                    //CheckFoundSignals.Add(IC);

                                    if (StrArr[1] == "-")
                                    {
                                        continue;
                                    }

                                    if (IC.Name.Substring(7, 2) == "CP" && IC.Name.Substring(IC.Name.Length - 4, 4) == "XQ01")
                                    {
                                        string[] caption_array = StrArr[5].ToUpper().Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                                        string preasure_dimention = null;
                                        foreach (var caption in caption_array)
                                        {
                                            if (caption == "МПА" || caption == "КПА")
                                            {
                                                preasure_dimention = caption;
                                            }
                                        }

                                        if (preasure_dimention == null)
                                        {
                                            new Exception($"Неверный формат записи в БД для сигнала {IC.Name}");
                                        }

                                        double new_value = -1;
                                        if (preasure_dimention == "МПА")
                                        {
                                            new_value = double.Parse(StrArr[1], formatter) + 0.101325;
                                        }
                                        else if (true)
                                        {
                                            new_value = double.Parse(StrArr[1], formatter) + 101.325;
                                        }

                                        IC.SetPropOnFindDataInDB(new_value.ToString(), StrArr[3], str, StrArr[4].Replace("<", ""));
                                        FoundSignalsInDB.Add(IC);
                                        CheckFoundSignals.Add(IC);

                                        break;
                                    }

                                    IC.SetPropOnFindDataInDB(StrArr[1], StrArr[3], str, StrArr[4].Replace("<", ""));
                                    FoundSignalsInDB.Add(IC);
                                    CheckFoundSignals.Add(IC);

                                    break;
                                }
                            }
                            k++;
                        }
                    }
                }

                // Нужно проверить массив всех элементов на сигналы _XQ08, найти для регулятора сигналы xc01 и xc02 в срезе
                foreach (var item in FoundSignalsInDB)
                {
                    if (item.Name.Contains("_XQ08"))
                    {
                        using (StreamReader sr = new StreamReader(Path, Encoding.Default))
                        {
                            string ReplacedName = null;
                            int k = 0;
                            while ((Line = sr.ReadLine()) != null)
                            {
                                if (k < 4)
                                {
                                    k++;
                                    continue;
                                }

                                Line = Line.ToUpper();

                                StrArr = Line.ToUpper().Split(new char[] { '\t' });
                                ReplacedName = item.Name.Replace("_XQ08", "");

                                if (StrArr[0].Contains(ReplacedName + "_XC01") && StrArr[1] == "ДА")
                                {
                                    item.NewValue = 100;
                                }
                                if (StrArr[0].Contains(ReplacedName + "_XC02") && StrArr[1] == "ДА")
                                {
                                    item.NewValue = 0;
                                }
                            }
                        }
                    }
                }


            }
            catch (FileNotFoundException)
            {
                Console.WriteLine("Файл " + Path + " не был найден");
            }
        }

        /// <summary> Нужно найти свежую информацию о датчике, если она есть в большом массиве, где данные не всегда записываются (06.07.18 slice format)</summary>
        private static void FindFreshDataInDB_06_07_18(ref List<SignalModel> FoundSignalsInDB, ref List<SignalModel> FoundSignalsInDBFresh, string RelatePathToFolder, double EndTime, ref ObservableCollection<string> _TextInformationFromListDB)
        {
            System.Threading.Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo("en-US");
            IFormatProvider formatter = new NumberFormatInfo { NumberDecimalSeparator = "." };
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
            Encoding ANSI = Encoding.GetEncoding(1251);
            string[] filePaths = Directory.GetFiles(RelatePathToFolder); // Массив путей со всеми элементами
            string LastValueOfSensor = null;
            string LastDateOfSensor = null;
            double ConvertedDate; // конвертируем дату
            string[] ConvertedTimeArr; // конвертируем время
            double ConvertedTimeDouble; // конечное время, переведенное
            //string VarStr; // присаиваем сюда значение item из цикла foreach
            bool IsEnd = false;
            int Count = 0;


            foreach (var SensorName in FoundSignalsInDB)
            {
                Count++;
                bool IsFound_XC = false; // Нужно для того, чтобы заменить _XQ08, в зависимости от сигналов _XC01 и _XC02
                string GridCategory = null;
                string TimeSansWithTag = null;
                //List<string> GridList = new List<string>();
                List<SignalModel> BoleanSignals = new List<SignalModel>();
                bool IsNameWithTag = false;
                string IsDost = null;
                FoundSignalsInDBFresh.Add((SignalModel)SensorName.Clone());
                //VarStr = SensorName;
                //string[] StrArr = SensorName.Split(new char[] { '\t' }, StringSplitOptions.RemoveEmptyEntries); // Разделил строку с маленькой базы данных
                foreach (var path in filePaths)
                {
                    string filename = Path.GetFileName(path); // Получаем имя файла
                    string RuteName = SensorName.Name.Substring(2, 3); // Получил 3 буквы
                    if (filename.IndexOf(RuteName) != -1) // Находим файлы, в которых содержатся 3 буквы
                    {
                        _TextInformationFromListDB.Clear();
                        _TextInformationFromListDB.Add($"Расчет {Count} сигнала из {FoundSignalsInDB.Count}!");
                        using (StreamReader sr = new StreamReader($"{RelatePathToFolder}/{filename}", ANSI))
                        {
                            string line;
                            string[] lineSplit;
                            int k = 0;
                            while ((line = sr.ReadLine()) != null)
                            {
                                if (k < 4)
                                {
                                    k++;
                                    continue;
                                }

                                line = line.ToUpper();

                                lineSplit = line.Split(new string[] { "\t", " " }, StringSplitOptions.RemoveEmptyEntries); // Делим строку с большой базы данных
                                ConvertedDate = double.Parse(lineSplit[0].Split(new string[] { "." }, StringSplitOptions.RemoveEmptyEntries)[0], formatter);
                                ConvertedTimeArr = lineSplit[1].Split(new string[] { ":" }, StringSplitOptions.RemoveEmptyEntries);
                                ConvertedTimeDouble = (ConvertedDate - 6) * 24 * 60 * 60 + double.Parse(ConvertedTimeArr[0], formatter) * 3600 + double.Parse(ConvertedTimeArr[1], formatter) * 60 + double.Parse(ConvertedTimeArr[2], formatter) / 1000;


                                if (IsNameWithTag && lineSplit[2].IndexOf(SensorName.Name) == -1) // Если пошел следующий датчик, нам нужно добавить предыдущий с #
                                {
                                    int result = 0;
                                    string status = null;
                                    foreach (var item in BoleanSignals)
                                    {
                                        switch (status = item.Status)
                                        {
                                            case "ДОСТ":
                                                result += (int)Math.Pow(2, int.Parse(item.Name.Replace($"{SensorName.Name}#", ""))) * ConvertBoolStringToInt((string)item.NewValue);
                                                break;
                                            case "ПОВТ.ДОСТ":
                                                result += (int)Math.Pow(2, int.Parse(item.Name.Replace($"{SensorName.Name}#", ""))) * ConvertBoolStringToInt((string)item.NewValue);
                                                break;
                                            default:
                                                break;
                                        }
                                    }

                                    FoundSignalsInDBFresh[^1].NewValue = result;
                                    FoundSignalsInDBFresh[^1].Date = TimeSansWithTag;
                                    IsNameWithTag = false;
                                    BoleanSignals = new List<SignalModel>();
                                }

                                if (EndTime < ConvertedTimeDouble) // Логика выхода из цикла, когда заданное время привышает время у датчика в БД
                                {
                                    IsEnd = true;
                                    break;
                                }

                                if (SensorName.Name.IndexOf("_Z0") != -1)// Если встречается датчик со значением Z0
                                {
                                    if (lineSplit[2].IndexOf(SensorName.Name) != -1)
                                    {
                                        GridCategory = SensorName.Category;
                                        IsNameWithTag = true;
                                        LastValueOfSensor = lineSplit[5];
                                        LastDateOfSensor = $"{lineSplit[0]} {lineSplit[1]}";
                                        TimeSansWithTag = LastDateOfSensor;
                                        IsDost = SensorName.Status;

                                        //GridList.Add($"{lineSplit[2].Replace($"{StrArr[0]}#", "")}\t{lineSplit[5]}\t{StrArr[2]}");

                                        var CloneIC = (SignalModel)SensorName.Clone();
                                        CloneIC.Name = lineSplit[2];
                                        CloneIC.SetPropOnFindDataInDB(lineSplit[5], lineSplit[6], null, null);
                                        BoleanSignals.Add(CloneIC);
                                    }
                                }
                                else if (SensorName.Name.IndexOf("_XA") != -1)
                                {
                                    if (lineSplit[2].IndexOf(SensorName.Name) == 0)
                                    {
                                        if (lineSplit[6] == "ДОСТ" || lineSplit[6] == "ПОВТ.ДОСТ")
                                        {
                                            LastValueOfSensor = lineSplit[5];
                                            LastDateOfSensor = $"{lineSplit[0]} {lineSplit[1]}";
                                            FoundSignalsInDBFresh[^1].NewValue = LastValueOfSensor;
                                            FoundSignalsInDBFresh[^1].Date = LastDateOfSensor;

                                        }
                                    }
                                }
                                else if (SensorName.Name.IndexOf("_XV01") != -1)
                                {
                                    if (lineSplit[2].IndexOf(SensorName.Name) == 0)
                                    {
                                        if (lineSplit[6] == "ДОСТ" || lineSplit[6] == "ПОВТ.ДОСТ")
                                        {
                                            LastValueOfSensor = lineSplit[5];
                                            LastDateOfSensor = $"{lineSplit[0]} {lineSplit[1]}";
                                            FoundSignalsInDBFresh[^1].NewValue = LastValueOfSensor;
                                            FoundSignalsInDBFresh[^1].Date = LastDateOfSensor;
                                        }
                                    }
                                }
                                else if (SensorName.Name.IndexOf("_XQ08") != -1)
                                {
                                    var ReplacedName = SensorName.Name.Replace("_XQ08", "");
                                    if (lineSplit[2].IndexOf(ReplacedName + "_XC01") == 0 && lineSplit[5] == "ДА" && (lineSplit[6] == "ДОСТ" || lineSplit[6] == "ПОВТ.ДОСТ"))
                                    {
                                        IsFound_XC = true;
                                        FoundSignalsInDBFresh[^1].NewValue = 100;
                                    }
                                    else if (lineSplit[2].IndexOf(ReplacedName + "_XC02") == 0 && lineSplit[5] == "ДА" && (lineSplit[6] == "ДОСТ" || lineSplit[6] == "ПОВТ.ДОСТ"))
                                    {
                                        IsFound_XC = true;
                                        FoundSignalsInDBFresh[^1].NewValue = 0;
                                    }
                                    else if (lineSplit[2].IndexOf(SensorName.Name) == 0)
                                    {
                                        if (lineSplit[4] == "ДОСТ" || lineSplit[4] == "ПОВТ.ДОСТ")
                                        {
                                            if (IsFound_XC)
                                            {
                                                IsFound_XC = false;
                                                LastDateOfSensor = $"{lineSplit[0]} {lineSplit[1]}";
                                                FoundSignalsInDBFresh[^1].Date = LastDateOfSensor;
                                                continue;
                                            }

                                            LastValueOfSensor = lineSplit[3];
                                            LastDateOfSensor = $"{lineSplit[0]} {lineSplit[1]}";
                                            FoundSignalsInDBFresh[^1].NewValue = LastValueOfSensor;
                                            FoundSignalsInDBFresh[^1].Date = LastDateOfSensor;
                                        }
                                    }
                                }
                                else
                                {
                                    if (lineSplit[2].IndexOf(SensorName.Name) == 0)
                                    {
                                        string status = null;
                                        int value_index = 0;
                                        if (lineSplit[4] == ">")
                                        {
                                            status = lineSplit[6];
                                            value_index = 5;
                                        }
                                        else
                                        {
                                            status = lineSplit[4];
                                            value_index = 3;
                                        }
                                        
                                        if (status == "ДОСТ" || status == "ПОВТ.ДОСТ")
                                        {

                                            // Если датчик с давлением
                                            if (SensorName.Name.Substring(7, 2) == "CP" && SensorName.Name.Substring(SensorName.Name.Length - 4, 4) == "XQ01")
                                            {
                                                string preasure_dimention = null;
                                                foreach (var caption in lineSplit)
                                                {
                                                    if (caption == "МПА" || caption == "КПА" || caption == "КПА(А)")
                                                    {
                                                        preasure_dimention = caption;
                                                    }
                                                }

                                                if (preasure_dimention == null)
                                                {
                                                    new Exception($"Неверный формат записи в БД для сигнала {SensorName.Name}");
                                                }

                                                if (preasure_dimention == "МПА")
                                                {
                                                    LastValueOfSensor = (double.Parse(lineSplit[value_index], formatter) + 0.101325).ToString();
                                                }
                                                else if (preasure_dimention == "КПА" || preasure_dimention == "КПА(А)")
                                                {
                                                    LastValueOfSensor = (double.Parse(lineSplit[value_index], formatter) + 101.325).ToString();
                                                }
                                            }
                                            else
                                            {
                                                LastValueOfSensor = lineSplit[value_index];
                                            }
                                            LastDateOfSensor = $"{lineSplit[0]} {lineSplit[1]}";
                                            FoundSignalsInDBFresh[^1].NewValue = LastValueOfSensor;
                                            FoundSignalsInDBFresh[^1].Date = LastDateOfSensor;
                                        }
                                    }
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
            }

        }

        /// <summary> Нужно найти свежую информацию о датчике, если она есть в большом массиве, где данные не всегда записываются</summary>
        private static void FindFreshDataInDB_28_04_22(ref List<SignalModel> FoundSignalsInDB, ref List<SignalModel> FoundSignalsInDBFresh, string RelatePathToFolder, double EndTime, ref ObservableCollection<string> _TextInformationFromListDB)
        {
            System.Threading.Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo("en-US");
            IFormatProvider formatter = new NumberFormatInfo { NumberDecimalSeparator = "." };
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
            Encoding ANSI = Encoding.GetEncoding(1251);
            string[] filePaths = Directory.GetFiles(RelatePathToFolder); // Массив путей со всеми элементами
            string LastValueOfSensor = null;
            string LastDateOfSensor = null;
            double ConvertedDate; // конвертируем дату
            string[] ConvertedTimeArr; // конвертируем время
            double ConvertedTimeDouble; // конечное время, переведенное
            //string VarStr; // присаиваем сюда значение item из цикла foreach
            bool IsEnd = false;
            int Count = 0;


            foreach (var SensorName in FoundSignalsInDB)
            {
                Count++;
                bool IsFound_XC = false; // Нужно для того, чтобы заменить _XQ08, в зависимости от сигналов _XC01 и _XC02
                string GridCategory = null;
                string TimeSansWithTag = null;
                //List<string> GridList = new List<string>();
                List<SignalModel> BoleanSignals = new List<SignalModel>();
                bool IsNameWithTag = false;
                string IsDost = null;
                FoundSignalsInDBFresh.Add((SignalModel)SensorName.Clone());
                //VarStr = SensorName;
                //string[] StrArr = SensorName.Split(new char[] { '\t' }, StringSplitOptions.RemoveEmptyEntries); // Разделил строку с маленькой базы данных
                foreach (var path in filePaths)
                {
                    string filename = Path.GetFileName(path); // Получаем имя файла
                    string RuteName = SensorName.Name.Substring(2, 3); // Получил 3 буквы
                    if (filename.IndexOf(RuteName) != -1) // Находим файлы, в которых содержатся 3 буквы
                    {
                        _TextInformationFromListDB.Clear();
                        _TextInformationFromListDB.Add($"Расчет {Count} сигнала из {FoundSignalsInDB.Count}!");
                        using (StreamReader sr = new StreamReader($"{RelatePathToFolder}/{filename}", Encoding.Default))
                        {
                            string line;
                            string[] lineSplit;
                            int k = 0;
                            while ((line = sr.ReadLine()) != null)
                            {
                                if (k < 4)
                                {
                                    k++;
                                    continue;
                                }

                                line = line.ToUpper();

                                lineSplit = line.ToUpper().Split(new string[] { "\t", " " }, StringSplitOptions.RemoveEmptyEntries); // Делим строку с большой базы данных
                                ConvertedDate = double.Parse(lineSplit[0].Split(new string[] { "." }, StringSplitOptions.RemoveEmptyEntries)[0], formatter);
                                ConvertedTimeArr = lineSplit[1].Split(new string[] { ":" }, StringSplitOptions.RemoveEmptyEntries);
                                ConvertedTimeDouble = (ConvertedDate - 6) * 24 * 60 * 60 + double.Parse(ConvertedTimeArr[0], formatter) * 3600 + double.Parse(ConvertedTimeArr[1], formatter) * 60 + double.Parse(ConvertedTimeArr[2], formatter) / 1000;


                                if (IsNameWithTag && lineSplit[2].IndexOf(SensorName.Name) == -1) // Если пошел следующий датчик, нам нужно добавить предыдущий с #
                                {
                                    int result = 0;
                                    string status = null;
                                    foreach (var item in BoleanSignals)
                                    {
                                        switch (status = item.Status)
                                        {
                                            case "ДОСТ":
                                                result += (int)Math.Pow(2, int.Parse(item.Name.Replace($"{SensorName.Name}#", ""))) * ConvertBoolStringToInt((string)item.NewValue);
                                                break;
                                            case "ПОВТ.ДОСТ":
                                                result += (int)Math.Pow(2, int.Parse(item.Name.Replace($"{SensorName.Name}#", ""))) * ConvertBoolStringToInt((string)item.NewValue);
                                                break;
                                            default:
                                                break;
                                        }
                                    }

                                    FoundSignalsInDBFresh[^1].NewValue = result;
                                    FoundSignalsInDBFresh[^1].Date = TimeSansWithTag;
                                    IsNameWithTag = false;
                                    BoleanSignals = new List<SignalModel>();
                                }

                                if (EndTime < ConvertedTimeDouble) // Логика выхода из цикла, когда заданное время привышает время у датчика в БД
                                {
                                    IsEnd = true;
                                    break;
                                }

                                if (SensorName.Name.IndexOf("_Z0") != -1)// Если встречается датчик со значением Z0
                                {
                                    if (lineSplit[2].IndexOf(SensorName.Name) != -1)
                                    {
                                        GridCategory = SensorName.Category;
                                        IsNameWithTag = true;
                                        LastValueOfSensor = lineSplit[5];
                                        LastDateOfSensor = $"{lineSplit[0]} {lineSplit[1]}";
                                        TimeSansWithTag = LastDateOfSensor;
                                        IsDost = SensorName.Status;

                                        //GridList.Add($"{lineSplit[2].Replace($"{StrArr[0]}#", "")}\t{lineSplit[5]}\t{StrArr[2]}");

                                        var CloneIC = (SignalModel)SensorName.Clone();
                                        CloneIC.Name = lineSplit[2];
                                        CloneIC.SetPropOnFindDataInDB(lineSplit[5], lineSplit[6], null, null);
                                        BoleanSignals.Add(CloneIC);
                                    }
                                }
                                else if (SensorName.Name.IndexOf("_XA") != -1)
                                {
                                    if (lineSplit[2].IndexOf(SensorName.Name) == 0)
                                    {
                                        if (lineSplit[6] == "ДОСТ" || lineSplit[6] == "ПОВТ.ДОСТ")
                                        {
                                            LastValueOfSensor = lineSplit[5];
                                            LastDateOfSensor = $"{lineSplit[0]} {lineSplit[1]}";
                                            FoundSignalsInDBFresh[^1].NewValue = LastValueOfSensor;
                                            FoundSignalsInDBFresh[^1].Date = LastDateOfSensor;

                                        }
                                    }
                                }
                                else if (SensorName.Name.IndexOf("_XV01") != -1)
                                {
                                    if (lineSplit[2].IndexOf(SensorName.Name) == 0)
                                    {
                                        if (lineSplit[6] == "ДОСТ" || lineSplit[6] == "ПОВТ.ДОСТ")
                                        {
                                            LastValueOfSensor = lineSplit[5];
                                            LastDateOfSensor = $"{lineSplit[0]} {lineSplit[1]}";
                                            FoundSignalsInDBFresh[^1].NewValue = LastValueOfSensor;
                                            FoundSignalsInDBFresh[^1].Date = LastDateOfSensor;
                                        }
                                    }
                                }
                                else if (SensorName.Name.IndexOf("_XQ08") != -1)
                                {
                                    var ReplacedName = SensorName.Name.Replace("_XQ08", "");
                                    if (lineSplit[2].IndexOf(ReplacedName + "_XC01") == 0 && lineSplit[5] == "ДА" && (lineSplit[6] == "ДОСТ" || lineSplit[6] == "ПОВТ.ДОСТ"))
                                    {
                                        IsFound_XC = true;
                                        FoundSignalsInDBFresh[^1].NewValue = 100;
                                    }
                                    else if (lineSplit[2].IndexOf(ReplacedName + "_XC02") == 0 && lineSplit[5] == "ДА" && (lineSplit[6] == "ДОСТ" || lineSplit[6] == "ПОВТ.ДОСТ"))
                                    {
                                        IsFound_XC = true;
                                        FoundSignalsInDBFresh[^1].NewValue = 0;
                                    }
                                    else if (lineSplit[2].IndexOf(SensorName.Name) == 0)
                                    {
                                        if (lineSplit[4] == "ДОСТ" || lineSplit[4] == "ПОВТ.ДОСТ")
                                        {
                                            if (IsFound_XC)
                                            {
                                                IsFound_XC = false;
                                                LastDateOfSensor = $"{lineSplit[0]} {lineSplit[1]}";
                                                FoundSignalsInDBFresh[^1].Date = LastDateOfSensor;
                                                continue;
                                            }

                                            LastValueOfSensor = lineSplit[3];
                                            LastDateOfSensor = $"{lineSplit[0]} {lineSplit[1]}";
                                            FoundSignalsInDBFresh[^1].NewValue = LastValueOfSensor;
                                            FoundSignalsInDBFresh[^1].Date = LastDateOfSensor;
                                        }
                                    }
                                }
                                else
                                {
                                    if (lineSplit[2].IndexOf(SensorName.Name) == 0)
                                    {
                                        string status = null;
                                        int value_index = 0;
                                        if (lineSplit[4] == ">")
                                        {
                                            status = lineSplit[6];
                                            value_index = 5;
                                        }
                                        else
                                        {
                                            status = lineSplit[4];
                                            value_index = 3;
                                        }

                                        if (status == "ДОСТ" || status == "ПОВТ.ДОСТ")
                                        {
                                            // Если датчик с давлением
                                            if (SensorName.Name.Substring(7, 2) == "CP" && SensorName.Name.Substring(SensorName.Name.Length - 4, 4) == "XQ01")
                                            {
                                                string preasure_dimention = null;
                                                foreach (var caption in lineSplit)
                                                {
                                                    if (caption == "МПА" || caption == "КПА")
                                                    {
                                                        preasure_dimention = caption;
                                                    }
                                                }

                                                if (preasure_dimention == null)
                                                {
                                                    new Exception($"Неверный формат записи в БД для сигнала {SensorName.Name}");
                                                }

                                                if (preasure_dimention == "МПА")
                                                {
                                                    LastValueOfSensor = (double.Parse(lineSplit[value_index], formatter) + 0.101325).ToString();
                                                }
                                                else if (preasure_dimention == "КПА")
                                                {
                                                    LastValueOfSensor = (double.Parse(lineSplit[value_index], formatter) + 101.325).ToString();
                                                }
                                                
                                            }
                                            else
                                            {
                                                LastValueOfSensor = lineSplit[value_index];
                                            }
                                            LastDateOfSensor = $"{lineSplit[0]} {lineSplit[1]}";
                                            FoundSignalsInDBFresh[^1].NewValue = LastValueOfSensor;
                                            FoundSignalsInDBFresh[^1].Date = LastDateOfSensor;
                                        }
                                    }
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
            }

        }

        ///<summary> Записываем информацию в файл !List_IC.txt </summary>
        private static void CreateFinalSignalsList(string WorkPath, ref List<SignalModel> StringArrayFromDB, bool IsReliable, List<SignalModel> NotFoundSignals, List<SignalModel> InvalidSignals, ref List<SignalModel> FinalSignalsList)
        {
            IFormatProvider formatter = new NumberFormatInfo { NumberDecimalSeparator = "." };
            int i = 0;
            List<SignalModel> StringArrayAfterSort = new List<SignalModel>(); // Конечный массив
            List<SignalModel> StringArrayAfterCategorySort = new List<SignalModel>(); // Массив после сортировки по категориям
            List<SignalModel> CorrectSignals = new List<SignalModel>(); // Корректные сигналы 

            for (int j = -1; j < 8; j++) // СОРТИРОВКА МАССИВА С ЭЛЕМЕНТАМИ
            {
                StringArrayAfterCategorySort = new List<SignalModel>();
                foreach (var item in StringArrayFromDB)
                {
                    //string[] ArrOfStr = item.Split(new string[] { "\t" }, StringSplitOptions.RemoveEmptyEntries);
                    if (item.Category == j.ToString())
                    {
                        StringArrayAfterCategorySort.Add(item);
                    }
                }

                var query = from Str in StringArrayAfterCategorySort
                                            orderby Str.Name.Substring(2, 3)
                                            select Str;
                foreach (var item in query)
                {
                    StringArrayAfterSort.Add(item);
                }
            }


            foreach (var item in StringArrayAfterSort) // Записываем в выходной файл ДОСТоверные сигналы
            {
                //string[] ArrOfStr = item.Split(new string[] { "\t" }, StringSplitOptions.RemoveEmptyEntries);
                double d;
                if (item.Status == "ДОСТ" || item.Status == "ПОВТ.ДОСТ")
                {
                    if (double.TryParse(item.NewValue.ToString(), NumberStyles.Number, formatter, out d))
                    {
                        CorrectSignals.Add(item);
                    }
                    else if (item.NewValue.ToString() == "ДА")
                    {
                        item.NewValue = 1;
                        CorrectSignals.Add(item);
                    }
                    else if (item.NewValue.ToString() == "НЕТ")
                    {
                        item.NewValue = 0;
                        CorrectSignals.Add(item);
                    }
                }
                else // Добавляем неДОСТоверные сигналы в массив неДОСТоверных сигналов InvalidSignals
                {
                    if (double.TryParse(item.NewValue.ToString(), NumberStyles.Number, formatter, out d))
                    {
                        InvalidSignals.Add(item);
                    }
                    else if (item.NewValue.ToString() == "ДА")
                    {
                        item.NewValue = 1;
                        InvalidSignals.Add(item);
                    }
                    else if (item.NewValue.ToString() == "НЕТ")
                    {
                        item.NewValue = 0;
                        InvalidSignals.Add(item);
                    }
                    else if (item.NewValue.ToString() == "РЕМОНТ")
                    {
                        item.NewValue = "РЕМОНТ";
                        InvalidSignals.Add(item);
                    }
                    else if (item.NewValue.ToString() == "-")
                    {
                        item.NewValue = "-";
                        InvalidSignals.Add(item);
                    }
                    
                }
                i++;
            }
            foreach (var item in CorrectSignals)
            {
                item.Number = FinalSignalsList.Count + 1;
                FinalSignalsList.Add(item);
            }
            foreach (var item in InvalidSignals)
            {
                item.Number = FinalSignalsList.Count + 1;
                FinalSignalsList.Add(item);
            }
            foreach (var item in NotFoundSignals)
            {
                item.Number = FinalSignalsList.Count + 1;
                FinalSignalsList.Add(item);
            }

            foreach (var item in FinalSignalsList)
            {
                IsRegulator_XQ08(item);
            }

        }

        public static List<SignalModel> GetListMethod(InfoData _InfoData, string EndTimeFormat, string EndDay,ref ObservableCollection<string> _TextInformationFromListDB, string SelectedSliceFormat, string SelectedDataBaseFormat)
        {
            
                Stopwatch SW = new Stopwatch();
                SW.Start();
                _TextInformationFromListDB.Clear();
                _TextInformationFromListDB.Add($"{_TextInformationFromListDB.Count + 1}) Поиск начался!");
            try
            {
                IFormatProvider formatter = new NumberFormatInfo { NumberDecimalSeparator = "." };
                bool IsReliable = true;
                string ReadPathFromDB = _InfoData.PathToDataFile;
                string WorkPath = _InfoData.PathToListFile;
                string RelatePathToFolder = _InfoData.PathToFolderForListBD;
                double EndTime = ConvertDataFormat(EndTimeFormat, EndDay, formatter);

                //List<string> InvalidSignals = new List<string>(); // Массив с неДОСТоверными сигналами
                //List<string> NotFoundSignals = new List<string>(); // Массив с ненайденными сигналами
                List<string> DBName = new List<string>(); // массив для записи тех элементов, которые нашлись в ДБ
                List<string> StringArrayFromDBFresh = new List<string>(); // конечный массив
                List<string> StringArrayFromDB = new List<string>();// только со среза, старый массив
                List<string> StringArrayFromIC = new List<string>(); // массив сигналов, прочитанных из файла

                List<SignalModel> ReadSignals = new List<SignalModel>(); // Сигналы, которые считали с файла
                List<SignalModel> FoundSignalsInDB = new List<SignalModel>(); // Сигналы, которые нашлись в срезе
                List<SignalModel> FoundSignalsInDBFresh = new List<SignalModel>(); // Сигналы, которые нашлись в изменяющейся базе данных

                List<SignalModel> CheckFoundSignals = new List<SignalModel>(); // Массив с сигналами, которые нашлись в срезе
                List<SignalModel> NotFoundSignals = new List<SignalModel>(); // Не найденные в БД сигналы
                List<SignalModel> InvalidSignals = new List<SignalModel>(); // НеДОСТоверные сигналы
                List<SignalModel> FinalSignalsList = new List<SignalModel>(); // Конечный список сигналов


                ReadDataFromIC(WorkPath, ref ReadSignals); // Прочитали сигналы и добавили в массив имена

                if (SelectedSliceFormat == "06.07.2018")
                {
                    FindDataInDB_06_07_18(ReadPathFromDB, ref FoundSignalsInDB, ref ReadSignals, ref IsReliable, ref CheckFoundSignals);

                }
                else if (SelectedSliceFormat == "22.02.2022")
                {
                    FindDataInDB_28_04_22(ReadPathFromDB, ref FoundSignalsInDB, ref ReadSignals, ref IsReliable, ref CheckFoundSignals);

                }

                if (SelectedDataBaseFormat == "06.07.2018")
                {
                    FindFreshDataInDB_06_07_18(ref FoundSignalsInDB, ref FoundSignalsInDBFresh, RelatePathToFolder, EndTime, ref _TextInformationFromListDB);
                }
                else if (SelectedDataBaseFormat == "22.02.2022")
                {
                    FindFreshDataInDB_28_04_22(ref FoundSignalsInDB, ref FoundSignalsInDBFresh, RelatePathToFolder, EndTime, ref _TextInformationFromListDB);
                }

                IsFoundName(ref ReadSignals, ref NotFoundSignals);

                CreateFinalSignalsList(WorkPath, ref FoundSignalsInDBFresh, IsReliable, NotFoundSignals, InvalidSignals, ref FinalSignalsList);

                SW.Stop();
                TimeSpan ts = SW.Elapsed;
                string elapsedTime = String.Format("{0:00}:{1:00}:{2:00}.{3:00}",
                ts.Hours, ts.Minutes, ts.Seconds,
                ts.Milliseconds / 10);
                _TextInformationFromListDB.Clear();
                _TextInformationFromListDB.Add($"Поиск закончился! Время выполнения - {elapsedTime}");
                MessageBox.Show($"Список создан");
                return FinalSignalsList;
            }
            catch (Exception ex)
            {
                _TextInformationFromListDB.Clear();
                _TextInformationFromListDB.Add($"{ex.Message}");
                return null;
            }
            
        }

        public static void WriteDataToIC(InfoData _InfoData, ref List<SignalModel> Signals)
        {
            using (StreamWriter sw = new StreamWriter(_InfoData.PathToListFile, false, Encoding.Default))
            {
                sw.WriteLine($"{Signals.Count};Count");

                foreach (var Signal in Signals)
                {
                    if (Signal.IsInvariable == true)
                    {
                        Signal.NewValue = Signal.OldValue;
                    }
                    sw.WriteLine(Signal.WriteDataToFile());
                }
            }
            MessageBox.Show($"Список сохранен!");
        }
    }
}
