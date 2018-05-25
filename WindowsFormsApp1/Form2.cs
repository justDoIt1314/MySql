using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MySql.Data;
using MySql.Data.MySqlClient;
using System.Collections;
using System.Configuration;
using System.Data.Common;
namespace WindowsFormsApp1
{
    public partial class Form2 : Form
    {
        static MySqlConnection mysqlConnect = TestDatebase.GetMySqlCon();
        static Form1 form1;
        private Timer timer;
        public Form2()
        {
            this.Load += new EventHandler(Form2_load);
            InitializeComponent();
        }

      
        private void Form2_load(object sender, EventArgs e)
        {
            timer = new Timer();
            timer.Interval = 1000;
            timer.Tick += new EventHandler(Timer_Tick);
            timer.Start();

        }
        private void Timer_Tick(object sender, EventArgs e)
        {
            string s1, s2;
            DateTime dateTime = DateTime.Now;
            s1 = dateTime.Year.ToString() + "年" + dateTime.Month.ToString() + "月" + dateTime.Day.ToString() + "日";
            s2 = dateTime.ToString("HH:mm:ss");
            this.Date.Text = s1;
            this.Time.Text = s2;
        }
        
        private void Stu_Sno_TextChanged(object sender, EventArgs e)
        {

        }

        private void Stu_Load_Click(object sender, EventArgs e)
        {
            mysqlConnect.Open();
            Form1 form1 = new Form1();
            string sno = this.Stu_Sno.Text;
            string sPassword = this.Stu_Password.Text;
            string sname = null;
            string sdept = null;
            string format = "select * from student where Sno like '{0}' and Password like '{1}'";
            string strCmd = string.Format(format, sno, sPassword);
            string stu_tag = null;
            MySqlCommand selectCmd = new MySqlCommand(strCmd, mysqlConnect);
            MySqlDataReader reader1 = selectCmd.ExecuteReader();
            Boolean isCorrect = false;
            try
            {
                while (reader1.Read())
                {
                    isCorrect = true;
                    sname = reader1.GetString("Sname");
                    sdept = reader1.GetString("Sdept");
                }
            }
            catch (Exception a)
            {
            }
            finally
            {
                reader1.Close();
            }
            if (isCorrect == true)
            {
                stu_tag = "你好！ " + sname;
               
                form1 = new Form1(sno, sname, sdept, stu_tag,this);
                form1.stu_Interface.Visible = true;
                form1.tea_Interface.Visible = false;
                form1.admin_Interface.Visible = false;
                form1.tabControl1.Visible = true;
                form1.commonBox1.Visible = true;
                form1.Show();
                this.Hide();

            }
            else
            {
                System.Windows.Forms.MessageBox.Show("帐号或密码错误,请检查后重新登录", "错误", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            mysqlConnect.Close();
        }

        private void Tea_Load_Click(object sender, EventArgs e)
        {
            mysqlConnect.Open();
            string Tea_no = this.Tea_no.Text;
            string Tea_password = this.Tea_Password.Text;
            string Tea_name = null;

            string format = "select * from teacher where Tea_no like '{0}' and Tea_password like '{1}'";
            string strCmd = string.Format(format, Tea_no, Tea_password);
            MySqlCommand selectCmd = new MySqlCommand(strCmd, mysqlConnect);
            MySqlDataReader reader1 = selectCmd.ExecuteReader();
            Boolean isCorrect = false;
            string tag;
            try
            {
                while (reader1.Read())
                {
                    isCorrect = true;
                    Tea_name = reader1.GetString("Tea_name");

                }
            }
            catch (Exception a)
            {
            }
            finally
            {
                reader1.Close();
            }
            if (isCorrect == true)
            {               
                tag = "你好！ " + Tea_name + "老师";
                form1 = new Form1(tag,Tea_no,this);
                form1.tabControl1.Visible = false;

                form1.stu_Interface.Visible = true;
                form1.tea_Interface.Visible = true;
                form1.admin_Interface.Visible = false;
                form1.Show();
                this.Hide();
            }
            else
            {
                System.Windows.Forms.MessageBox.Show("帐号或密码错误,请检查后重新登录", "错误", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            mysqlConnect.Close();
        }

        private void Admin_Load_Click(object sender, EventArgs e)
        {
            mysqlConnect.Open();
            string Admin_no = this.Admin_no.Text;
            string Admin_password = this.Admin_Password.Text;
            string Admin_name = null;

            string format = "select * from administer where Admin_no like '{0}' and Admin_password like '{1}'";
            string strCmd = string.Format(format, Admin_no, Admin_password);
            MySqlCommand selectCmd = new MySqlCommand(strCmd, mysqlConnect);
            MySqlDataReader reader1 = selectCmd.ExecuteReader();
            string tag;
            Boolean isCorrect = false;
            try
            {
                while (reader1.Read())
                {
                    isCorrect = true;
                    Admin_name = reader1.GetString("Admin_name");

                }
            }
            catch (Exception a)
            {
            }
            finally
            {
                reader1.Close();
            }
            if (isCorrect == true)
            {                
                tag = "你好！ " + Admin_name;
                form1 = new Form1(tag, Admin_no, this);

                form1.stu_Interface.Visible = true;
                form1.tea_Interface.Visible = true;
                form1.admin_Interface.Visible = true;
                form1.tabControl1.Visible = false;
                form1.Show();
                this.Hide();
               
            }
            else
            {
                System.Windows.Forms.MessageBox.Show("帐号或密码错误,请检查后重新登录", "错误", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            mysqlConnect.Close();
        }
    }
}
