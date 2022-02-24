using DBSelectionForm.Models;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Windows.Data;
using System.Windows.Markup;
using System.Windows.Media;

namespace DBSelectionForm.Converters
{
    class ValuesToColorConverters : IValueConverter
    {

        public object Convert(object values, Type targetType, object parameter, CultureInfo culture)
        {
            var SM = values as SignalModel;
            if (SM == null)
            {
                return new SolidColorBrush(Colors.White);
            }
            if (SM.NewValue == null)
            {
                return new SolidColorBrush(Colors.White);
            }
            if (double.Parse(SM.NewValue.ToString(), culture) != double.Parse(SM.OldValue.ToString(), culture))
            {
                return new SolidColorBrush(Colors.Pink);
            }

            return new SolidColorBrush(Colors.White);
        }


        public object ConvertBack(object value, Type targetTypes, object parameter, CultureInfo culture)
        {
            throw new Exception("The method or operation is not implemented.");
        }

    }
}
