namespace Lamtamnix.Examples
{
    using System;
    using System.IO;
    using System.Windows.Forms;

    using Lamtamnix.Core;

    internal partial class MainForm : Form
    {
        private PluginContainer<MyBasePlugin> _container;

        public MainForm()
        {
            InitializeComponent();

            _folderBrowserDialog1.SelectedPath = Application.StartupPath;
            textBox1.Text = _folderBrowserDialog1.SelectedPath;

            ReloadPlugins();
            UpdateCurrentDomainData();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            ChoosePluginsDirectory();
            ReloadPlugins();
            UpdateCurrentDomainData();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            ReloadPlugins();
            UpdateCurrentDomainData();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            UpdateResourcesUsage();
            UpdateCurrentDomainData();
        }

        /// <summary>
        /// Button <see cref="button4"/> click handler.
        /// Increases memory usage of each loaded plugin by calling special method.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The arguments of the event.</param>
        private void IncreaseMemoryUsageButtonClickHandler(object sender, EventArgs e)
        {
            foreach (var plugin in _container.Plugins)
            {
                plugin.IncreaseMemoryUsage(1000);
            }
        }

        private void UpdateCurrentDomainData()
        {
            AppDomain currentDomain = AppDomain.CurrentDomain;
            textBox2.Text = currentDomain.FriendlyName;
            textBox3.Text = currentDomain.GetAssemblies().Length.ToString();
        }

        /// <summary>
        /// Reloads all plugins from directory from <see cref="_folderBrowserDialog1"/>.
        /// Previously loaded plugins will be disposed.
        /// </summary>
        private async void ReloadPlugins()
        {
            if (_container != null)
            {
                _container.Dispose();
            }

            _container = await Loader.LoadInstancesAsync<MyBasePlugin, DirectoryPluginTypeLoader<MyBasePlugin>>(_folderBrowserDialog1.SelectedPath);

            UpdateResourcesUsage();
        }

        private void ChoosePluginsDirectory()
        {
            do
            {
                _folderBrowserDialog1.ShowDialog(this);
            }
            while (!Directory.Exists(_folderBrowserDialog1.SelectedPath));

            textBox1.Text = _folderBrowserDialog1.SelectedPath;
        }

        private void UpdateResourcesUsage()
        {
            int i = 1;
            listView1.Items.Clear();
            foreach (var record in _container.GetResourcesUsage())
            {
                ResourceUsageInfo info = record.Value;
                var items = new[]
                {
                    i.ToString(),
                    info.TotalAllocatedMemorySize.ToString(),
                    info.SurvivedMemorySize.ToString(),
                    info.TotalProcessorTime.ToString(),
                    info.DomainFrindlyName,
                    info.LoadedAssembliesCount.ToString()
                };

                listView1.Items.Add(new ListViewItem(items));
                i++;
            }
        }
    }
}
