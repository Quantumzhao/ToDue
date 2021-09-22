using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Windows.Data;
using System.Windows.Controls;
using System.Windows;

namespace ToDue2.Converters
{
	class ColorConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			var diff = (DateTime)(value ?? DateTime.MinValue) - DateTime.Now;
			if (diff >= TimeSpan.FromDays(7))
			{
				return Application.Current?.Resources["OK"];
			}
			else if (diff >= TimeSpan.FromDays(1))
			{
				return Application.Current?.Resources["Warning"];
			}
			else if (diff >= TimeSpan.FromDays(-7))
			{
				return Application.Current?.Resources["Alert"];
			}
			else
			{
				return Application.Current?.Resources["PressedBackground"];
			}
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}
}
