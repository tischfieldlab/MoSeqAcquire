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
using MoSeqAcquire.Models.Recording;
using MoSeqAcquire.Models.Triggers;

namespace MoSeqAcquire.Models.Management
{
    public static class ProtocolHelpers
    {
        #region MediaSourceProviders
        public static IEnumerable<Type> FindProviderTypes()
        {
            return ExtractPluginsImplementing<MediaSource>(Properties.Settings.Default.MediaSourcePluginPaths);
            /*return Assembly.GetExecutingAssembly()
                .GetTypes()
                .Where(t => !t.IsAbstract && typeof(MediaSource).IsAssignableFrom(t));*/
        }
        public static IEnumerable<Type> GetKnownTypesForProviders()
        {
            return FindProviderTypes().SelectMany(r => Attribute.GetCustomAttributes(r, typeof(KnownTypeAttribute)).Select(kt => (kt as KnownTypeAttribute).KnownType));
        }
        #endregion

        #region RecorderProviders
        public static IEnumerable<Type> FindRecorderTypes()
        {
            return ExtractPluginsImplementing<IMediaWriter>(Properties.Settings.Default.RecorderPluginPaths);
            /*return Assembly.GetExecutingAssembly()
                .GetTypes()
                .Where(t => !t.IsAbstract && typeof(IMediaWriter).IsAssignableFrom(t));
                */
        }
        public static IEnumerable<Type> GetKnownTypesForRecorders()
        {
            return FindRecorderTypes().SelectMany(r => Attribute.GetCustomAttributes(r, typeof(KnownTypeAttribute)).Select(kt => (kt as KnownTypeAttribute).KnownType));
        }
        #endregion

        public static IEnumerable<Type> GetKnownTypes()
        {
            return GetKnownTypesForProviders().Concat(GetKnownTypesForRecorders());
        }


        #region TriggerProviders
        public static IEnumerable<Type> FindTriggerTypes()
        {
            return ExtractPluginsImplementing<Trigger>(Properties.Settings.Default.RecorderPluginPaths);
        }
        public static IEnumerable<Type> FindTriggerActions()
        {
            return ExtractPluginsImplementing<TriggerAction>(Properties.Settings.Default.RecorderPluginPaths);
        }
        #endregion



        public static List<Type> ExtractPluginsImplementing<T>(StringCollection SearchPaths, bool IncludeSelf=true)
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
            if(filteredList.Count > 0)
            {
                Console.WriteLine(string.Join("\n", filteredList));
            }
            else
            {
                Console.WriteLine("None found!");
            }
            return filteredList;
        }
        public static List<Assembly> FindAssemblies(StringCollection SearchPaths)
        {
            Dictionary<String, Assembly> plugInAssemblyList = new Dictionary<String, Assembly>();
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
            Console.WriteLine("Searching path \"" + Path + "\" for plugins");
            List<Assembly> assemblyList = new List<Assembly>();
            DirectoryInfo dInfo = new DirectoryInfo(Path);
            if (dInfo.Exists)
            {
                FileInfo[] files = dInfo.GetFiles("*.dll", SearchOption.AllDirectories);
                if (null != files)
                {
                    foreach (FileInfo file in files)
                    {
                        Console.WriteLine(" -> Loading assembly \"" + file.FullName + "\"...");
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
