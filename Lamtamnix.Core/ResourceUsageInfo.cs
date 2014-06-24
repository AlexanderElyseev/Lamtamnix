namespace Lamtamnix.Core
{
    using System;

    /// <summary>
    /// Class-container with information about usage of system resources by some plugin.
    /// </summary>
    public sealed class ResourceUsageInfo
    {
        /// <summary>
        /// Gets the number of bytes that survived the last collection and that are known to be referenced
        /// by the application domain with the plugin (<see cref="AppDomain.MonitoringSurvivedMemorySize"/>).
        /// </summary>
        public Int64 SurvivedMemorySize { get; set; }

        /// <summary>
        /// Gets the total size, in bytes, of all memory allocations that have been made by the 
        /// application domain with the plugin since it was created, without subtracting memory that
        /// has been collected (<see cref="AppDomain.MonitoringTotalAllocatedMemorySize"/>).
        /// </summary>
        public Int64 TotalAllocatedMemorySize { get; set; }

        /// <summary>
        /// Gets the total processor time that has been used by all threads while executing in the application
        /// domain with the plugin, since the process started (<see cref="AppDomain.MonitoringTotalProcessorTime"/>).
        /// </summary>
        public TimeSpan TotalProcessorTime { get; set; }
    }
}
