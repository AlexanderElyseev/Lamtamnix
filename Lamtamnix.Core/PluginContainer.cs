namespace Lamtamnix.Core
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.Contracts;
    using System.Threading.Tasks;

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
        /// <exception cref="ObjectDisposedException">You can not use this instance after disposing.</exception>
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
        /// Returns information about usage of system resources by the plugins in this container.
        /// </summary>
        /// <exception cref="ObjectDisposedException">You can not use this instance after disposing.</exception>
        /// <exception cref="InvalidOperationException">You have to enable <see cref="AppDomain.MonitoringIsEnabled"/> before using.</exception>
        /// <returns>Usage of system resources by plugins.</returns>
        public Dictionary<TPlugin, ResourceUsageInfo> GetResourcesUsage()
        {
            if (_disposed)
                throw new ObjectDisposedException(null);

            if (!AppDomain.MonitoringIsEnabled)
                throw new InvalidOperationException("You have to enable appdomain monitoring (AppDomain.MonitoringIsEnabled).");

            Dictionary<TPlugin, ResourceUsageInfo> data = new Dictionary<TPlugin, ResourceUsageInfo>(_plugins.Count);
            foreach (var entry in _plugins)
            {
                AppDomain pluginDomain = entry.Value;
                var usageInfo = new ResourceUsageInfo
                {
                    SurvivedMemorySize = pluginDomain.MonitoringSurvivedMemorySize,
                    TotalAllocatedMemorySize = pluginDomain.MonitoringTotalAllocatedMemorySize,
                    TotalProcessorTime = pluginDomain.MonitoringTotalProcessorTime,
                    DomainFrindlyName = pluginDomain.FriendlyName
                };
                data[entry.Key] = usageInfo;
            }

            return data;
        }

        /// <summary>
        /// Returns information about usage of system resources by the plugins in this container.
        /// Asynchronous version of <see cref="GetResourcesUsage"/>.
        /// </summary>
        /// <returns>Usage of system resources by plugins.</returns>
        public async Task<Dictionary<TPlugin, ResourceUsageInfo>> GetResourcesUsageAsync()
        {
            return await Task.Factory.StartNew(() => GetResourcesUsage());
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
