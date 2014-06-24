namespace Lamtamnix.Core
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;

    /// <summary>
    /// Loader for plugins.
    /// </summary>
    public static class Loader
    {
        /// <summary>
        /// Index for loader appdomains frinedly names.
        /// </summary>
        private static int loaderDomainIndex = 1;

        /// <summary>
        /// Index for plugin appdomain frinedlys names.
        /// </summary>
        private static int pluginDomainIndex = 1;

        /// <summary>
        /// Loads instances of plugins with specified type loader.
        /// <remarks>
        ///     <list type="bullet">
        ///         <item><description>Loader will create single instance of each types from type loader.</description></item>
        ///         <item><description>Type loader will be created in separate domain.</description></item>
        ///         <item><description>Each plugin will be created in separate domain.</description></item>
        ///     </list>
        /// </remarks>
        /// </summary>
        /// <typeparam name="TPlugin">Type of plugins that will be instantiated.</typeparam>
        /// <typeparam name="TPluginTypeLoader">Type of loader for plugins.</typeparam>
        /// <param name="loaderArgs">Arguments for constructor of type loader.</param>
        /// <returns>Container with instances of plugins form type loader.</returns>
        public static PluginContainer<TPlugin> LoadInstances<TPlugin, TPluginTypeLoader>(params object[] loaderArgs)
            where TPlugin : BasePlugin
            where TPluginTypeLoader : PluginTypeLoader
        {
            var loaderDomain = AppDomain.CreateDomain("LamtamnixLoaderDomain-" + loaderDomainIndex);
            Interlocked.Increment(ref loaderDomainIndex);

            var loader = (PluginTypeLoader)loaderDomain.CreateInstanceAndUnwrap(
                typeof(TPluginTypeLoader).Assembly.FullName,
                typeof(TPluginTypeLoader).FullName,
                false,
                0,
                null,
                loaderArgs,
                null,
                null);
            
            var plugins = new Dictionary<TPlugin, AppDomain>();
            var pluginTypes = loader.GetPluginTypes().Where(t => !t.IsAbstract && t.IsAssignableFrom(typeof(TPlugin)));

            foreach (Type pluginType in pluginTypes)
            {
                var pluginDomain = AppDomain.CreateDomain("LamtamnixPluginDomain-" + pluginDomainIndex);
                Interlocked.Increment(ref pluginDomainIndex);

                var plugin = (TPlugin)pluginDomain.CreateInstanceAndUnwrap(pluginType.Assembly.FullName, pluginType.FullName);

                plugins.Add(plugin, pluginDomain);
            }

            AppDomain.Unload(loaderDomain);

            return new PluginContainer<TPlugin>(plugins);
        }

        /// <summary>
        /// Loads plugins asynchronously with specified type loader.
        /// <remarks>Each plugin will be created in separate domain.</remarks>
        /// </summary>
        /// <typeparam name="TPlugin">Type of plugins that will be instantiated.</typeparam>
        /// <typeparam name="TPluginTypeLoader">Type of loader for plugins.</typeparam>
        /// <param name="loaderArgs">Arguments for constructor of type loader.</param>
        /// <returns>Container with instances of plugins form type loader.</returns>
        public static async Task<PluginContainer<TPlugin>> LoadInstancesAsync<TPlugin, TPluginTypeLoader>(params object[] loaderArgs)
            where TPlugin : BasePlugin 
            where TPluginTypeLoader : PluginTypeLoader
        {
            return await Task.Factory.StartNew(() => LoadInstances<TPlugin, TPluginTypeLoader>(loaderArgs));
        }
    }
}
