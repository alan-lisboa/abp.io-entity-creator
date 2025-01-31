namespace EntityCreator
{
    partial class EntityPropertyForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            checkBox1 = new CheckBox();
            textBox2 = new TextBox();
            label3 = new Label();
            label2 = new Label();
            comboBox1 = new ComboBox();
            textBox1 = new TextBox();
            label1 = new Label();
            button2 = new Button();
            SuspendLayout();
            // 
            // checkBox1
            // 
            checkBox1.AutoSize = true;
            checkBox1.Location = new Point(12, 188);
            checkBox1.Name = "checkBox1";
            checkBox1.Size = new Size(84, 19);
            checkBox1.TabIndex = 22;
            checkBox1.Text = "Is Required";
            checkBox1.UseVisualStyleBackColor = true;
            // 
            // textBox2
            // 
            textBox2.Location = new Point(12, 144);
            textBox2.Name = "textBox2";
            textBox2.Size = new Size(256, 23);
            textBox2.TabIndex = 21;
            textBox2.TextAlign = HorizontalAlignment.Right;
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Location = new Point(12, 126);
            label3.Name = "label3";
            label3.Size = new Size(27, 15);
            label3.TabIndex = 20;
            label3.Text = "Size";
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new Point(12, 73);
            label2.Name = "label2";
            label2.Size = new Size(32, 15);
            label2.TabIndex = 19;
            label2.Text = "Type";
            // 
            // comboBox1
            // 
            comboBox1.DropDownStyle = ComboBoxStyle.DropDownList;
            comboBox1.FormattingEnabled = true;
            comboBox1.Items.AddRange(new object[] { "string", "int", "long", "Guid", "decimal", "float", "double", "DateTime", "TimeSpan" });
            comboBox1.Location = new Point(12, 91);
            comboBox1.Name = "comboBox1";
            comboBox1.Size = new Size(256, 23);
            comboBox1.TabIndex = 18;
            comboBox1.SelectedValueChanged += ComboBox1_SelectedValueChanged;
            // 
            // textBox1
            // 
            textBox1.Location = new Point(12, 35);
            textBox1.Name = "textBox1";
            textBox1.Size = new Size(256, 23);
            textBox1.TabIndex = 17;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(12, 17);
            label1.Name = "label1";
            label1.Size = new Size(39, 15);
            label1.TabIndex = 16;
            label1.Text = "Name";
            // 
            // button2
            // 
            button2.Location = new Point(297, 276);
            button2.Name = "button2";
            button2.Size = new Size(75, 27);
            button2.TabIndex = 23;
            button2.Text = "Add";
            button2.UseVisualStyleBackColor = true;
            button2.Click += Button2_Click;
            // 
            // EntityPropertyForm
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(384, 315);
            Controls.Add(button2);
            Controls.Add(checkBox1);
            Controls.Add(textBox2);
            Controls.Add(label3);
            Controls.Add(label2);
            Controls.Add(comboBox1);
            Controls.Add(textBox1);
            Controls.Add(label1);
            Name = "EntityPropertyForm";
            Text = "Add Property";
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private CheckBox checkBox1;
        private TextBox textBox2;
        private Label label3;
        private Label label2;
        private ComboBox comboBox1;
        private TextBox textBox1;
        private Label label1;
        private Button button2;
    }
}