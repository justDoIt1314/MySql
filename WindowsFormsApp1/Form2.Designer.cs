namespace WindowsFormsApp1
{
    partial class Form2
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
            this.tabControl2 = new System.Windows.Forms.TabControl();
            this.tabPage3 = new System.Windows.Forms.TabPage();
            this.group_stu = new System.Windows.Forms.GroupBox();
            this.Stu_Sno = new System.Windows.Forms.TextBox();
            this.label63 = new System.Windows.Forms.Label();
            this.Stu_Load = new System.Windows.Forms.Button();
            this.label64 = new System.Windows.Forms.Label();
            this.Stu_Password = new System.Windows.Forms.TextBox();
            this.stu_tag = new System.Windows.Forms.Label();
            this.tabPage4 = new System.Windows.Forms.TabPage();
            this.Tea_no = new System.Windows.Forms.TextBox();
            this.Tea_tag = new System.Windows.Forms.Label();
            this.label90 = new System.Windows.Forms.Label();
            this.Tea_Password = new System.Windows.Forms.TextBox();
            this.Tea_Load = new System.Windows.Forms.Button();
            this.label97 = new System.Windows.Forms.Label();
            this.tabPage5 = new System.Windows.Forms.TabPage();
            this.Admin_Load = new System.Windows.Forms.Button();
            this.Admin_no = new System.Windows.Forms.TextBox();
            this.Admin_Password = new System.Windows.Forms.TextBox();
            this.label66 = new System.Windows.Forms.Label();
            this.label65 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.tabControl2.SuspendLayout();
            this.tabPage3.SuspendLayout();
            this.group_stu.SuspendLayout();
            this.tabPage4.SuspendLayout();
            this.tabPage5.SuspendLayout();
            this.SuspendLayout();
            // 
            // tabControl2
            // 
            this.tabControl2.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tabControl2.Controls.Add(this.tabPage3);
            this.tabControl2.Controls.Add(this.tabPage4);
            this.tabControl2.Controls.Add(this.tabPage5);
            this.tabControl2.Location = new System.Drawing.Point(426, 195);
            this.tabControl2.Margin = new System.Windows.Forms.Padding(4);
            this.tabControl2.Name = "tabControl2";
            this.tabControl2.SelectedIndex = 0;
            this.tabControl2.Size = new System.Drawing.Size(319, 159);
            this.tabControl2.TabIndex = 26;
            // 
            // tabPage3
            // 
            this.tabPage3.BackColor = System.Drawing.Color.Transparent;
            this.tabPage3.BackgroundImage = global::WindowsFormsApp1.Properties.Resources.pc04;
            this.tabPage3.Controls.Add(this.group_stu);
            this.tabPage3.Controls.Add(this.stu_tag);
            this.tabPage3.ForeColor = System.Drawing.SystemColors.ControlText;
            this.tabPage3.Location = new System.Drawing.Point(4, 25);
            this.tabPage3.Margin = new System.Windows.Forms.Padding(4);
            this.tabPage3.Name = "tabPage3";
            this.tabPage3.Padding = new System.Windows.Forms.Padding(4);
            this.tabPage3.Size = new System.Drawing.Size(311, 130);
            this.tabPage3.TabIndex = 0;
            this.tabPage3.Text = "学生";
            // 
            // group_stu
            // 
            this.group_stu.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.group_stu.BackgroundImage = global::WindowsFormsApp1.Properties.Resources.pc04;
            this.group_stu.Controls.Add(this.Stu_Sno);
            this.group_stu.Controls.Add(this.label63);
            this.group_stu.Controls.Add(this.Stu_Load);
            this.group_stu.Controls.Add(this.label64);
            this.group_stu.Controls.Add(this.Stu_Password);
            this.group_stu.Location = new System.Drawing.Point(2, 1);
            this.group_stu.Margin = new System.Windows.Forms.Padding(4);
            this.group_stu.Name = "group_stu";
            this.group_stu.Padding = new System.Windows.Forms.Padding(4);
            this.group_stu.Size = new System.Drawing.Size(301, 121);
            this.group_stu.TabIndex = 31;
            this.group_stu.TabStop = false;
            // 
            // Stu_Sno
            // 
            this.Stu_Sno.BackColor = System.Drawing.Color.Aqua;
            this.Stu_Sno.Location = new System.Drawing.Point(83, 12);
            this.Stu_Sno.Margin = new System.Windows.Forms.Padding(4);
            this.Stu_Sno.Name = "Stu_Sno";
            this.Stu_Sno.Size = new System.Drawing.Size(201, 25);
            this.Stu_Sno.TabIndex = 0;
            this.Stu_Sno.TextChanged += new System.EventHandler(this.Stu_Sno_TextChanged);
            // 
            // label63
            // 
            this.label63.AutoSize = true;
            this.label63.Font = new System.Drawing.Font("宋体", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label63.ForeColor = System.Drawing.SystemColors.HotTrack;
            this.label63.Location = new System.Drawing.Point(7, 17);
            this.label63.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label63.Name = "label63";
            this.label63.Size = new System.Drawing.Size(72, 20);
            this.label63.TabIndex = 11;
            this.label63.Text = "学号：";
            // 
            // Stu_Load
            // 
            this.Stu_Load.BackColor = System.Drawing.SystemColors.Highlight;
            this.Stu_Load.Location = new System.Drawing.Point(7, 75);
            this.Stu_Load.Margin = new System.Windows.Forms.Padding(4);
            this.Stu_Load.Name = "Stu_Load";
            this.Stu_Load.Size = new System.Drawing.Size(100, 29);
            this.Stu_Load.TabIndex = 25;
            this.Stu_Load.Tag = "Stu";
            this.Stu_Load.Text = "登录";
            this.Stu_Load.UseVisualStyleBackColor = false;
            this.Stu_Load.Click += new System.EventHandler(this.Stu_Load_Click);
            // 
            // label64
            // 
            this.label64.AutoSize = true;
            this.label64.Font = new System.Drawing.Font("宋体", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label64.ForeColor = System.Drawing.SystemColors.HotTrack;
            this.label64.Location = new System.Drawing.Point(7, 48);
            this.label64.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label64.Name = "label64";
            this.label64.Size = new System.Drawing.Size(72, 20);
            this.label64.TabIndex = 15;
            this.label64.Text = "密码：";
            // 
            // Stu_Password
            // 
            this.Stu_Password.BackColor = System.Drawing.Color.Aqua;
            this.Stu_Password.Location = new System.Drawing.Point(83, 48);
            this.Stu_Password.Margin = new System.Windows.Forms.Padding(4);
            this.Stu_Password.Name = "Stu_Password";
            this.Stu_Password.PasswordChar = '*';
            this.Stu_Password.Size = new System.Drawing.Size(201, 25);
            this.Stu_Password.TabIndex = 1;
            // 
            // stu_tag
            // 
            this.stu_tag.AutoSize = true;
            this.stu_tag.Location = new System.Drawing.Point(277, 15);
            this.stu_tag.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.stu_tag.Name = "stu_tag";
            this.stu_tag.Size = new System.Drawing.Size(23, 15);
            this.stu_tag.TabIndex = 87;
            this.stu_tag.Text = "01";
            this.stu_tag.Visible = false;
            // 
            // tabPage4
            // 
            this.tabPage4.BackgroundImage = global::WindowsFormsApp1.Properties.Resources.pc04;
            this.tabPage4.Controls.Add(this.Tea_no);
            this.tabPage4.Controls.Add(this.Tea_tag);
            this.tabPage4.Controls.Add(this.label90);
            this.tabPage4.Controls.Add(this.Tea_Password);
            this.tabPage4.Controls.Add(this.Tea_Load);
            this.tabPage4.Controls.Add(this.label97);
            this.tabPage4.Location = new System.Drawing.Point(4, 25);
            this.tabPage4.Margin = new System.Windows.Forms.Padding(4);
            this.tabPage4.Name = "tabPage4";
            this.tabPage4.Padding = new System.Windows.Forms.Padding(4);
            this.tabPage4.Size = new System.Drawing.Size(311, 130);
            this.tabPage4.TabIndex = 1;
            this.tabPage4.Text = "老师";
            this.tabPage4.UseVisualStyleBackColor = true;
            // 
            // Tea_no
            // 
            this.Tea_no.BackColor = System.Drawing.Color.Aqua;
            this.Tea_no.Location = new System.Drawing.Point(88, 14);
            this.Tea_no.Margin = new System.Windows.Forms.Padding(4);
            this.Tea_no.Name = "Tea_no";
            this.Tea_no.Size = new System.Drawing.Size(201, 25);
            this.Tea_no.TabIndex = 0;
            // 
            // Tea_tag
            // 
            this.Tea_tag.AutoSize = true;
            this.Tea_tag.Location = new System.Drawing.Point(216, 15);
            this.Tea_tag.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.Tea_tag.Name = "Tea_tag";
            this.Tea_tag.Size = new System.Drawing.Size(23, 15);
            this.Tea_tag.TabIndex = 87;
            this.Tea_tag.Text = "01";
            this.Tea_tag.Visible = false;
            // 
            // label90
            // 
            this.label90.AutoSize = true;
            this.label90.Font = new System.Drawing.Font("宋体", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label90.ForeColor = System.Drawing.SystemColors.HotTrack;
            this.label90.Location = new System.Drawing.Point(12, 19);
            this.label90.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label90.Name = "label90";
            this.label90.Size = new System.Drawing.Size(72, 20);
            this.label90.TabIndex = 11;
            this.label90.Text = "工号：";
            // 
            // Tea_Password
            // 
            this.Tea_Password.BackColor = System.Drawing.Color.Aqua;
            this.Tea_Password.Location = new System.Drawing.Point(88, 48);
            this.Tea_Password.Margin = new System.Windows.Forms.Padding(4);
            this.Tea_Password.Name = "Tea_Password";
            this.Tea_Password.PasswordChar = '*';
            this.Tea_Password.Size = new System.Drawing.Size(201, 25);
            this.Tea_Password.TabIndex = 1;
            // 
            // Tea_Load
            // 
            this.Tea_Load.BackColor = System.Drawing.SystemColors.Highlight;
            this.Tea_Load.Location = new System.Drawing.Point(16, 78);
            this.Tea_Load.Margin = new System.Windows.Forms.Padding(4);
            this.Tea_Load.Name = "Tea_Load";
            this.Tea_Load.Size = new System.Drawing.Size(100, 29);
            this.Tea_Load.TabIndex = 25;
            this.Tea_Load.Tag = "Stu";
            this.Tea_Load.Text = "登录";
            this.Tea_Load.UseVisualStyleBackColor = false;
            this.Tea_Load.Click += new System.EventHandler(this.Tea_Load_Click);
            // 
            // label97
            // 
            this.label97.AutoSize = true;
            this.label97.Font = new System.Drawing.Font("宋体", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label97.ForeColor = System.Drawing.SystemColors.HotTrack;
            this.label97.Location = new System.Drawing.Point(12, 54);
            this.label97.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label97.Name = "label97";
            this.label97.Size = new System.Drawing.Size(72, 20);
            this.label97.TabIndex = 15;
            this.label97.Text = "密码：";
            // 
            // tabPage5
            // 
            this.tabPage5.BackgroundImage = global::WindowsFormsApp1.Properties.Resources.pc04;
            this.tabPage5.Controls.Add(this.Admin_Load);
            this.tabPage5.Controls.Add(this.Admin_no);
            this.tabPage5.Controls.Add(this.Admin_Password);
            this.tabPage5.Controls.Add(this.label66);
            this.tabPage5.Controls.Add(this.label65);
            this.tabPage5.Location = new System.Drawing.Point(4, 25);
            this.tabPage5.Margin = new System.Windows.Forms.Padding(4);
            this.tabPage5.Name = "tabPage5";
            this.tabPage5.Padding = new System.Windows.Forms.Padding(4);
            this.tabPage5.Size = new System.Drawing.Size(311, 130);
            this.tabPage5.TabIndex = 2;
            this.tabPage5.Text = "管理员";
            this.tabPage5.UseVisualStyleBackColor = true;
            // 
            // Admin_Load
            // 
            this.Admin_Load.BackColor = System.Drawing.SystemColors.Highlight;
            this.Admin_Load.Location = new System.Drawing.Point(8, 87);
            this.Admin_Load.Margin = new System.Windows.Forms.Padding(4);
            this.Admin_Load.Name = "Admin_Load";
            this.Admin_Load.Size = new System.Drawing.Size(100, 29);
            this.Admin_Load.TabIndex = 25;
            this.Admin_Load.Tag = "Stu";
            this.Admin_Load.Text = "登录";
            this.Admin_Load.UseVisualStyleBackColor = false;
            this.Admin_Load.Click += new System.EventHandler(this.Admin_Load_Click);
            // 
            // Admin_no
            // 
            this.Admin_no.BackColor = System.Drawing.Color.Aqua;
            this.Admin_no.Location = new System.Drawing.Point(88, 16);
            this.Admin_no.Margin = new System.Windows.Forms.Padding(4);
            this.Admin_no.Name = "Admin_no";
            this.Admin_no.Size = new System.Drawing.Size(201, 25);
            this.Admin_no.TabIndex = 0;
            // 
            // Admin_Password
            // 
            this.Admin_Password.BackColor = System.Drawing.Color.Aqua;
            this.Admin_Password.Location = new System.Drawing.Point(88, 49);
            this.Admin_Password.Margin = new System.Windows.Forms.Padding(4);
            this.Admin_Password.Name = "Admin_Password";
            this.Admin_Password.PasswordChar = '*';
            this.Admin_Password.Size = new System.Drawing.Size(201, 25);
            this.Admin_Password.TabIndex = 1;
            // 
            // label66
            // 
            this.label66.AutoSize = true;
            this.label66.Font = new System.Drawing.Font("宋体", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label66.ForeColor = System.Drawing.SystemColors.HotTrack;
            this.label66.Location = new System.Drawing.Point(8, 54);
            this.label66.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label66.Name = "label66";
            this.label66.Size = new System.Drawing.Size(72, 20);
            this.label66.TabIndex = 15;
            this.label66.Text = "密码：";
            // 
            // label65
            // 
            this.label65.AutoSize = true;
            this.label65.Font = new System.Drawing.Font("宋体", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label65.ForeColor = System.Drawing.SystemColors.HotTrack;
            this.label65.Location = new System.Drawing.Point(8, 21);
            this.label65.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label65.Name = "label65";
            this.label65.Size = new System.Drawing.Size(72, 20);
            this.label65.TabIndex = 11;
            this.label65.Text = "工号：";
            // 
            // label1
            // 
            this.label1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("华文行楷", 22.2F, ((System.Drawing.FontStyle)((System.Drawing.FontStyle.Bold | System.Drawing.FontStyle.Italic))), System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label1.ForeColor = System.Drawing.Color.DarkViolet;
            this.label1.Location = new System.Drawing.Point(447, 80);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(290, 40);
            this.label1.TabIndex = 27;
            this.label1.Text = "实验室管理系统";
            // 
            // Form2
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.Highlight;
            this.BackgroundImage = global::WindowsFormsApp1.Properties.Resources.pc02;
            this.ClientSize = new System.Drawing.Size(1144, 595);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.tabControl2);
            this.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.Name = "Form2";
            this.ShowInTaskbar = false;
            this.Text = "系统登录";
            this.tabControl2.ResumeLayout(false);
            this.tabPage3.ResumeLayout(false);
            this.tabPage3.PerformLayout();
            this.group_stu.ResumeLayout(false);
            this.group_stu.PerformLayout();
            this.tabPage4.ResumeLayout(false);
            this.tabPage4.PerformLayout();
            this.tabPage5.ResumeLayout(false);
            this.tabPage5.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TabControl tabControl2;
        private System.Windows.Forms.TabPage tabPage3;
        private System.Windows.Forms.GroupBox group_stu;
        private System.Windows.Forms.TextBox Stu_Sno;
        private System.Windows.Forms.Label label63;
        private System.Windows.Forms.Button Stu_Load;
        private System.Windows.Forms.Label label64;
        private System.Windows.Forms.TextBox Stu_Password;
        private System.Windows.Forms.Label stu_tag;
        private System.Windows.Forms.TabPage tabPage4;
        private System.Windows.Forms.TextBox Tea_no;
        private System.Windows.Forms.Label label90;
        private System.Windows.Forms.Button Tea_Load;
        private System.Windows.Forms.Label label97;
        private System.Windows.Forms.TextBox Tea_Password;
        private System.Windows.Forms.Label Tea_tag;
        private System.Windows.Forms.TabPage tabPage5;
        private System.Windows.Forms.Button Admin_Load;
        private System.Windows.Forms.TextBox Admin_no;
        private System.Windows.Forms.TextBox Admin_Password;
        private System.Windows.Forms.Label label66;
        private System.Windows.Forms.Label label65;
        private System.Windows.Forms.Label label1;
    }
}