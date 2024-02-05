using CustomComboBox.Controls;

namespace WinFormsLibrary1
{
    partial class Form1
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
            backgroundWorker1 = new System.ComponentModel.BackgroundWorker();
            comboBox1 = new GroupedComboBox.GroupedComboBox();
            label1 = new Label();
            label2 = new Label();
            backgroundWorker2 = new System.ComponentModel.BackgroundWorker();
            groupBox1 = new GroupBox();
            modernuiGroupedComboBox1 = new CustomGroupedComboBox();
            label3 = new Label();
            groupedComboBox21 = new GroupedComboBox2.TesteGroupedComboBox();
            groupBox1.SuspendLayout();
            SuspendLayout();
            // 
            // comboBox1
            // 
            comboBox1.DataSource = null;
            comboBox1.DropDownHeight = 706;
            comboBox1.DropDownStyle = ComboBoxStyle.DropDownList;
            comboBox1.DropDownWidth = 331;
            comboBox1.ForeColor = Color.DimGray;
            comboBox1.FormattingEnabled = true;
            comboBox1.IntegralHeight = false;
            comboBox1.Location = new Point(6, 45);
            comboBox1.Name = "comboBox1";
            comboBox1.Size = new Size(144, 24);
            comboBox1.TabIndex = 2;
            comboBox1.TabStop = false;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(6, 19);
            label1.Name = "label1";
            label1.Size = new Size(166, 15);
            label1.TabIndex = 2;
            label1.Text = "Standard Grouped ComboBox";
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.ForeColor = Color.DimGray;
            label2.Location = new Point(223, 19);
            label2.Name = "label2";
            label2.Size = new Size(161, 15);
            label2.TabIndex = 3;
            label2.Text = "Custom Grouped ComboBox";
            label2.Click += label2_Click;
            // 
            // groupBox1
            // 
            groupBox1.BackColor = Color.White;
            groupBox1.Controls.Add(modernuiGroupedComboBox1);
            groupBox1.Controls.Add(label3);
            groupBox1.Controls.Add(groupedComboBox21);
            groupBox1.Controls.Add(comboBox1);
            groupBox1.Controls.Add(label1);
            groupBox1.Controls.Add(label2);
            groupBox1.ForeColor = Color.Black;
            groupBox1.Location = new Point(12, 12);
            groupBox1.Name = "groupBox1";
            groupBox1.Size = new Size(776, 426);
            groupBox1.TabIndex = 4;
            groupBox1.TabStop = false;
            groupBox1.Text = "groupBox1";
            // 
            // modernuiGroupedComboBox1
            // 
            modernuiGroupedComboBox1.BackColor = Color.White;
            modernuiGroupedComboBox1.BorderColor = Color.FromArgb(127, 1, 1);
            modernuiGroupedComboBox1.BorderSize = 1;
            modernuiGroupedComboBox1.DropDownStyle = ComboBoxStyle.DropDown;
            modernuiGroupedComboBox1.Font = new Font("Segoe UI", 10F);
            modernuiGroupedComboBox1.ForeColor = Color.DimGray;
            modernuiGroupedComboBox1.GroupMember = "";
            modernuiGroupedComboBox1.IconColor = Color.FromArgb(127, 1, 1);
            modernuiGroupedComboBox1.ListBackColor = Color.FromArgb(230, 228, 245);
            modernuiGroupedComboBox1.ListTextColor = Color.DimGray;
            modernuiGroupedComboBox1.Location = new Point(185, 46);
            modernuiGroupedComboBox1.MinimumSize = new Size(200, 30);
            modernuiGroupedComboBox1.Name = "modernuiGroupedComboBox1";
            modernuiGroupedComboBox1.Padding = new Padding(1);
            modernuiGroupedComboBox1.Size = new Size(355, 30);
            modernuiGroupedComboBox1.TabIndex = 6;
            modernuiGroupedComboBox1.Texts = "";
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Location = new Point(568, 19);
            label3.Name = "label3";
            label3.Size = new Size(142, 15);
            label3.TabIndex = 5;
            label3.Text = "GroupedComboBox Teste";
            // 
            // groupedComboBox21
            // 
            groupedComboBox21.DrawMode = DrawMode.OwnerDrawVariable;
            groupedComboBox21.FormattingEnabled = true;
            groupedComboBox21.GroupMember = "";
            groupedComboBox21.Location = new Point(568, 46);
            groupedComboBox21.Name = "groupedComboBox21";
            groupedComboBox21.Size = new Size(149, 24);
            groupedComboBox21.TabIndex = 4;
            groupedComboBox21.SelectedIndexChanged += groupedComboBox21_SelectedIndexChanged;
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.Pink;
            ClientSize = new Size(800, 450);
            Controls.Add(groupBox1);
            Name = "Form1";
            Text = "Form1";
            groupBox1.ResumeLayout(false);
            groupBox1.PerformLayout();
            ResumeLayout(false);
        }

        #endregion

        private GroupedComboBox.GroupedComboBox comboBox1;
        private System.ComponentModel.BackgroundWorker backgroundWorker1;
        private Label label1;
        private Label label2;
        private System.ComponentModel.BackgroundWorker backgroundWorker2;
        private GroupBox groupBox1;
        private Label label3;
        private GroupedComboBox2.TesteGroupedComboBox groupedComboBox21;
        private CustomComboBox.Controls.CustomGroupedComboBox Ui;
        private CustomComboBox.Controls.CustomGroupedComboBox modernuiGroupedComboBox1;
    }
}