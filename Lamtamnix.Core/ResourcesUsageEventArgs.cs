namespace Lamtamnix.Core
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.Contracts;

    /// <summary>
    /// Arguments of event with information about resources usages.
    /// </summary>
    public sealed class ResourcesUsageEventArgs<TPlugin> : EventArgs
    {
        /// <summary>
        /// Information about resources usages.
        /// </summary>
        public Dictionary<TPlugin, ResourcesUsageInfo> ResourcesUsage { get; private set; }

        /// <summary>
        /// Constructor of the class.
        /// </summary>
        /// <param name="info">Information about resources usages.</param>
        public ResourcesUsageEventArgs(Dictionary<TPlugin, ResourcesUsageInfo> info)
        {
            Contract.Requires<ArgumentNullException>(info != null);

            ResourcesUsage = info;
        }
    }
}
