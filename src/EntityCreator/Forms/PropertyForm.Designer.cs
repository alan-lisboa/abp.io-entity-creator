namespace EntityCreator
{
    partial class PropertyForm
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
            button2 = new Button();
            textBox1 = new TextBox();
            label1 = new Label();
            comboBox1 = new ComboBox();
            label2 = new Label();
            textBox2 = new TextBox();
            label3 = new Label();
            checkBox1 = new CheckBox();
            checkBox2 = new CheckBox();
            label4 = new Label();
            button4 = new Button();
            button3 = new Button();
            listBox1 = new ListBox();
            label5 = new Label();
            SuspendLayout();
            // 
            // button2
            // 
            button2.Location = new Point(400, 385);
            button2.Name = "button2";
            button2.Size = new Size(75, 27);
            button2.TabIndex = 8;
            button2.Text = "Add";
            button2.UseVisualStyleBackColor = true;
            button2.Click += Button2_Click;
            // 
            // textBox1
            // 
            textBox1.Location = new Point(12, 39);
            textBox1.Name = "textBox1";
            textBox1.Size = new Size(256, 23);
            textBox1.TabIndex = 10;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(12, 21);
            label1.Name = "label1";
            label1.Size = new Size(39, 15);
            label1.TabIndex = 9;
            label1.Text = "Name";
            // 
            // comboBox1
            // 
            comboBox1.DropDownStyle = ComboBoxStyle.DropDownList;
            comboBox1.FormattingEnabled = true;
            comboBox1.Items.AddRange(new object[] { "string", "int", "long", "Guid", "decimal", "float", "double", "DateTime", "TimeSpan", "Entity", "ValueObject", "AggregatedRoot" });
            comboBox1.Location = new Point(12, 95);
            comboBox1.Name = "comboBox1";
            comboBox1.Size = new Size(256, 23);
            comboBox1.TabIndex = 11;
            comboBox1.SelectedValueChanged += ComboBox1_SelectedValueChanged;
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new Point(12, 77);
            label2.Name = "label2";
            label2.Size = new Size(32, 15);
            label2.TabIndex = 12;
            label2.Text = "Type";
            // 
            // textBox2
            // 
            textBox2.Location = new Point(285, 95);
            textBox2.Name = "textBox2";
            textBox2.Size = new Size(114, 23);
            textBox2.TabIndex = 14;
            textBox2.TextAlign = HorizontalAlignment.Right;
            textBox2.Visible = false;
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Location = new Point(285, 77);
            label3.Name = "label3";
            label3.Size = new Size(27, 15);
            label3.TabIndex = 13;
            label3.Text = "Size";
            label3.Visible = false;
            // 
            // checkBox1
            // 
            checkBox1.AutoSize = true;
            checkBox1.Location = new Point(12, 141);
            checkBox1.Name = "checkBox1";
            checkBox1.Size = new Size(84, 19);
            checkBox1.TabIndex = 15;
            checkBox1.Text = "Is Required";
            checkBox1.UseVisualStyleBackColor = true;
            // 
            // checkBox2
            // 
            checkBox2.AutoSize = true;
            checkBox2.Location = new Point(146, 141);
            checkBox2.Name = "checkBox2";
            checkBox2.Size = new Size(91, 19);
            checkBox2.TabIndex = 16;
            checkBox2.Text = "Is Collection";
            checkBox2.UseVisualStyleBackColor = true;
            checkBox2.Visible = false;
            // 
            // label4
            // 
            label4.AutoSize = true;
            label4.Location = new Point(12, 189);
            label4.Name = "label4";
            label4.Size = new Size(60, 15);
            label4.TabIndex = 20;
            label4.Text = "Properties";
            label4.Visible = false;
            // 
            // button4
            // 
            button4.Location = new Point(412, 176);
            button4.Name = "button4";
            button4.Size = new Size(29, 28);
            button4.TabIndex = 19;
            button4.Text = "-";
            button4.UseVisualStyleBackColor = true;
            button4.Visible = false;
            // 
            // button3
            // 
            button3.Location = new Point(447, 176);
            button3.Name = "button3";
            button3.Size = new Size(29, 28);
            button3.TabIndex = 18;
            button3.Text = "+";
            button3.UseVisualStyleBackColor = true;
            button3.Visible = false;
            button3.Click += Button3_Click;
            // 
            // listBox1
            // 
            listBox1.Font = new Font("Courier New", 9.75F, FontStyle.Regular, GraphicsUnit.Point, 0);
            listBox1.FormattingEnabled = true;
            listBox1.Location = new Point(12, 210);
            listBox1.Name = "listBox1";
            listBox1.Size = new Size(463, 116);
            listBox1.TabIndex = 17;
            listBox1.Visible = false;
            // 
            // label5
            // 
            label5.AutoSize = true;
            label5.Location = new Point(12, 391);
            label5.Name = "label5";
            label5.Size = new Size(66, 15);
            label5.TabIndex = 21;
            label5.Text = "* Never plu";
            // 
            // PropertyForm
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(492, 432);
            Controls.Add(label5);
            Controls.Add(label4);
            Controls.Add(button4);
            Controls.Add(button3);
            Controls.Add(listBox1);
            Controls.Add(checkBox2);
            Controls.Add(checkBox1);
            Controls.Add(textBox2);
            Controls.Add(label3);
            Controls.Add(label2);
            Controls.Add(comboBox1);
            Controls.Add(textBox1);
            Controls.Add(label1);
            Controls.Add(button2);
            FormBorderStyle = FormBorderStyle.FixedSingle;
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "PropertyForm";
            Text = "Add Property";
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Button button2;
        private TextBox textBox1;
        private Label label1;
        private ComboBox comboBox1;
        private Label label2;
        private TextBox textBox2;
        private Label label3;
        private CheckBox checkBox1;
        private CheckBox checkBox2;
        private Label label4;
        private Button button4;
        private Button button3;
        private ListBox listBox1;
        private Label label5;
    }
}