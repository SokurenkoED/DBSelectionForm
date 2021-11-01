using DBSelectionForm.Infastructure.Commands;
using DBSelectionForm.Models;
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

        #region Время "С"

        private string _TimeFrom = InfoData.TimeFrom;

        /// <summary>Время "С"</summary>
        public string TimeFrom
        {
            get => _TimeFrom;
            set
            {
                Set(ref _TimeFrom, value);
                Set(ref InfoData.TimeFrom, value);
            }
        }

        #endregion

        #region Время "По"

        private string _TimeTo = InfoData.TimeTo;

        /// <summary>Время "По"</summary>
        public string TimeTo
        {
            get => _TimeTo;
            set
            {
                Set(ref _TimeTo, value);
                Set(ref InfoData.TimeTo, value);
            }
        }

        #endregion

        #region Имя датчика

        private string _SensorName = InfoData.SensorName;

        /// <summary>Имя датчика</summary>
        public string SensorName
        {
            get => _SensorName;
            set {
                Set(ref _SensorName, value);
                Set(ref InfoData.SensorName, value);
            }
        }

        #endregion

        #region Путь до папки с БД

        private string _PathToFolder = InfoData.PathToFolder;

        /// <summary>Путь до папки с БД</summary>
        public string PathToFolder
        {
            get => _PathToFolder;
            set {
                Set(ref _PathToFolder, value);
                Set(ref InfoData.PathToFolder, value);
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

        #endregion

        public MainWindowViewModel()
        {
            #region Команды

            ColseApplicationCommand = new LambdaCommand(OnColseApplicationCommandExecuted, CanColseApplicationCommandExecute);

            #endregion
        }
    }
}
