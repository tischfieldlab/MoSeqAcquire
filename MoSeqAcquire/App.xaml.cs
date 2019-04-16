using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows;
using MoSeqAcquire.Properties;
using MoSeqAcquire.Views;
using MoSeqAcquire.Views.Controls;

namespace MoSeqAcquire
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected WaitingWindowManager loading;

        public App()
        {
            this.Startup += this.StartupHandler;
            this.loading = new WaitingWindowManager(typeof(Splash));
            this.loading.BeginWaiting();


            Settings.Default.PluginPaths.Cast<string>().ForEach((p) => RegisterProbePath(p));

            AppDomain.CurrentDomain.AssemblyResolve += LoadResolveEventHandler;

            //Prevent the computer from entering sleep
            Models.Utility.PowerManagement.StartPreventSleep();
        }
        public static void SetCurrentStatus(String StatusText)
        {
            (App.Current as App).loading.SetCurrentStatus(StatusText);
        }

        private void StartupHandler(object sender, StartupEventArgs e)
        {
            try
            {
                this.MainWindow = new MainWindow();
                this.MainWindow.WindowState = WindowState.Maximized;
                this.MainWindow.Loaded += (s, evt) =>
                {
                    this.loading.EndWaiting();
                };
                this.MainWindow.Show();
            }
            catch (Exception ex)
            {
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
