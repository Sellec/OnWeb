using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Reflection;
using System.Web.Configuration;
using System.Web.WebPages.Razor.Configuration;

namespace OnWeb.Binding.Razor
{
    /// <summary>
    /// </summary>
    public static class WebStartup
    {
        private static bool Initialized = false;
        private static object SyncRoot = new object();

        private static FieldInfo _cfgReadOnlyCollection = typeof(ConfigurationElementCollection).GetField("bReadOnly", BindingFlags.Instance | BindingFlags.NonPublic);
        private static FieldInfo _cfgReadOnly = typeof(ConfigurationElement).GetField("_bReadOnly", BindingFlags.Instance | BindingFlags.NonPublic);
        private static FieldInfo _cfgValues = typeof(ConfigurationElement).GetField("_values", BindingFlags.Instance | BindingFlags.NonPublic);
        private static PropertyInfo _cfgValuesElements = null;
        private static FieldInfo _cfgEntryValue = null;

        public static void PreApplicationStartMethod()
        {
            //Initialized = false;
            Initialize();
        }

        private static void ConfigurationElementReadOnly(ConfigurationElement element, bool isReadOnly)
        {
            try
            {
                if (element is ConfigurationElementCollection)
                {
                    if (_cfgReadOnlyCollection != null)
                    {
                        _cfgReadOnlyCollection.SetValue(element, isReadOnly);
                        foreach (var entry in (element as ConfigurationElementCollection))
                        {
                            if (entry is ConfigurationElement) ConfigurationElementReadOnly(entry as ConfigurationElement, isReadOnly);
                            else
                            {
                                try
                                {
                                    if (_cfgEntryValue == null) _cfgEntryValue = entry.GetType().GetField("_value", BindingFlags.Instance | BindingFlags.NonPublic);
                                    if (_cfgEntryValue != null)
                                    {
                                        var _element = _cfgEntryValue.GetValue(entry) as ConfigurationElement;
                                        if (_element != null) ConfigurationElementReadOnly(_element, isReadOnly);
                                    }
                                }
                                catch (Exception) { }
                            }
                        }

                }
                }

                if (_cfgReadOnly != null && _cfgValues != null)
                {
                    _cfgReadOnly.SetValue(element, isReadOnly);

                    var values = _cfgValues.GetValue(element);
                    if (values != null)
                    {
                        if (_cfgValuesElements == null) _cfgValuesElements = values.GetType().GetProperty("ConfigurationElements", BindingFlags.Instance | BindingFlags.NonPublic);

                        var elements = _cfgValuesElements.GetValue(values) as System.Collections.IEnumerable;
                        if (elements != null)
                            foreach (ConfigurationElement value in elements)
                            {
                                ConfigurationElementReadOnly(value, isReadOnly);
                            }
                    }
                }
            }
            catch (Exception) { }
        }

        public static void Initialize()
        {
            try
            {
                lock (SyncRoot)
                {
                    if (!Initialized)
                    {
                        try
                        {
                            /*
                             * Устанавливаем провайдер сессий
                             * */
                            var sessionStateSection = WebConfigurationManager.GetWebApplicationSection("system.web/sessionState") as SessionStateSection;
                            if (sessionStateSection != null)
                            {
                                var providerType = typeof(Providers.TraceSessionStateProvider);

                                ConfigurationElementReadOnly(sessionStateSection, false);

                                sessionStateSection.Mode = System.Web.SessionState.SessionStateMode.Custom;
                                sessionStateSection.CustomProvider = providerType.Name;
                                sessionStateSection.Timeout = TimeSpan.FromMinutes(24 * 60 * 365);

                                sessionStateSection.Providers.Clear();
                                sessionStateSection.Providers.Add(new ProviderSettings(providerType.Name, providerType.FullName));

                                //sessionStateSection.Mode = System.Web.SessionState.SessionStateMode.Off;
                            }
                        }
                        catch (Exception ex)
                        {
                            Debug.WriteLine("SessionStateSection: {0}", ex.Message);
                        }

                        try
                        {
                            /*
                             * Задаем настройки компиляции
                             * */
                            var compilationSection = WebConfigurationManager.GetSection("system.web/compilation") as CompilationSection;
                            if (compilationSection != null)
                            {
                                ConfigurationElementReadOnly(compilationSection, false);

#if DEBUG
                                compilationSection.Debug = true;
                                //compilationSection.OptimizeCompilations = true;
#else
                                compilationSection.Debug = false;
                                //compilationSection.OptimizeCompilations = false;
#endif

                                var dd = AppDomain.CurrentDomain.GetAssemblies().Where(x => x.FullName.Contains("netstandard")).ToList();

                                //var ass = compilationSection.Assemblies.Cast<AssemblyInfo>().Select(x => x.Assembly).ToList();
                                //var assList = new List<Assembly>();

                                //foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
                                //    if (!ass.Contains(assembly.FullName))
                                //    {
                                //        ass.Add(assembly.FullName);
                                //        assList.Add(assembly);
                                //    }

                                //var ass2 = ass.OrderBy(x => x).ToList();

                                ////compilationSection.Assemblies.Clear();
                                ////foreach (var assembly in ass2) compilationSection.Assemblies.Add(new AssemblyInfo(assembly));

                                var s_dynamicallyAddedReferencedAssemblyField = typeof(System.Web.Compilation.BuildManager).GetField("s_dynamicallyAddedReferencedAssembly", BindingFlags.Static | BindingFlags.NonPublic);
                                if (s_dynamicallyAddedReferencedAssemblyField != null)
                                {
                                    var s_dynamicallyAddedReferencedAssembly = s_dynamicallyAddedReferencedAssemblyField.GetValue(null) as HashSet<Assembly>;
                                    if (s_dynamicallyAddedReferencedAssembly != null)
                                        foreach (var assembly in dd)
                                            s_dynamicallyAddedReferencedAssembly.Add(assembly);
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            Debug.WriteLine("CompilationSection: {0}", ex.Message);
                        }

                        Initialized = true;
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Error: {0}", ex.ToString());
                throw;
            }
        }
    }
}