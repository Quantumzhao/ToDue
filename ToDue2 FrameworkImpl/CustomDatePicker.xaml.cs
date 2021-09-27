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
				IndefiniteToggle.IsChecked = !App.IsValidDate(Picker.SelectedDate);
				if (SelectedDate != DateTime.MinValue)
				{
					_LastNonNullValue = (DateTime)SelectedDate;
					IndefiniteToggle.IsChecked = false;
				}
				else
				{
					IndefiniteToggle.IsChecked = true;
				}
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
				if (value == SelectedDate) return;

				if (!App.IsValidDate(value))
				{
					value = DateTime.MinValue;
				}

				SetValue(SelectedDateProperty, value);
				RaiseEvent(new UselessRoutedEventArgs(SelectedDateChangedEvent, this));

				if (value != DateTime.MinValue)
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

		public static readonly RoutedEvent SelectedDateChangedEvent = EventManager.RegisterRoutedEvent(
		"SelectedDateChanged", RoutingStrategy.Bubble, typeof(EventHandler<UselessRoutedEventArgs>), typeof(CustomDatePicker));

		// Provide CLR accessors for the event
		public event RoutedEventHandler SelectedDateChanged
		{
			add { AddHandler(SelectedDateChangedEvent, value); }
			remove { RemoveHandler(SelectedDateChangedEvent, value); }
		}

		public DateTime _LastNonNullValue = DateTime.Now;

		private void IndefiniteToggle_Checked(object sender, RoutedEventArgs e)
		{
			if (SelectedDate != DateTime.MinValue)
			{
				_LastNonNullValue = (DateTime)SelectedDate;
				SelectedDate = DateTime.MinValue;
			}

			Container.IsOpen = false;
		}

		private void IndefiniteToggle_Unchecked(object sender, RoutedEventArgs e)
		{
			if (!App.IsValidDate(SelectedDate)) SelectedDate = _LastNonNullValue;
			Container.IsOpen = false;
		}

		private void Control_Loaded(object sender, RoutedEventArgs e)
		{
			//if (!App.IsValidDate(SelectedDate)) IndefiniteToggle.IsChecked = true;
		}
	}

	public class UselessRoutedEventArgs : RoutedEventArgs
	{
		public UselessRoutedEventArgs(RoutedEvent routedEvent, object source) : base(routedEvent, source) { }
	}
}
