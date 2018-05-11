namespace WindowsFormsApp1
{
    partial class Teacher_Form
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
            this.tea_Interface = new System.Windows.Forms.GroupBox();
            this.Tea_Clock = new System.Windows.Forms.Button();
            this.Exit_Tea = new System.Windows.Forms.Button();
            this.label75 = new System.Windows.Forms.Label();
            this.Tea_New_Again = new System.Windows.Forms.TextBox();
            this.Tea_Start = new System.Windows.Forms.TextBox();
            this.label72 = new System.Windows.Forms.Label();
            this.label78 = new System.Windows.Forms.Label();
            this.Tea_End = new System.Windows.Forms.TextBox();
            this.Tea_Confirm = new System.Windows.Forms.Button();
            this.label79 = new System.Windows.Forms.Label();
            this.label73 = new System.Windows.Forms.Label();
            this.Tea_New_Password = new System.Windows.Forms.TextBox();
            this.label74 = new System.Windows.Forms.Label();
            this.label76 = new System.Windows.Forms.Label();
            this.Tea_Find = new System.Windows.Forms.Button();
            this.label77 = new System.Windows.Forms.Label();
            this.label82 = new System.Windows.Forms.Label();
            this.tea_Interface.SuspendLayout();
            this.SuspendLayout();
            // 
            // tea_Interface
            // 
            this.tea_Interface.Controls.Add(this.Tea_Clock);
            this.tea_Interface.Controls.Add(this.Exit_Tea);
            this.tea_Interface.Controls.Add(this.label75);
            this.tea_Interface.Controls.Add(this.Tea_New_Again);
            this.tea_Interface.Controls.Add(this.Tea_Start);
            this.tea_Interface.Controls.Add(this.label72);
            this.tea_Interface.Controls.Add(this.label78);
            this.tea_Interface.Controls.Add(this.Tea_End);
            this.tea_Interface.Controls.Add(this.Tea_Confirm);
            this.tea_Interface.Controls.Add(this.label79);
            this.tea_Interface.Controls.Add(this.label73);
            this.tea_Interface.Controls.Add(this.Tea_New_Password);
            this.tea_Interface.Controls.Add(this.label74);
            this.tea_Interface.Controls.Add(this.label76);
            this.tea_Interface.Controls.Add(this.Tea_Find);
            this.tea_Interface.Controls.Add(this.label77);
            this.tea_Interface.Controls.Add(this.label82);
            this.tea_Interface.Location = new System.Drawing.Point(411, 26);
            this.tea_Interface.Name = "tea_Interface";
            this.tea_Interface.Size = new System.Drawing.Size(290, 504);
            this.tea_Interface.TabIndex = 32;
            this.tea_Interface.TabStop = false;
            this.tea_Interface.Text = "教师界面";
            this.tea_Interface.Visible = false;
            // 
            // Tea_Clock
            // 
            this.Tea_Clock.Location = new System.Drawing.Point(16, 85);
            this.Tea_Clock.Name = "Tea_Clock";
            this.Tea_Clock.Size = new System.Drawing.Size(75, 23);
            this.Tea_Clock.TabIndex = 36;
            this.Tea_Clock.Text = "考勤";
            this.Tea_Clock.UseVisualStyleBackColor = true;
            // 
            // Exit_Tea
            // 
            this.Exit_Tea.BackColor = System.Drawing.SystemColors.Info;
            this.Exit_Tea.Location = new System.Drawing.Point(22, 433);
            this.Exit_Tea.Name = "Exit_Tea";
            this.Exit_Tea.Size = new System.Drawing.Size(75, 23);
            this.Exit_Tea.TabIndex = 32;
            this.Exit_Tea.Tag = "Tea";
            this.Exit_Tea.Text = "退出";
            this.Exit_Tea.UseVisualStyleBackColor = false;
            this.Exit_Tea.Visible = false;
            // 
            // label75
            // 
            this.label75.AutoSize = true;
            this.label75.Font = new System.Drawing.Font("宋体", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label75.Location = new System.Drawing.Point(19, 54);
            this.label75.Name = "label75";
            this.label75.Size = new System.Drawing.Size(29, 20);
            this.label75.TabIndex = 31;
            this.label75.Text = "第";
            // 
            // Tea_New_Again
            // 
            this.Tea_New_Again.Location = new System.Drawing.Point(92, 355);
            this.Tea_New_Again.Name = "Tea_New_Again";
            this.Tea_New_Again.PasswordChar = '*';
            this.Tea_New_Again.Size = new System.Drawing.Size(100, 25);
            this.Tea_New_Again.TabIndex = 30;
            // 
            // Tea_Start
            // 
            this.Tea_Start.Location = new System.Drawing.Point(54, 51);
            this.Tea_Start.Name = "Tea_Start";
            this.Tea_Start.Size = new System.Drawing.Size(57, 25);
            this.Tea_Start.TabIndex = 0;
            // 
            // label72
            // 
            this.label72.AutoSize = true;
            this.label72.Font = new System.Drawing.Font("宋体", 12F);
            this.label72.Location = new System.Drawing.Point(15, 358);
            this.label72.Name = "label72";
            this.label72.Size = new System.Drawing.Size(109, 20);
            this.label72.TabIndex = 29;
            this.label72.Text = "再次输入：";
            // 
            // label78
            // 
            this.label78.AutoSize = true;
            this.label78.Location = new System.Drawing.Point(117, 56);
            this.label78.Name = "label78";
            this.label78.Size = new System.Drawing.Size(15, 15);
            this.label78.TabIndex = 33;
            this.label78.Text = "-";
            // 
            // Tea_End
            // 
            this.Tea_End.Location = new System.Drawing.Point(138, 51);
            this.Tea_End.Name = "Tea_End";
            this.Tea_End.Size = new System.Drawing.Size(58, 25);
            this.Tea_End.TabIndex = 1;
            // 
            // Tea_Confirm
            // 
            this.Tea_Confirm.Location = new System.Drawing.Point(18, 389);
            this.Tea_Confirm.Name = "Tea_Confirm";
            this.Tea_Confirm.Size = new System.Drawing.Size(75, 23);
            this.Tea_Confirm.TabIndex = 28;
            this.Tea_Confirm.Text = "确认";
            this.Tea_Confirm.UseVisualStyleBackColor = true;
            // 
            // label79
            // 
            this.label79.AutoSize = true;
            this.label79.Font = new System.Drawing.Font("宋体", 12F);
            this.label79.Location = new System.Drawing.Point(202, 54);
            this.label79.Name = "label79";
            this.label79.Size = new System.Drawing.Size(49, 20);
            this.label79.TabIndex = 35;
            this.label79.Text = "节课";
            // 
            // label73
            // 
            this.label73.AutoSize = true;
            this.label73.Font = new System.Drawing.Font("宋体", 12F);
            this.label73.Location = new System.Drawing.Point(16, 330);
            this.label73.Name = "label73";
            this.label73.Size = new System.Drawing.Size(89, 20);
            this.label73.TabIndex = 27;
            this.label73.Text = "新密码：";
            // 
            // Tea_New_Password
            // 
            this.Tea_New_Password.Location = new System.Drawing.Point(92, 325);
            this.Tea_New_Password.Name = "Tea_New_Password";
            this.Tea_New_Password.PasswordChar = '*';
            this.Tea_New_Password.Size = new System.Drawing.Size(100, 25);
            this.Tea_New_Password.TabIndex = 26;
            // 
            // label74
            // 
            this.label74.AutoSize = true;
            this.label74.BackColor = System.Drawing.SystemColors.GradientActiveCaption;
            this.label74.Location = new System.Drawing.Point(19, 300);
            this.label74.Name = "label74";
            this.label74.Size = new System.Drawing.Size(299, 15);
            this.label74.TabIndex = 25;
            this.label74.Text = "修改密码-----------------------------";
            // 
            // label76
            // 
            this.label76.AutoSize = true;
            this.label76.Location = new System.Drawing.Point(33, 26);
            this.label76.Name = "label76";
            this.label76.Size = new System.Drawing.Size(0, 15);
            this.label76.TabIndex = 2;
            // 
            // Tea_Find
            // 
            this.Tea_Find.BackColor = System.Drawing.SystemColors.Info;
            this.Tea_Find.Location = new System.Drawing.Point(18, 232);
            this.Tea_Find.Name = "Tea_Find";
            this.Tea_Find.Size = new System.Drawing.Size(75, 23);
            this.Tea_Find.TabIndex = 24;
            this.Tea_Find.Text = "查询";
            this.Tea_Find.UseVisualStyleBackColor = false;
            // 
            // label77
            // 
            this.label77.AutoSize = true;
            this.label77.BackColor = System.Drawing.SystemColors.GradientActiveCaption;
            this.label77.Location = new System.Drawing.Point(16, 193);
            this.label77.Name = "label77";
            this.label77.Size = new System.Drawing.Size(112, 15);
            this.label77.TabIndex = 20;
            this.label77.Text = "查询：学生名单";
            // 
            // label82
            // 
            this.label82.AutoSize = true;
            this.label82.BackColor = System.Drawing.SystemColors.GradientActiveCaption;
            this.label82.Location = new System.Drawing.Point(16, 25);
            this.label82.Name = "label82";
            this.label82.Size = new System.Drawing.Size(315, 15);
            this.label82.TabIndex = 9;
            this.label82.Text = "上机考勤-------------------------------";
            // 
            // Teacher_Form
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1088, 576);
            this.Controls.Add(this.tea_Interface);
            this.Name = "Teacher_Form";
            this.Text = "Teacher_Form";
            this.tea_Interface.ResumeLayout(false);
            this.tea_Interface.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox tea_Interface;
        private System.Windows.Forms.Button Tea_Clock;
        private System.Windows.Forms.Button Exit_Tea;
        private System.Windows.Forms.Label label75;
        private System.Windows.Forms.TextBox Tea_New_Again;
        private System.Windows.Forms.TextBox Tea_Start;
        private System.Windows.Forms.Label label72;
        private System.Windows.Forms.Label label78;
        private System.Windows.Forms.TextBox Tea_End;
        private System.Windows.Forms.Button Tea_Confirm;
        private System.Windows.Forms.Label label79;
        private System.Windows.Forms.Label label73;
        private System.Windows.Forms.TextBox Tea_New_Password;
        private System.Windows.Forms.Label label74;
        private System.Windows.Forms.Label label76;
        private System.Windows.Forms.Button Tea_Find;
        private System.Windows.Forms.Label label77;
        private System.Windows.Forms.Label label82;
    }
}