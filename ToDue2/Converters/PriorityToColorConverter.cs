using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Windows.Data;
using System.Windows.Controls;
using System.Windows;

namespace ToDue2.Converters
{
	class PriorityToColorConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			if ((bool)(value ?? false))
			{
				return Application.Current.Resources["Alert"];
			}
			else
			{
				return Application.Current.Resources["LightForeground"];
			}
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}
}
