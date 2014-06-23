namespace Lamtamnix.Core
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Base class of plugin type loaders.
    /// </summary>
    public abstract class PluginTypeLoader : MarshalByRefObject
    {
        /// <summary>
        /// Returns list of plugins types that should be created by <see cref="Loader"/>.
        /// </summary>
        /// <returns>List of plugins that should be created.</returns>
        public abstract IEnumerable<Type> GetPluginTypes();
    }
}
