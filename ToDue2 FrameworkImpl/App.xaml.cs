using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media;

namespace ToDue2
{
	/// <summary>
	/// Interaction logic for App.xaml
	/// </summary>
	public partial class App : Application
	{
		[DllImport("user32.dll")]
		[return: MarshalAs(UnmanagedType.Bool)]
		public static extern bool SetForegroundWindow(IntPtr hWnd);

		protected override void OnStartup(StartupEventArgs e)
		{
			if (Process.GetProcessesByName(Process.GetCurrentProcess().ProcessName).Length > 1)
			{
				Environment.Exit(-2);
				return;
			}

			TryAddToStartupLocation();

			base.OnStartup(e);
		}

		public void TryAddToStartupLocation()
		{
			string startup = Environment.GetFolderPath(Environment.SpecialFolder.Startup);
			if (!Directory.Exists(startup)) return;

			var path = $"{startup}\\ToDue2.url";
			if (!File.Exists(path))
			{
				using (StreamWriter writer = new StreamWriter(path))
				{
					string app = Assembly.GetExecutingAssembly().Location;
					writer.WriteLine("[InternetShortcut]");
					writer.WriteLine("URL=file:///" + app);
					writer.WriteLine("IconIndex=0");
					string icon = app.Replace('\\', '/');
					writer.WriteLine("IconFile=" + icon);
				}
			}
		}

		public void TryRemoveFromStartupLocation()
		{
			string startup = Environment.GetFolderPath(Environment.SpecialFolder.Startup);
			if (!Directory.Exists(startup)) return;

			var path = $"{startup}\\ToDue2";
			if (File.Exists(path))
			{
				File.Delete(path);
			}
		}

		public static bool IsValidDate(DateTime? date)
		{
			return date != null && date >= DateTime.Now.Subtract(TimeSpan.FromDays(7));
		}
	}
}
