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
            return ExtractPluginsImplementing<Component>(Properties.Settings.Default.PluginPaths).Select(c => GetSpecification(c));
        }
        private static ComponentSpecification GetSpecification(Type ComponentType)
        {
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
            return ExtractPluginsImplementing<Trigger>(Properties.Settings.Default.PluginPaths);
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
        public static List<Type> ExtractPluginsImplementing<T>(StringCollection SearchPaths, bool IncludeSelf=true, bool UseCache=true)
        {
            if (UseCache && __pluginTypeCache.ContainsKey(typeof(T)))
            {
                return __pluginTypeCache[typeof(T)];
            }
            else
            {
                List<Type> availableTypes = new List<Type>();
                if (IncludeSelf)
                {
                    availableTypes.AddRange(Assembly.GetExecutingAssembly().GetTypes());
                }
                foreach (Assembly currentAssembly in FindAssemblies(SearchPaths))
                {
                    availableTypes.AddRange(currentAssembly.GetTypes());
                }
                List<Type> filteredList = availableTypes.FindAll(delegate (Type t)
                {
                    return !t.IsAbstract && typeof(T).IsAssignableFrom(t); // t.IsSubclassOf(typeof(T));
                });
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
        public static List<Assembly> FindAssemblies(StringCollection SearchPaths, bool InlcudeCwd = true)
        {
            Dictionary<String, Assembly> plugInAssemblyList = new Dictionary<String, Assembly>();
            if (InlcudeCwd)
            {
                var cwd = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
                var assemblies = FindAssembliesForPath(cwd);
                foreach (var a in assemblies)
                {
                    if (!plugInAssemblyList.ContainsKey(a.FullName))
                    {
                        plugInAssemblyList.Add(a.FullName, a);
                    }
                }
            }
            foreach (var path in SearchPaths)
            {
                var assemblies = FindAssembliesForPath(path);
                foreach (var a in assemblies)
                {
                    if (!plugInAssemblyList.ContainsKey(a.FullName))
                    {
                        plugInAssemblyList.Add(a.FullName, a);
                    }
                }
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
                        catch { }
                    }
                }
            }
            return assemblyList;
        }
    }
}
