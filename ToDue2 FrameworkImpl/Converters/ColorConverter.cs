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
			var diff = (DateTime)(value ?? DateTime.Now) - DateTime.Now;
			if (diff >= TimeSpan.FromDays(7))
			{
				return Application.Current?.MainWindow?.Resources["OK"];
			}
			else if (diff >= TimeSpan.FromDays(1))
			{
				return Application.Current?.MainWindow?.Resources["Warning"];
			}
			else if (diff >= TimeSpan.FromDays(-7))
			{
				return Application.Current?.MainWindow?.Resources["Alert"];
			}
			else
			{
				return Application.Current?.MainWindow?.Resources["PressedBackground"];
			}
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}
}
