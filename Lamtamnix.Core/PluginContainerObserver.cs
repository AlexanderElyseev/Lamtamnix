namespace Lamtamnix.Core
{
    using System;
    using System.Diagnostics.Contracts;
    using System.Threading;

    /// <summary>
    /// Class for observing changes in instance of <see cref="PluginContainer{TPlugin}"/>.
    /// </summary>
    /// <typeparam name="TPlugin">Type of plugins stored in the container.</typeparam>
    public sealed class PluginContainerObserver<TPlugin> : IDisposable
        where TPlugin : BasePlugin
    {
        /// <summary>
        /// Instance of <see cref="PluginContainer{TPlugin}"/> to observe.
        /// </summary>
        private readonly PluginContainer<TPlugin> _pluginContainer;

        /// <summary>
        /// Timer for <see cref="ResourcesUsagesUpdate"/>.
        /// </summary>
        private readonly Timer _timer;

        /// <summary>
        /// Backing field for <see cref="UpdateInterval"/>.
        /// </summary>
        private uint _updateInterval = 5000;

        /// <summary>
        /// Backing field for <see cref="ResourcesUsagesUpdate"/> event.
        /// </summary>
        private EventHandler<ResourcesUsageEventArgs<TPlugin>> _event;

        /// <summary>
        /// Interval (in milliseconds) for event firing.
        /// </summary>
        public uint UpdateInterval
        {
            get { return _updateInterval; }
            set { _updateInterval = value; }
        }

        /// <summary>
        /// Event which is fired periodically (each <see cref="UpdateInterval"/> milliseconds).
        /// </summary>
        public event EventHandler<ResourcesUsageEventArgs<TPlugin>> ResourcesUsagesUpdate
        {
            add
            {
                _event += value;
                if (_event.GetInvocationList().Length == 1)
                    _timer.Change(0, UpdateInterval);
            }

            remove
            {
                _event -= value;
                if (_event.GetInvocationList().Length == 0)
                    _timer.Change(0, Timeout.Infinite);
            }
        }

        /// <summary>
        /// Constructor of the class.
        /// </summary>
        /// <param name="pluginContainer">Instance of <see cref="PluginContainer{TPlugin}"/> to observe.</param>
        public PluginContainerObserver(PluginContainer<TPlugin> pluginContainer)
        {
            Contract.Requires<ArgumentNullException>(pluginContainer != null);

            _pluginContainer = pluginContainer;
            _timer = new Timer(s => OnResourcesUsagesUpdated());
        }

        /// <summary>
        /// Finalizer of the class.
        /// Used for calling <see cref="Dispose(bool)"/>.
        /// </summary>
        ~PluginContainerObserver()
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
            if (disposing)
                _timer.Dispose();
        }
    
        /// <summary>
        /// Event invocator for <see cref="ResourcesUsagesUpdate"/>.
        /// </summary>
        private void OnResourcesUsagesUpdated()
        {
            EventHandler<ResourcesUsageEventArgs<TPlugin>> handler = _event;
            if (handler != null)
                handler(this, new ResourcesUsageEventArgs<TPlugin>(_pluginContainer.GetResourcesUsage()));
        }
    }
}
