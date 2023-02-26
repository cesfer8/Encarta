using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using Xamarin.Forms;

namespace Encarta.Converters
{
    public class IsQrToIconConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            bool isQr = (bool)value;
            //<Label Text="&#xE804;" HorizontalOptions="CenterAndExpand" VerticalOptions="CenterAndExpand" FontSize="50" FontFamily="fontello"/>
            if (isQr)
            {
                return "&#xE806;";
            }
            return "&#xE807;";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
