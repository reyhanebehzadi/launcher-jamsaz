using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Windows;
using Jamsaz.Launcher.Classes;

namespace Jamsaz.Launcher
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {

        protected override void OnStartup(StartupEventArgs e)
        {

            //System.Diagnostics.Process[] processes = System.Diagnostics.Process.GetProcesses();

            //if (processes.Any(c => c.ProcessName.Contains("Jamsaz.Launcher")))
            //    Application.Current.Shutdown();

            base.OnStartup(e);

            var window = new MainWindow();

            MainWindow = window;

            //ShutdownMode = ShutdownMode.OnMainWindowClose;

            MainWindow.Show();
        }

        protected override void OnExit(ExitEventArgs e)
        {
            base.OnExit(e);

            if (AppHost.serviceHost != null)
                AppHost.serviceHost.Close();
        }
    }
}
