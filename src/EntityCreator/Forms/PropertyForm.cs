using EntityCreator.Models;

namespace EntityCreator;

public partial class PropertyForm : Form
{
    public PropertyModel Model { get; set; }

    public PropertyForm()
    {
        InitializeComponent();
        Model = new PropertyModel();
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

        if (comboBox1.Text == "Entity" || comboBox1.Text == "ValueObject")
        {
            if (Model.Properties == null || Model.Properties.Count == 0)
            {
                MessageBox.Show("Properties are required");
                return;
            }
        }

        Model.Name = textBox1.Text;
        Model.Type = comboBox1.Text;
        Model.Size = size;
        Model.IsRequired = checkBox1.Checked;
        Model.IsCollection = checkBox2.Checked;
        
        Close();
    }

    private void ComboBox1_SelectedValueChanged(object sender, EventArgs e)
    {
        label3.Visible = (comboBox1.Text == "string");
        textBox2.Visible = (comboBox1.Text == "string");
        
        checkBox2.Visible = (comboBox1.Text == "Entity" || comboBox1.Text == "ValueObject");
        label4.Visible = (comboBox1.Text == "Entity" || comboBox1.Text == "ValueObject");
        button3.Visible = (comboBox1.Text == "Entity" || comboBox1.Text == "ValueObject");
        button4.Visible = (comboBox1.Text == "Entity" || comboBox1.Text == "ValueObject");
        listBox1.Visible = (comboBox1.Text == "Entity" || comboBox1.Text == "ValueObject");
    }

    private void Button3_Click(object sender, EventArgs e)
    {
        EntityPropertyForm propertyForm = new EntityPropertyForm();
        propertyForm.ShowDialog();

        var model = propertyForm.Model;

        if (string.IsNullOrEmpty(model.Name))
            return;

        model.Index = (Model.Properties?.Count ?? 0) + 1;

        string item = model.Name + "<" + model.Type + ">";

        if (model.Size > 0)
            item += "(" + model.Size + ")";

        if (Model.Properties == null)
            Model.Properties = [];

        Model.Properties.Add(model);

        listBox1.Items.Add(item);
    }
}
