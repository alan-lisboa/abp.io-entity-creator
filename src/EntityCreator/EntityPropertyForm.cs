namespace EntityCreator
{
    public partial class EntityPropertyForm : Form
    {
        public PropertyModel Model { get; set; }

        public EntityPropertyForm()
        {
            InitializeComponent();
            Model = new PropertyModel();
        }

        private void ComboBox1_SelectedValueChanged(object sender, EventArgs e)
        {
            if (comboBox1.Text == "string")
            {
                textBox2.Enabled = true;
            }
            else
            {
                textBox2.Enabled = false;
            }
        }

        private void Button2_Click(object sender, EventArgs e)
        {
            int size = 0;

            if (textBox1.Text.Length <= 0)
            {
                MessageBox.Show("Name is requerid!");
                return;
            }

            if (textBox2.Text.Length > 0 && !int.TryParse(textBox2.Text, out size))
            {
                MessageBox.Show("Size is invalid");
                return;
            }

            if (comboBox1.Text.Length == 0)
            {
                MessageBox.Show("Type is required");
                return;
            }

            if (!comboBox1.Text.Equals("string", StringComparison.CurrentCultureIgnoreCase) && size > 0)
            {
                MessageBox.Show("Size is only for string type");
                return;
            }

            Model.Name = textBox1.Text;
            Model.Type = comboBox1.Text;
            Model.Size = size;
            Model.IsRequired = checkBox1.Checked;
            
            Close();
        }
    }
}
