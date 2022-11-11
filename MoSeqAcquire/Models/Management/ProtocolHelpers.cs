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
using Serilog;

namespace MoSeqAcquire.Models.Management
{
    public static class ProtocolHelpers
    {
        private static readonly ILogger Log = Serilog.Log.Logger;

        public static IEnumerable<ComponentSpecification> FindComponents()
        {
            return ExtractPluginsImplementing<Component>(Properties.Settings.Default.PluginPaths).Select(GetSpecification).Where(s => !s.IsHidden);
        }
        private static ComponentSpecification GetSpecification(Type componentType)
        {
            App.SetCurrentStatus($"Loading Component {componentType.FullName}....");
            if (typeof(MediaSource).IsAssignableFrom(componentType))
            {
                return new MediaSourceSpecification(componentType);
            }
            else if (typeof(IMediaWriter).IsAssignableFrom(componentType))
            {
                return new RecorderSpecification(componentType);
            }
            else if (typeof(TriggerAction).IsAssignableFrom(componentType))
            {
                return new TriggerItemSpecification(componentType);
            }
            else
            {
                return new ComponentSpecification(componentType);
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
        public static IEnumerable<ComponentSpecification> FindTriggerEvents()
        {
            //return ExtractPluginsImplementing<TriggerEvent>(new StringCollection());
            return FindComponents().Where(cs => typeof(TriggerEvent).IsAssignableFrom(cs.ComponentType));
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

                Log.Information("Found the following plugins implementing {PluginType}: {PluginList}", typeof(T), filteredList);
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
            Log.Information("Searching path {PluginPath} for plugins", Path);
            List<Assembly> assemblyList = new List<Assembly>();
            DirectoryInfo dInfo = new DirectoryInfo(Path);
            if (dInfo.Exists)
            {
                FileInfo[] files = dInfo.GetFiles("*.dll", SearchOption.AllDirectories);
                if (null != files)
                {
                    foreach (FileInfo file in files)
                    {
                        Log.Debug(" -> Loading dll file \"{Filename}\"", file.FullName);
                        try
                        {
                            assemblyList.Add(Assembly.LoadFile(file.FullName));
                        }
                        catch (Exception e)
                        {
                            Log.Warning(e, " -> Error Loading dll file \"{Filename}\"", file.FullName);
                        }
                    }
                }
            }
            return assemblyList;
        }
    }
}
