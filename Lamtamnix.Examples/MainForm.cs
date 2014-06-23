namespace Lamtamnix.Examples
{
    using System;
    using System.IO;
    using System.Windows.Forms;

    using Lamtamnix.Core;

    internal partial class MainForm : Form
    {
        private PluginContainer<MyBasePlugin> container;

        private DirectoryPluginTypeLoader<MyBasePlugin> typeLoader;

        public MainForm()
        {
            InitializeComponent();

            _folderBrowserDialog1.SelectedPath = Application.StartupPath;
            textBox1.Text = _folderBrowserDialog1.SelectedPath;
            typeLoader = new DirectoryPluginTypeLoader<MyBasePlugin>(Application.StartupPath);

            ReloadPlugins();
        }

        private async void ReloadPlugins()
        {
            if (container != null)
            { 
                container.Dispose();
            }

            container = await Loader.LoadAsync<MyBasePlugin, DirectoryPluginTypeLoader<MyBasePlugin>>(Application.StartupPath);

            listView1.Clear();
            foreach (MyBasePlugin plugin in container.Plugins)
            {
                listView1.Items.Add(plugin.GetType().FullName);
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
            typeLoader = new DirectoryPluginTypeLoader<MyBasePlugin>(Application.StartupPath);

            ReloadPlugins();
        }
    }
}
