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
            double TryParseNewValue;
            double TryParseOldValue;
            var PinkBrush = new SolidColorBrush(Colors.Pink);
            var WhiteBrush = new SolidColorBrush(Colors.White);
            if (SM == null)
            {
                return WhiteBrush;
            }
            if (!double.TryParse(SM.NewValue.ToString(), out TryParseNewValue))
            {
                return PinkBrush;
            }
            if (!double.TryParse(SM.OldValue.ToString(), out TryParseOldValue))
            {
                return PinkBrush;
            }
            if (TryParseNewValue != TryParseOldValue)
            {
                return PinkBrush;
            }

            return WhiteBrush;
        }


        public object ConvertBack(object value, Type targetTypes, object parameter, CultureInfo culture)
        {
            throw new Exception("The method or operation is not implemented.");
        }

    }
}
