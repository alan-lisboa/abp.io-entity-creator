using EntityCreator.Generators;
using EntityCreator.Models;
using Humanizer;
using System.IO;

namespace EntityCreator
{
    public partial class MainForm : Form
    {
        public List<PropertyModel> Properties { get; set; }

        public MainForm()
        {
            InitializeComponent();
            Properties = [];

            var directory = Directory.GetCurrentDirectory();
            LoadDir(directory);

            textBox3.Focus();
        }

        private void Button1_Click(object sender, EventArgs e)
        {
            textBox2.Text = "";

            FolderBrowserDialog folderBrowserDialog = new();
            var result = folderBrowserDialog.ShowDialog();
            if (result != DialogResult.OK)
                return;
            
            string path = folderBrowserDialog.SelectedPath;

            LoadDir(path);

            if (string.IsNullOrEmpty(textBox2.Text))
                MessageBox.Show("Project file not found!");
        }

        private void Button3_Click(object sender, EventArgs e)
        {
            PropertyForm propertyForm = new();
            propertyForm.ShowDialog();

            var model = propertyForm.Model;

            if (string.IsNullOrEmpty(model.Name))
                return;

            model.Index = Properties.Count + 1;

            string item = model.Name + "<" + model.Type + ">";
            
            if (model.Size > 0)
                item += "(" + model.Size + ")";

            Properties.Add(model);

            listBox1.Items.Add(item);
        }

        private void Button2_click(object sender, EventArgs e)
        {
            if (textBox3.Text.Length == 0)
            {
                MessageBox.Show("Entity name is requerid");
                return;
            }

            if (Properties.Count == 0)
            {
                MessageBox.Show("At least one property it's necessary");
                return;
            }

            var confirm = MessageBox.Show(
                "Confirm to proceed?", "Confirm", MessageBoxButtons.YesNo);
            
            if (confirm == DialogResult.No)
                return;

            var entity = new EntityModel
            {
                Name = textBox3.Text,
                Namespace = textBox4.Text,
                Location = textBox2.Text,
                Properties = Properties
            };

            EntityGenerator entityGenerator = new();
            entityGenerator.Generate(entity);

            MessageBox.Show("Files Generated!\r\n\r\n" +
                "Don't forget to run 'dotnet ef migrations' to add updates");
        }

        private void LoadDir(string path)
        {
            var files = Directory.GetFiles(path).Where(x => x.EndsWith(".sln"));

            if (!files.Any())
                return;

            var file = Path.GetFileName(files.First());
            var project = string.Empty;
            var @namespace = string.Empty;

            if (!string.IsNullOrEmpty(file))
            {
                @namespace = file.Replace(".sln", "");
                int index = @namespace.IndexOf('.') + 1;
                project = @namespace[index..];
            }

            textBox1.Text = project;
            textBox2.Text = path;
            textBox4.Text = @namespace;
        }
    }
}
