using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace ToDue2
{
	/// <summary>
	/// Interaction logic for CustomDatePicker.xaml
	/// </summary>
	public partial class CustomDatePicker : UserControl
	{
		public CustomDatePicker()
		{
			InitializeComponent();

			Display.Click += (s, e) => Container.IsOpen = true;
			Picker.SelectedDatesChanged += (s, e) =>
			{
				SelectedDate = Picker.SelectedDate;
				Container.IsOpen = false;
			};
		}

		public static readonly DependencyProperty SelectedDateProperty = 
			DependencyProperty.Register("SelectedDate", typeof(DateTime?), typeof(CustomDatePicker), 
				new PropertyMetadata(DateTime.Now));

		public DateTime? SelectedDate
		{
			get => (DateTime?)GetValue(SelectedDateProperty);
			set
			{
				if (value < DateTime.Now.Subtract(TimeSpan.FromDays(7)))
				{
					value = null;
				}

				SetValue(SelectedDateProperty, value);

				if (value != null)
				{
					_LastNonNullValue = (DateTime)value;
					IndefiniteToggle.IsChecked = false;
				}
				else
				{
					IndefiniteToggle.IsChecked = true;
				}
			}
		}

		public DateTime _LastNonNullValue = DateTime.Now;

		private void IndefiniteToggle_Checked(object sender, RoutedEventArgs e)
		{
			SelectedDate = null;
			Container.IsOpen = false;
		}

		private void IndefiniteToggle_Unchecked(object sender, RoutedEventArgs e)
		{
			SelectedDate = _LastNonNullValue;
			Container.IsOpen = false;
		}
	}
}
