﻿using DBSelectionForm.Infastructure.Commands;
using DBSelectionForm.Models;
using DBSelectionForm.Services;
using DBSelectionForm.ViewModels.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace DBSelectionForm.ViewModels
{
    internal class MainWindowViewModel : ViewModel
    {
        private readonly string Path = $"{Environment.CurrentDirectory}\\IndoData.json";
        private FileIOService _fileIOservice;
        private InfoData _InfoData;

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

        private string _Title = "Выборка из базы данных";

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
            _InfoData = new InfoData {SensorName = _SensorName, PathToFolder = _PathToFolder, TimeTo = _TimeTo, TimeFrom = _TimeFrom };
            GetData.GetDataMethod(_InfoData);
            _fileIOservice.SaveData(_InfoData);
        }

        #endregion

        #endregion

        public MainWindowViewModel()
        {

            _fileIOservice = new FileIOService(Path);

            try
            {
                _InfoData = _fileIOservice.LoadData();
                if (_InfoData.PathToFolder != null)
                {
                    _PathToFolder = _InfoData.PathToFolder;
                }
                _SensorName = _InfoData.SensorName;
                _TimeFrom = _InfoData.TimeFrom;
                _TimeTo = _InfoData.TimeTo;


            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }


            #region Команды

            ColseApplicationCommand = new LambdaCommand(OnColseApplicationCommandExecuted, CanColseApplicationCommandExecute);

            GetDataCommand = new LambdaCommand(OnGetDataCommandExecuted, CanGetDataCommandExecute);

            #endregion
        }
    }
}
