using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows;
using MoSeqAcquire.Properties;

namespace MoSeqAcquire
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {

        public App()
        {

            Settings.Default.MediaSourcePluginPaths.Cast<string>().ForEach((p) => RegisterProbePath(p));
            Settings.Default.RecorderPluginPaths.Cast<string>().ForEach((p) => RegisterProbePath(p));

            AppDomain.CurrentDomain.AssemblyResolve += LoadResolveEventHandler;

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
        /*
        private Assembly CurrentDomain_AssemblyResolve(object sender, ResolveEventArgs args)
        {
            // missing resources are... missing
            if (args.Name.Contains(".resources"))
                return null;

            // check for assemblies already loaded
            Assembly assembly = AppDomain.CurrentDomain.GetAssemblies().FirstOrDefault(a => a.FullName == args.Name);
            if (assembly != null)
                return assembly;

            // Try to load by filename - split out the filename of the full assembly name
            // and append the base path of the original assembly (ie. look in the same dir)
            // NOTE: this doesn't account for special search paths but then that never
            //           worked before either.
            string filename = args.Name.Split(',')[0] + ".dll".ToLower();


            // try load the assembly from install path (this should always be automatic)
            try
            {
                // this allows addins to load before form has loaded
                return Assembly.LoadFrom(filename);
            }
            catch
            {
            }

            var search_paths = new List<string>();
            search_paths.AddRange(Settings.Default.MediaSourcePluginPaths.Cast<string>().ToArray());
            search_paths.AddRange(Settings.Default.RecorderPluginPaths.Cast<string>().ToArray());

            foreach (var sp in search_paths)
            {
                // try load from install addins folder
                string asmFile = FindFileInPath(filename, sp);
                if (!string.IsNullOrEmpty(asmFile))
                {
                    try
                    {
                        return Assembly.LoadFrom(asmFile);
                    }
                    catch
                    {
                    }
                }
            }

            return null;
        }
        private string FindFileInPath(string filename, string path)
        {
            filename = filename.ToLower();

            foreach (var fullFile in Directory.GetFiles(path))
            {
                var file = Path.GetFileName(fullFile).ToLower();
                if (file == filename)
                    return fullFile;
            }
            foreach (var dir in Directory.GetDirectories(path))
            {
                var file = FindFileInPath(filename, dir);
                if (!string.IsNullOrEmpty(file))
                    return file;
            }

            return null;
        }*/
    }
}
