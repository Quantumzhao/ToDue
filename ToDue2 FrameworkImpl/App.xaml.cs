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

namespace ToDue2_FrameworkImpl
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
            RenderOptions.ProcessRenderMode = RenderMode.SoftwareOnly;
            var appName = Path.GetFileNameWithoutExtension(Process.GetCurrentProcess().MainModule.FileName);
            var programStarted = new EventWaitHandle(false, EventResetMode.AutoReset, appName, out var createNew);
            if (!createNew)
            {
                Process current = Process.GetCurrentProcess();
                foreach (Process process in Process.GetProcessesByName(current.ProcessName))
                {
                    if (process.Id != current.Id)
                    {
                        SetForegroundWindow(process.MainWindowHandle);
                        App.Current.Shutdown();
                        Environment.Exit(-1);
                        break;
                    }
                }
            }
            else
            {
                TryAddToStartupLocation();
            }

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

            var path = $"{startup}\\ToDue2.url";
            if (File.Exists(path))
            {
                Directory.Delete(path);
            }
        }
    }
}
