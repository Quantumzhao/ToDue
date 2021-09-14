using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;
using ToDue2.Properties;

namespace ToDue2
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window, INotifyPropertyChanged
	{
		public ObservableCollection<TodoItem> PinnedItems { get; private set; }
		public ObservableSortedList TodoItems { get; private set; }
		private DispatcherTimer _Timer;

		private DateTime _DisplayedDueDate = DateTime.Now;
		public DateTime DisplayedDueDate
		{
			get => _DisplayedDueDate;
			set
			{
				_DisplayedDueDate = value;
				PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(DisplayedDueDate)));
			}
		}

		public MainWindow()
		{
			SetTheme(Settings.Default.IsLight);

			InitializeComponent();
			_Timer = new DispatcherTimer(DispatcherPriority.Background);
			_Timer.Interval = TimeSpan.FromHours(6);
			_Timer.Tick += (s, e) => Refresh();
			_Timer.Start();
		}

		public event PropertyChangedEventHandler PropertyChanged;

		public void SaveTodoList()
		{
			var arr = TodoItems.Select(o => (TodoStruct)o).ToArray();
			using (MemoryStream ms = new MemoryStream())
			{
				BinaryFormatter bf = new BinaryFormatter();
				bf.Serialize(ms, arr);
				ms.Position = 0;
				byte[] buffer = new byte[(int)ms.Length];
				ms.Read(buffer, 0, buffer.Length);
				Settings.Default.TodoItems = Convert.ToBase64String(buffer);
				Settings.Default.Save();
			}
		}

		public void SavePinnedList()
		{
			var arr = PinnedItems.Select(o => (TodoStruct)o).ToArray();
			using (MemoryStream ms = new MemoryStream())
			{
				BinaryFormatter bf = new BinaryFormatter();
				bf.Serialize(ms, arr);
				ms.Position = 0;
				byte[] buffer = new byte[(int)ms.Length];
				ms.Read(buffer, 0, buffer.Length);
				Settings.Default.PinnedItems = Convert.ToBase64String(buffer);
				Settings.Default.Save();
			}
		}

		private void Window_Loaded(object sender, RoutedEventArgs e)
		{
			WindowInteropHelper wndHelper = new WindowInteropHelper(this);
			int exStyle = (int)Win32.GetWindowLong(wndHelper.Handle, (int)Win32.GetWindowLongFields.GWL_EXSTYLE);
			exStyle |= (int)Win32.ExtendedWindowStyles.WS_EX_TOOLWINDOW;
			Win32.SetWindowLong(wndHelper.Handle, (int)Win32.GetWindowLongFields.GWL_EXSTYLE, (IntPtr)exStyle);

			(App.Current as App).TryAddToStartupLocation();

			Dark.IsChecked = !Settings.Default.IsLight;

			if (Settings.Default.ShowBackground)
			{
				ShowBackground.IsChecked = true;
				MainPanel.Background = this.Resources["Background"] as SolidColorBrush;
			}
			else
			{
				ShowBackground.IsChecked = false;
			}

			SetScaleAndMargin(Settings.Default.Scale);

			var settings = Settings.Default;

			this.Left = settings.StartupLocationX;
			this.Top = settings.StartupLocationY;
			this.Opacity = settings.Opacity;

			this.LocationChanged += (s, e1) => SaveWindowLocation(null, null);

#if true
			if (settings.TodoItems == string.Empty)
			{
				TodoItems = new ObservableSortedList();
			}
			else using (MemoryStream ms = new MemoryStream(Convert.FromBase64String(Settings.Default.TodoItems)))
			{
				BinaryFormatter bf = new BinaryFormatter();
				TodoItems = new ObservableSortedList((bf.Deserialize(ms) as TodoStruct[]).Select(s => (TodoItem)s));
			}

			if (settings.PinnedItems == string.Empty)
			{
				PinnedItems = new ObservableCollection<TodoItem>();
			}
			else using (MemoryStream ms = new MemoryStream(Convert.FromBase64String(Settings.Default.PinnedItems)))
			{
				BinaryFormatter bf = new BinaryFormatter();
				PinnedItems = new ObservableCollection<TodoItem>((bf.Deserialize(ms) as TodoStruct[]).Select(s => (TodoItem)s));
			}

#else
			TodoItems = new ObservableSortedList();
			TodoItems.Add(new TodoItem(DateTime.Now, "Test", true));
#endif
			TodoItems.ForEach(todo => todo.PropertyChanged += (s, e4) => SaveTodoList());
			foreach (var pinned in PinnedItems) pinned.PropertyChanged += (s, e3) => SavePinnedList();
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(TodoItems)));
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(PinnedItems)));
		}

		#region MenuItem Actions
		private void Exit_Click(object sender, RoutedEventArgs e)
		{
			SaveWindowLocation(null, null);
			this.Close();
		}

		private void Transparency_Click(object sender, RoutedEventArgs e)
		{
			this.Opacity = double.Parse((sender as Control).Tag as string);
			Settings.Default.Opacity = this.Opacity;
			Settings.Default.Save();
		}

		private void Refresh_Click(object sender, RoutedEventArgs e)
		{
			Refresh();
		}

		private void Theme_Checked(object sender, RoutedEventArgs e)
		{
			Settings.Default.IsLight = false;
			Settings.Default.Save();
		}

		private void Theme_UnChecked(object sender, RoutedEventArgs e)
		{
			Settings.Default.IsLight = true;
			Settings.Default.Save();
		}

		private void Reset_Click(object sender, RoutedEventArgs e)
		{
			this.Top = 0;
			this.Left = 0;
			Settings.Default.StartupLocationX = 0;
			Settings.Default.StartupLocationY = 0;
			Settings.Default.Save();
		}

		private void Scale_Click(object sender, RoutedEventArgs e)
		{
			var scale = double.Parse((sender as Control).Tag as string);
			SetScaleAndMargin(scale);
		}
		#endregion

		private void InputBox_Enter(object sender, KeyEventArgs e)
		{
			if (e.Key != Key.Enter || InputBox.Text == string.Empty) return;

			if (!(Toggle.IsChecked ?? false))
			{
				var todo = new TodoItem(DueDate.SelectedDate ?? DateTime.Now, InputBox.Text, Priority.IsChecked ?? false);
				todo.PropertyChanged += (s, e2) => SaveTodoList();
				TodoItems.Add(todo);
				SaveTodoList();
			}
			else
			{
				var pinned = new TodoItem(DateTime.MinValue, InputBox.Text, Priority.IsChecked ?? false);
				pinned.PropertyChanged += (s, e1) => SavePinnedList();
				PinnedItems.Add(pinned);
				SavePinnedList();
			}

			InputBox.Text = string.Empty;
			DueDate.SelectedDate = DateTime.Now;
		}

		#region Try to stay on desktop
		public Task CancelShowDesktop(Window window)
		{
			return Task.Factory.StartNew(() =>
			{
				this.Dispatcher.Invoke(() =>
				{
					Thread.Sleep(100);
					window.WindowState = WindowState.Normal;
					window.Topmost = true;
					window.Topmost = false;
					Win32.SetBottom(this);
				});
			});
		}

		private async void Window_StateChanged(object sender, EventArgs e)
		{
			//await CancelShowDesktop(this);
		}

		#endregion

		public void RemoveItem(object sender, RoutedEventArgs e)
		{
			TodoItems.Remove((sender as Control).DataContext as TodoItem);
			SaveTodoList();
		}

		private void Refresh()
		{
			//PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(TodoItems)));
			TodoItems.Refresh();
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(DisplayedDueDate)));
		}

		private void PinIcon_Click(object sender, RoutedEventArgs e)
		{
			PinnedItems.Remove((sender as Control).DataContext as TodoItem);
			SavePinnedList();
		}

		private void SetScaleAndMargin(double scale)
		{
			MainPanel.RenderTransform = new ScaleTransform(scale, scale);
			if (scale == 0.75)
			{
				MainPanel.Margin = new Thickness(-50, -80, 0, 0);
				Settings.Default.AdjustedMarginX = -50;
				Settings.Default.AdjustedMarginY = -80;
			}
			else if (scale == 1)
			{
				MainPanel.Margin = new Thickness();
				Settings.Default.AdjustedMarginX = 0;
				Settings.Default.AdjustedMarginY = 0;
			}
			else // scale == 0.5
			{
				MainPanel.Margin = new Thickness(-100, -160, 0, 0);
				Settings.Default.AdjustedMarginX = -100;
				Settings.Default.AdjustedMarginY = -160;
			}
			Settings.Default.Scale = scale;
			Settings.Default.Save();
		}

		private void MainPanel_MouseDown(object sender, MouseButtonEventArgs e)
		{
			try
			{
				Window.DragMove();
			}
			catch { }
		}

		public void SaveWindowLocation(object sender, RoutedEventArgs e)
		{
			Settings.Default.StartupLocationX = (int)this.Left;
			Settings.Default.StartupLocationY = (int)this.Top;
			Settings.Default.Save();
		}

		private void MyDatePicker_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
		{
			var date = (sender as MyDatePicker).SelectedDate;
			var todo = (sender as MyDatePicker).DataContext as TodoItem;
			todo.DueDate = date ?? DateTime.Now;
			TodoItems.Remove(todo);
			TodoItems.Add(todo);
			SaveTodoList();
		}

		private void Todo_TextChanged(object sender, TextChangedEventArgs e)
		{
			var text = (sender as TextBox).Text;
			var todoItem = (sender as TextBox).DataContext as TodoItem;
			todoItem.Content = text;
			SaveTodoList();
		}

		private void Pinned_TextChanged(object sender, TextChangedEventArgs e)
		{
			var text = (sender as TextBox).Text;
			var todoItem = (sender as TextBox).DataContext as TodoItem;
			todoItem.Content = text;
			SavePinnedList();
		}

		private void SetTheme(bool isLight)
		{
			if (isLight)
			{
				this.Resources["Foreground"] = App.Current.Resources["LightForeground"];
				this.Resources["Alert"] = App.Current.Resources["LightAlert"];
				this.Resources["Warning"] = App.Current.Resources["LightWarning"];
				this.Resources["OK"] = App.Current.Resources["LightOK"];
				this.Resources["HighlightBackground"] = App.Current.Resources["LightHighlightBackground"];
				this.Resources["PressedBackground"] = App.Current.Resources["LightPressedBackground"];
				this.Resources["Background"] = App.Current.Resources["DarkBackground"];
			}
			else
			{
				this.Resources["Foreground"] = App.Current.Resources["DarkForeground"];
				this.Resources["Alert"] = App.Current.Resources["DarkAlert"];
				this.Resources["Warning"] = App.Current.Resources["DarkWarning"];
				this.Resources["OK"] = App.Current.Resources["DarkOK"];
				this.Resources["HighlightBackground"] = App.Current.Resources["DarkHighlightBackground"];
				this.Resources["PressedBackground"] = App.Current.Resources["DarkPressedBackground"];
				this.Resources["Background"] = App.Current.Resources["LightBackground"];
			}
		}

		private void ShowBackground_Checked(object sender, RoutedEventArgs e)
		{
			MainPanel.Background = this.Resources["Background"] as SolidColorBrush;
			Settings.Default.ShowBackground = true;
			Settings.Default.Save();
		}

		private void ShowBackground_Unchecked(object sender, RoutedEventArgs e)
		{
			MainPanel.Background = this.Resources["TransparentBackground"] as SolidColorBrush;
			Settings.Default.ShowBackground = false;
			Settings.Default.Save();
		}

		private void MainPanel_MouseUp(object sender, MouseButtonEventArgs e)
		{
			SaveWindowLocation(null, null);
		}
	}
}
