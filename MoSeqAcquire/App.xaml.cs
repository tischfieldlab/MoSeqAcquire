using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows;
using MoSeqAcquire.Properties;
using MoSeqAcquire.Views;
using MoSeqAcquire.Views.Controls;
using Serilog;

namespace MoSeqAcquire
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected WaitingWindowManager loading;
        public ILogger Log;

        public App()
        {
            this.Log = Serilog.Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Debug()
                .WriteTo.Console()
                .WriteTo.File("logs\\app.log", rollingInterval: RollingInterval.Day)
                .CreateLogger();

            this.Log.Information("Starting up application");

            this.Startup += this.StartupHandler;
            this.loading = new WaitingWindowManager(typeof(Splash));
            this.loading.BeginWaiting();

            
            foreach (var p in Settings.Default.PluginPaths)
            {
                this.Log.Information("Registering pluign path {plugin_path}", p);
                RegisterProbePath(p);
            }

            AppDomain.CurrentDomain.AssemblyResolve += LoadResolveEventHandler;

            //Prevent the computer from entering sleep
            Models.Utility.PowerManagement.StartPreventSleep();
        }
        public static void SetCurrentStatus(String StatusText)
        {
            try
            {
                (App.Current as App).loading.SetCurrentStatus(StatusText);
            }
            catch { }
        }

        private void StartupHandler(object sender, StartupEventArgs e)
        {
            try
            {
                this.MainWindow = new MainWindow()
                {
                    WindowState = WindowState.Maximized,
                    ShowInTaskbar = true,
                    ShowActivated = true
                };
                this.MainWindow.Loaded += (s, evt) => { this.loading.EndWaiting(); };
                this.MainWindow.Show();
            }
            catch (Exception ex)
            {
                this.Log.Error(ex, "Encountered exception at highest level");
                var m = "";// ex.Message;
                var exc = ex;
                while (exc != null)
                {
                    m += "\n\n" + exc.Message;
                    exc = exc.InnerException;
                }
                
                MessageBox.Show(m);
                MessageBox.Show(ex.StackTrace);
            }
            //this.MainWindow.Show();
        }

        private static List<String> __privateProbPaths = new List<String>();
        protected static void RegisterProbePath(String ProbePath)
        {
            __privateProbPaths.Add(ProbePath);
            if (Directory.Exists(ProbePath))
            {
                foreach(var dir in Directory.EnumerateDirectories(ProbePath))
                {
                    RegisterProbePath(dir);
                }
            }
        }
        static Assembly LoadResolveEventHandler(object sender, ResolveEventArgs args)
        {
            string folderPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            if (!args.Name.StartsWith("http:", StringComparison.InvariantCultureIgnoreCase))
            {
                foreach (var probe in __privateProbPaths)
                {
                    var idx = args.Name.IndexOf(",") != -1 ? args.Name.IndexOf(",") : args.Name.Length;
                    string assemblyPath = Path.Combine(folderPath, probe, args.Name.Substring(0, idx) + ".dll");
                    if (File.Exists(assemblyPath))
                    {
                        Assembly assembly = Assembly.LoadFrom(assemblyPath);
                        return assembly;
                    }
                }
            }
            return null;
        }
    }
}
