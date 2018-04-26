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
        public Form2()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            string connstr = "server=localhost;User Id=root;password=123456;Database=company";
            MySqlConnection mycon = new MySqlConnection(connstr);
            mycon.Open();
            MySqlCommand mycmd = new MySqlCommand("insert into product(prod_id,prod_name) values('P1005','摄像机')",mycon);
            if(mycmd.ExecuteNonQuery()>0)
            {
                this.label1.Text = "数据插入成功！";
            }
            mycon.Close();



           
        }
    }
}
