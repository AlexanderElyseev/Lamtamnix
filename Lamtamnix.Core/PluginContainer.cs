namespace Lamtamnix.Core
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Class container with loaded plugins.
    /// </summary>
    /// <typeparam name="TPlugin">Type of plugins stored in this container.</typeparam>
    public sealed class PluginContainer<TPlugin> : MarshalByRefObject, IDisposable
        where TPlugin : BasePlugin
    {
        /// <summary>
        /// Flag of disposed object.
        /// </summary>
        private bool _disposed;

        /// <summary>
        /// List of stored plugins.
        /// </summary>
        private readonly Dictionary<TPlugin, AppDomain> _plugins;

        /// <summary>
        /// Plugins stored in this container.
        /// </summary>
        public IEnumerable<TPlugin> Plugins
        {
            get
            {
                if (_disposed)
                    throw new ObjectDisposedException(null);

                return _plugins.Keys;
            }
        }

        /// <summary>
        /// Constructor of class.
        /// Initializes container with the list of plugins.
        /// </summary>
        /// <param name="plugins">List of plugins with related appdomains.</param>
        internal PluginContainer(Dictionary<TPlugin, AppDomain> plugins)
        {
            _plugins = plugins;
        }

        /// <summary>
        /// Implementation of <see cref="IDisposable"/>.
        /// Unload all plugins stored in this container.
        /// </summary>
        public void Dispose()
        {
            if (_disposed)
                throw new ObjectDisposedException(null);

            foreach (var pluginDomain in _plugins.Values)
            {
                try
                {
                    AppDomain.Unload(pluginDomain);
                }
                catch (Exception)
                {
                }
            }

            _disposed = true;
        }
    }
}
