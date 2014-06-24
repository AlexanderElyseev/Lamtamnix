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
        }

        private async void ReloadPlugins()
        {
            if (_container != null)
            {
                _container.Dispose();
            }

            _container = await Loader.LoadInstancesAsync<MyBasePlugin, DirectoryPluginTypeLoader<MyBasePlugin>>(Application.StartupPath);
        }

        private void UpdateUsages()
        {
            int i = 1;
            listView1.Items.Clear();
            foreach (var record in _container.GetResourcesUsage())
            {
                var items = new[]
                                {
                                    i.ToString(),
                                    record.Value.TotalAllocatedMemorySize.ToString(),
                                    record.Value.SurvivedMemorySize.ToString(),
                                    record.Value.TotalProcessorTime.ToString()
                                };

                listView1.Items.Add(new ListViewItem(items));
                i++;
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            ReloadPlugins();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            do
            {
                _folderBrowserDialog1.ShowDialog(this);
            }
            while (!Directory.Exists(_folderBrowserDialog1.SelectedPath));

            textBox1.Text = _folderBrowserDialog1.SelectedPath;

            ReloadPlugins();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            UpdateUsages();
        }
    }
}
