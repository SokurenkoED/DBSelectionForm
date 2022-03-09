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
            IFormatProvider formatter = new NumberFormatInfo { NumberDecimalSeparator = "." };

            var SM = values as SignalModel;
            double TryParseNewValue;
            double TryParseOldValue;
            var BlueBrush = new SolidColorBrush(Colors.Aqua);
            var WhiteBrush = new SolidColorBrush(Colors.White);
            var PinkBrush = new SolidColorBrush(Colors.Pink);
            var RedBrush = new SolidColorBrush(Colors.Red);
            if (SM == null)
            {
                return WhiteBrush;
            }
            if (SM.Status == null)
            {
                return RedBrush;
            }
            if (SM.Status != "дост" && SM.Status != "повт.дост")
            {
                return PinkBrush;
            }
            if (!double.TryParse(SM.NewValue.ToString(), NumberStyles.Any, formatter, out TryParseNewValue))
            {
                return BlueBrush;
            }
            if (!double.TryParse(SM.OldValue.ToString(), NumberStyles.Any, formatter, out TryParseOldValue))
            {
                return BlueBrush;
            }
            if (TryParseNewValue != TryParseOldValue)
            {
                return BlueBrush;
            }

            return WhiteBrush;
        }


        public object ConvertBack(object value, Type targetTypes, object parameter, CultureInfo culture)
        {
            throw new Exception("The method or operation is not implemented.");
        }  

    }
}
