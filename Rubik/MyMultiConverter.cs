using System;
using System.Globalization;
using System.Linq;
using System.Windows.Data;
using System.Windows.Markup;

namespace Rubik
{
    public class MyMultiConverter : MarkupExtension, IMultiValueConverter
    {
        private static Lazy<IMultiValueConverter> Converter = new Lazy<IMultiValueConverter>(() => new MyMultiConverter());
        
        public override object ProvideValue(IServiceProvider serviceProvider)
        {
           return Converter.Value;
        }

        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            return string.Join(",",values);
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            return ((string)value).Split(',');
        }
    }
}