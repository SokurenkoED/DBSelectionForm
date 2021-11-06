﻿using DBSelectionForm.Infastructure.Commands;
using DBSelectionForm.Models;
using DBSelectionForm.Services;
using DBSelectionForm.ViewModels.Base;
using Microsoft.Win32;
using Microsoft.WindowsAPICodePack.Dialogs;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;

namespace DBSelectionForm.ViewModels
{

    internal class MainWindowViewModel : ViewModel
    {
        private readonly string Path = $"{Environment.CurrentDirectory}\\IndoData.json";
        private FileIOService _fileIOservice;
        private InfoData _InfoData;

        #region Путь до выходного файла !List

        private string _PathToListFile;

        /// <summary>Путь до выходного файла !List</summary>
        public string PathToListFile
        {
            get => _PathToListFile;
            set
            {
                Set(ref _PathToListFile, value);
            }
        }

        #endregion

        #region Путь до базы данных холодного реактора

        private string _PathToDataFile;

        /// <summary>Путь базы данных холодного реактора</summary>
        public string PathToDataFile
        {
            get => _PathToDataFile;
            set
            {
                Set(ref _PathToDataFile, value);
            }
        }

        #endregion

        #region Элементы в текстбоксе

        private string _DataToTextBox;

        /// <summary>Элементы в текстбоксе</summary>
        public string DataToTextBox
        {
            get => _DataToTextBox;
            set
            {
                Set(ref _DataToTextBox, value);
            }
        }

        #endregion

        #region Сообщения о процессе выполнения

        private ObservableCollection<string> _TextInformation = new AsyncObservableCollection<string>();

        /// <summary>Сообщения о процессе выполнения</summary>
        public ObservableCollection<string> TextInformation
        {
            get => _TextInformation;
            set
            {
                Set(ref _TextInformation, value);
            }
        }

        #endregion

        #region Время "С"

        private string _TimeFrom;

        /// <summary>Время "С"</summary>
        public string TimeFrom
        {
            get => _TimeFrom;
            set
            {
                Set(ref _TimeFrom, value);
            }
        }

        #endregion

        #region Время "По"

        private string _TimeTo;

        /// <summary>Время "По"</summary>
        public string TimeTo
        {
            get => _TimeTo;
            set
            {
                Set(ref _TimeTo, value);
            }
        }

        #endregion

        #region Имя датчика

        private string _SensorName;

        /// <summary>Имя датчика</summary>
        public string SensorName
        {
            get => _SensorName;
            set {
                Set(ref _SensorName, value);
            }
        }

        #endregion

        #region Путь до папки с БД

        private string _PathToFolder = @"C:\Users\Evgeniy-boss\source\repos\SokurenkoED\DBSelection\bin\Debug\net5.0\06-09_07_2018";

        /// <summary>Путь до папки с БД</summary>
        public string PathToFolder
        {
            get => _PathToFolder;
            set {
                Set(ref _PathToFolder, value);
            }
        }

        #endregion

        #region Заголовок окна

        private string _Title = "Работа с базой данных";

        /// <summary>Заголовок окна</summary>
        public string Title
        {
            get => _Title;
            set => Set(ref _Title, value);
        }

        #endregion

        #region Команды

        #region ColseApplicationCommand

        public ICommand ColseApplicationCommand { get;}
        private bool CanColseApplicationCommandExecute(object p) => true;
        private void OnColseApplicationCommandExecuted(object p)
        {
            Application.Current.Shutdown();
        }

        #endregion

        #region GetDataCommand

        public ICommand GetDataCommand { get; }
        private bool CanGetDataCommandExecute(object p) => true;
        private void OnGetDataCommandExecuted(object p)
        {
            _InfoData = new InfoData {SensorName = _SensorName, PathToFolder = _PathToFolder, TimeTo = _TimeTo, TimeFrom = _TimeFrom, PathToListFile = _PathToListFile, PathToDataFile = _PathToDataFile };
            _fileIOservice.SaveData(_InfoData);

            
            Task task = Task.Factory.StartNew(() => GetData.GetDataMethod(_InfoData, ref _TextInformation));
            
        }

        #endregion

        #region OpenFileDialog (находим путь до папки с базой данных)

        public ICommand OpenFileDialogCommand { get; }
        private bool CanOpenFileDialogCommandExecute(object p) => true;
        private void OnOpenFileDialogCommandExecuted(object p)
        {
            CommonOpenFileDialog dialog = new CommonOpenFileDialog();
            //dialog.InitialDirectory = "C:\\Users";
            dialog.IsFolderPicker = true;
            if (dialog.ShowDialog() == CommonFileDialogResult.Ok)
            {
                PathToFolder =  dialog.FileName;
            }
        }

        #endregion

        #region OpenFileDialogForDataFileCommand (находим путь до файла data с данными о холодном реакторе)

