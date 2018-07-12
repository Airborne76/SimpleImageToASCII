using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Data;

namespace SimpleImageToASCII.Convert
{
    public class BoolNullToBool: IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if ((bool?)value==true)
            {
                return false;
            }
            else if ((bool?)value == false)
            {
                return true;
            }
            else
            {
                return true;
            }
        }
        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
