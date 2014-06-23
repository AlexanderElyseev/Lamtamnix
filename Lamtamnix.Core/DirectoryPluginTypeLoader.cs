namespace Lamtamnix.Core
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.Contracts;
    using System.IO;
    using System.Linq;
    using System.Reflection;

    /// <summary>
    /// Class of plugin type loaders from all assemblies at the directory.
    /// </summary>
    /// <typeparam name="TPlugin">Type for searching in the directory.</typeparam>
    public class DirectoryPluginTypeLoader<TPlugin> : PluginTypeLoader
        where TPlugin : BasePlugin
    {
        /// <summary>
        /// Directory for scanning.
        /// </summary>
        private readonly string _directory;

        /// <summary>
        /// Class constructor with directory for scanning.
        /// </summary>
        /// <param name="directory">Directory for scanning.</param>
        public DirectoryPluginTypeLoader(string directory)
        {
            Contract.Requires<ArgumentNullException>(directory != null);
            Contract.Requires<ArgumentException>(directory != string.Empty);

            _directory = directory;
        }

        /// <summary>
        /// Returns list of plugins types that from assemblies in the directory.
        /// </summary>
        /// <returns>List of plugins that should be created.</returns>
        public override IEnumerable<Type> GetPluginTypes()
        {
            var plugins = new List<Type>();
            foreach (string fileName in Directory.GetFiles(_directory))
            {
                Assembly assembly;
                try
                {
                    assembly = Assembly.LoadFile(fileName);
                }
                catch (BadImageFormatException)
                {
                    continue;
                }
                
                foreach (Type type in assembly.ExportedTypes.Where(t => !t.IsAbstract && t.IsAssignableFrom(typeof(TPlugin))))
                {
                    plugins.Add(type);
                }
            }

            return plugins;
        }
    }
}