        public ICommand OpenFileDialogForDataFileCommand { get; }
        private bool CanOpenFileDialogForDataFileCommandExecute(object p) => true;
        private void OnOpenFileDialogForDataFileCommandExecuted(object p)
        {
            CommonOpenFileDialog dialog = new CommonOpenFileDialog();
            if (dialog.ShowDialog() == CommonFileDialogResult.Ok)
            {
                PathToDataFile = dialog.FileName;
            }
        }

        #endregion

        #region OpenFileDialogForExitFileCommand (находим путь до выходного файла)

        public ICommand OpenFileDialogForExitFileCommand { get; }
        private bool CanOpenFileDialogForExitFileCommandExecute(object p) => true;
        private void OnOpenFileDialogForExitFileCommandExecuted(object p)
        {
            CommonOpenFileDialog dialog = new CommonOpenFileDialog();
            if (dialog.ShowDialog() == CommonFileDialogResult.Ok)
            {
                PathToListFile = dialog.FileName;
            }
        }

        #endregion

        #region OpenExitFile

        public ICommand OpenExitFile { get; }
        private bool CanOpenExitFileExecute(object p) => true;
        private void OnOpenExitFileExecuted(object p)
        {
            try
            {
                using (StreamReader sr = new StreamReader(PathToListFile, Encoding.Default))
                {
                    DataToTextBox = sr.ReadToEnd();
                }
                _InfoData = new InfoData { SensorName = _SensorName, PathToFolder = _PathToFolder, TimeTo = _TimeTo, TimeFrom = _TimeFrom, PathToListFile = _PathToListFile, PathToDataFile = _PathToDataFile };
                _fileIOservice.SaveData(_InfoData);
            }
            catch (ArgumentNullException)
            {
                MessageBox.Show($"Ошибка! Файл не был найден!");
            }
        }

        #endregion

        #region SaveExitFile

        public ICommand SaveExitFile { get; }
        private bool CanSaveExitFileExecute(object p) => true;
        private void OnSaveExitFileExecuted(object p)
        {
            try
            {
                using (StreamWriter sw = new StreamWriter(PathToListFile, false, System.Text.Encoding.Default))
                {
                    sw.WriteLine(DataToTextBox);
                    MessageBox.Show("Файл сохранен!");
                }
                _InfoData = new InfoData { SensorName = _SensorName, PathToFolder = _PathToFolder, TimeTo = _TimeTo, TimeFrom = _TimeFrom, PathToListFile = _PathToListFile, PathToDataFile = _PathToDataFile };
                _fileIOservice.SaveData(_InfoData);
            }
            catch (ArgumentNullException)
            {
                MessageBox.Show($"Ошибка! Файл не был найден!");
            }
        }

        #endregion

        #endregion

        public MainWindowViewModel()
        {

            #region Сохранение и загрузка данных 

            _fileIOservice = new FileIOService(Path);

            _InfoData = _fileIOservice.LoadData();

            if (_InfoData.PathToFolder != null)
            {
                _PathToFolder = _InfoData.PathToFolder;
            }
            if (_InfoData.PathToFolder != null)
            {
                _SensorName = _InfoData.SensorName;
            }
            if (_InfoData.PathToFolder != null)
            {
                _TimeFrom = _InfoData.TimeFrom;
            }
            if (_InfoData.PathToFolder != null)
            {
                _TimeTo = _InfoData.TimeTo;
            }
            if (_InfoData.PathToDataFile != null)
            {
                _PathToDataFile = _InfoData.PathToDataFile;
            }
            if (_InfoData.PathToListFile != null)
            {
                _PathToListFile = _InfoData.PathToListFile;
            }


            #endregion

            #region Команды

            ColseApplicationCommand = new LambdaCommand(OnColseApplicationCommandExecuted, CanColseApplicationCommandExecute);

            GetDataCommand = new LambdaCommand(OnGetDataCommandExecuted, CanGetDataCommandExecute);

            OpenFileDialogCommand = new LambdaCommand(OnOpenFileDialogCommandExecuted, CanOpenFileDialogCommandExecute);

            OpenExitFile = new LambdaCommand(OnOpenExitFileExecuted, CanOpenExitFileExecute);

            SaveExitFile = new LambdaCommand(OnSaveExitFileExecuted, CanSaveExitFileExecute);

            OpenFileDialogForDataFileCommand = new LambdaCommand(OnOpenFileDialogForDataFileCommandExecuted, CanOpenFileDialogForDataFileCommandExecute);

            OpenFileDialogForExitFileCommand = new LambdaCommand(OnOpenFileDialogForExitFileCommandExecuted, CanOpenFileDialogForExitFileCommandExecute);

            #endregion

            #region Мусор



            #endregion
        }
    }
}
