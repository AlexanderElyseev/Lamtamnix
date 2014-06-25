namespace Lamtamnix.Examples
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;

    using Lamtamnix.Core;

    public class MyBasePlugin : BasePlugin
    {
        private readonly List<object> _objects = new List<object>(); 

        public MyBasePlugin()
        {
            
        }

        public int GetInt()
        {
//            File.ReadAllBytes("C:\\file.tmp");
            return 42;
        }

        public int IncreaseMemoryUsage(int count)
        {
            _objects.AddRange(Enumerable.Range(0, count).Select(a => new object()));
            return _objects.Count;
        }

        public override string ToString()
        {
            return base.ToString();
        }
    }
}