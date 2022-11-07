using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using MoSeqAcquire.Models.Acquisition;
using MoSeqAcquire.Models.Attributes;
using MoSeqAcquire.Models.Core;
using MoSeqAcquire.Models.Recording;
using MoSeqAcquire.Models.Triggers;

namespace MoSeqAcquire.Models.Management
{
    public static class ProtocolHelpers
    {
        public static IEnumerable<ComponentSpecification> FindComponents()
        {
            return ExtractPluginsImplementing<Component>(Properties.Settings.Default.PluginPaths).Select(c => GetSpecification(c)).Where(s => !s.IsHidden);
        }
        private static ComponentSpecification GetSpecification(Type ComponentType)
        {
            App.SetCurrentStatus($"Loading Component {ComponentType.FullName}....");
            if (typeof(MediaSource).IsAssignableFrom(ComponentType))
            {
                return new MediaSourceSpecification(ComponentType);
            }
            else if (typeof(IMediaWriter).IsAssignableFrom(ComponentType))
            {
                return new RecorderSpecification(ComponentType);
            }
            else
            {
                return new ComponentSpecification(ComponentType);
            }
        }
        #region MediaSourceProviders
        public static IEnumerable<ComponentSpecification> FindProviderTypes()
        {
            return FindComponents().Where(cs => cs is MediaSourceSpecification);
        }
        #endregion

        #region RecorderProviders
        public static IEnumerable<ComponentSpecification> FindRecorderTypes()
        {
            return FindComponents().Where(cs => cs is RecorderSpecification);
        }
        #endregion

        #region TriggerProviders
        public static IEnumerable<Type> FindTriggerTypes()
        {
            return ExtractPluginsImplementing<TriggerEvent>(new StringCollection());
        }
        public static IEnumerable<ComponentSpecification> FindTriggerActions()
        {
            return FindComponents().Where(cs => typeof(TriggerAction).IsAssignableFrom(cs.ComponentType));
        }
        #endregion

        public static IEnumerable<Type> GetKnownTypes()
        {
            return FindComponents().SelectMany(cs => cs.KnownTypes);
        }

        private static Dictionary<Type, List<Type>> __pluginTypeCache = new Dictionary<Type, List<Type>>();
        public static List<Type> ExtractPluginsImplementing<T>(StringCollection SearchPaths, bool UseCache=true)
        {
            if (UseCache && __pluginTypeCache.ContainsKey(typeof(T)))
            {
                return __pluginTypeCache[typeof(T)];
            }
            if (UseCache && __pluginTypeCache.Keys.Any(t => t.IsAssignableFrom(typeof(T))))
            {
                //looks like we may have the types but stored under a more abstract type
                return __pluginTypeCache.Keys
                                        .Where(t => t.IsAssignableFrom(typeof(T)))
                                        .SelectMany(t => __pluginTypeCache[t])
                                        .ToList();
            }
            else
            {
                var filteredList = FindAssemblies(SearchPaths)
                                    .SelectMany(a => a.GetTypes())
                                    .Where(t => !t.IsAbstract && typeof(T).IsAssignableFrom(t))
                                    .ToList();

                Console.WriteLine("Found the following plugins implementing \"" + typeof(T).AssemblyQualifiedName + "\":");
                if (filteredList.Count > 0)
                {
                    Console.WriteLine(string.Join("\n", filteredList));
                }
                else
                {
                    Console.WriteLine("None found!");
                }
                __pluginTypeCache[typeof(T)] = filteredList;
                return filteredList;
            }
        }
        public static List<Assembly> FindAssemblies(StringCollection SearchPaths, bool InlcudeCwd = true, bool IncludeCurrentAssembly = true)
        {
            Dictionary<String, Assembly> plugInAssemblyList = new Dictionary<String, Assembly>();
            var currentAssembly = Assembly.GetExecutingAssembly();

            if (InlcudeCwd)
            {
                SearchPaths.Add(Path.GetDirectoryName(currentAssembly.Location));
            }

            SearchPaths.Cast<string>()
                       .SelectMany(p => FindAssembliesForPath(p))
                       .Distinct(a => a.FullName)
                       .ForEach(a => plugInAssemblyList.Add(a.FullName, a));

            if (IncludeCurrentAssembly)
            {
                plugInAssemblyList.Add(currentAssembly.FullName, currentAssembly);
            }
            
            return plugInAssemblyList.Values.ToList();
        }
        public static List<Assembly> FindAssembliesForPath(String Path)
        {
            //Console.WriteLine("Searching path \"" + Path + "\" for plugins");
            List<Assembly> assemblyList = new List<Assembly>();
            DirectoryInfo dInfo = new DirectoryInfo(Path);
            if (dInfo.Exists)
            {
                FileInfo[] files = dInfo.GetFiles("*.dll", SearchOption.AllDirectories);
                if (null != files)
                {
                    foreach (FileInfo file in files)
                    {
                        //Console.WriteLine(" -> Loading assembly \"" + file.FullName + "\"...");
                        try
                        {
                            assemblyList.Add(Assembly.LoadFile(file.FullName));
                        }
                        catch (Exception)
                        {
                            Console.WriteLine($" -> Error Loading assembly \"{file.FullName}\"...");
                        }
                    }
                }
            }
            return assemblyList;
        }
    }
}
