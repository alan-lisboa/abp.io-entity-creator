using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace EntityCreator
{
    public partial class PropertyForm : Form
    {
        public PropertyModel Model { get; set; }

        public PropertyForm()
        {
            InitializeComponent();
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

            Model = new PropertyModel 
            { 
                Name = textBox1.Text, 
                Type = comboBox1.Text, 
                Size = size,
                IsRequired = checkBox1.Checked
            };
            
            Close();
        }
    }
}
