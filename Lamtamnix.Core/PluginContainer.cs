namespace Lamtamnix.Core
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.Contracts;

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
            Contract.Requires<ArgumentNullException>(plugins != null);

            _plugins = plugins;
        }

        /// <summary>
        /// Finalizer of class.
        /// Used for calling <see cref="Dispose(bool)"/>.
        /// </summary>
        ~PluginContainer()
        {
            Dispose(false);
        }

        /// <summary>
        /// Implementation of <see cref="IDisposable"/> interface.
        /// Used for calling <see cref="Dispose(bool)"/>.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Disposing this instance.
        /// Unloads all plugins stored in this container.
        /// </summary>
        /// <param name="disposing"></param>
        private void Dispose(bool disposing)
        {
            if (_disposed)
                throw new ObjectDisposedException(null);

            if (disposing)
            {
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
            }

            _disposed = true;
        }
    }
}
