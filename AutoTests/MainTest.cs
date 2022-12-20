using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Text;
using DBSelectionForm.Models;
using DBSelectionForm.Services;
using AutoTests.AutoTest;
using AutoTests.SensorsTest;
using AutoTests.DBTest;

namespace AutoTests
{
    public class MainTest
    {
        static InfoData GetInfoData()
        {
            FileIOService _fileIOservice;
            string Path = $"{Environment.CurrentDirectory.Replace("AutoTests", "DBSelectionForm")}\\IndoData.json";
            InfoData _InfoData = new InfoData();
            _fileIOservice = new FileIOService(Path);
            if ((_InfoData = _fileIOservice.LoadData()) == null)
            {
                Console.WriteLine("Сначала запустите основной проект и укажите акутальные данные до файлов проекта");
                throw new Exception("Сначала запустите основной проект и укажите акутальные данные до файлов проекта");
            }
            return _InfoData;
        }

        static void Main(string[] args)
        {

            #region Настроечные данные

            InfoData _InfoData = GetInfoData();

            ObservableCollection<string> _TextInformation = new ObservableCollection<string>();

            List<string> _TimeDimension = new List<string>() { "сек", "мин", "час" };

            #endregion

            #region Тестирование работы с датчиками

            Interpol.InterpolTestNO1();

            AutoTest.SensorsTest.FirstPoints.FirstPointNO1(ref _TextInformation, _TimeDimension);

            AutoTest.SensorsTest.FirstPoints.FirstPointNO2(ref _TextInformation, _TimeDimension);

            AutoTest.SensorsTest.FirstPoints.FirstPointNO3(ref _TextInformation, _TimeDimension);

            AutoTest.SensorsTest.LastPoints.LastPointNO1(ref _TextInformation, _TimeDimension);

            AutoTest.SensorsTest.LastPoints.LastPointNO2(ref _TextInformation, _TimeDimension);

            AutoTest.SensorsTest.LastPoints.LastPointNO3(ref _TextInformation, _TimeDimension);

            AutoTest.SensorsTest.BodyPoints.BodyTestNO1(ref _TextInformation, _TimeDimension);


            #endregion

            #region Тестирование работы с задвижками

            AutoTest.ValvesTest.FirstPoints.FirstPointNO1(ref _TextInformation, _TimeDimension);

            AutoTest.ValvesTest.FirstPoints.FirstPointNO2(ref _TextInformation, _TimeDimension);

            AutoTest.ValvesTest.FirstPoints.FirstPointNO3(ref _TextInformation, _TimeDimension);

            AutoTest.ValvesTest.LastPoints.LastPointNO1(ref _TextInformation, _TimeDimension);

            AutoTest.ValvesTest.LastPoints.LastPointNO2(ref _TextInformation, _TimeDimension);

            AutoTest.ValvesTest.LastPoints.LastPointNO3(ref _TextInformation, _TimeDimension);

            #endregion

            #region Тестирование работы программы в различных БД

            DBTest.EncodingTest.EncodingTestNO1();

            DBTest.EncodingTest.EncodingTestNO2();

            #endregion

            #region Тестирование работы с накладками



            #endregion


        }
    }
}
