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

            // Domain ------------------------------

            //Create Domain Model
            ModelCreator modelCreator = new(@namespace, path);
            modelCreator.Create(entity, Properties);

            //Create IRepositry Contract
            RepositoryContractCreator repositoryContractCreator = new(@namespace, path);
            repositoryContractCreator.Create(entity);

            //Create Localization terms
            var localizationUpdater = new LocalizationUpdater(@namespace, path);
            localizationUpdater.Update(entity, Properties);

            // EF ------------------------------

            //Update EntityFramework Context
            EFContextUpdater efContextUpdater = new(@namespace, path);
            efContextUpdater.Update(entity, Properties);

            //Create Repositry 
            EFRepositoryCreator efRepositoryCreator = new(@namespace, path);
            efRepositoryCreator.Create(entity);

            // Contracts ------------------------------

            //Create Dtos
            DtosCreator dtosCreator = new(@namespace, path);
            dtosCreator.Create(entity, Properties);

            //Create IAppService Contract 
            AppServiceContractCreator appServiceContractCreator = new(@namespace, path);
            appServiceContractCreator.Create(entity);

            //Create Permission Contract 
            PermissionUpdater permissionUpdater = new(@namespace, path);
            permissionUpdater.Update(entity);

            //Define Permissions in provider
            PermissionProviderUpdater permissionProviderUpdater = new(@namespace, path);
            permissionProviderUpdater.Update(entity);

            // Application ------------------------------

            //Create AppService (Application)
            AppServiceCreator appServiceCreator = new(@namespace, path);
            appServiceCreator.Create(entity, Properties);

            //Create AutoMapper (Application)
            AppMapperUpdater appMapperUpdater = new(@namespace, path);
            appMapperUpdater.Update(entity);

            // Mvc ------------------------------

            //Menus
            MvcMenuUpdater mvcMenuUpdater = new(@namespace, path);
            mvcMenuUpdater.Update(entity);

            //Create View Models
            MvcViewModelCreator mvcViewModelCreator = new(@namespace, path);
            mvcViewModelCreator.Create(entity, Properties);

            //Create Index Page 
            MvcIndexPageCreator indexPageCreator = new(@namespace, path);
            indexPageCreator.Create(entity, Properties);

            //Create EditModal 
            MvcEditModalCreator editModalCreator = new(@namespace, path);
            editModalCreator.Create(entity);

            //Create CreateModal 
            MvcCreateModalCreator createModalCreator = new(@namespace, path);
            createModalCreator.Create(entity);

            MessageBox.Show("Files Generated!\r\n\r\n" +
                "Don't forget to run 'dotnet ef migrations' to add updates");
        }
    }
}
