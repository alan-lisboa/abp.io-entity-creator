using Humanizer;

namespace EntityCreator
{
    public partial class MainForm : Form
    {
        public List<PropertyModel> Properties { get; set; }

        public MainForm()
        {
            InitializeComponent();
            Properties = [];
        }

        private void Button1_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog folderBrowserDialog = new();
            var result = folderBrowserDialog.ShowDialog();
            if (result != DialogResult.OK)
                return;
            
            string path = folderBrowserDialog.SelectedPath;
            
            var files = Directory.GetFiles(path).Where(x => x.EndsWith(".sln"));

            if (!files.Any())
            {
                MessageBox.Show("Project file not found!");
                return;
            }

            var file = Path.GetFileName(files.First());
            var project = string.Empty;
            var namespc = string.Empty;

            if (!string.IsNullOrEmpty(file))
            {
                namespc = file.Replace(".sln", "");
                int index = namespc.IndexOf(".") + 1;
                project = namespc[index..];
            }

            textBox1.Text = project;
            textBox2.Text = path;
            textBox4.Text = namespc;
        }

        private void Button3_Click(object sender, EventArgs e)
        {
            PropertyForm propertyForm = new PropertyForm();
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

            string @namespace = textBox4.Text;
            string path = textBox2.Text;
            string entity = textBox3.Text;

            //Create Domain Model (Domain)
            ModelCreator modelCreator = new(@namespace, path);
            modelCreator.Create(entity, Properties);

            //Create IRepositry Contract (Domain, only for advanced mode)
            //RepositoryContractCreator repositoryContractCreator = new(@namespace, path);
            //repositoryContractCreator.Create(entity);

            //Update EntityFramework Context
            EFContextUpdater efContextUpdater = new(@namespace, path);
            efContextUpdater.Update(entity, Properties);

            //Create Repositry (EF,only for advanced mode)

            //Create Domain Contract (Contracts)
            DomainContractCreator domainContractCreator = new(@namespace, path);
            domainContractCreator.Create(entity, Properties);

            //Create CreateUpdate Contract (Contracts)
            EditContractCreator editContractCreator = new(@namespace, path);
            editContractCreator.Create(entity, Properties);

            //Create IAppService Contract (Contracts)
            AppServiceContractCreator appServiceContractCreator = new(@namespace, path);
            appServiceContractCreator.Create(entity);

            //Create Permission Contract (Contracts)
            PermissionUpdater permissionUpdater = new(@namespace, path);
            permissionUpdater.Update(entity);

            //Define Permissions in provider
            PermissionProviderUpdater permissionProviderUpdater = new(@namespace, path);
            permissionProviderUpdater.Update(entity);

            //Create AppService (Application)
            AppServiceCreator appServiceCreator = new(@namespace, path);
            appServiceCreator.Create(entity);

            //Create AutoMapper (Application)
            AppMapperUpdater appMapperUpdater = new(@namespace, path);
            appMapperUpdater.Update(entity);

            //Create Index Page (Web)
            IndexPageCreator indexPageCreator = new(@namespace, path);
            indexPageCreator.Create(entity);

            //Create Index JS (Web)
            IndexJsCreator indexJsCreator = new(@namespace, path);
            indexJsCreator.Create(entity, Properties);

            //Create EditModal (Web)
            EditModalCreator editModalCreator = new(@namespace, path);
            editModalCreator.Create(entity);

            //Create CreateModal (Web)
            CreateModalCreator createModalCreator = new(@namespace, path);
            createModalCreator.Create(entity);

            MessageBox.Show("Files Generated!\r\n" +
                "Don't forget to execute 'dotnet ef migrations' to add updates");
        }
    }
}
