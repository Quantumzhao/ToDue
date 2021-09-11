using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading;
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
using ToDue2.Properties;

namespace ToDue2
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window, INotifyPropertyChanged
	{
		public ObservableCollection<TodoItem> TodoItems { get; private set; }

		public string DateOfNow
		{
			get
			{
				var now = DateTime.Now;
				return $"{now.Month}/{now.Day}";
			}
		}

		public MainWindow()
		{
			InitializeComponent();

		}

		public event PropertyChangedEventHandler PropertyChanged;

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

		public void SaveTodoList()
		{
			using (MemoryStream ms = new MemoryStream())
			{
				BinaryFormatter bf = new BinaryFormatter();
				bf.Serialize(ms, TodoItems);
				ms.Position = 0;
				byte[] buffer = new byte[(int)ms.Length];
				ms.Read(buffer, 0, buffer.Length);
				Settings.Default.TodoItems = Convert.ToBase64String(buffer);
				Settings.Default.Save();
			}
		}

		private void Window_Loaded(object sender, RoutedEventArgs e)
		{
			var settings = Settings.Default;

			//Location = settings.StartupLocation;
			//Opacity = settings.Opacity;

			if (settings.TodoItems == string.Empty)
			{
				TodoItems = new ObservableCollection<TodoItem>();
				TodoItems.Add(new TodoItem(DateTime.Now, "Test"));
				PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(TodoItems)));
			}
			else using (MemoryStream ms = new MemoryStream(Convert.FromBase64String(Settings.Default.TodoItems)))
			{
				BinaryFormatter bf = new BinaryFormatter();
				TodoItems = bf.Deserialize(ms) as ObservableCollection<TodoItem>;
			}
		}

		public void RemoveItem(TodoItem item)
		{
			TodoItems.Remove(item);
			SaveTodoList();
		}
	}
}
