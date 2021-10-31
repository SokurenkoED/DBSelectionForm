using DBSelectionForm.ViewModels.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DBSelectionForm.ViewModels
{
    internal class MainWindowViewModel : ViewModel
    {

        #region Заголовок окна

        private string _Title = "Выборка из базы данных";

        /// <summary>Заголовок окна</summary>
        public string Title
        {
            get => _Title;
            set => Set(ref _Title, value);
        }

        #endregion


    }
}
