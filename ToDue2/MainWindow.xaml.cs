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
			InitializeComponent();
			_Timer = new DispatcherTimer(DispatcherPriority.Background);
			_Timer.Interval = TimeSpan.FromDays(1);
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
			if (Settings.Default.HideFromTaskManager)
			{
				WindowInteropHelper wndHelper = new WindowInteropHelper(this);
				int exStyle = (int)Win32.GetWindowLong(wndHelper.Handle, (int)Win32.GetWindowLongFields.GWL_EXSTYLE);
				exStyle |= (int)Win32.ExtendedWindowStyles.WS_EX_TOOLWINDOW;
				Win32.SetWindowLong(wndHelper.Handle, (int)Win32.GetWindowLongFields.GWL_EXSTYLE, (IntPtr)exStyle);
			}

			ShowInTaskBar.IsChecked = Settings.Default.ShowInTaskBar;
			HideFromTaskManager.IsChecked = Settings.Default.HideFromTaskManager;
			AutoStart.IsChecked = Settings.Default.AutoStart;

			var settings = Settings.Default;

			//Location = settings.StartupLocation;
			//Opacity = settings.Opacity;

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
#else
			TodoItems = new ObservableSortedList();
			TodoItems.Add(new TodoItem(DateTime.Now, "Test", true));
#endif
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(TodoItems)));
		}

		#region MenuItem Actions
		private void Exit_Click(object sender, RoutedEventArgs e)
		{
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
			
		}

		private void Theme_UnChecked(object sender, RoutedEventArgs e)
		{

		}

		private void Reset_Click(object sender, RoutedEventArgs e)
		{
			this.Top = 0;
			this.Left = 0;
			Settings.Default.StartupLocation = new System.Drawing.Point(0, 0);
			Settings.Default.Save();
		}

		private void AutoStart_Checked(object sender, RoutedEventArgs e)
		{
			(App.Current as App).TryAddToStartupLocation();
			Settings.Default.AutoStart = true;
			Settings.Default.Save();
		}

		private void AutoStart_Unchecked(object sender, RoutedEventArgs e)
		{
			(App.Current as App).TryRemoveFromStartupLocation();
			Settings.Default.AutoStart = false;
			Settings.Default.Save();
		}

		private void Hide_Checked(object sender, RoutedEventArgs e)
		{
			Settings.Default.HideFromTaskManager = true;
			Settings.Default.Save();
		}

		private void Hide_Unchecked(object sender, RoutedEventArgs e)
		{
			Settings.Default.HideFromTaskManager = false;
			Settings.Default.Save();
		}

		private void ShowInTaskBar_Checked(object sender, RoutedEventArgs e)
		{
			this.ShowInTaskbar = true;
			Settings.Default.ShowInTaskBar = true;
			Settings.Default.Save();
		}

		private void ShowInTaskBar_Unchecked(object sender, RoutedEventArgs e)
		{
			this.ShowInTaskbar = false;
			Settings.Default.ShowInTaskBar = false;
			Settings.Default.Save();
		}
		#endregion

		#region Add button and related stuff
		private void InputBox_Enter(object sender, KeyEventArgs e)
		{
			if (e.Key != Key.Enter || InputBox.Text == string.Empty) return;

			if (!(Toggle.IsChecked ?? false))
			{
				TodoItems.Add(new TodoItem(DueDate.SelectedDate ?? DateTime.Now, InputBox.Text, Priority.IsChecked ?? false));
				SaveTodoList();
			}
			else
			{
				PinnedItems.Add(new TodoItem(DateTime.MinValue, InputBox.Text, Priority.IsChecked ?? false));
			}

			InputBox.Text = string.Empty;
		}

		#endregion

		#region Try to stay on desktop and bottom
		[DllImport("user32.dll")]
		static extern bool SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter, int X,
			int Y, int cx, int cy, uint uFlags);

		const uint SWP_NOSIZE = 0x0001;
		const uint SWP_NOMOVE = 0x0002;
		const uint SWP_NOACTIVATE = 0x0010;

		static readonly IntPtr HWND_BOTTOM = new IntPtr(1);
		public static void SetBottom(Window window)
		{
			IntPtr hWnd = new WindowInteropHelper(window).Handle;
			SetWindowPos(hWnd, HWND_BOTTOM, 0, 0, 0, 0, SWP_NOSIZE | SWP_NOMOVE | SWP_NOACTIVATE);
		}

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
				});
			});
		}

		private async void Window_StateChanged(object sender, EventArgs e)
		{
			await CancelShowDesktop(this);
		}

		#endregion

		public void RemoveItem(object sender, RoutedEventArgs e)
		{
			TodoItems.Remove((sender as Control).DataContext as TodoItem);
			SaveTodoList();
		}

		private void Refresh()
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(TodoItems)));
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(DisplayedDueDate)));
		}
	}
}
