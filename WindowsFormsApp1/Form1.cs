
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;



namespace WindowsFormsApp1
{
    public partial class Form1 : Form
    {
        

        static int year_now = System.DateTime.Now.Year;
        static int month_now = System.DateTime.Now.Month;
        static int day_now = System.DateTime.Now.Day;
        static MySqlConnection mysqlConnect = TestDatebase.GetMySqlCon();


        /// <summary>
        /// 实现预约功能，判断是否能预约，以及预约失败后显示该设备被预约的时段
        /// </summary>
        /// <param name='lab'>实验室编号</param>
        /// <param name='dev_no'>设备号</param>
        /// <param name='sno'>学号</param>
        /// <param name='sname'>学生姓名</param>         
        /// <param name='sdept'>学生所属的学院</param>
        /// <param name='dateTimePicker1'>一个时间选择器的对象</param>
        /// <param name='start'>开始的节数</param>
        /// <param name='end'>结束的节数</param>
        /// <param name='commBox'>一个文本输入框，用于打印一些必要的信息</param>
        /// <returns></returns>
        public static void CommonClick(String lab, int dev_no, String sno, String sname, String sdept,
            DateTimePicker dateTimePicker1, int start, int end, RichTextBox commBox)
        {
            
            int year = dateTimePicker1.Value.Year;
            int month = dateTimePicker1.Value.Month;
            int day = dateTimePicker1.Value.Day;
            String format1 = "{0}-{1}-{2}";
            String dateStr = string.Format(format1, year, month, day);

            String format2 = "insert into my_order(Lno,Sno,Dev_no,Order_time,Start_course,End_course,All_course) values('{0}','{1}',{2},'{3}','{4}','{5}','{6}')";
            string order_cmd = string.Format(format2, lab, sno, dev_no, dateStr, start, end,end-start+1);

            String format3 = "select * from lab where Lno like '{0}'";
            String labStr = String.Format(format3, lab);
            if(sno==null)
            {
                System.Windows.Forms.MessageBox.Show("预约失败，请输入学号", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            if (CheckTime(year_now, month_now, day_now, dateTimePicker1, start, end, commBox) == true)
            {
                return;
            }
            if(CheckInfor(sno,sname,sdept)==false)
            {
                System.Windows.Forms.MessageBox.Show("预约失败，请检查你的信息是否正确", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            if(CheckConflict(lab, dev_no,  sno, dateTimePicker1,  start,  end, commBox)==true)
            {
                System.Windows.Forms.MessageBox.Show("预约失败，请检查预约时间是否发生冲突", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            mysqlConnect.Open();
            MySqlCommand mycmd_order = new MySqlCommand(order_cmd, mysqlConnect);

            MySqlCommand mycmd_lab = new MySqlCommand(labStr, mysqlConnect);
            // MySqlDataReader reader_lab = mycmd_lab.ExecuteReader();
            String lab_name = TestDatebase.getResultset(mycmd_lab, "Lname"); 
            String lab_addr = TestDatebase.getResultset(mycmd_lab, "Addr");
           
            try
            {
                if (mycmd_order.ExecuteNonQuery() > 0)
                {
                    String tip = "";
                    tip+= "你成功预约了" + lab_addr + lab_name + "的" + dev_no + "号设备";
                    System.Windows.Forms.MessageBox.Show(tip, "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception)
            {

            }
            finally
            {
                mysqlConnect.Close();
            }
                

        }
        /// <summary>
        /// 判断预约设备的时间是否发生冲突，并提示该设备已经被预约的时间段
        /// </summary>
        /// <param name='lab'>实验室编号</param>
        /// <param name='dev_no'>设备号</param>
        /// <param name='sno'>学号</param>
        /// <param name='sname'>学生姓名</param>         
        /// <param name='sdept'>学生所属的学院</param>
        /// <param name='dateTimePicker1'>一个时间选择器的对象</param>
        /// <param name='start'>开始的节数</param>
        /// <param name='end'>结束的节数</param>
        /// <param name='commBox'>一个文本输入框，用于打印一些必要的信息</param>
        /// <returns>返回true 表示有冲突 </returns>
        public static Boolean CheckConflict(String lab, int dev_no, String sno, 
            DateTimePicker dateTimePicker1, int start, int end, RichTextBox commBox)
        {
            
            String select_cmd = "select Lno,Dev_no,Order_time,Start_course,End_course from my_order";
            mysqlConnect.Open();
            int year = dateTimePicker1.Value.Year;
            int month = dateTimePicker1.Value.Month;
            int day = dateTimePicker1.Value.Day;
            String format1 = "{0}-{1}-{2}";
            String dateStr = string.Format(format1, year, month, day);
            DateTime dt = Convert.ToDateTime(dateStr);

            String format3 = "select * from lab where Lno like '{0}'";
            String labStr = String.Format(format3, lab);
            MySqlCommand lab_cmd = new MySqlCommand(labStr,mysqlConnect);
            String lab_name = TestDatebase.getResultset(lab_cmd,"Lname");

            
            MySqlCommand mycmd = new MySqlCommand(select_cmd, mysqlConnect);
            MySqlDataReader reader = mycmd.ExecuteReader();
            
            Boolean conflict = false;
            try
            {
                while (reader.Read())
                {
                    if (reader.GetString("Lno").Equals(lab) && reader.GetInt32("Dev_no")==dev_no )
                    {
                        if (reader.GetMySqlDateTime("Order_time").Year == year && reader.GetMySqlDateTime("Order_time").Month == month && reader.GetMySqlDateTime("Order_time").Day == day)
                        {
                            int start_course = reader.GetInt32("Start_course");
                            int end_course = reader.GetInt32("End_course");
                            if (start_course >= end || start <= end_course)
                                conflict = true;
                            break;
                        }
                    }
                }
            }
            catch (Exception e)
            {
            }
            finally
            {
                reader.Close();
            }
            String format2 = "select Lno,Dev_no,Order_time,Start_course,End_course from my_order where Lno like '{0}' and Dev_no={1}";
            String select_cmd2 = string.Format(format2, lab, dev_no);
            MySqlCommand mycmd2 = new MySqlCommand(select_cmd2, mysqlConnect);
            MySqlDataReader reader2 = mycmd2.ExecuteReader();
            DateTime date_now = DateTime.Now;
            int days_now = CalDays(date_now.Year, date_now.Month, date_now.Day);
            String text = "显示该设备预约信息：\n实验室名称    设备号     日期      时间段\n";
           
                try
                {
                    while (reader2.Read())
                    {
                        if (reader2.GetString("Lno").Equals(lab) && reader2.GetInt32("Dev_no") == dev_no )
                        {
                            DateTime dateTime = reader2.GetDateTime("Order_time");
                            int days = CalDays(dateTime.Year, dateTime.Month, dateTime.Day);
                            if(days>=days_now)
                            {
                                int start_course = reader2.GetInt32("Start_course");
                                int end_course = reader2.GetInt32("End_course");
                                text = text + lab_name + "            " + dev_no + "  " + dateTime.ToString("yyyy年MM月dd日")+"  "+
                                    start_course+"-"+end_course+"节"+"\n";
                            }
                        }
                    }
                }
                catch (Exception e)
                {
                }
                finally
                {
                    reader2.Close();
                }
            commBox.Text = text;

            mysqlConnect.Close();
            return conflict;
        }
        /// <summary>
        /// 判断学生信息是否在数据库中
        /// </summary>
        /// <param name='sno'>学号</param>
        /// <param name='sname'>学生姓名</param>         
        /// <param name='sdept'>学生所属的学院</param>
        /// <returns>返回true 表示所填信息正确，false表示所填信息错误 </returns>
        public static Boolean CheckInfor(String sno, String sname, String sdept)
        {
            String select_cmd = "select * from student";
            mysqlConnect.Open();

            MySqlCommand mycmd = new MySqlCommand(select_cmd, mysqlConnect);
            MySqlDataReader reader = mycmd.ExecuteReader();
            try
            {
                while (reader.Read())
                {
                    if (reader.GetString("Sno").Equals(sno)&&reader.GetString("Sname").Equals(sname)&&reader.GetString("Sdept").Equals(sdept))
                    {
                        mysqlConnect.Close();
                        return true;
                    }
                }
            }
            catch (Exception e)
            {
            }
            finally
            {
                reader.Close();
            }
            mysqlConnect.Close();
            return false;
        }
        /// <summary>
        /// 判断预约时间是否符合要求
        /// </summary>
        /// <param name='sno'>学号</param>
        /// <param name='sname'>学生姓名</param>         
        /// <param name='sdept'>学生所属的学院</param>
        /// <param name='dateTimePicker1'>一个时间选择器的对象</param>
        /// <param name='start'>开始的节数</param>
        /// <param name='end'>结束的节数</param>
        /// <param name='commBox'>一个文本输入框，用于打印一些必要的信息</param>
        /// <returns>返回true 表示预约时间不符合要求 </returns>
        public static Boolean CheckTime(int year_now,int month_now,int day_now, DateTimePicker dateTimePicker1,
            int start,int end, RichTextBox commBox)
        {

            int year = dateTimePicker1.Value.Year;
            int month = dateTimePicker1.Value.Month;
            int day = dateTimePicker1.Value.Day;
            int days = 0;
            int dayNums = 365;
            if ((year_now % 4 == 0 && year_now % 100 != 0) || year_now % 400 == 0)
            {
                dayNums += 1;
            }
            days = (year - year_now) * dayNums + CalDays(year, month, day)-CalDays(year_now, month_now, day_now) ;
            if (days > 7|| days<0 )
            {
                System.Windows.Forms.MessageBox.Show("只能预约7日以内的时间", "警告", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return true;
            }
            if(start>end)
            {
                System.Windows.Forms.MessageBox.Show("预约的开始和结束有问题", "警告", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return true;
            }
            return false;
        }
        /// <summary>
        /// 计算该日期是一年中的第几天
        /// </summary>
        /// <param name='year'>学号</param>
        /// <param name='month'>学生姓名</param>         
        /// <param name='day'>学生所属的学院</param>
        /// <returns>返回true 表示预约时间不符合要求 </returns>
        public static int CalDays(int year, int month, int day)
        {
            int days = 0;
            int[] myMonth = new int[12];

            myMonth[0] = 0;
            myMonth[1] = 31;
            myMonth[2] = 59;
            myMonth[3] = 90;
            myMonth[4] = 120;
            myMonth[5] = 151;
            myMonth[6] = 181;
            myMonth[7] = 212;
            myMonth[8] = 243;
            myMonth[9] = 273;
            myMonth[10] = 304;
            myMonth[11] = 334;

            if ((year_now % 4 == 0 && year_now % 100 != 0) || year_now % 400 == 0)
            {
                myMonth[1] = myMonth[1] + 1;
            }
            return (myMonth[month - 1] + day);
           
        }
        public Form1()
        {
            InitializeComponent();
            
        }

        
        private void Form1_Load(object sender, EventArgs e)
        {
            
           
           
        }
      

        private void Form1_KeyDown(object sender, System.Windows.Forms.KeyEventArgs e)
        {
            Application.Exit();
        }

      
        private void InitializeComponent()
        {
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.tea_Interface = new System.Windows.Forms.GroupBox();
            this.Tea_Clock = new System.Windows.Forms.Button();
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
            this.admin_Interface = new System.Windows.Forms.GroupBox();
            this.Admin_Tea_Reset = new System.Windows.Forms.Button();
            this.Statistics = new System.Windows.Forms.Button();
            this.Admin_Stu_Reset = new System.Windows.Forms.Button();
            this.label60 = new System.Windows.Forms.Label();
            this.Admin_Register_Tea = new System.Windows.Forms.Button();
            this.label81 = new System.Windows.Forms.Label();
            this.Admin_Tea_Name = new System.Windows.Forms.TextBox();
            this.Admin_Tea_Password = new System.Windows.Forms.TextBox();
            this.label93 = new System.Windows.Forms.Label();
            this.label94 = new System.Windows.Forms.Label();
            this.Admin_Tea_no = new System.Windows.Forms.TextBox();
            this.label95 = new System.Windows.Forms.Label();
            this.label96 = new System.Windows.Forms.Label();
            this.Admin_Register_Stu = new System.Windows.Forms.Button();
            this.label86 = new System.Windows.Forms.Label();
            this.Admin_Stu_Sdept = new System.Windows.Forms.TextBox();
            this.Admin_Stu_Name = new System.Windows.Forms.TextBox();
            this.label92 = new System.Windows.Forms.Label();
            this.Admin_Stu_Password = new System.Windows.Forms.TextBox();
            this.label85 = new System.Windows.Forms.Label();
            this.label83 = new System.Windows.Forms.Label();
            this.Admin_Sno = new System.Windows.Forms.TextBox();
            this.Admin_New_Again = new System.Windows.Forms.TextBox();
            this.label84 = new System.Windows.Forms.Label();
            this.Admin_Confirm = new System.Windows.Forms.Button();
            this.label87 = new System.Windows.Forms.Label();
            this.Admin_New = new System.Windows.Forms.TextBox();
            this.label88 = new System.Windows.Forms.Label();
            this.label89 = new System.Windows.Forms.Label();
            this.label91 = new System.Windows.Forms.Label();
            this.lab1_addr = new System.Windows.Forms.Label();
            this.lab1_name = new System.Windows.Forms.Label();
            this.label54 = new System.Windows.Forms.Label();
            this.label55 = new System.Windows.Forms.Label();
            this.Lab1_04 = new System.Windows.Forms.Button();
            this.Lab1_02 = new System.Windows.Forms.Button();
            this.Lab1_01 = new System.Windows.Forms.Button();
            this.label26 = new System.Windows.Forms.Label();
            this.Lab1_15 = new System.Windows.Forms.Button();
            this.pictureBox26 = new System.Windows.Forms.PictureBox();
            this.label27 = new System.Windows.Forms.Label();
            this.Lab1_14 = new System.Windows.Forms.Button();
            this.pictureBox27 = new System.Windows.Forms.PictureBox();
            this.label28 = new System.Windows.Forms.Label();
            this.Lab1_13 = new System.Windows.Forms.Button();
            this.label29 = new System.Windows.Forms.Label();
            this.pictureBox28 = new System.Windows.Forms.PictureBox();
            this.pictureBox29 = new System.Windows.Forms.PictureBox();
            this.Lab1_12 = new System.Windows.Forms.Button();
            this.label30 = new System.Windows.Forms.Label();
            this.Lab1_11 = new System.Windows.Forms.Button();
            this.pictureBox30 = new System.Windows.Forms.PictureBox();
            this.label31 = new System.Windows.Forms.Label();
            this.Lab1_20 = new System.Windows.Forms.Button();
            this.pictureBox31 = new System.Windows.Forms.PictureBox();
            this.label32 = new System.Windows.Forms.Label();
            this.Lab1_19 = new System.Windows.Forms.Button();
            this.pictureBox32 = new System.Windows.Forms.PictureBox();
            this.label33 = new System.Windows.Forms.Label();
            this.Lab1_18 = new System.Windows.Forms.Button();
            this.label34 = new System.Windows.Forms.Label();
            this.pictureBox33 = new System.Windows.Forms.PictureBox();
            this.pictureBox34 = new System.Windows.Forms.PictureBox();
            this.Lab1_17 = new System.Windows.Forms.Button();
            this.label35 = new System.Windows.Forms.Label();
            this.Lab1_16 = new System.Windows.Forms.Button();
            this.pictureBox35 = new System.Windows.Forms.PictureBox();
            this.label36 = new System.Windows.Forms.Label();
            this.Lab1_25 = new System.Windows.Forms.Button();
            this.pictureBox36 = new System.Windows.Forms.PictureBox();
            this.label37 = new System.Windows.Forms.Label();
            this.Lab1_24 = new System.Windows.Forms.Button();
            this.pictureBox37 = new System.Windows.Forms.PictureBox();
            this.label38 = new System.Windows.Forms.Label();
            this.Lab1_23 = new System.Windows.Forms.Button();
            this.label39 = new System.Windows.Forms.Label();
            this.pictureBox38 = new System.Windows.Forms.PictureBox();
            this.pictureBox39 = new System.Windows.Forms.PictureBox();
            this.Lab1_22 = new System.Windows.Forms.Button();
            this.label40 = new System.Windows.Forms.Label();
            this.Lab1_21 = new System.Windows.Forms.Button();
            this.pictureBox40 = new System.Windows.Forms.PictureBox();
            this.label41 = new System.Windows.Forms.Label();
            this.Lab1_10 = new System.Windows.Forms.Button();
            this.pictureBox41 = new System.Windows.Forms.PictureBox();
            this.label42 = new System.Windows.Forms.Label();
            this.Lab1_09 = new System.Windows.Forms.Button();
            this.pictureBox42 = new System.Windows.Forms.PictureBox();
            this.label43 = new System.Windows.Forms.Label();
            this.Lab1_08 = new System.Windows.Forms.Button();
            this.label44 = new System.Windows.Forms.Label();
            this.pictureBox43 = new System.Windows.Forms.PictureBox();
            this.pictureBox44 = new System.Windows.Forms.PictureBox();
            this.Lab1_07 = new System.Windows.Forms.Button();
            this.label45 = new System.Windows.Forms.Label();
            this.Lab1_06 = new System.Windows.Forms.Button();
            this.pictureBox45 = new System.Windows.Forms.PictureBox();
            this.label46 = new System.Windows.Forms.Label();
            this.Lab1_05 = new System.Windows.Forms.Button();
            this.label47 = new System.Windows.Forms.Label();
            this.pictureBox46 = new System.Windows.Forms.PictureBox();
            this.pictureBox47 = new System.Windows.Forms.PictureBox();
            this.label48 = new System.Windows.Forms.Label();
            this.Lab1_03 = new System.Windows.Forms.Button();
            this.pictureBox48 = new System.Windows.Forms.PictureBox();
            this.label49 = new System.Windows.Forms.Label();
            this.label50 = new System.Windows.Forms.Label();
            this.pictureBox49 = new System.Windows.Forms.PictureBox();
            this.pictureBox50 = new System.Windows.Forms.PictureBox();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.lab2_addr = new System.Windows.Forms.Label();
            this.lab2_name = new System.Windows.Forms.Label();
            this.label51 = new System.Windows.Forms.Label();
            this.label11 = new System.Windows.Forms.Label();
            this.Lab2_01 = new System.Windows.Forms.Button();
            this.label21 = new System.Windows.Forms.Label();
            this.Lab2_15 = new System.Windows.Forms.Button();
            this.pictureBox21 = new System.Windows.Forms.PictureBox();
            this.label22 = new System.Windows.Forms.Label();
            this.Lab2_14 = new System.Windows.Forms.Button();
            this.pictureBox22 = new System.Windows.Forms.PictureBox();
            this.label23 = new System.Windows.Forms.Label();
            this.Lab2_13 = new System.Windows.Forms.Button();
            this.label24 = new System.Windows.Forms.Label();
            this.pictureBox23 = new System.Windows.Forms.PictureBox();
            this.pictureBox24 = new System.Windows.Forms.PictureBox();
            this.Lab2_12 = new System.Windows.Forms.Button();
            this.label25 = new System.Windows.Forms.Label();
            this.Lab2_11 = new System.Windows.Forms.Button();
            this.pictureBox25 = new System.Windows.Forms.PictureBox();
            this.label16 = new System.Windows.Forms.Label();
            this.Lab2_20 = new System.Windows.Forms.Button();
            this.pictureBox16 = new System.Windows.Forms.PictureBox();
            this.label17 = new System.Windows.Forms.Label();
            this.Lab2_19 = new System.Windows.Forms.Button();
            this.pictureBox17 = new System.Windows.Forms.PictureBox();
            this.label18 = new System.Windows.Forms.Label();
            this.Lab2_18 = new System.Windows.Forms.Button();
            this.label19 = new System.Windows.Forms.Label();
            this.pictureBox18 = new System.Windows.Forms.PictureBox();
            this.pictureBox19 = new System.Windows.Forms.PictureBox();
            this.Lab2_17 = new System.Windows.Forms.Button();
            this.label20 = new System.Windows.Forms.Label();
            this.Lab2_16 = new System.Windows.Forms.Button();
            this.pictureBox20 = new System.Windows.Forms.PictureBox();
            this.label12 = new System.Windows.Forms.Label();
            this.Lab2_24 = new System.Windows.Forms.Button();
            this.pictureBox12 = new System.Windows.Forms.PictureBox();
            this.label13 = new System.Windows.Forms.Label();
            this.Lab2_23 = new System.Windows.Forms.Button();
            this.label14 = new System.Windows.Forms.Label();
            this.pictureBox13 = new System.Windows.Forms.PictureBox();
            this.pictureBox14 = new System.Windows.Forms.PictureBox();
            this.Lab2_22 = new System.Windows.Forms.Button();
            this.label15 = new System.Windows.Forms.Label();
            this.Lab2_21 = new System.Windows.Forms.Button();
            this.pictureBox15 = new System.Windows.Forms.PictureBox();
            this.label10 = new System.Windows.Forms.Label();
            this.Lab2_10 = new System.Windows.Forms.Button();
            this.pictureBox10 = new System.Windows.Forms.PictureBox();
            this.label7 = new System.Windows.Forms.Label();
            this.Lab2_09 = new System.Windows.Forms.Button();
            this.pictureBox7 = new System.Windows.Forms.PictureBox();
            this.label8 = new System.Windows.Forms.Label();
            this.Lab2_08 = new System.Windows.Forms.Button();
            this.label9 = new System.Windows.Forms.Label();
            this.pictureBox8 = new System.Windows.Forms.PictureBox();
            this.pictureBox9 = new System.Windows.Forms.PictureBox();
            this.Lab2_07 = new System.Windows.Forms.Button();
            this.label4 = new System.Windows.Forms.Label();
            this.Lab2_06 = new System.Windows.Forms.Button();
            this.pictureBox4 = new System.Windows.Forms.PictureBox();
            this.label5 = new System.Windows.Forms.Label();
            this.Lab2_05 = new System.Windows.Forms.Button();
            this.label6 = new System.Windows.Forms.Label();
            this.pictureBox5 = new System.Windows.Forms.PictureBox();
            this.pictureBox6 = new System.Windows.Forms.PictureBox();
            this.Lab2_04 = new System.Windows.Forms.Button();
            this.label3 = new System.Windows.Forms.Label();
            this.Lab2_03 = new System.Windows.Forms.Button();
            this.pictureBox3 = new System.Windows.Forms.PictureBox();
            this.label2 = new System.Windows.Forms.Label();
            this.Lab2_02 = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.pictureBox2 = new System.Windows.Forms.PictureBox();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.label52 = new System.Windows.Forms.Label();
            this.dateTimePicker1 = new System.Windows.Forms.DateTimePicker();
            this.label53 = new System.Windows.Forms.Label();
            this.order_start = new System.Windows.Forms.TextBox();
            this.label56 = new System.Windows.Forms.Label();
            this.order_end = new System.Windows.Forms.TextBox();
            this.label57 = new System.Windows.Forms.Label();
            this.label58 = new System.Windows.Forms.Label();
            this.lab121 = new System.Windows.Forms.Label();
            this.lab123 = new System.Windows.Forms.Label();
            this.label61 = new System.Windows.Forms.Label();
            this.label59 = new System.Windows.Forms.Label();
            this.InitButton = new System.Windows.Forms.Button();
            this.label62 = new System.Windows.Forms.Label();
            this.Button_find = new System.Windows.Forms.Button();
            this.tabControl2 = new System.Windows.Forms.TabControl();
            this.tabPage3 = new System.Windows.Forms.TabPage();
            this.group_stu = new System.Windows.Forms.GroupBox();
            this.Stu_Sno = new System.Windows.Forms.TextBox();
            this.label63 = new System.Windows.Forms.Label();
            this.Stu_Load = new System.Windows.Forms.Button();
            this.label64 = new System.Windows.Forms.Label();
            this.Stu_Password = new System.Windows.Forms.TextBox();
            this.stu_tag = new System.Windows.Forms.Label();
            this.Exit_Stu = new System.Windows.Forms.Button();
            this.tabPage4 = new System.Windows.Forms.TabPage();
            this.group_Tea = new System.Windows.Forms.GroupBox();
            this.Tea_no = new System.Windows.Forms.TextBox();
            this.label90 = new System.Windows.Forms.Label();
            this.Tea_Load = new System.Windows.Forms.Button();
            this.label97 = new System.Windows.Forms.Label();
            this.Tea_Password = new System.Windows.Forms.TextBox();
            this.Tea_tag = new System.Windows.Forms.Label();
            this.Exit_Tea = new System.Windows.Forms.Button();
            this.tabPage5 = new System.Windows.Forms.TabPage();
            this.group_Admin = new System.Windows.Forms.GroupBox();
            this.Admin_no = new System.Windows.Forms.TextBox();
            this.label65 = new System.Windows.Forms.Label();
            this.Admin_Load = new System.Windows.Forms.Button();
            this.label66 = new System.Windows.Forms.Label();
            this.Admin_Password = new System.Windows.Forms.TextBox();
            this.Admin_tag = new System.Windows.Forms.Label();
            this.Exit_Admin = new System.Windows.Forms.Button();
            this.commonBox1 = new System.Windows.Forms.RichTextBox();
            this.stu_Interface = new System.Windows.Forms.GroupBox();
            this.Sdept = new System.Windows.Forms.Label();
            this.Sname = new System.Windows.Forms.Label();
            this.Sno = new System.Windows.Forms.Label();
            this.Confirm_New = new System.Windows.Forms.TextBox();
            this.label71 = new System.Windows.Forms.Label();
            this.Confirm = new System.Windows.Forms.Button();
            this.label70 = new System.Windows.Forms.Label();
            this.New_Password = new System.Windows.Forms.TextBox();
            this.label69 = new System.Windows.Forms.Label();
            this.tabControl1.SuspendLayout();
            this.tabPage1.SuspendLayout();
            this.tea_Interface.SuspendLayout();
            this.admin_Interface.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox26)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox27)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox28)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox29)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox30)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox31)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox32)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox33)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox34)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox35)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox36)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox37)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox38)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox39)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox40)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox41)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox42)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox43)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox44)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox45)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox46)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox47)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox48)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox49)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox50)).BeginInit();
            this.tabPage2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox21)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox22)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox23)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox24)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox25)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox16)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox17)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox18)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox19)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox20)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox12)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox13)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox14)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox15)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox10)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox7)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox8)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox9)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox4)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox5)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox6)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox3)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.tabControl2.SuspendLayout();
            this.tabPage3.SuspendLayout();
            this.group_stu.SuspendLayout();
            this.tabPage4.SuspendLayout();
            this.group_Tea.SuspendLayout();
            this.tabPage5.SuspendLayout();
            this.group_Admin.SuspendLayout();
            this.stu_Interface.SuspendLayout();
            this.SuspendLayout();
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.tabPage1);
            this.tabControl1.Controls.Add(this.tabPage2);
            this.tabControl1.Location = new System.Drawing.Point(662, 12);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(435, 526);
            this.tabControl1.TabIndex = 0;
            this.tabControl1.Tag = "lab1";
            this.tabControl1.Visible = false;
            // 
            // tabPage1
            // 
            this.tabPage1.Controls.Add(this.lab1_addr);
            this.tabPage1.Controls.Add(this.lab1_name);
            this.tabPage1.Controls.Add(this.label54);
            this.tabPage1.Controls.Add(this.label55);
            this.tabPage1.Controls.Add(this.Lab1_04);
            this.tabPage1.Controls.Add(this.Lab1_02);
            this.tabPage1.Controls.Add(this.Lab1_01);
            this.tabPage1.Controls.Add(this.label26);
            this.tabPage1.Controls.Add(this.Lab1_15);
            this.tabPage1.Controls.Add(this.pictureBox26);
            this.tabPage1.Controls.Add(this.label27);
            this.tabPage1.Controls.Add(this.Lab1_14);
            this.tabPage1.Controls.Add(this.pictureBox27);
            this.tabPage1.Controls.Add(this.label28);
            this.tabPage1.Controls.Add(this.Lab1_13);
            this.tabPage1.Controls.Add(this.label29);
            this.tabPage1.Controls.Add(this.pictureBox28);
            this.tabPage1.Controls.Add(this.pictureBox29);
            this.tabPage1.Controls.Add(this.Lab1_12);
            this.tabPage1.Controls.Add(this.label30);
            this.tabPage1.Controls.Add(this.Lab1_11);
            this.tabPage1.Controls.Add(this.pictureBox30);
            this.tabPage1.Controls.Add(this.label31);
            this.tabPage1.Controls.Add(this.Lab1_20);
            this.tabPage1.Controls.Add(this.pictureBox31);
            this.tabPage1.Controls.Add(this.label32);
            this.tabPage1.Controls.Add(this.Lab1_19);
            this.tabPage1.Controls.Add(this.pictureBox32);
            this.tabPage1.Controls.Add(this.label33);
            this.tabPage1.Controls.Add(this.Lab1_18);
            this.tabPage1.Controls.Add(this.label34);
            this.tabPage1.Controls.Add(this.pictureBox33);
            this.tabPage1.Controls.Add(this.pictureBox34);
            this.tabPage1.Controls.Add(this.Lab1_17);
            this.tabPage1.Controls.Add(this.label35);
            this.tabPage1.Controls.Add(this.Lab1_16);
            this.tabPage1.Controls.Add(this.pictureBox35);
            this.tabPage1.Controls.Add(this.label36);
            this.tabPage1.Controls.Add(this.Lab1_25);
            this.tabPage1.Controls.Add(this.pictureBox36);
            this.tabPage1.Controls.Add(this.label37);
            this.tabPage1.Controls.Add(this.Lab1_24);
            this.tabPage1.Controls.Add(this.pictureBox37);
            this.tabPage1.Controls.Add(this.label38);
            this.tabPage1.Controls.Add(this.Lab1_23);
            this.tabPage1.Controls.Add(this.label39);
            this.tabPage1.Controls.Add(this.pictureBox38);
            this.tabPage1.Controls.Add(this.pictureBox39);
            this.tabPage1.Controls.Add(this.Lab1_22);
            this.tabPage1.Controls.Add(this.label40);
            this.tabPage1.Controls.Add(this.Lab1_21);
            this.tabPage1.Controls.Add(this.pictureBox40);
            this.tabPage1.Controls.Add(this.label41);
            this.tabPage1.Controls.Add(this.Lab1_10);
            this.tabPage1.Controls.Add(this.pictureBox41);
            this.tabPage1.Controls.Add(this.label42);
            this.tabPage1.Controls.Add(this.Lab1_09);
            this.tabPage1.Controls.Add(this.pictureBox42);
            this.tabPage1.Controls.Add(this.label43);
            this.tabPage1.Controls.Add(this.Lab1_08);
            this.tabPage1.Controls.Add(this.label44);
            this.tabPage1.Controls.Add(this.pictureBox43);
            this.tabPage1.Controls.Add(this.pictureBox44);
            this.tabPage1.Controls.Add(this.Lab1_07);
            this.tabPage1.Controls.Add(this.label45);
            this.tabPage1.Controls.Add(this.Lab1_06);
            this.tabPage1.Controls.Add(this.pictureBox45);
            this.tabPage1.Controls.Add(this.label46);
            this.tabPage1.Controls.Add(this.Lab1_05);
            this.tabPage1.Controls.Add(this.label47);
            this.tabPage1.Controls.Add(this.pictureBox46);
            this.tabPage1.Controls.Add(this.pictureBox47);
            this.tabPage1.Controls.Add(this.label48);
            this.tabPage1.Controls.Add(this.Lab1_03);
            this.tabPage1.Controls.Add(this.pictureBox48);
            this.tabPage1.Controls.Add(this.label49);
            this.tabPage1.Controls.Add(this.label50);
            this.tabPage1.Controls.Add(this.pictureBox49);
            this.tabPage1.Controls.Add(this.pictureBox50);
            this.tabPage1.Location = new System.Drawing.Point(4, 25);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage1.Size = new System.Drawing.Size(427, 497);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "Lab1";
            this.tabPage1.UseVisualStyleBackColor = true;
            this.tabPage1.UseWaitCursor = true;
            // 
            // tea_Interface
            // 
            this.tea_Interface.Controls.Add(this.admin_Interface);
            this.tea_Interface.Controls.Add(this.Tea_Clock);
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
            this.tea_Interface.Location = new System.Drawing.Point(6, 0);
            this.tea_Interface.Name = "tea_Interface";
            this.tea_Interface.Size = new System.Drawing.Size(290, 504);
            this.tea_Interface.TabIndex = 31;
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
            this.Tea_Clock.Click += new System.EventHandler(this.Tea_Clock_Click);
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
            this.Tea_Start.TextChanged += new System.EventHandler(this.Tea_Start_TextChanged);
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
            this.Tea_Confirm.Click += new System.EventHandler(this.Tea_Confirm_Click);
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
            this.Tea_Find.Click += new System.EventHandler(this.Tea_Find_Click);
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
            // admin_Interface
            // 
            this.admin_Interface.Controls.Add(this.Admin_Tea_Reset);
            this.admin_Interface.Controls.Add(this.Statistics);
            this.admin_Interface.Controls.Add(this.Admin_Stu_Reset);
            this.admin_Interface.Controls.Add(this.label60);
            this.admin_Interface.Controls.Add(this.Admin_Register_Tea);
            this.admin_Interface.Controls.Add(this.label81);
            this.admin_Interface.Controls.Add(this.Admin_Tea_Name);
            this.admin_Interface.Controls.Add(this.Admin_Tea_Password);
            this.admin_Interface.Controls.Add(this.label93);
            this.admin_Interface.Controls.Add(this.label94);
            this.admin_Interface.Controls.Add(this.Admin_Tea_no);
            this.admin_Interface.Controls.Add(this.label95);
            this.admin_Interface.Controls.Add(this.label96);
            this.admin_Interface.Controls.Add(this.Admin_Register_Stu);
            this.admin_Interface.Controls.Add(this.label86);
            this.admin_Interface.Controls.Add(this.Admin_Stu_Sdept);
            this.admin_Interface.Controls.Add(this.Admin_Stu_Name);
            this.admin_Interface.Controls.Add(this.label92);
            this.admin_Interface.Controls.Add(this.Admin_Stu_Password);
            this.admin_Interface.Controls.Add(this.label85);
            this.admin_Interface.Controls.Add(this.label83);
            this.admin_Interface.Controls.Add(this.Admin_Sno);
            this.admin_Interface.Controls.Add(this.Admin_New_Again);
            this.admin_Interface.Controls.Add(this.label84);
            this.admin_Interface.Controls.Add(this.Admin_Confirm);
            this.admin_Interface.Controls.Add(this.label87);
            this.admin_Interface.Controls.Add(this.Admin_New);
            this.admin_Interface.Controls.Add(this.label88);
            this.admin_Interface.Controls.Add(this.label89);
            this.admin_Interface.Controls.Add(this.label91);
            this.admin_Interface.Location = new System.Drawing.Point(5, 0);
            this.admin_Interface.Name = "admin_Interface";
            this.admin_Interface.Size = new System.Drawing.Size(290, 504);
            this.admin_Interface.TabIndex = 32;
            this.admin_Interface.TabStop = false;
            this.admin_Interface.Text = "管理员界面";
            this.admin_Interface.Visible = false;
            this.admin_Interface.Enter += new System.EventHandler(this.groupBox3_Enter);
            // 
            // Admin_Tea_Reset
            // 
            this.Admin_Tea_Reset.Location = new System.Drawing.Point(154, 301);
            this.Admin_Tea_Reset.Name = "Admin_Tea_Reset";
            this.Admin_Tea_Reset.Size = new System.Drawing.Size(75, 23);
            this.Admin_Tea_Reset.TabIndex = 60;
            this.Admin_Tea_Reset.Text = "重置密码";
            this.Admin_Tea_Reset.UseVisualStyleBackColor = true;
            this.Admin_Tea_Reset.Click += new System.EventHandler(this.Admin_Tea_Reset_Click);
            // 
            // Statistics
            // 
            this.Statistics.BackColor = System.Drawing.SystemColors.Info;
            this.Statistics.Location = new System.Drawing.Point(187, 467);
            this.Statistics.Name = "Statistics";
            this.Statistics.Size = new System.Drawing.Size(75, 23);
            this.Statistics.TabIndex = 18;
            this.Statistics.Text = "统计";
            this.Statistics.UseVisualStyleBackColor = false;
            this.Statistics.Click += new System.EventHandler(this.Statistics_Click);
            // 
            // Admin_Stu_Reset
            // 
            this.Admin_Stu_Reset.Location = new System.Drawing.Point(155, 160);
            this.Admin_Stu_Reset.Name = "Admin_Stu_Reset";
            this.Admin_Stu_Reset.Size = new System.Drawing.Size(75, 23);
            this.Admin_Stu_Reset.TabIndex = 59;
            this.Admin_Stu_Reset.Text = "重置密码";
            this.Admin_Stu_Reset.UseVisualStyleBackColor = true;
            this.Admin_Stu_Reset.Click += new System.EventHandler(this.Admin_Stu_Reset_Click);
            // 
            // label60
            // 
            this.label60.AutoSize = true;
            this.label60.BackColor = System.Drawing.SystemColors.GradientActiveCaption;
            this.label60.Location = new System.Drawing.Point(17, 474);
            this.label60.Name = "label60";
            this.label60.Size = new System.Drawing.Size(187, 15);
            this.label60.TabIndex = 17;
            this.label60.Text = "提示：统计当天设备利用率";
            // 
            // Admin_Register_Tea
            // 
            this.Admin_Register_Tea.Location = new System.Drawing.Point(18, 301);
            this.Admin_Register_Tea.Name = "Admin_Register_Tea";
            this.Admin_Register_Tea.Size = new System.Drawing.Size(75, 23);
            this.Admin_Register_Tea.TabIndex = 58;
            this.Admin_Register_Tea.Text = "注册";
            this.Admin_Register_Tea.UseVisualStyleBackColor = true;
            this.Admin_Register_Tea.Click += new System.EventHandler(this.Admin_Register_Tea_Click);
            // 
            // label81
            // 
            this.label81.AutoSize = true;
            this.label81.Font = new System.Drawing.Font("宋体", 12F);
            this.label81.Location = new System.Drawing.Point(15, 273);
            this.label81.Name = "label81";
            this.label81.Size = new System.Drawing.Size(69, 20);
            this.label81.TabIndex = 54;
            this.label81.Text = "姓名：";
            // 
            // Admin_Tea_Name
            // 
            this.Admin_Tea_Name.Location = new System.Drawing.Point(77, 273);
            this.Admin_Tea_Name.Name = "Admin_Tea_Name";
            this.Admin_Tea_Name.Size = new System.Drawing.Size(152, 25);
            this.Admin_Tea_Name.TabIndex = 56;
            // 
            // Admin_Tea_Password
            // 
            this.Admin_Tea_Password.Location = new System.Drawing.Point(78, 246);
            this.Admin_Tea_Password.Name = "Admin_Tea_Password";
            this.Admin_Tea_Password.PasswordChar = '*';
            this.Admin_Tea_Password.Size = new System.Drawing.Size(152, 25);
            this.Admin_Tea_Password.TabIndex = 53;
            // 
            // label93
            // 
            this.label93.AutoSize = true;
            this.label93.Font = new System.Drawing.Font("宋体", 12F);
            this.label93.Location = new System.Drawing.Point(15, 249);
            this.label93.Name = "label93";
            this.label93.Size = new System.Drawing.Size(69, 20);
            this.label93.TabIndex = 52;
            this.label93.Text = "密码：";
            // 
            // label94
            // 
            this.label94.AutoSize = true;
            this.label94.Font = new System.Drawing.Font("宋体", 12F);
            this.label94.Location = new System.Drawing.Point(15, 222);
            this.label94.Name = "label94";
            this.label94.Size = new System.Drawing.Size(69, 20);
            this.label94.TabIndex = 50;
            this.label94.Text = "工号：";
            // 
            // Admin_Tea_no
            // 
            this.Admin_Tea_no.Location = new System.Drawing.Point(77, 218);
            this.Admin_Tea_no.Name = "Admin_Tea_no";
            this.Admin_Tea_no.Size = new System.Drawing.Size(152, 25);
            this.Admin_Tea_no.TabIndex = 51;
            // 
            // label95
            // 
            this.label95.AutoSize = true;
            this.label95.Location = new System.Drawing.Point(40, 197);
            this.label95.Name = "label95";
            this.label95.Size = new System.Drawing.Size(0, 15);
            this.label95.TabIndex = 48;
            // 
            // label96
            // 
            this.label96.AutoSize = true;
            this.label96.BackColor = System.Drawing.SystemColors.GradientActiveCaption;
            this.label96.Location = new System.Drawing.Point(17, 190);
            this.label96.Name = "label96";
            this.label96.Size = new System.Drawing.Size(450, 15);
            this.label96.TabIndex = 49;
            this.label96.Text = "添加教师帐号和重置教师密码-------------------------------";
            // 
            // Admin_Register_Stu
            // 
            this.Admin_Register_Stu.Location = new System.Drawing.Point(14, 160);
            this.Admin_Register_Stu.Name = "Admin_Register_Stu";
            this.Admin_Register_Stu.Size = new System.Drawing.Size(75, 23);
            this.Admin_Register_Stu.TabIndex = 47;
            this.Admin_Register_Stu.Text = "注册";
            this.Admin_Register_Stu.UseVisualStyleBackColor = true;
            this.Admin_Register_Stu.Click += new System.EventHandler(this.Admin_Register_Stu_Click);
            // 
            // label86
            // 
            this.label86.AutoSize = true;
            this.label86.Font = new System.Drawing.Font("宋体", 12F);
            this.label86.Location = new System.Drawing.Point(16, 108);
            this.label86.Name = "label86";
            this.label86.Size = new System.Drawing.Size(69, 20);
            this.label86.TabIndex = 43;
            this.label86.Text = "姓名：";
            // 
            // Admin_Stu_Sdept
            // 
            this.Admin_Stu_Sdept.Location = new System.Drawing.Point(77, 133);
            this.Admin_Stu_Sdept.Name = "Admin_Stu_Sdept";
            this.Admin_Stu_Sdept.Size = new System.Drawing.Size(152, 25);
            this.Admin_Stu_Sdept.TabIndex = 46;
            // 
            // Admin_Stu_Name
            // 
            this.Admin_Stu_Name.Location = new System.Drawing.Point(77, 103);
            this.Admin_Stu_Name.Name = "Admin_Stu_Name";
            this.Admin_Stu_Name.Size = new System.Drawing.Size(152, 25);
            this.Admin_Stu_Name.TabIndex = 45;
            // 
            // label92
            // 
            this.label92.AutoSize = true;
            this.label92.Font = new System.Drawing.Font("宋体", 12F);
            this.label92.Location = new System.Drawing.Point(16, 138);
            this.label92.Name = "label92";
            this.label92.Size = new System.Drawing.Size(69, 20);
            this.label92.TabIndex = 44;
            this.label92.Text = "学院：";
            // 
            // Admin_Stu_Password
            // 
            this.Admin_Stu_Password.Location = new System.Drawing.Point(77, 75);
            this.Admin_Stu_Password.Name = "Admin_Stu_Password";
            this.Admin_Stu_Password.PasswordChar = '*';
            this.Admin_Stu_Password.Size = new System.Drawing.Size(152, 25);
            this.Admin_Stu_Password.TabIndex = 42;
            // 
            // label85
            // 
            this.label85.AutoSize = true;
            this.label85.Font = new System.Drawing.Font("宋体", 12F);
            this.label85.Location = new System.Drawing.Point(15, 78);
            this.label85.Name = "label85";
            this.label85.Size = new System.Drawing.Size(69, 20);
            this.label85.TabIndex = 41;
            this.label85.Text = "密码：";
            // 
            // label83
            // 
            this.label83.AutoSize = true;
            this.label83.Font = new System.Drawing.Font("宋体", 12F);
            this.label83.Location = new System.Drawing.Point(15, 51);
            this.label83.Name = "label83";
            this.label83.Size = new System.Drawing.Size(69, 20);
            this.label83.TabIndex = 39;
            this.label83.Text = "学号：";
            // 
            // Admin_Sno
            // 
            this.Admin_Sno.Location = new System.Drawing.Point(77, 46);
            this.Admin_Sno.Name = "Admin_Sno";
            this.Admin_Sno.Size = new System.Drawing.Size(152, 25);
            this.Admin_Sno.TabIndex = 40;
            // 
            // Admin_New_Again
            // 
            this.Admin_New_Again.Location = new System.Drawing.Point(93, 412);
            this.Admin_New_Again.Name = "Admin_New_Again";
            this.Admin_New_Again.PasswordChar = '*';
            this.Admin_New_Again.Size = new System.Drawing.Size(100, 25);
            this.Admin_New_Again.TabIndex = 30;
            // 
            // label84
            // 
            this.label84.AutoSize = true;
            this.label84.Font = new System.Drawing.Font("宋体", 12F);
            this.label84.Location = new System.Drawing.Point(16, 415);
            this.label84.Name = "label84";
            this.label84.Size = new System.Drawing.Size(109, 20);
            this.label84.TabIndex = 29;
            this.label84.Text = "再次输入：";
            // 
            // Admin_Confirm
            // 
            this.Admin_Confirm.Location = new System.Drawing.Point(18, 434);
            this.Admin_Confirm.Name = "Admin_Confirm";
            this.Admin_Confirm.Size = new System.Drawing.Size(54, 23);
            this.Admin_Confirm.TabIndex = 28;
            this.Admin_Confirm.Text = "确认";
            this.Admin_Confirm.UseVisualStyleBackColor = true;
            this.Admin_Confirm.Click += new System.EventHandler(this.Admin_Confirm_Click);
            // 
            // label87
            // 
            this.label87.AutoSize = true;
            this.label87.Font = new System.Drawing.Font("宋体", 12F);
            this.label87.Location = new System.Drawing.Point(17, 387);
            this.label87.Name = "label87";
            this.label87.Size = new System.Drawing.Size(89, 20);
            this.label87.TabIndex = 27;
            this.label87.Text = "新密码：";
            // 
            // Admin_New
            // 
            this.Admin_New.Location = new System.Drawing.Point(93, 382);
            this.Admin_New.Name = "Admin_New";
            this.Admin_New.PasswordChar = '*';
            this.Admin_New.Size = new System.Drawing.Size(100, 25);
            this.Admin_New.TabIndex = 26;
            // 
            // label88
            // 
            this.label88.AutoSize = true;
            this.label88.BackColor = System.Drawing.SystemColors.GradientActiveCaption;
            this.label88.Location = new System.Drawing.Point(20, 357);
            this.label88.Name = "label88";
            this.label88.Size = new System.Drawing.Size(299, 15);
            this.label88.TabIndex = 25;
            this.label88.Text = "修改密码-----------------------------";
            // 
            // label89
            // 
            this.label89.AutoSize = true;
            this.label89.Location = new System.Drawing.Point(33, 26);
            this.label89.Name = "label89";
            this.label89.Size = new System.Drawing.Size(0, 15);
            this.label89.TabIndex = 2;
            // 
            // label91
            // 
            this.label91.AutoSize = true;
            this.label91.BackColor = System.Drawing.SystemColors.GradientActiveCaption;
            this.label91.Location = new System.Drawing.Point(16, 25);
            this.label91.Name = "label91";
            this.label91.Size = new System.Drawing.Size(450, 15);
            this.label91.TabIndex = 9;
            this.label91.Text = "添加学生帐号和重置学生密码-------------------------------";
            // 
            // lab1_addr
            // 
            this.lab1_addr.AutoSize = true;
            this.lab1_addr.Location = new System.Drawing.Point(88, 41);
            this.lab1_addr.Name = "lab1_addr";
            this.lab1_addr.Size = new System.Drawing.Size(92, 15);
            this.lab1_addr.TabIndex = 164;
            this.lab1_addr.Text = "先骕楼X4313";
            // 
            // lab1_name
            // 
            this.lab1_name.AutoSize = true;
            this.lab1_name.Location = new System.Drawing.Point(88, 12);
            this.lab1_name.Name = "lab1_name";
            this.lab1_name.Size = new System.Drawing.Size(37, 15);
            this.lab1_name.TabIndex = 163;
            this.lab1_name.Text = "机房";
            // 
            // label54
            // 
            this.label54.AutoSize = true;
            this.label54.Location = new System.Drawing.Point(6, 41);
            this.label54.Name = "label54";
            this.label54.Size = new System.Drawing.Size(84, 15);
            this.label54.TabIndex = 162;
            this.label54.Text = "地    址：";
            // 
            // label55
            // 
            this.label55.AutoSize = true;
            this.label55.Location = new System.Drawing.Point(6, 12);
            this.label55.Name = "label55";
            this.label55.Size = new System.Drawing.Size(82, 15);
            this.label55.TabIndex = 161;
            this.label55.Text = "实验室名：";
            // 
            // Lab1_04
            // 
            this.Lab1_04.BackColor = System.Drawing.Color.Green;
            this.Lab1_04.Location = new System.Drawing.Point(259, 120);
            this.Lab1_04.Name = "Lab1_04";
            this.Lab1_04.Size = new System.Drawing.Size(66, 28);
            this.Lab1_04.TabIndex = 160;
            this.Lab1_04.Tag = "lab1";
            this.Lab1_04.Text = "预约";
            this.Lab1_04.UseVisualStyleBackColor = false;
            this.Lab1_04.Click += new System.EventHandler(this.Lab1_04_Click);
            // 
            // Lab1_02
            // 
            this.Lab1_02.BackColor = System.Drawing.Color.Green;
            this.Lab1_02.Location = new System.Drawing.Point(91, 120);
            this.Lab1_02.Name = "Lab1_02";
            this.Lab1_02.Size = new System.Drawing.Size(68, 28);
            this.Lab1_02.TabIndex = 159;
            this.Lab1_02.Tag = "lab1";
            this.Lab1_02.Text = "预约";
            this.Lab1_02.UseVisualStyleBackColor = false;
            this.Lab1_02.Click += new System.EventHandler(this.Lab1_02_Click);
            // 
            // Lab1_01
            // 
            this.Lab1_01.BackColor = System.Drawing.Color.Green;
            this.Lab1_01.Location = new System.Drawing.Point(8, 120);
            this.Lab1_01.Name = "Lab1_01";
            this.Lab1_01.Size = new System.Drawing.Size(68, 28);
            this.Lab1_01.TabIndex = 158;
            this.Lab1_01.Tag = "lab1";
            this.Lab1_01.Text = "预约";
            this.Lab1_01.UseVisualStyleBackColor = false;
            this.Lab1_01.Click += new System.EventHandler(this.Lab1_01_Click);
            // 
            // label26
            // 
            this.label26.AutoSize = true;
            this.label26.Location = new System.Drawing.Point(382, 253);
            this.label26.Name = "label26";
            this.label26.Size = new System.Drawing.Size(23, 15);
            this.label26.TabIndex = 157;
            this.label26.Text = "15";
            // 
            // Lab1_15
            // 
            this.Lab1_15.BackColor = System.Drawing.Color.Green;
            this.Lab1_15.Location = new System.Drawing.Point(343, 293);
            this.Lab1_15.Name = "Lab1_15";
            this.Lab1_15.Size = new System.Drawing.Size(66, 28);
            this.Lab1_15.TabIndex = 156;
            this.Lab1_15.Tag = "lab1";
            this.Lab1_15.Text = "预约";
            this.Lab1_15.UseVisualStyleBackColor = false;
            this.Lab1_15.Click += new System.EventHandler(this.Lab1_15_Click);
            // 
            // pictureBox26
            // 
            this.pictureBox26.Image = global::WindowsFormsApp1.Properties.Resources.pc12;
            this.pictureBox26.Location = new System.Drawing.Point(343, 250);
            this.pictureBox26.Name = "pictureBox26";
            this.pictureBox26.Size = new System.Drawing.Size(32, 34);
            this.pictureBox26.TabIndex = 155;
            this.pictureBox26.TabStop = false;
            // 
            // label27
            // 
            this.label27.AutoSize = true;
            this.label27.Location = new System.Drawing.Point(303, 253);
            this.label27.Name = "label27";
            this.label27.Size = new System.Drawing.Size(23, 15);
            this.label27.TabIndex = 154;
            this.label27.Text = "14";
            // 
            // Lab1_14
            // 
            this.Lab1_14.BackColor = System.Drawing.Color.Green;
            this.Lab1_14.Location = new System.Drawing.Point(260, 293);
            this.Lab1_14.Name = "Lab1_14";
            this.Lab1_14.Size = new System.Drawing.Size(66, 28);
            this.Lab1_14.TabIndex = 153;
            this.Lab1_14.Tag = "lab1";
            this.Lab1_14.Text = "预约";
            this.Lab1_14.UseVisualStyleBackColor = false;
            this.Lab1_14.Click += new System.EventHandler(this.Lab1_14_Click);
            // 
            // pictureBox27
            // 
            this.pictureBox27.Image = global::WindowsFormsApp1.Properties.Resources.pc12;
            this.pictureBox27.Location = new System.Drawing.Point(260, 250);
            this.pictureBox27.Name = "pictureBox27";
            this.pictureBox27.Size = new System.Drawing.Size(32, 34);
            this.pictureBox27.TabIndex = 152;
            this.pictureBox27.TabStop = false;
            // 
            // label28
            // 
            this.label28.AutoSize = true;
            this.label28.Location = new System.Drawing.Point(215, 253);
            this.label28.Name = "label28";
            this.label28.Size = new System.Drawing.Size(23, 15);
            this.label28.TabIndex = 151;
            this.label28.Text = "13";
            // 
            // Lab1_13
            // 
            this.Lab1_13.BackColor = System.Drawing.Color.Green;
            this.Lab1_13.Location = new System.Drawing.Point(172, 293);
            this.Lab1_13.Name = "Lab1_13";
            this.Lab1_13.Size = new System.Drawing.Size(66, 28);
            this.Lab1_13.TabIndex = 150;
            this.Lab1_13.Tag = "lab1";
            this.Lab1_13.Text = "预约";
            this.Lab1_13.UseVisualStyleBackColor = false;
            this.Lab1_13.Click += new System.EventHandler(this.Lab1_13_Click);
            // 
            // label29
            // 
            this.label29.AutoSize = true;
            this.label29.Location = new System.Drawing.Point(137, 253);
            this.label29.Name = "label29";
            this.label29.Size = new System.Drawing.Size(23, 15);
            this.label29.TabIndex = 149;
            this.label29.Text = "12";
            // 
            // pictureBox28
            // 
            this.pictureBox28.Image = global::WindowsFormsApp1.Properties.Resources.pc12;
            this.pictureBox28.Location = new System.Drawing.Point(177, 250);
            this.pictureBox28.Name = "pictureBox28";
            this.pictureBox28.Size = new System.Drawing.Size(37, 34);
            this.pictureBox28.TabIndex = 148;
            this.pictureBox28.TabStop = false;
            // 
            // pictureBox29
            // 
            this.pictureBox29.Image = global::WindowsFormsApp1.Properties.Resources.pc12;
            this.pictureBox29.Location = new System.Drawing.Point(94, 253);
            this.pictureBox29.Name = "pictureBox29";
            this.pictureBox29.Size = new System.Drawing.Size(31, 31);
            this.pictureBox29.TabIndex = 147;
            this.pictureBox29.TabStop = false;
            // 
            // Lab1_12
            // 
            this.Lab1_12.BackColor = System.Drawing.Color.Green;
            this.Lab1_12.Location = new System.Drawing.Point(94, 293);
            this.Lab1_12.Name = "Lab1_12";
            this.Lab1_12.Size = new System.Drawing.Size(66, 28);
            this.Lab1_12.TabIndex = 146;
            this.Lab1_12.Tag = "lab1";
            this.Lab1_12.Text = "预约";
            this.Lab1_12.UseVisualStyleBackColor = false;
            this.Lab1_12.Click += new System.EventHandler(this.Lab1_12_Click);
            // 
            // label30
            // 
            this.label30.AutoSize = true;
            this.label30.Location = new System.Drawing.Point(49, 253);
            this.label30.Name = "label30";
            this.label30.Size = new System.Drawing.Size(23, 15);
            this.label30.TabIndex = 145;
            this.label30.Text = "11";
            // 
            // Lab1_11
            // 
            this.Lab1_11.BackColor = System.Drawing.Color.Green;
            this.Lab1_11.Location = new System.Drawing.Point(8, 293);
            this.Lab1_11.Name = "Lab1_11";
            this.Lab1_11.Size = new System.Drawing.Size(66, 28);
            this.Lab1_11.TabIndex = 144;
            this.Lab1_11.Tag = "lab1";
            this.Lab1_11.Text = "预约";
            this.Lab1_11.UseVisualStyleBackColor = false;
            this.Lab1_11.Click += new System.EventHandler(this.Lab1_11_Click);
            // 
            // pictureBox30
            // 
            this.pictureBox30.Image = global::WindowsFormsApp1.Properties.Resources.pc12;
            this.pictureBox30.Location = new System.Drawing.Point(11, 253);
            this.pictureBox30.Name = "pictureBox30";
            this.pictureBox30.Size = new System.Drawing.Size(32, 34);
            this.pictureBox30.TabIndex = 143;
            this.pictureBox30.TabStop = false;
            // 
            // label31
            // 
            this.label31.AutoSize = true;
            this.label31.Location = new System.Drawing.Point(381, 330);
            this.label31.Name = "label31";
            this.label31.Size = new System.Drawing.Size(23, 15);
            this.label31.TabIndex = 142;
            this.label31.Text = "20";
            // 
            // Lab1_20
            // 
            this.Lab1_20.BackColor = System.Drawing.Color.Green;
            this.Lab1_20.Location = new System.Drawing.Point(342, 370);
            this.Lab1_20.Name = "Lab1_20";
            this.Lab1_20.Size = new System.Drawing.Size(66, 28);
            this.Lab1_20.TabIndex = 141;
            this.Lab1_20.Tag = "lab1";
            this.Lab1_20.Text = "预约";
            this.Lab1_20.UseVisualStyleBackColor = false;
            this.Lab1_20.Click += new System.EventHandler(this.Lab1_20_Click);
            // 
            // pictureBox31
            // 
            this.pictureBox31.Image = global::WindowsFormsApp1.Properties.Resources.pc12;
            this.pictureBox31.Location = new System.Drawing.Point(342, 327);
            this.pictureBox31.Name = "pictureBox31";
            this.pictureBox31.Size = new System.Drawing.Size(32, 34);
            this.pictureBox31.TabIndex = 140;
            this.pictureBox31.TabStop = false;
            // 
            // label32
            // 
            this.label32.AutoSize = true;
            this.label32.Location = new System.Drawing.Point(302, 330);
            this.label32.Name = "label32";
            this.label32.Size = new System.Drawing.Size(23, 15);
            this.label32.TabIndex = 139;
            this.label32.Text = "19";
            // 
            // Lab1_19
            // 
            this.Lab1_19.BackColor = System.Drawing.Color.Green;
            this.Lab1_19.Location = new System.Drawing.Point(259, 370);
            this.Lab1_19.Name = "Lab1_19";
            this.Lab1_19.Size = new System.Drawing.Size(66, 28);
            this.Lab1_19.TabIndex = 138;
            this.Lab1_19.Tag = "lab1";
            this.Lab1_19.Text = "预约";
            this.Lab1_19.UseVisualStyleBackColor = false;
            this.Lab1_19.Click += new System.EventHandler(this.Lab1_19_Click);
            // 
            // pictureBox32
            // 
            this.pictureBox32.Image = global::WindowsFormsApp1.Properties.Resources.pc12;
            this.pictureBox32.Location = new System.Drawing.Point(259, 327);
            this.pictureBox32.Name = "pictureBox32";
            this.pictureBox32.Size = new System.Drawing.Size(32, 34);
            this.pictureBox32.TabIndex = 137;
            this.pictureBox32.TabStop = false;
            // 
            // label33
            // 
            this.label33.AutoSize = true;
            this.label33.Location = new System.Drawing.Point(214, 330);
            this.label33.Name = "label33";
            this.label33.Size = new System.Drawing.Size(23, 15);
            this.label33.TabIndex = 136;
            this.label33.Text = "18";
            // 
            // Lab1_18
            // 
            this.Lab1_18.BackColor = System.Drawing.Color.Green;
            this.Lab1_18.Location = new System.Drawing.Point(171, 370);
            this.Lab1_18.Name = "Lab1_18";
            this.Lab1_18.Size = new System.Drawing.Size(66, 28);
            this.Lab1_18.TabIndex = 135;
            this.Lab1_18.Tag = "lab1";
            this.Lab1_18.Text = "预约";
            this.Lab1_18.UseVisualStyleBackColor = false;
            this.Lab1_18.Click += new System.EventHandler(this.Lab1_18_Click);
            // 
            // label34
            // 
            this.label34.AutoSize = true;
            this.label34.Location = new System.Drawing.Point(136, 330);
            this.label34.Name = "label34";
            this.label34.Size = new System.Drawing.Size(23, 15);
            this.label34.TabIndex = 134;
            this.label34.Text = "17";
            // 
            // pictureBox33
            // 
            this.pictureBox33.Image = global::WindowsFormsApp1.Properties.Resources.pc12;
            this.pictureBox33.Location = new System.Drawing.Point(176, 327);
            this.pictureBox33.Name = "pictureBox33";
            this.pictureBox33.Size = new System.Drawing.Size(37, 34);
            this.pictureBox33.TabIndex = 133;
            this.pictureBox33.TabStop = false;
            // 
            // pictureBox34
            // 
            this.pictureBox34.Image = global::WindowsFormsApp1.Properties.Resources.pc12;
            this.pictureBox34.Location = new System.Drawing.Point(93, 330);
            this.pictureBox34.Name = "pictureBox34";
            this.pictureBox34.Size = new System.Drawing.Size(31, 31);
            this.pictureBox34.TabIndex = 132;
            this.pictureBox34.TabStop = false;
            // 
            // Lab1_17
            // 
            this.Lab1_17.BackColor = System.Drawing.Color.Green;
            this.Lab1_17.Location = new System.Drawing.Point(93, 370);
            this.Lab1_17.Name = "Lab1_17";
            this.Lab1_17.Size = new System.Drawing.Size(66, 28);
            this.Lab1_17.TabIndex = 131;
            this.Lab1_17.Tag = "lab1";
            this.Lab1_17.Text = "预约";
            this.Lab1_17.UseVisualStyleBackColor = false;
            this.Lab1_17.Click += new System.EventHandler(this.Lab1_17_Click);
            // 
            // label35
            // 
            this.label35.AutoSize = true;
            this.label35.Location = new System.Drawing.Point(48, 330);
            this.label35.Name = "label35";
            this.label35.Size = new System.Drawing.Size(23, 15);
            this.label35.TabIndex = 130;
            this.label35.Text = "16";
            // 
            // Lab1_16
            // 
            this.Lab1_16.BackColor = System.Drawing.Color.Green;
            this.Lab1_16.Location = new System.Drawing.Point(7, 370);
            this.Lab1_16.Name = "Lab1_16";
            this.Lab1_16.Size = new System.Drawing.Size(66, 28);
            this.Lab1_16.TabIndex = 129;
            this.Lab1_16.Tag = "lab1";
            this.Lab1_16.Text = "预约";
            this.Lab1_16.UseVisualStyleBackColor = false;
            this.Lab1_16.Click += new System.EventHandler(this.Lab1_16_Click);
            // 
            // pictureBox35
            // 
            this.pictureBox35.Image = global::WindowsFormsApp1.Properties.Resources.pc12;
            this.pictureBox35.Location = new System.Drawing.Point(10, 330);
            this.pictureBox35.Name = "pictureBox35";
            this.pictureBox35.Size = new System.Drawing.Size(32, 34);
            this.pictureBox35.TabIndex = 128;
            this.pictureBox35.TabStop = false;
            // 
            // label36
            // 
            this.label36.AutoSize = true;
            this.label36.Location = new System.Drawing.Point(384, 412);
            this.label36.Name = "label36";
            this.label36.Size = new System.Drawing.Size(23, 15);
            this.label36.TabIndex = 127;
            this.label36.Text = "25";
            // 
            // Lab1_25
            // 
            this.Lab1_25.BackColor = System.Drawing.Color.Green;
            this.Lab1_25.Location = new System.Drawing.Point(345, 452);
            this.Lab1_25.Name = "Lab1_25";
            this.Lab1_25.Size = new System.Drawing.Size(66, 28);
            this.Lab1_25.TabIndex = 126;
            this.Lab1_25.Tag = "lab1";
            this.Lab1_25.Text = "预约";
            this.Lab1_25.UseVisualStyleBackColor = false;
            this.Lab1_25.Click += new System.EventHandler(this.Lab1_25_Click);
            // 
            // pictureBox36
            // 
            this.pictureBox36.Image = global::WindowsFormsApp1.Properties.Resources.pc12;
            this.pictureBox36.Location = new System.Drawing.Point(345, 409);
            this.pictureBox36.Name = "pictureBox36";
            this.pictureBox36.Size = new System.Drawing.Size(32, 34);
            this.pictureBox36.TabIndex = 125;
            this.pictureBox36.TabStop = false;
            // 
            // label37
            // 
            this.label37.AutoSize = true;
            this.label37.Location = new System.Drawing.Point(305, 412);
            this.label37.Name = "label37";
            this.label37.Size = new System.Drawing.Size(23, 15);
            this.label37.TabIndex = 124;
            this.label37.Text = "24";
            // 
            // Lab1_24
            // 
            this.Lab1_24.BackColor = System.Drawing.Color.Green;
            this.Lab1_24.Location = new System.Drawing.Point(262, 452);
            this.Lab1_24.Name = "Lab1_24";
            this.Lab1_24.Size = new System.Drawing.Size(66, 28);
            this.Lab1_24.TabIndex = 123;
            this.Lab1_24.Tag = "lab1";
            this.Lab1_24.Text = "预约";
            this.Lab1_24.UseVisualStyleBackColor = false;
            this.Lab1_24.Click += new System.EventHandler(this.Lab1_24_Click);
            // 
            // pictureBox37
            // 
            this.pictureBox37.Image = global::WindowsFormsApp1.Properties.Resources.pc12;
            this.pictureBox37.Location = new System.Drawing.Point(262, 409);
            this.pictureBox37.Name = "pictureBox37";
            this.pictureBox37.Size = new System.Drawing.Size(32, 34);
            this.pictureBox37.TabIndex = 122;
            this.pictureBox37.TabStop = false;
            // 
            // label38
            // 
            this.label38.AutoSize = true;
            this.label38.Location = new System.Drawing.Point(217, 412);
            this.label38.Name = "label38";
            this.label38.Size = new System.Drawing.Size(23, 15);
            this.label38.TabIndex = 121;
            this.label38.Text = "23";
            // 
            // Lab1_23
            // 
            this.Lab1_23.BackColor = System.Drawing.Color.Green;
            this.Lab1_23.Location = new System.Drawing.Point(174, 452);
            this.Lab1_23.Name = "Lab1_23";
            this.Lab1_23.Size = new System.Drawing.Size(66, 28);
            this.Lab1_23.TabIndex = 120;
            this.Lab1_23.Tag = "lab1";
            this.Lab1_23.Text = "预约";
            this.Lab1_23.UseVisualStyleBackColor = false;
            this.Lab1_23.Click += new System.EventHandler(this.Lab1_23_Click);
            // 
            // label39
            // 
            this.label39.AutoSize = true;
            this.label39.Location = new System.Drawing.Point(139, 412);
            this.label39.Name = "label39";
            this.label39.Size = new System.Drawing.Size(23, 15);
            this.label39.TabIndex = 119;
            this.label39.Text = "22";
            // 
            // pictureBox38
            // 
            this.pictureBox38.Image = global::WindowsFormsApp1.Properties.Resources.pc12;
            this.pictureBox38.Location = new System.Drawing.Point(179, 409);
            this.pictureBox38.Name = "pictureBox38";
            this.pictureBox38.Size = new System.Drawing.Size(37, 34);
            this.pictureBox38.TabIndex = 118;
            this.pictureBox38.TabStop = false;
            // 
            // pictureBox39
            // 
            this.pictureBox39.Image = global::WindowsFormsApp1.Properties.Resources.pc12;
            this.pictureBox39.Location = new System.Drawing.Point(96, 412);
            this.pictureBox39.Name = "pictureBox39";
            this.pictureBox39.Size = new System.Drawing.Size(31, 31);
            this.pictureBox39.TabIndex = 117;
            this.pictureBox39.TabStop = false;
            // 
            // Lab1_22
            // 
            this.Lab1_22.BackColor = System.Drawing.Color.Green;
            this.Lab1_22.Location = new System.Drawing.Point(96, 452);
            this.Lab1_22.Name = "Lab1_22";
            this.Lab1_22.Size = new System.Drawing.Size(66, 28);
            this.Lab1_22.TabIndex = 116;
            this.Lab1_22.Tag = "lab1";
            this.Lab1_22.Text = "预约";
            this.Lab1_22.UseVisualStyleBackColor = false;
            this.Lab1_22.Click += new System.EventHandler(this.Lab1_22_Click);
            // 
            // label40
            // 
            this.label40.AutoSize = true;
            this.label40.Location = new System.Drawing.Point(51, 412);
            this.label40.Name = "label40";
            this.label40.Size = new System.Drawing.Size(23, 15);
            this.label40.TabIndex = 115;
            this.label40.Text = "21";
            // 
            // Lab1_21
            // 
            this.Lab1_21.BackColor = System.Drawing.Color.Green;
            this.Lab1_21.Location = new System.Drawing.Point(10, 452);
            this.Lab1_21.Name = "Lab1_21";
            this.Lab1_21.Size = new System.Drawing.Size(66, 28);
            this.Lab1_21.TabIndex = 114;
            this.Lab1_21.Tag = "lab1";
            this.Lab1_21.Text = "预约";
            this.Lab1_21.UseVisualStyleBackColor = false;
            this.Lab1_21.Click += new System.EventHandler(this.Lab1_21_Click);
            // 
            // pictureBox40
            // 
            this.pictureBox40.Image = global::WindowsFormsApp1.Properties.Resources.pc12;
            this.pictureBox40.Location = new System.Drawing.Point(13, 412);
            this.pictureBox40.Name = "pictureBox40";
            this.pictureBox40.Size = new System.Drawing.Size(32, 34);
            this.pictureBox40.TabIndex = 113;
            this.pictureBox40.TabStop = false;
            // 
            // label41
            // 
            this.label41.AutoSize = true;
            this.label41.Location = new System.Drawing.Point(381, 169);
            this.label41.Name = "label41";
            this.label41.Size = new System.Drawing.Size(23, 15);
            this.label41.TabIndex = 112;
            this.label41.Text = "10";
            // 
            // Lab1_10
            // 
            this.Lab1_10.BackColor = System.Drawing.Color.Green;
            this.Lab1_10.Location = new System.Drawing.Point(342, 209);
            this.Lab1_10.Name = "Lab1_10";
            this.Lab1_10.Size = new System.Drawing.Size(66, 28);
            this.Lab1_10.TabIndex = 111;
            this.Lab1_10.Tag = "lab1";
            this.Lab1_10.Text = "预约";
            this.Lab1_10.UseVisualStyleBackColor = false;
            this.Lab1_10.Click += new System.EventHandler(this.Lab1_10_Click);
            // 
            // pictureBox41
            // 
            this.pictureBox41.Image = global::WindowsFormsApp1.Properties.Resources.pc12;
            this.pictureBox41.Location = new System.Drawing.Point(342, 166);
            this.pictureBox41.Name = "pictureBox41";
            this.pictureBox41.Size = new System.Drawing.Size(32, 34);
            this.pictureBox41.TabIndex = 110;
            this.pictureBox41.TabStop = false;
            // 
            // label42
            // 
            this.label42.AutoSize = true;
            this.label42.Location = new System.Drawing.Point(302, 169);
            this.label42.Name = "label42";
            this.label42.Size = new System.Drawing.Size(23, 15);
            this.label42.TabIndex = 109;
            this.label42.Text = "09";
            // 
            // Lab1_09
            // 
            this.Lab1_09.BackColor = System.Drawing.Color.Green;
            this.Lab1_09.Location = new System.Drawing.Point(259, 209);
            this.Lab1_09.Name = "Lab1_09";
            this.Lab1_09.Size = new System.Drawing.Size(66, 28);
            this.Lab1_09.TabIndex = 108;
            this.Lab1_09.Tag = "lab1";
            this.Lab1_09.Text = "预约";
            this.Lab1_09.UseVisualStyleBackColor = false;
            this.Lab1_09.Click += new System.EventHandler(this.Lab1_09_Click);
            // 
            // pictureBox42
            // 
            this.pictureBox42.Image = global::WindowsFormsApp1.Properties.Resources.pc12;
            this.pictureBox42.Location = new System.Drawing.Point(259, 166);
            this.pictureBox42.Name = "pictureBox42";
            this.pictureBox42.Size = new System.Drawing.Size(32, 34);
            this.pictureBox42.TabIndex = 107;
            this.pictureBox42.TabStop = false;
            // 
            // label43
            // 
            this.label43.AutoSize = true;
            this.label43.Location = new System.Drawing.Point(214, 169);
            this.label43.Name = "label43";
            this.label43.Size = new System.Drawing.Size(23, 15);
            this.label43.TabIndex = 106;
            this.label43.Text = "08";
            // 
            // Lab1_08
            // 
            this.Lab1_08.BackColor = System.Drawing.Color.Green;
            this.Lab1_08.Location = new System.Drawing.Point(171, 209);
            this.Lab1_08.Name = "Lab1_08";
            this.Lab1_08.Size = new System.Drawing.Size(66, 28);
            this.Lab1_08.TabIndex = 105;
            this.Lab1_08.Tag = "lab1";
            this.Lab1_08.Text = "预约";
            this.Lab1_08.UseVisualStyleBackColor = false;
            this.Lab1_08.Click += new System.EventHandler(this.Lab1_08_Click);
            // 
            // label44
            // 
            this.label44.AutoSize = true;
            this.label44.Location = new System.Drawing.Point(136, 169);
            this.label44.Name = "label44";
            this.label44.Size = new System.Drawing.Size(23, 15);
            this.label44.TabIndex = 104;
            this.label44.Text = "07";
            // 
            // pictureBox43
            // 
            this.pictureBox43.Image = global::WindowsFormsApp1.Properties.Resources.pc12;
            this.pictureBox43.Location = new System.Drawing.Point(176, 166);
            this.pictureBox43.Name = "pictureBox43";
            this.pictureBox43.Size = new System.Drawing.Size(37, 34);
            this.pictureBox43.TabIndex = 103;
            this.pictureBox43.TabStop = false;
            // 
            // pictureBox44
            // 
            this.pictureBox44.Image = global::WindowsFormsApp1.Properties.Resources.pc12;
            this.pictureBox44.Location = new System.Drawing.Point(93, 169);
            this.pictureBox44.Name = "pictureBox44";
            this.pictureBox44.Size = new System.Drawing.Size(31, 31);
            this.pictureBox44.TabIndex = 102;
            this.pictureBox44.TabStop = false;
            // 
            // Lab1_07
            // 
            this.Lab1_07.BackColor = System.Drawing.Color.Green;
            this.Lab1_07.Location = new System.Drawing.Point(93, 209);
            this.Lab1_07.Name = "Lab1_07";
            this.Lab1_07.Size = new System.Drawing.Size(66, 28);
            this.Lab1_07.TabIndex = 101;
            this.Lab1_07.Tag = "lab1";
            this.Lab1_07.Text = "预约";
            this.Lab1_07.UseVisualStyleBackColor = false;
            this.Lab1_07.Click += new System.EventHandler(this.Lab1_07_Click);
            // 
            // label45
            // 
            this.label45.AutoSize = true;
            this.label45.Location = new System.Drawing.Point(48, 169);
            this.label45.Name = "label45";
            this.label45.Size = new System.Drawing.Size(23, 15);
            this.label45.TabIndex = 100;
            this.label45.Text = "06";
            // 
            // Lab1_06
            // 
            this.Lab1_06.BackColor = System.Drawing.Color.Green;
            this.Lab1_06.Location = new System.Drawing.Point(7, 209);
            this.Lab1_06.Name = "Lab1_06";
            this.Lab1_06.Size = new System.Drawing.Size(66, 28);
            this.Lab1_06.TabIndex = 99;
            this.Lab1_06.Tag = "lab1";
            this.Lab1_06.Text = "预约";
            this.Lab1_06.UseVisualStyleBackColor = false;
            this.Lab1_06.Click += new System.EventHandler(this.Lab1_06_Click);
            // 
            // pictureBox45
            // 
            this.pictureBox45.Image = global::WindowsFormsApp1.Properties.Resources.pc12;
            this.pictureBox45.Location = new System.Drawing.Point(10, 169);
            this.pictureBox45.Name = "pictureBox45";
            this.pictureBox45.Size = new System.Drawing.Size(32, 34);
            this.pictureBox45.TabIndex = 98;
            this.pictureBox45.TabStop = false;
            // 
            // label46
            // 
            this.label46.AutoSize = true;
            this.label46.Location = new System.Drawing.Point(385, 83);
            this.label46.Name = "label46";
            this.label46.Size = new System.Drawing.Size(23, 15);
            this.label46.TabIndex = 97;
            this.label46.Text = "05";
            // 
            // Lab1_05
            // 
            this.Lab1_05.BackColor = System.Drawing.Color.Green;
            this.Lab1_05.Location = new System.Drawing.Point(342, 120);
            this.Lab1_05.Name = "Lab1_05";
            this.Lab1_05.Size = new System.Drawing.Size(66, 28);
            this.Lab1_05.TabIndex = 96;
            this.Lab1_05.Tag = "lab1";
            this.Lab1_05.Text = "预约";
            this.Lab1_05.UseVisualStyleBackColor = false;
            this.Lab1_05.Click += new System.EventHandler(this.Lab1_05_Click);
            // 
            // label47
            // 
            this.label47.AutoSize = true;
            this.label47.Location = new System.Drawing.Point(302, 83);
            this.label47.Name = "label47";
            this.label47.Size = new System.Drawing.Size(23, 15);
            this.label47.TabIndex = 95;
            this.label47.Text = "04";
            // 
            // pictureBox46
            // 
            this.pictureBox46.Image = global::WindowsFormsApp1.Properties.Resources.pc12;
            this.pictureBox46.Location = new System.Drawing.Point(342, 83);
            this.pictureBox46.Name = "pictureBox46";
            this.pictureBox46.Size = new System.Drawing.Size(37, 34);
            this.pictureBox46.TabIndex = 94;
            this.pictureBox46.TabStop = false;
            // 
            // pictureBox47
            // 
            this.pictureBox47.Image = global::WindowsFormsApp1.Properties.Resources.pc12;
            this.pictureBox47.Location = new System.Drawing.Point(259, 83);
            this.pictureBox47.Name = "pictureBox47";
            this.pictureBox47.Size = new System.Drawing.Size(31, 31);
            this.pictureBox47.TabIndex = 93;
            this.pictureBox47.TabStop = false;
            // 
            // label48
            // 
            this.label48.AutoSize = true;
            this.label48.Location = new System.Drawing.Point(214, 80);
            this.label48.Name = "label48";
            this.label48.Size = new System.Drawing.Size(23, 15);
            this.label48.TabIndex = 91;
            this.label48.Text = "03";
            // 
            // Lab1_03
            // 
            this.Lab1_03.BackColor = System.Drawing.Color.Green;
            this.Lab1_03.Location = new System.Drawing.Point(171, 120);
            this.Lab1_03.Name = "Lab1_03";
            this.Lab1_03.Size = new System.Drawing.Size(66, 28);
            this.Lab1_03.TabIndex = 90;
            this.Lab1_03.Tag = "lab1";
            this.Lab1_03.Text = "预约";
            this.Lab1_03.UseVisualStyleBackColor = false;
            this.Lab1_03.Click += new System.EventHandler(this.Lab1_03_Click);
            // 
            // pictureBox48
            // 
            this.pictureBox48.Image = global::WindowsFormsApp1.Properties.Resources.pc12;
            this.pictureBox48.Location = new System.Drawing.Point(176, 80);
            this.pictureBox48.Name = "pictureBox48";
            this.pictureBox48.Size = new System.Drawing.Size(32, 34);
            this.pictureBox48.TabIndex = 89;
            this.pictureBox48.TabStop = false;
            // 
            // label49
            // 
            this.label49.AutoSize = true;
            this.label49.Location = new System.Drawing.Point(136, 80);
            this.label49.Name = "label49";
            this.label49.Size = new System.Drawing.Size(23, 15);
            this.label49.TabIndex = 88;
            this.label49.Text = "02";
            // 
            // label50
            // 
            this.label50.AutoSize = true;
            this.label50.Location = new System.Drawing.Point(50, 83);
            this.label50.Name = "label50";
            this.label50.Size = new System.Drawing.Size(23, 15);
            this.label50.TabIndex = 86;
            this.label50.Text = "01";
            // 
            // pictureBox49
            // 
            this.pictureBox49.Image = global::WindowsFormsApp1.Properties.Resources.pc12;
            this.pictureBox49.Location = new System.Drawing.Point(93, 80);
            this.pictureBox49.Name = "pictureBox49";
            this.pictureBox49.Size = new System.Drawing.Size(37, 34);
            this.pictureBox49.TabIndex = 85;
            this.pictureBox49.TabStop = false;
            // 
            // pictureBox50
            // 
            this.pictureBox50.Image = global::WindowsFormsApp1.Properties.Resources.pc12;
            this.pictureBox50.Location = new System.Drawing.Point(13, 83);
            this.pictureBox50.Name = "pictureBox50";
            this.pictureBox50.Size = new System.Drawing.Size(31, 31);
            this.pictureBox50.TabIndex = 84;
            this.pictureBox50.TabStop = false;
            // 
            // tabPage2
            // 
            this.tabPage2.Controls.Add(this.lab2_addr);
            this.tabPage2.Controls.Add(this.lab2_name);
            this.tabPage2.Controls.Add(this.label51);
            this.tabPage2.Controls.Add(this.label11);
            this.tabPage2.Controls.Add(this.Lab2_01);
            this.tabPage2.Controls.Add(this.label21);
            this.tabPage2.Controls.Add(this.Lab2_15);
            this.tabPage2.Controls.Add(this.pictureBox21);
            this.tabPage2.Controls.Add(this.label22);
            this.tabPage2.Controls.Add(this.Lab2_14);
            this.tabPage2.Controls.Add(this.pictureBox22);
            this.tabPage2.Controls.Add(this.label23);
            this.tabPage2.Controls.Add(this.Lab2_13);
            this.tabPage2.Controls.Add(this.label24);
            this.tabPage2.Controls.Add(this.pictureBox23);
            this.tabPage2.Controls.Add(this.pictureBox24);
            this.tabPage2.Controls.Add(this.Lab2_12);
            this.tabPage2.Controls.Add(this.label25);
            this.tabPage2.Controls.Add(this.Lab2_11);
            this.tabPage2.Controls.Add(this.pictureBox25);
            this.tabPage2.Controls.Add(this.label16);
            this.tabPage2.Controls.Add(this.Lab2_20);
            this.tabPage2.Controls.Add(this.pictureBox16);
            this.tabPage2.Controls.Add(this.label17);
            this.tabPage2.Controls.Add(this.Lab2_19);
            this.tabPage2.Controls.Add(this.pictureBox17);
            this.tabPage2.Controls.Add(this.label18);
            this.tabPage2.Controls.Add(this.Lab2_18);
            this.tabPage2.Controls.Add(this.label19);
            this.tabPage2.Controls.Add(this.pictureBox18);
            this.tabPage2.Controls.Add(this.pictureBox19);
            this.tabPage2.Controls.Add(this.Lab2_17);
            this.tabPage2.Controls.Add(this.label20);
            this.tabPage2.Controls.Add(this.Lab2_16);
            this.tabPage2.Controls.Add(this.pictureBox20);
            this.tabPage2.Controls.Add(this.label12);
            this.tabPage2.Controls.Add(this.Lab2_24);
            this.tabPage2.Controls.Add(this.pictureBox12);
            this.tabPage2.Controls.Add(this.label13);
            this.tabPage2.Controls.Add(this.Lab2_23);
            this.tabPage2.Controls.Add(this.label14);
            this.tabPage2.Controls.Add(this.pictureBox13);
            this.tabPage2.Controls.Add(this.pictureBox14);
            this.tabPage2.Controls.Add(this.Lab2_22);
            this.tabPage2.Controls.Add(this.label15);
            this.tabPage2.Controls.Add(this.Lab2_21);
            this.tabPage2.Controls.Add(this.pictureBox15);
            this.tabPage2.Controls.Add(this.label10);
            this.tabPage2.Controls.Add(this.Lab2_10);
            this.tabPage2.Controls.Add(this.pictureBox10);
            this.tabPage2.Controls.Add(this.label7);
            this.tabPage2.Controls.Add(this.Lab2_09);
            this.tabPage2.Controls.Add(this.pictureBox7);
            this.tabPage2.Controls.Add(this.label8);
            this.tabPage2.Controls.Add(this.Lab2_08);
            this.tabPage2.Controls.Add(this.label9);
            this.tabPage2.Controls.Add(this.pictureBox8);
            this.tabPage2.Controls.Add(this.pictureBox9);
            this.tabPage2.Controls.Add(this.Lab2_07);
            this.tabPage2.Controls.Add(this.label4);
            this.tabPage2.Controls.Add(this.Lab2_06);
            this.tabPage2.Controls.Add(this.pictureBox4);
            this.tabPage2.Controls.Add(this.label5);
            this.tabPage2.Controls.Add(this.Lab2_05);
            this.tabPage2.Controls.Add(this.label6);
            this.tabPage2.Controls.Add(this.pictureBox5);
            this.tabPage2.Controls.Add(this.pictureBox6);
            this.tabPage2.Controls.Add(this.Lab2_04);
            this.tabPage2.Controls.Add(this.label3);
            this.tabPage2.Controls.Add(this.Lab2_03);
            this.tabPage2.Controls.Add(this.pictureBox3);
            this.tabPage2.Controls.Add(this.label2);
            this.tabPage2.Controls.Add(this.Lab2_02);
            this.tabPage2.Controls.Add(this.label1);
            this.tabPage2.Controls.Add(this.pictureBox2);
            this.tabPage2.Controls.Add(this.pictureBox1);
            this.tabPage2.Location = new System.Drawing.Point(4, 25);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage2.Size = new System.Drawing.Size(427, 497);
            this.tabPage2.TabIndex = 1;
            this.tabPage2.Text = "Lab2";
            this.tabPage2.UseVisualStyleBackColor = true;
            // 
            // lab2_addr
            // 
            this.lab2_addr.AutoSize = true;
            this.lab2_addr.Location = new System.Drawing.Point(90, 41);
            this.lab2_addr.Name = "lab2_addr";
            this.lab2_addr.Size = new System.Drawing.Size(92, 15);
            this.lab2_addr.TabIndex = 87;
            this.lab2_addr.Text = "先骕楼X6510";
            // 
            // lab2_name
            // 
            this.lab2_name.AutoSize = true;
            this.lab2_name.Location = new System.Drawing.Point(90, 12);
            this.lab2_name.Name = "lab2_name";
            this.lab2_name.Size = new System.Drawing.Size(97, 15);
            this.lab2_name.TabIndex = 86;
            this.lab2_name.Text = "物联网实验室";
            // 
            // label51
            // 
            this.label51.AutoSize = true;
            this.label51.Location = new System.Drawing.Point(5, 41);
            this.label51.Name = "label51";
            this.label51.Size = new System.Drawing.Size(84, 15);
            this.label51.TabIndex = 85;
            this.label51.Text = "地    址：";
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Location = new System.Drawing.Point(5, 12);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(82, 15);
            this.label11.TabIndex = 84;
            this.label11.Text = "实验室名：";
            // 
            // Lab2_01
            // 
            this.Lab2_01.BackColor = System.Drawing.Color.Green;
            this.Lab2_01.Location = new System.Drawing.Point(8, 119);
            this.Lab2_01.Name = "Lab2_01";
            this.Lab2_01.Size = new System.Drawing.Size(66, 28);
            this.Lab2_01.TabIndex = 83;
            this.Lab2_01.Tag = "lab2";
            this.Lab2_01.Text = "预约";
            this.Lab2_01.UseVisualStyleBackColor = false;
            this.Lab2_01.Click += new System.EventHandler(this.Lab2_01_Click);
            // 
            // label21
            // 
            this.label21.AutoSize = true;
            this.label21.Location = new System.Drawing.Point(382, 252);
            this.label21.Name = "label21";
            this.label21.Size = new System.Drawing.Size(23, 15);
            this.label21.TabIndex = 82;
            this.label21.Text = "15";
            // 
            // Lab2_15
            // 
            this.Lab2_15.BackColor = System.Drawing.Color.Green;
            this.Lab2_15.Location = new System.Drawing.Point(343, 292);
            this.Lab2_15.Name = "Lab2_15";
            this.Lab2_15.Size = new System.Drawing.Size(66, 28);
            this.Lab2_15.TabIndex = 81;
            this.Lab2_15.Tag = "lab2";
            this.Lab2_15.Text = "预约";
            this.Lab2_15.UseVisualStyleBackColor = false;
            this.Lab2_15.Click += new System.EventHandler(this.Lab2_15_Click);
            // 
            // pictureBox21
            // 
            this.pictureBox21.Image = global::WindowsFormsApp1.Properties.Resources.pc12;
            this.pictureBox21.Location = new System.Drawing.Point(343, 249);
            this.pictureBox21.Name = "pictureBox21";
            this.pictureBox21.Size = new System.Drawing.Size(32, 34);
            this.pictureBox21.TabIndex = 80;
            this.pictureBox21.TabStop = false;
            // 
            // label22
            // 
            this.label22.AutoSize = true;
            this.label22.Location = new System.Drawing.Point(303, 252);
            this.label22.Name = "label22";
            this.label22.Size = new System.Drawing.Size(23, 15);
            this.label22.TabIndex = 79;
            this.label22.Text = "14";
            // 
            // Lab2_14
            // 
            this.Lab2_14.BackColor = System.Drawing.Color.Green;
            this.Lab2_14.Location = new System.Drawing.Point(260, 292);
            this.Lab2_14.Name = "Lab2_14";
            this.Lab2_14.Size = new System.Drawing.Size(66, 28);
            this.Lab2_14.TabIndex = 78;
            this.Lab2_14.Tag = "lab2";
            this.Lab2_14.Text = "预约";
            this.Lab2_14.UseVisualStyleBackColor = false;
            this.Lab2_14.Click += new System.EventHandler(this.Lab2_14_Click);
            // 
            // pictureBox22
            // 
            this.pictureBox22.Image = global::WindowsFormsApp1.Properties.Resources.pc12;
            this.pictureBox22.Location = new System.Drawing.Point(260, 249);
            this.pictureBox22.Name = "pictureBox22";
            this.pictureBox22.Size = new System.Drawing.Size(32, 34);
            this.pictureBox22.TabIndex = 77;
            this.pictureBox22.TabStop = false;
            // 
            // label23
            // 
            this.label23.AutoSize = true;
            this.label23.Location = new System.Drawing.Point(215, 252);
            this.label23.Name = "label23";
            this.label23.Size = new System.Drawing.Size(23, 15);
            this.label23.TabIndex = 76;
            this.label23.Text = "13";
            // 
            // Lab2_13
            // 
            this.Lab2_13.BackColor = System.Drawing.Color.Green;
            this.Lab2_13.Location = new System.Drawing.Point(172, 292);
            this.Lab2_13.Name = "Lab2_13";
            this.Lab2_13.Size = new System.Drawing.Size(66, 28);
            this.Lab2_13.TabIndex = 75;
            this.Lab2_13.Tag = "lab2";
            this.Lab2_13.Text = "预约";
            this.Lab2_13.UseVisualStyleBackColor = false;
            this.Lab2_13.Click += new System.EventHandler(this.Lab2_13_Click);
            // 
            // label24
            // 
            this.label24.AutoSize = true;
            this.label24.Location = new System.Drawing.Point(137, 252);
            this.label24.Name = "label24";
            this.label24.Size = new System.Drawing.Size(23, 15);
            this.label24.TabIndex = 74;
            this.label24.Text = "12";
            // 
            // pictureBox23
            // 
            this.pictureBox23.Image = global::WindowsFormsApp1.Properties.Resources.pc12;
            this.pictureBox23.Location = new System.Drawing.Point(177, 249);
            this.pictureBox23.Name = "pictureBox23";
            this.pictureBox23.Size = new System.Drawing.Size(37, 34);
            this.pictureBox23.TabIndex = 73;
            this.pictureBox23.TabStop = false;
            // 
            // pictureBox24
            // 
            this.pictureBox24.Image = global::WindowsFormsApp1.Properties.Resources.pc12;
            this.pictureBox24.Location = new System.Drawing.Point(94, 252);
            this.pictureBox24.Name = "pictureBox24";
            this.pictureBox24.Size = new System.Drawing.Size(31, 31);
            this.pictureBox24.TabIndex = 72;
            this.pictureBox24.TabStop = false;
            // 
            // Lab2_12
            // 
            this.Lab2_12.BackColor = System.Drawing.Color.Green;
            this.Lab2_12.Location = new System.Drawing.Point(94, 292);
            this.Lab2_12.Name = "Lab2_12";
            this.Lab2_12.Size = new System.Drawing.Size(66, 28);
            this.Lab2_12.TabIndex = 71;
            this.Lab2_12.Tag = "lab2";
            this.Lab2_12.Text = "预约";
            this.Lab2_12.UseVisualStyleBackColor = false;
            this.Lab2_12.Click += new System.EventHandler(this.Lab2_12_Click);
            // 
            // label25
            // 
            this.label25.AutoSize = true;
            this.label25.Location = new System.Drawing.Point(49, 252);
            this.label25.Name = "label25";
            this.label25.Size = new System.Drawing.Size(23, 15);
            this.label25.TabIndex = 70;
            this.label25.Text = "11";
            // 
            // Lab2_11
            // 
            this.Lab2_11.BackColor = System.Drawing.Color.Green;
            this.Lab2_11.Location = new System.Drawing.Point(8, 292);
            this.Lab2_11.Name = "Lab2_11";
            this.Lab2_11.Size = new System.Drawing.Size(66, 28);
            this.Lab2_11.TabIndex = 69;
            this.Lab2_11.Tag = "lab2";
            this.Lab2_11.Text = "预约";
            this.Lab2_11.UseVisualStyleBackColor = false;
            this.Lab2_11.Click += new System.EventHandler(this.Lab2_11_Click);
            // 
            // pictureBox25
            // 
            this.pictureBox25.Image = global::WindowsFormsApp1.Properties.Resources.pc12;
            this.pictureBox25.Location = new System.Drawing.Point(11, 252);
            this.pictureBox25.Name = "pictureBox25";
            this.pictureBox25.Size = new System.Drawing.Size(32, 34);
            this.pictureBox25.TabIndex = 68;
            this.pictureBox25.TabStop = false;
            // 
            // label16
            // 
            this.label16.AutoSize = true;
            this.label16.Location = new System.Drawing.Point(381, 329);
            this.label16.Name = "label16";
            this.label16.Size = new System.Drawing.Size(23, 15);
            this.label16.TabIndex = 67;
            this.label16.Text = "20";
            // 
            // Lab2_20
            // 
            this.Lab2_20.BackColor = System.Drawing.Color.Green;
            this.Lab2_20.Location = new System.Drawing.Point(342, 369);
            this.Lab2_20.Name = "Lab2_20";
            this.Lab2_20.Size = new System.Drawing.Size(66, 28);
            this.Lab2_20.TabIndex = 66;
            this.Lab2_20.Tag = "lab2";
            this.Lab2_20.Text = "预约";
            this.Lab2_20.UseVisualStyleBackColor = false;
            this.Lab2_20.Click += new System.EventHandler(this.Lab2_20_Click);
            // 
            // pictureBox16
            // 
            this.pictureBox16.Image = global::WindowsFormsApp1.Properties.Resources.pc12;
            this.pictureBox16.Location = new System.Drawing.Point(342, 326);
            this.pictureBox16.Name = "pictureBox16";
            this.pictureBox16.Size = new System.Drawing.Size(32, 34);
            this.pictureBox16.TabIndex = 65;
            this.pictureBox16.TabStop = false;
            // 
            // label17
            // 
            this.label17.AutoSize = true;
            this.label17.Location = new System.Drawing.Point(302, 329);
            this.label17.Name = "label17";
            this.label17.Size = new System.Drawing.Size(23, 15);
            this.label17.TabIndex = 64;
            this.label17.Text = "19";
            // 
            // Lab2_19
            // 
            this.Lab2_19.BackColor = System.Drawing.Color.Green;
            this.Lab2_19.Location = new System.Drawing.Point(259, 369);
            this.Lab2_19.Name = "Lab2_19";
            this.Lab2_19.Size = new System.Drawing.Size(66, 28);
            this.Lab2_19.TabIndex = 63;
            this.Lab2_19.Tag = "lab2";
            this.Lab2_19.Text = "预约";
            this.Lab2_19.UseVisualStyleBackColor = false;
            this.Lab2_19.Click += new System.EventHandler(this.Lab2_19_Click);
            // 
            // pictureBox17
            // 
            this.pictureBox17.Image = global::WindowsFormsApp1.Properties.Resources.pc12;
            this.pictureBox17.Location = new System.Drawing.Point(259, 326);
            this.pictureBox17.Name = "pictureBox17";
            this.pictureBox17.Size = new System.Drawing.Size(32, 34);
            this.pictureBox17.TabIndex = 62;
            this.pictureBox17.TabStop = false;
            // 
            // label18
            // 
            this.label18.AutoSize = true;
            this.label18.Location = new System.Drawing.Point(214, 329);
            this.label18.Name = "label18";
            this.label18.Size = new System.Drawing.Size(23, 15);
            this.label18.TabIndex = 61;
            this.label18.Text = "18";
            // 
            // Lab2_18
            // 
            this.Lab2_18.BackColor = System.Drawing.Color.Green;
            this.Lab2_18.Location = new System.Drawing.Point(171, 369);
            this.Lab2_18.Name = "Lab2_18";
            this.Lab2_18.Size = new System.Drawing.Size(66, 28);
            this.Lab2_18.TabIndex = 60;
            this.Lab2_18.Tag = "lab2";
            this.Lab2_18.Text = "预约";
            this.Lab2_18.UseVisualStyleBackColor = false;
            this.Lab2_18.Click += new System.EventHandler(this.Lab2_18_Click);
            // 
            // label19
            // 
            this.label19.AutoSize = true;
            this.label19.Location = new System.Drawing.Point(136, 329);
            this.label19.Name = "label19";
            this.label19.Size = new System.Drawing.Size(23, 15);
            this.label19.TabIndex = 59;
            this.label19.Text = "17";
            // 
            // pictureBox18
            // 
            this.pictureBox18.Image = global::WindowsFormsApp1.Properties.Resources.pc12;
            this.pictureBox18.Location = new System.Drawing.Point(176, 326);
            this.pictureBox18.Name = "pictureBox18";
            this.pictureBox18.Size = new System.Drawing.Size(37, 34);
            this.pictureBox18.TabIndex = 58;
            this.pictureBox18.TabStop = false;
            // 
            // pictureBox19
            // 
            this.pictureBox19.Image = global::WindowsFormsApp1.Properties.Resources.pc12;
            this.pictureBox19.Location = new System.Drawing.Point(93, 329);
            this.pictureBox19.Name = "pictureBox19";
            this.pictureBox19.Size = new System.Drawing.Size(31, 31);
            this.pictureBox19.TabIndex = 57;
            this.pictureBox19.TabStop = false;
            // 
            // Lab2_17
            // 
            this.Lab2_17.BackColor = System.Drawing.Color.Green;
            this.Lab2_17.Location = new System.Drawing.Point(93, 369);
            this.Lab2_17.Name = "Lab2_17";
            this.Lab2_17.Size = new System.Drawing.Size(66, 28);
            this.Lab2_17.TabIndex = 56;
            this.Lab2_17.Tag = "lab2";
            this.Lab2_17.Text = "预约";
            this.Lab2_17.UseVisualStyleBackColor = false;
            this.Lab2_17.Click += new System.EventHandler(this.Lab2_17_Click);
            // 
            // label20
            // 
            this.label20.AutoSize = true;
            this.label20.Location = new System.Drawing.Point(48, 329);
            this.label20.Name = "label20";
            this.label20.Size = new System.Drawing.Size(23, 15);
            this.label20.TabIndex = 55;
            this.label20.Text = "16";
            // 
            // Lab2_16
            // 
            this.Lab2_16.BackColor = System.Drawing.Color.Green;
            this.Lab2_16.Location = new System.Drawing.Point(7, 369);
            this.Lab2_16.Name = "Lab2_16";
            this.Lab2_16.Size = new System.Drawing.Size(66, 28);
            this.Lab2_16.TabIndex = 54;
            this.Lab2_16.Tag = "lab2";
            this.Lab2_16.Text = "预约";
            this.Lab2_16.UseVisualStyleBackColor = false;
            this.Lab2_16.Click += new System.EventHandler(this.Lab2_16_Click);
            // 
            // pictureBox20
            // 
            this.pictureBox20.Image = global::WindowsFormsApp1.Properties.Resources.pc12;
            this.pictureBox20.Location = new System.Drawing.Point(10, 329);
            this.pictureBox20.Name = "pictureBox20";
            this.pictureBox20.Size = new System.Drawing.Size(32, 34);
            this.pictureBox20.TabIndex = 53;
            this.pictureBox20.TabStop = false;
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.Location = new System.Drawing.Point(305, 411);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(23, 15);
            this.label12.TabIndex = 49;
            this.label12.Text = "24";
            // 
            // Lab2_24
            // 
            this.Lab2_24.BackColor = System.Drawing.Color.Green;
            this.Lab2_24.Location = new System.Drawing.Point(262, 451);
            this.Lab2_24.Name = "Lab2_24";
            this.Lab2_24.Size = new System.Drawing.Size(66, 28);
            this.Lab2_24.TabIndex = 48;
            this.Lab2_24.Tag = "lab2";
            this.Lab2_24.Text = "预约";
            this.Lab2_24.UseVisualStyleBackColor = false;
            this.Lab2_24.Click += new System.EventHandler(this.Lab2_24_Click);
            // 
            // pictureBox12
            // 
            this.pictureBox12.Image = global::WindowsFormsApp1.Properties.Resources.pc12;
            this.pictureBox12.Location = new System.Drawing.Point(262, 408);
            this.pictureBox12.Name = "pictureBox12";
            this.pictureBox12.Size = new System.Drawing.Size(32, 34);
            this.pictureBox12.TabIndex = 47;
            this.pictureBox12.TabStop = false;
            // 
            // label13
            // 
            this.label13.AutoSize = true;
            this.label13.Location = new System.Drawing.Point(217, 411);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(23, 15);
            this.label13.TabIndex = 46;
            this.label13.Text = "23";
            // 
            // Lab2_23
            // 
            this.Lab2_23.BackColor = System.Drawing.Color.Green;
            this.Lab2_23.Location = new System.Drawing.Point(174, 451);
            this.Lab2_23.Name = "Lab2_23";
            this.Lab2_23.Size = new System.Drawing.Size(66, 28);
            this.Lab2_23.TabIndex = 45;
            this.Lab2_23.Tag = "lab2";
            this.Lab2_23.Text = "预约";
            this.Lab2_23.UseVisualStyleBackColor = false;
            this.Lab2_23.Click += new System.EventHandler(this.Lab2_23_Click);
            // 
            // label14
            // 
            this.label14.AutoSize = true;
            this.label14.Location = new System.Drawing.Point(139, 411);
            this.label14.Name = "label14";
            this.label14.Size = new System.Drawing.Size(23, 15);
            this.label14.TabIndex = 44;
            this.label14.Text = "22";
            // 
            // pictureBox13
            // 
            this.pictureBox13.Image = global::WindowsFormsApp1.Properties.Resources.pc12;
            this.pictureBox13.Location = new System.Drawing.Point(179, 408);
            this.pictureBox13.Name = "pictureBox13";
            this.pictureBox13.Size = new System.Drawing.Size(37, 34);
            this.pictureBox13.TabIndex = 43;
            this.pictureBox13.TabStop = false;
            // 
            // pictureBox14
            // 
            this.pictureBox14.Image = global::WindowsFormsApp1.Properties.Resources.pc12;
            this.pictureBox14.Location = new System.Drawing.Point(96, 411);
            this.pictureBox14.Name = "pictureBox14";
            this.pictureBox14.Size = new System.Drawing.Size(31, 31);
            this.pictureBox14.TabIndex = 42;
            this.pictureBox14.TabStop = false;
            // 
            // Lab2_22
            // 
            this.Lab2_22.BackColor = System.Drawing.Color.Green;
            this.Lab2_22.Location = new System.Drawing.Point(96, 451);
            this.Lab2_22.Name = "Lab2_22";
            this.Lab2_22.Size = new System.Drawing.Size(66, 28);
            this.Lab2_22.TabIndex = 41;
            this.Lab2_22.Tag = "lab2";
            this.Lab2_22.Text = "预约";
            this.Lab2_22.UseVisualStyleBackColor = false;
            this.Lab2_22.Click += new System.EventHandler(this.Lab2_22_Click);
            // 
            // label15
            // 
            this.label15.AutoSize = true;
            this.label15.Location = new System.Drawing.Point(51, 411);
            this.label15.Name = "label15";
            this.label15.Size = new System.Drawing.Size(23, 15);
            this.label15.TabIndex = 40;
            this.label15.Text = "21";
            // 
            // Lab2_21
            // 
            this.Lab2_21.BackColor = System.Drawing.Color.Green;
            this.Lab2_21.Location = new System.Drawing.Point(10, 451);
            this.Lab2_21.Name = "Lab2_21";
            this.Lab2_21.Size = new System.Drawing.Size(66, 28);
            this.Lab2_21.TabIndex = 39;
            this.Lab2_21.Tag = "lab2";
            this.Lab2_21.Text = "预约";
            this.Lab2_21.UseVisualStyleBackColor = false;
            this.Lab2_21.Click += new System.EventHandler(this.Lab2_21_Click);
            // 
            // pictureBox15
            // 
            this.pictureBox15.Image = global::WindowsFormsApp1.Properties.Resources.pc12;
            this.pictureBox15.Location = new System.Drawing.Point(13, 411);
            this.pictureBox15.Name = "pictureBox15";
            this.pictureBox15.Size = new System.Drawing.Size(32, 34);
            this.pictureBox15.TabIndex = 38;
            this.pictureBox15.TabStop = false;
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(381, 168);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(23, 15);
            this.label10.TabIndex = 37;
            this.label10.Text = "10";
            // 
            // Lab2_10
            // 
            this.Lab2_10.BackColor = System.Drawing.Color.Green;
            this.Lab2_10.Location = new System.Drawing.Point(342, 208);
            this.Lab2_10.Name = "Lab2_10";
            this.Lab2_10.Size = new System.Drawing.Size(66, 28);
            this.Lab2_10.TabIndex = 36;
            this.Lab2_10.Tag = "lab2";
            this.Lab2_10.Text = "预约";
            this.Lab2_10.UseVisualStyleBackColor = false;
            this.Lab2_10.Click += new System.EventHandler(this.Lab2_10_Click);
            // 
            // pictureBox10
            // 
            this.pictureBox10.Image = global::WindowsFormsApp1.Properties.Resources.pc12;
            this.pictureBox10.Location = new System.Drawing.Point(342, 165);
            this.pictureBox10.Name = "pictureBox10";
            this.pictureBox10.Size = new System.Drawing.Size(32, 34);
            this.pictureBox10.TabIndex = 35;
            this.pictureBox10.TabStop = false;
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(302, 168);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(23, 15);
            this.label7.TabIndex = 28;
            this.label7.Text = "09";
            // 
            // Lab2_09
            // 
            this.Lab2_09.BackColor = System.Drawing.Color.Green;
            this.Lab2_09.Location = new System.Drawing.Point(259, 208);
            this.Lab2_09.Name = "Lab2_09";
            this.Lab2_09.Size = new System.Drawing.Size(66, 28);
            this.Lab2_09.TabIndex = 27;
            this.Lab2_09.Tag = "lab2";
            this.Lab2_09.Text = "预约";
            this.Lab2_09.UseVisualStyleBackColor = false;
            this.Lab2_09.Click += new System.EventHandler(this.Lab2_09_Click);
            // 
            // pictureBox7
            // 
            this.pictureBox7.Image = global::WindowsFormsApp1.Properties.Resources.pc12;
            this.pictureBox7.Location = new System.Drawing.Point(259, 165);
            this.pictureBox7.Name = "pictureBox7";
            this.pictureBox7.Size = new System.Drawing.Size(32, 34);
            this.pictureBox7.TabIndex = 26;
            this.pictureBox7.TabStop = false;
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(214, 168);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(23, 15);
            this.label8.TabIndex = 25;
            this.label8.Text = "08";
            // 
            // Lab2_08
            // 
            this.Lab2_08.BackColor = System.Drawing.Color.Green;
            this.Lab2_08.Location = new System.Drawing.Point(171, 208);
            this.Lab2_08.Name = "Lab2_08";
            this.Lab2_08.Size = new System.Drawing.Size(66, 28);
            this.Lab2_08.TabIndex = 24;
            this.Lab2_08.Tag = "lab2";
            this.Lab2_08.Text = "预约";
            this.Lab2_08.UseVisualStyleBackColor = false;
            this.Lab2_08.Click += new System.EventHandler(this.Lab2_08_Click);
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(136, 168);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(23, 15);
            this.label9.TabIndex = 23;
            this.label9.Text = "07";
            // 
            // pictureBox8
            // 
            this.pictureBox8.Image = global::WindowsFormsApp1.Properties.Resources.pc12;
            this.pictureBox8.Location = new System.Drawing.Point(176, 165);
            this.pictureBox8.Name = "pictureBox8";
            this.pictureBox8.Size = new System.Drawing.Size(37, 34);
            this.pictureBox8.TabIndex = 22;
            this.pictureBox8.TabStop = false;
            // 
            // pictureBox9
            // 
            this.pictureBox9.Image = global::WindowsFormsApp1.Properties.Resources.pc12;
            this.pictureBox9.Location = new System.Drawing.Point(93, 168);
            this.pictureBox9.Name = "pictureBox9";
            this.pictureBox9.Size = new System.Drawing.Size(31, 31);
            this.pictureBox9.TabIndex = 21;
            this.pictureBox9.TabStop = false;
            // 
            // Lab2_07
            // 
            this.Lab2_07.BackColor = System.Drawing.Color.Green;
            this.Lab2_07.Location = new System.Drawing.Point(93, 208);
            this.Lab2_07.Name = "Lab2_07";
            this.Lab2_07.Size = new System.Drawing.Size(66, 28);
            this.Lab2_07.TabIndex = 20;
            this.Lab2_07.Tag = "lab2";
            this.Lab2_07.Text = "预约";
            this.Lab2_07.UseVisualStyleBackColor = false;
            this.Lab2_07.Click += new System.EventHandler(this.Lab2_07_Click);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(48, 168);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(23, 15);
            this.label4.TabIndex = 19;
            this.label4.Text = "06";
            // 
            // Lab2_06
            // 
            this.Lab2_06.BackColor = System.Drawing.Color.Green;
            this.Lab2_06.Location = new System.Drawing.Point(7, 208);
            this.Lab2_06.Name = "Lab2_06";
            this.Lab2_06.Size = new System.Drawing.Size(66, 28);
            this.Lab2_06.TabIndex = 18;
            this.Lab2_06.Tag = "lab2";
            this.Lab2_06.Text = "预约";
            this.Lab2_06.UseVisualStyleBackColor = false;
            this.Lab2_06.Click += new System.EventHandler(this.Lab2_06_Click);
            // 
            // pictureBox4
            // 
            this.pictureBox4.Image = global::WindowsFormsApp1.Properties.Resources.pc12;
            this.pictureBox4.Location = new System.Drawing.Point(10, 168);
            this.pictureBox4.Name = "pictureBox4";
            this.pictureBox4.Size = new System.Drawing.Size(32, 34);
            this.pictureBox4.TabIndex = 17;
            this.pictureBox4.TabStop = false;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(385, 82);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(23, 15);
            this.label5.TabIndex = 16;
            this.label5.Text = "05";
            // 
            // Lab2_05
            // 
            this.Lab2_05.BackColor = System.Drawing.Color.Green;
            this.Lab2_05.Location = new System.Drawing.Point(342, 119);
            this.Lab2_05.Name = "Lab2_05";
            this.Lab2_05.Size = new System.Drawing.Size(66, 28);
            this.Lab2_05.TabIndex = 15;
            this.Lab2_05.Tag = "lab2";
            this.Lab2_05.Text = "预约";
            this.Lab2_05.UseVisualStyleBackColor = false;
            this.Lab2_05.Click += new System.EventHandler(this.Lab2_05_Click);
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(302, 82);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(23, 15);
            this.label6.TabIndex = 14;
            this.label6.Text = "04";
            // 
            // pictureBox5
            // 
            this.pictureBox5.Image = global::WindowsFormsApp1.Properties.Resources.pc12;
            this.pictureBox5.Location = new System.Drawing.Point(342, 82);
            this.pictureBox5.Name = "pictureBox5";
            this.pictureBox5.Size = new System.Drawing.Size(37, 34);
            this.pictureBox5.TabIndex = 13;
            this.pictureBox5.TabStop = false;
            // 
            // pictureBox6
            // 
            this.pictureBox6.Image = global::WindowsFormsApp1.Properties.Resources.pc12;
            this.pictureBox6.Location = new System.Drawing.Point(259, 82);
            this.pictureBox6.Name = "pictureBox6";
            this.pictureBox6.Size = new System.Drawing.Size(31, 31);
            this.pictureBox6.TabIndex = 12;
            this.pictureBox6.TabStop = false;
            // 
            // Lab2_04
            // 
            this.Lab2_04.BackColor = System.Drawing.Color.Green;
            this.Lab2_04.Location = new System.Drawing.Point(259, 119);
            this.Lab2_04.Name = "Lab2_04";
            this.Lab2_04.Size = new System.Drawing.Size(66, 28);
            this.Lab2_04.TabIndex = 11;
            this.Lab2_04.Tag = "lab2";
            this.Lab2_04.Text = "预约";
            this.Lab2_04.UseVisualStyleBackColor = false;
            this.Lab2_04.Click += new System.EventHandler(this.Lab2_04_Click);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(214, 79);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(23, 15);
            this.label3.TabIndex = 10;
            this.label3.Text = "03";
            // 
            // Lab2_03
            // 
            this.Lab2_03.BackColor = System.Drawing.Color.Green;
            this.Lab2_03.Location = new System.Drawing.Point(171, 119);
            this.Lab2_03.Name = "Lab2_03";
            this.Lab2_03.Size = new System.Drawing.Size(66, 28);
            this.Lab2_03.TabIndex = 9;
            this.Lab2_03.Tag = "lab2";
            this.Lab2_03.Text = "预约";
            this.Lab2_03.UseVisualStyleBackColor = false;
            this.Lab2_03.Click += new System.EventHandler(this.Lab2_03_Click);
            // 
            // pictureBox3
            // 
            this.pictureBox3.Image = global::WindowsFormsApp1.Properties.Resources.pc12;
            this.pictureBox3.Location = new System.Drawing.Point(176, 79);
            this.pictureBox3.Name = "pictureBox3";
            this.pictureBox3.Size = new System.Drawing.Size(32, 34);
            this.pictureBox3.TabIndex = 8;
            this.pictureBox3.TabStop = false;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(136, 79);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(23, 15);
            this.label2.TabIndex = 7;
            this.label2.Text = "02";
            // 
            // Lab2_02
            // 
            this.Lab2_02.BackColor = System.Drawing.Color.Green;
            this.Lab2_02.Location = new System.Drawing.Point(93, 119);
            this.Lab2_02.Name = "Lab2_02";
            this.Lab2_02.Size = new System.Drawing.Size(66, 28);
            this.Lab2_02.TabIndex = 6;
            this.Lab2_02.Tag = "lab2";
            this.Lab2_02.Text = "预约";
            this.Lab2_02.UseVisualStyleBackColor = false;
            this.Lab2_02.Click += new System.EventHandler(this.Lab2_02_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(50, 82);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(23, 15);
            this.label1.TabIndex = 5;
            this.label1.Text = "01";
            this.label1.Click += new System.EventHandler(this.label1_Click);
            // 
            // pictureBox2
            // 
            this.pictureBox2.Image = global::WindowsFormsApp1.Properties.Resources.pc12;
            this.pictureBox2.Location = new System.Drawing.Point(93, 79);
            this.pictureBox2.Name = "pictureBox2";
            this.pictureBox2.Size = new System.Drawing.Size(37, 34);
            this.pictureBox2.TabIndex = 4;
            this.pictureBox2.TabStop = false;
            // 
            // pictureBox1
            // 
            this.pictureBox1.Image = global::WindowsFormsApp1.Properties.Resources.pc12;
            this.pictureBox1.Location = new System.Drawing.Point(13, 82);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(31, 31);
            this.pictureBox1.TabIndex = 3;
            this.pictureBox1.TabStop = false;
            // 
            // label52
            // 
            this.label52.AutoSize = true;
            this.label52.Location = new System.Drawing.Point(33, 26);
            this.label52.Name = "label52";
            this.label52.Size = new System.Drawing.Size(0, 15);
            this.label52.TabIndex = 2;
            // 
            // dateTimePicker1
            // 
            this.dateTimePicker1.Location = new System.Drawing.Point(91, 155);
            this.dateTimePicker1.Name = "dateTimePicker1";
            this.dateTimePicker1.Size = new System.Drawing.Size(153, 25);
            this.dateTimePicker1.TabIndex = 3;
            this.dateTimePicker1.ValueChanged += new System.EventHandler(this.dateTimePicker1_ValueChanged);
            // 
            // label53
            // 
            this.label53.AutoSize = true;
            this.label53.Font = new System.Drawing.Font("宋体", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label53.Location = new System.Drawing.Point(18, 202);
            this.label53.Name = "label53";
            this.label53.Size = new System.Drawing.Size(29, 20);
            this.label53.TabIndex = 4;
            this.label53.Text = "第";
            // 
            // order_start
            // 
            this.order_start.Location = new System.Drawing.Point(53, 199);
            this.order_start.Name = "order_start";
            this.order_start.Size = new System.Drawing.Size(57, 25);
            this.order_start.TabIndex = 5;
            // 
            // label56
            // 
            this.label56.AutoSize = true;
            this.label56.Location = new System.Drawing.Point(116, 204);
            this.label56.Name = "label56";
            this.label56.Size = new System.Drawing.Size(15, 15);
            this.label56.TabIndex = 6;
            this.label56.Text = "-";
            // 
            // order_end
            // 
            this.order_end.Location = new System.Drawing.Point(137, 199);
            this.order_end.Name = "order_end";
            this.order_end.Size = new System.Drawing.Size(58, 25);
            this.order_end.TabIndex = 7;
            // 
            // label57
            // 
            this.label57.AutoSize = true;
            this.label57.Font = new System.Drawing.Font("宋体", 12F);
            this.label57.Location = new System.Drawing.Point(201, 202);
            this.label57.Name = "label57";
            this.label57.Size = new System.Drawing.Size(49, 20);
            this.label57.TabIndex = 8;
            this.label57.Text = "节课";
            // 
            // label58
            // 
            this.label58.AutoSize = true;
            this.label58.BackColor = System.Drawing.SystemColors.GradientActiveCaption;
            this.label58.Location = new System.Drawing.Point(16, 25);
            this.label58.Name = "label58";
            this.label58.Size = new System.Drawing.Size(277, 15);
            this.label58.TabIndex = 9;
            this.label58.Text = "预约：请填好相关信息点击右侧进行预约";
            // 
            // lab121
            // 
            this.lab121.AutoSize = true;
            this.lab121.Font = new System.Drawing.Font("宋体", 12F);
            this.lab121.Location = new System.Drawing.Point(15, 61);
            this.lab121.Name = "lab121";
            this.lab121.Size = new System.Drawing.Size(69, 20);
            this.lab121.TabIndex = 10;
            this.lab121.Text = "学号：";
            // 
            // lab123
            // 
            this.lab123.AutoSize = true;
            this.lab123.Font = new System.Drawing.Font("宋体", 12F);
            this.lab123.Location = new System.Drawing.Point(15, 93);
            this.lab123.Name = "lab123";
            this.lab123.Size = new System.Drawing.Size(69, 20);
            this.lab123.TabIndex = 11;
            this.lab123.Text = "姓名：";
            // 
            // label61
            // 
            this.label61.AutoSize = true;
            this.label61.Font = new System.Drawing.Font("宋体", 12F);
            this.label61.Location = new System.Drawing.Point(15, 122);
            this.label61.Name = "label61";
            this.label61.Size = new System.Drawing.Size(69, 20);
            this.label61.TabIndex = 12;
            this.label61.Text = "学院：";
            // 
            // label59
            // 
            this.label59.AutoSize = true;
            this.label59.Font = new System.Drawing.Font("宋体", 12F);
            this.label59.Location = new System.Drawing.Point(15, 158);
            this.label59.Name = "label59";
            this.label59.Size = new System.Drawing.Size(69, 20);
            this.label59.TabIndex = 16;
            this.label59.Text = "日期：";
            // 
            // InitButton
            // 
            this.InitButton.BackColor = System.Drawing.SystemColors.Info;
            this.InitButton.Location = new System.Drawing.Point(18, 459);
            this.InitButton.Name = "InitButton";
            this.InitButton.Size = new System.Drawing.Size(75, 23);
            this.InitButton.TabIndex = 19;
            this.InitButton.Text = "初始化";
            this.InitButton.UseVisualStyleBackColor = false;
            this.InitButton.Click += new System.EventHandler(this.InitButton_Click);
            // 
            // label62
            // 
            this.label62.AutoSize = true;
            this.label62.BackColor = System.Drawing.SystemColors.GradientActiveCaption;
            this.label62.Location = new System.Drawing.Point(17, 229);
            this.label62.Name = "label62";
            this.label62.Size = new System.Drawing.Size(284, 15);
            this.label62.TabIndex = 20;
            this.label62.Text = "查询：查询你的预约信息--------------";
            // 
            // Button_find
            // 
            this.Button_find.BackColor = System.Drawing.SystemColors.Info;
            this.Button_find.Location = new System.Drawing.Point(18, 256);
            this.Button_find.Name = "Button_find";
            this.Button_find.Size = new System.Drawing.Size(75, 23);
            this.Button_find.TabIndex = 24;
            this.Button_find.Text = "查询";
            this.Button_find.UseVisualStyleBackColor = false;
            this.Button_find.Click += new System.EventHandler(this.Button_find_Click_1);
            // 
            // tabControl2
            // 
            this.tabControl2.Controls.Add(this.tabPage3);
            this.tabControl2.Controls.Add(this.tabPage4);
            this.tabControl2.Controls.Add(this.tabPage5);
            this.tabControl2.Location = new System.Drawing.Point(-3, 1);
            this.tabControl2.Name = "tabControl2";
            this.tabControl2.SelectedIndex = 0;
            this.tabControl2.Size = new System.Drawing.Size(312, 128);
            this.tabControl2.TabIndex = 25;
            // 
            // tabPage3
            // 
            this.tabPage3.Controls.Add(this.group_stu);
            this.tabPage3.Controls.Add(this.stu_tag);
            this.tabPage3.Controls.Add(this.Exit_Stu);
            this.tabPage3.Location = new System.Drawing.Point(4, 25);
            this.tabPage3.Name = "tabPage3";
            this.tabPage3.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage3.Size = new System.Drawing.Size(304, 99);
            this.tabPage3.TabIndex = 0;
            this.tabPage3.Text = "学生";
            this.tabPage3.UseVisualStyleBackColor = true;
            // 
            // group_stu
            // 
            this.group_stu.Controls.Add(this.Stu_Sno);
            this.group_stu.Controls.Add(this.label63);
            this.group_stu.Controls.Add(this.Stu_Load);
            this.group_stu.Controls.Add(this.label64);
            this.group_stu.Controls.Add(this.Stu_Password);
            this.group_stu.Location = new System.Drawing.Point(3, 2);
            this.group_stu.Name = "group_stu";
            this.group_stu.Size = new System.Drawing.Size(226, 97);
            this.group_stu.TabIndex = 31;
            this.group_stu.TabStop = false;
            // 
            // Stu_Sno
            // 
            this.Stu_Sno.Location = new System.Drawing.Point(68, 20);
            this.Stu_Sno.Name = "Stu_Sno";
            this.Stu_Sno.Size = new System.Drawing.Size(152, 25);
            this.Stu_Sno.TabIndex = 0;
            // 
            // label63
            // 
            this.label63.AutoSize = true;
            this.label63.Font = new System.Drawing.Font("宋体", 12F);
            this.label63.Location = new System.Drawing.Point(11, 27);
            this.label63.Name = "label63";
            this.label63.Size = new System.Drawing.Size(69, 20);
            this.label63.TabIndex = 11;
            this.label63.Text = "学号：";
            // 
            // Stu_Load
            // 
            this.Stu_Load.BackColor = System.Drawing.SystemColors.Info;
            this.Stu_Load.Location = new System.Drawing.Point(14, 71);
            this.Stu_Load.Name = "Stu_Load";
            this.Stu_Load.Size = new System.Drawing.Size(75, 23);
            this.Stu_Load.TabIndex = 25;
            this.Stu_Load.Tag = "Stu";
            this.Stu_Load.Text = "登录";
            this.Stu_Load.UseVisualStyleBackColor = false;
            this.Stu_Load.Click += new System.EventHandler(this.Stu_Load_Click);
            // 
            // label64
            // 
            this.label64.AutoSize = true;
            this.label64.Font = new System.Drawing.Font("宋体", 12F);
            this.label64.Location = new System.Drawing.Point(11, 52);
            this.label64.Name = "label64";
            this.label64.Size = new System.Drawing.Size(69, 20);
            this.label64.TabIndex = 15;
            this.label64.Text = "密码：";
            // 
            // Stu_Password
            // 
            this.Stu_Password.Location = new System.Drawing.Point(68, 47);
            this.Stu_Password.Name = "Stu_Password";
            this.Stu_Password.PasswordChar = '*';
            this.Stu_Password.Size = new System.Drawing.Size(152, 25);
            this.Stu_Password.TabIndex = 1;
            // 
            // stu_tag
            // 
            this.stu_tag.AutoSize = true;
            this.stu_tag.Location = new System.Drawing.Point(208, 12);
            this.stu_tag.Name = "stu_tag";
            this.stu_tag.Size = new System.Drawing.Size(23, 15);
            this.stu_tag.TabIndex = 87;
            this.stu_tag.Text = "01";
            this.stu_tag.Visible = false;
            // 
            // Exit_Stu
            // 
            this.Exit_Stu.BackColor = System.Drawing.SystemColors.Info;
            this.Exit_Stu.Location = new System.Drawing.Point(120, 70);
            this.Exit_Stu.Name = "Exit_Stu";
            this.Exit_Stu.Size = new System.Drawing.Size(75, 23);
            this.Exit_Stu.TabIndex = 26;
            this.Exit_Stu.Tag = "Stu";
            this.Exit_Stu.Text = "退出";
            this.Exit_Stu.UseVisualStyleBackColor = false;
            this.Exit_Stu.Visible = false;
            this.Exit_Stu.Click += new System.EventHandler(this.Exit_Stu_Click);
            // 
            // tabPage4
            // 
            this.tabPage4.Controls.Add(this.group_Tea);
            this.tabPage4.Controls.Add(this.Tea_tag);
            this.tabPage4.Controls.Add(this.Exit_Tea);
            this.tabPage4.Location = new System.Drawing.Point(4, 25);
            this.tabPage4.Name = "tabPage4";
            this.tabPage4.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage4.Size = new System.Drawing.Size(304, 99);
            this.tabPage4.TabIndex = 1;
            this.tabPage4.Text = "老师";
            this.tabPage4.UseVisualStyleBackColor = true;
            this.tabPage4.Click += new System.EventHandler(this.tabPage4_Click);
            // 
            // group_Tea
            // 
            this.group_Tea.Controls.Add(this.Tea_no);
            this.group_Tea.Controls.Add(this.label90);
            this.group_Tea.Controls.Add(this.Tea_Load);
            this.group_Tea.Controls.Add(this.label97);
            this.group_Tea.Controls.Add(this.Tea_Password);
            this.group_Tea.Location = new System.Drawing.Point(3, 3);
            this.group_Tea.Name = "group_Tea";
            this.group_Tea.Size = new System.Drawing.Size(226, 97);
            this.group_Tea.TabIndex = 32;
            this.group_Tea.TabStop = false;
            // 
            // Tea_no
            // 
            this.Tea_no.Location = new System.Drawing.Point(68, 20);
            this.Tea_no.Name = "Tea_no";
            this.Tea_no.Size = new System.Drawing.Size(152, 25);
            this.Tea_no.TabIndex = 0;
            // 
            // label90
            // 
            this.label90.AutoSize = true;
            this.label90.Font = new System.Drawing.Font("宋体", 12F);
            this.label90.Location = new System.Drawing.Point(11, 27);
            this.label90.Name = "label90";
            this.label90.Size = new System.Drawing.Size(69, 20);
            this.label90.TabIndex = 11;
            this.label90.Text = "工号：";
            // 
            // Tea_Load
            // 
            this.Tea_Load.BackColor = System.Drawing.SystemColors.Info;
            this.Tea_Load.Location = new System.Drawing.Point(14, 71);
            this.Tea_Load.Name = "Tea_Load";
            this.Tea_Load.Size = new System.Drawing.Size(75, 23);
            this.Tea_Load.TabIndex = 25;
            this.Tea_Load.Tag = "Stu";
            this.Tea_Load.Text = "登录";
            this.Tea_Load.UseVisualStyleBackColor = false;
            this.Tea_Load.Click += new System.EventHandler(this.Tea_Load_Click);
            // 
            // label97
            // 
            this.label97.AutoSize = true;
            this.label97.Font = new System.Drawing.Font("宋体", 12F);
            this.label97.Location = new System.Drawing.Point(11, 52);
            this.label97.Name = "label97";
            this.label97.Size = new System.Drawing.Size(69, 20);
            this.label97.TabIndex = 15;
            this.label97.Text = "密码：";
            // 
            // Tea_Password
            // 
            this.Tea_Password.Location = new System.Drawing.Point(68, 47);
            this.Tea_Password.Name = "Tea_Password";
            this.Tea_Password.PasswordChar = '*';
            this.Tea_Password.Size = new System.Drawing.Size(152, 25);
            this.Tea_Password.TabIndex = 1;
            // 
            // Tea_tag
            // 
            this.Tea_tag.AutoSize = true;
            this.Tea_tag.Location = new System.Drawing.Point(162, 12);
            this.Tea_tag.Name = "Tea_tag";
            this.Tea_tag.Size = new System.Drawing.Size(23, 15);
            this.Tea_tag.TabIndex = 87;
            this.Tea_tag.Text = "01";
            this.Tea_tag.Visible = false;
            // 
            // Exit_Tea
            // 
            this.Exit_Tea.BackColor = System.Drawing.SystemColors.Info;
            this.Exit_Tea.Location = new System.Drawing.Point(104, 68);
            this.Exit_Tea.Name = "Exit_Tea";
            this.Exit_Tea.Size = new System.Drawing.Size(75, 23);
            this.Exit_Tea.TabIndex = 32;
            this.Exit_Tea.Tag = "Tea";
            this.Exit_Tea.Text = "退出";
            this.Exit_Tea.UseVisualStyleBackColor = false;
            this.Exit_Tea.Visible = false;
            this.Exit_Tea.Click += new System.EventHandler(this.ExitButton2_Click);
            // 
            // tabPage5
            // 
            this.tabPage5.Controls.Add(this.group_Admin);
            this.tabPage5.Controls.Add(this.Admin_tag);
            this.tabPage5.Controls.Add(this.Exit_Admin);
            this.tabPage5.Location = new System.Drawing.Point(4, 25);
            this.tabPage5.Name = "tabPage5";
            this.tabPage5.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage5.Size = new System.Drawing.Size(304, 99);
            this.tabPage5.TabIndex = 2;
            this.tabPage5.Text = "管理员";
            this.tabPage5.UseVisualStyleBackColor = true;
            // 
            // group_Admin
            // 
            this.group_Admin.Controls.Add(this.Admin_no);
            this.group_Admin.Controls.Add(this.label65);
            this.group_Admin.Controls.Add(this.Admin_Load);
            this.group_Admin.Controls.Add(this.label66);
            this.group_Admin.Controls.Add(this.Admin_Password);
            this.group_Admin.Location = new System.Drawing.Point(3, 2);
            this.group_Admin.Name = "group_Admin";
            this.group_Admin.Size = new System.Drawing.Size(226, 97);
            this.group_Admin.TabIndex = 33;
            this.group_Admin.TabStop = false;
            // 
            // Admin_no
            // 
            this.Admin_no.Location = new System.Drawing.Point(68, 20);
            this.Admin_no.Name = "Admin_no";
            this.Admin_no.Size = new System.Drawing.Size(152, 25);
            this.Admin_no.TabIndex = 0;
            // 
            // label65
            // 
            this.label65.AutoSize = true;
            this.label65.Font = new System.Drawing.Font("宋体", 12F);
            this.label65.Location = new System.Drawing.Point(11, 27);
            this.label65.Name = "label65";
            this.label65.Size = new System.Drawing.Size(69, 20);
            this.label65.TabIndex = 11;
            this.label65.Text = "工号：";
            // 
            // Admin_Load
            // 
            this.Admin_Load.BackColor = System.Drawing.SystemColors.Info;
            this.Admin_Load.Location = new System.Drawing.Point(14, 71);
            this.Admin_Load.Name = "Admin_Load";
            this.Admin_Load.Size = new System.Drawing.Size(75, 23);
            this.Admin_Load.TabIndex = 25;
            this.Admin_Load.Tag = "Stu";
            this.Admin_Load.Text = "登录";
            this.Admin_Load.UseVisualStyleBackColor = false;
            this.Admin_Load.Click += new System.EventHandler(this.Admin_Load_Click);
            // 
            // label66
            // 
            this.label66.AutoSize = true;
            this.label66.Font = new System.Drawing.Font("宋体", 12F);
            this.label66.Location = new System.Drawing.Point(11, 52);
            this.label66.Name = "label66";
            this.label66.Size = new System.Drawing.Size(69, 20);
            this.label66.TabIndex = 15;
            this.label66.Text = "密码：";
            // 
            // Admin_Password
            // 
            this.Admin_Password.Location = new System.Drawing.Point(68, 47);
            this.Admin_Password.Name = "Admin_Password";
            this.Admin_Password.PasswordChar = '*';
            this.Admin_Password.Size = new System.Drawing.Size(152, 25);
            this.Admin_Password.TabIndex = 1;
            // 
            // Admin_tag
            // 
            this.Admin_tag.AutoSize = true;
            this.Admin_tag.Location = new System.Drawing.Point(175, 12);
            this.Admin_tag.Name = "Admin_tag";
            this.Admin_tag.Size = new System.Drawing.Size(23, 15);
            this.Admin_tag.TabIndex = 87;
            this.Admin_tag.Text = "01";
            this.Admin_tag.Visible = false;
            // 
            // Exit_Admin
            // 
            this.Exit_Admin.BackColor = System.Drawing.SystemColors.Info;
            this.Exit_Admin.Location = new System.Drawing.Point(94, 68);
            this.Exit_Admin.Name = "Exit_Admin";
            this.Exit_Admin.Size = new System.Drawing.Size(75, 23);
            this.Exit_Admin.TabIndex = 32;
            this.Exit_Admin.Tag = "Admin";
            this.Exit_Admin.Text = "退出";
            this.Exit_Admin.UseVisualStyleBackColor = false;
            this.Exit_Admin.Visible = false;
            this.Exit_Admin.Click += new System.EventHandler(this.Exit_Admin_Click);
            // 
            // commonBox1
            // 
            this.commonBox1.Location = new System.Drawing.Point(-3, 145);
            this.commonBox1.Name = "commonBox1";
            this.commonBox1.Size = new System.Drawing.Size(308, 390);
            this.commonBox1.TabIndex = 1;
            this.commonBox1.Text = "";
            this.commonBox1.Visible = false;
            // 
            // stu_Interface
            // 
            this.stu_Interface.Controls.Add(this.tea_Interface);
            this.stu_Interface.Controls.Add(this.Sdept);
            this.stu_Interface.Controls.Add(this.Sname);
            this.stu_Interface.Controls.Add(this.Sno);
            this.stu_Interface.Controls.Add(this.Confirm_New);
            this.stu_Interface.Controls.Add(this.label71);
            this.stu_Interface.Controls.Add(this.Confirm);
            this.stu_Interface.Controls.Add(this.label70);
            this.stu_Interface.Controls.Add(this.New_Password);
            this.stu_Interface.Controls.Add(this.label69);
            this.stu_Interface.Controls.Add(this.lab123);
            this.stu_Interface.Controls.Add(this.label52);
            this.stu_Interface.Controls.Add(this.Button_find);
            this.stu_Interface.Controls.Add(this.dateTimePicker1);
            this.stu_Interface.Controls.Add(this.label62);
            this.stu_Interface.Controls.Add(this.label53);
            this.stu_Interface.Controls.Add(this.InitButton);
            this.stu_Interface.Controls.Add(this.order_start);
            this.stu_Interface.Controls.Add(this.label56);
            this.stu_Interface.Controls.Add(this.order_end);
            this.stu_Interface.Controls.Add(this.label59);
            this.stu_Interface.Controls.Add(this.label57);
            this.stu_Interface.Controls.Add(this.label58);
            this.stu_Interface.Controls.Add(this.lab121);
            this.stu_Interface.Controls.Add(this.label61);
            this.stu_Interface.Location = new System.Drawing.Point(321, 23);
            this.stu_Interface.Name = "stu_Interface";
            this.stu_Interface.Size = new System.Drawing.Size(290, 504);
            this.stu_Interface.TabIndex = 26;
            this.stu_Interface.TabStop = false;
            this.stu_Interface.Text = "学生界面";
            this.stu_Interface.Visible = false;
            // 
            // Sdept
            // 
            this.Sdept.AutoSize = true;
            this.Sdept.Location = new System.Drawing.Point(90, 127);
            this.Sdept.Name = "Sdept";
            this.Sdept.Size = new System.Drawing.Size(47, 15);
            this.Sdept.TabIndex = 27;
            this.Sdept.Text = "sdept";
            // 
            // Sname
            // 
            this.Sname.AutoSize = true;
            this.Sname.Location = new System.Drawing.Point(90, 94);
            this.Sname.Name = "Sname";
            this.Sname.Size = new System.Drawing.Size(47, 15);
            this.Sname.TabIndex = 27;
            this.Sname.Text = "sname";
            // 
            // Sno
            // 
            this.Sno.AutoSize = true;
            this.Sno.Location = new System.Drawing.Point(90, 58);
            this.Sno.Name = "Sno";
            this.Sno.Size = new System.Drawing.Size(31, 15);
            this.Sno.TabIndex = 27;
            this.Sno.Text = "sno";
            // 
            // Confirm_New
            // 
            this.Confirm_New.Location = new System.Drawing.Point(92, 355);
            this.Confirm_New.Name = "Confirm_New";
            this.Confirm_New.PasswordChar = '*';
            this.Confirm_New.Size = new System.Drawing.Size(100, 25);
            this.Confirm_New.TabIndex = 30;
            // 
            // label71
            // 
            this.label71.AutoSize = true;
            this.label71.Font = new System.Drawing.Font("宋体", 12F);
            this.label71.Location = new System.Drawing.Point(15, 358);
            this.label71.Name = "label71";
            this.label71.Size = new System.Drawing.Size(109, 20);
            this.label71.TabIndex = 29;
            this.label71.Text = "再次输入：";
            // 
            // Confirm
            // 
            this.Confirm.Location = new System.Drawing.Point(18, 389);
            this.Confirm.Name = "Confirm";
            this.Confirm.Size = new System.Drawing.Size(75, 23);
            this.Confirm.TabIndex = 28;
            this.Confirm.Text = "确认";
            this.Confirm.UseVisualStyleBackColor = true;
            this.Confirm.Click += new System.EventHandler(this.Confirm_Click);
            // 
            // label70
            // 
            this.label70.AutoSize = true;
            this.label70.Font = new System.Drawing.Font("宋体", 12F);
            this.label70.Location = new System.Drawing.Point(16, 330);
            this.label70.Name = "label70";
            this.label70.Size = new System.Drawing.Size(89, 20);
            this.label70.TabIndex = 27;
            this.label70.Text = "新密码：";
            // 
            // New_Password
            // 
            this.New_Password.Location = new System.Drawing.Point(92, 325);
            this.New_Password.Name = "New_Password";
            this.New_Password.PasswordChar = '*';
            this.New_Password.Size = new System.Drawing.Size(100, 25);
            this.New_Password.TabIndex = 26;
            // 
            // label69
            // 
            this.label69.AutoSize = true;
            this.label69.BackColor = System.Drawing.SystemColors.GradientActiveCaption;
            this.label69.Location = new System.Drawing.Point(19, 300);
            this.label69.Name = "label69";
            this.label69.Size = new System.Drawing.Size(299, 15);
            this.label69.TabIndex = 25;
            this.label69.Text = "修改密码-----------------------------";
            // 
            // Form1
            // 
            this.BackColor = System.Drawing.SystemColors.ButtonFace;
            this.ClientSize = new System.Drawing.Size(1203, 555);
            this.Controls.Add(this.stu_Interface);
            this.Controls.Add(this.tabControl2);
            this.Controls.Add(this.commonBox1);
            this.Controls.Add(this.tabControl1);
            this.Name = "Form1";
            this.ShowInTaskbar = false;
            this.Text = "实验室管理系统";
            this.Load += new System.EventHandler(this.Form1_Load_1);
            this.tabControl1.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            this.tabPage1.PerformLayout();
            this.tea_Interface.ResumeLayout(false);
            this.tea_Interface.PerformLayout();
            this.admin_Interface.ResumeLayout(false);
            this.admin_Interface.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox26)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox27)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox28)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox29)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox30)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox31)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox32)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox33)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox34)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox35)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox36)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox37)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox38)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox39)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox40)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox41)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox42)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox43)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox44)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox45)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox46)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox47)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox48)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox49)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox50)).EndInit();
            this.tabPage2.ResumeLayout(false);
            this.tabPage2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox21)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox22)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox23)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox24)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox25)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox16)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox17)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox18)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox19)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox20)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox12)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox13)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox14)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox15)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox10)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox7)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox8)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox9)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox4)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox5)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox6)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox3)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.tabControl2.ResumeLayout(false);
            this.tabPage3.ResumeLayout(false);
            this.tabPage3.PerformLayout();
            this.group_stu.ResumeLayout(false);
            this.group_stu.PerformLayout();
            this.tabPage4.ResumeLayout(false);
            this.tabPage4.PerformLayout();
            this.group_Tea.ResumeLayout(false);
            this.group_Tea.PerformLayout();
            this.tabPage5.ResumeLayout(false);
            this.tabPage5.PerformLayout();
            this.group_Admin.ResumeLayout(false);
            this.group_Admin.PerformLayout();
            this.stu_Interface.ResumeLayout(false);
            this.stu_Interface.PerformLayout();
            this.ResumeLayout(false);

        }

        private void button1_Click(object sender, EventArgs e)
        {
            //this.button1.Text
            
      
        }
        private void button2_Click(object sender, EventArgs e)
        {

        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void Lab2_05_Click(object sender, EventArgs e)
        {
            String sno = this.Sno.Text.ToString();
            String sname = this.Sname.Text.ToString();
            String sdept = this.Sdept.Text.ToString();
            int start = Convert.ToInt32(this.order_start.Text.Trim());
            int end = Convert.ToInt32(this.order_end.Text.Trim());
            CommonClick(this.Lab2_05.Tag.ToString(), 5, sno, sname, sdept, this.dateTimePicker1, start, end, this.commonBox1);

        }

        private void Lab2_19_Click(object sender, EventArgs e)
        {
            String sno = this.Sno.Text.ToString();
            String sname = this.Sname.Text.ToString();
            String sdept = this.Sdept.Text.ToString();
            int start = Convert.ToInt32(this.order_start.Text.Trim());
            int end = Convert.ToInt32(this.order_end.Text.Trim());
            CommonClick(this.Lab2_19.Tag.ToString(), 19, sno, sname, sdept, this.dateTimePicker1, start, end, this.commonBox1);

        }

        private void Lab2_24_Click(object sender, EventArgs e)
        {
            String sno = this.Sno.Text.ToString();
            String sname = this.Sname.Text.ToString();
            String sdept = this.Sdept.Text.ToString();
            int start = Convert.ToInt32(this.order_start.Text.Trim());
            int end = Convert.ToInt32(this.order_end.Text.Trim());
            CommonClick(this.Lab2_24.Tag.ToString(), 24, sno, sname, sdept, this.dateTimePicker1, start, end, this.commonBox1);

        }

        private void Lab1_01_Click(object sender, EventArgs e)
        {
            String sno = this.Sno.Text.ToString();
            String sname = this.Sname.Text.ToString();
            String sdept = this.Sdept.Text.ToString();
            int start = Convert.ToInt32(this.order_start.Text.Trim());
            int end = Convert.ToInt32(this.order_end.Text.Trim());
            CommonClick(this.Lab1_01.Tag.ToString(), 1, sno,sname, sdept, this.dateTimePicker1, start, end,this.commonBox1);
     
        }

        private void Lab1_03_Click(object sender, EventArgs e)
        {
            String sno = this.Sno.Text.ToString();
            String sname = this.Sname.Text.ToString();
            String sdept = this.Sdept.Text.ToString();
            int start = Convert.ToInt32(this.order_start.Text.Trim());
            int end = Convert.ToInt32(this.order_end.Text.Trim());
            CommonClick(this.Lab1_03.Tag.ToString(), 3, sno, sname, sdept, this.dateTimePicker1, start, end, this.commonBox1);

        }

        private void button47_Click(object sender, EventArgs e)
        {

        }

        private void Lab1_04_Click(object sender, EventArgs e)
        {
            String sno = this.Sno.Text.ToString();
            String sname = this.Sname.Text.ToString();
            String sdept = this.Sdept.Text.ToString();
            int start = Convert.ToInt32(this.order_start.Text.Trim());
            int end = Convert.ToInt32(this.order_end.Text.Trim());
            CommonClick(this.Lab1_04.Tag.ToString(), 4, sno, sname, sdept, this.dateTimePicker1, start, end, this.commonBox1);

        }

        private void Lab1_05_Click(object sender, EventArgs e)
        {
            String sno = this.Sno.Text.ToString();
            String sname = this.Sname.Text.ToString();
            String sdept = this.Sdept.Text.ToString();
            int start = Convert.ToInt32(this.order_start.Text.Trim());
            int end = Convert.ToInt32(this.order_end.Text.Trim());
            CommonClick(this.Lab1_05.Tag.ToString(), 5, sno, sname, sdept, this.dateTimePicker1, start, end, this.commonBox1);

        }

        private void Lab1_25_Click(object sender, EventArgs e)
        {
            String sno = this.Sno.Text.ToString();
            String sname = this.Sname.Text.ToString();
            String sdept = this.Sdept.Text.ToString();
            int start = Convert.ToInt32(this.order_start.Text.Trim());
            int end = Convert.ToInt32(this.order_end.Text.Trim());
            CommonClick(this.Lab1_25.Tag.ToString(), 25, sno, sname, sdept, this.dateTimePicker1, start, end, this.commonBox1);

        }

        private void Lab2_02_Click(object sender, EventArgs e)
        {
            String sno = this.Sno.Text.ToString();
            String sname = this.Sname.Text.ToString();
            String sdept = this.Sdept.Text.ToString();
            int start = Convert.ToInt32(this.order_start.Text.Trim());
            int end = Convert.ToInt32(this.order_end.Text.Trim());
            CommonClick(this.Lab2_02.Tag.ToString(), 2, sno, sname, sdept, this.dateTimePicker1, start, end, this.commonBox1);

        }

        private void Lab2_01_Click(object sender, EventArgs e)
        {
            String sno = this.Sno.Text.ToString();
            String sname = this.Sname.Text.ToString();
            String sdept = this.Sdept.Text.ToString();
            int start = Convert.ToInt32(this.order_start.Text.Trim());
            int end = Convert.ToInt32(this.order_end.Text.Trim());
            CommonClick(this.Lab2_01.Tag.ToString(), 1, sno, sname, sdept, this.dateTimePicker1, start, end, this.commonBox1);

        }

        private void Lab2_18_Click(object sender, EventArgs e)
        {
            String sno = this.Sno.Text.ToString();
            String sname = this.Sname.Text.ToString();
            String sdept = this.Sdept.Text.ToString();
            int start = Convert.ToInt32(this.order_start.Text.Trim());
            int end = Convert.ToInt32(this.order_end.Text.Trim());
            CommonClick(this.Lab2_18.Tag.ToString(), 18, sno, sname, sdept, this.dateTimePicker1, start, end, this.commonBox1);

        }

        private void Lab1_02_Click(object sender, EventArgs e)
        {
            String sno = this.Sno.Text.ToString();
            String sname = this.Sname.Text.ToString();
            String sdept = this.Sdept.Text.ToString();
            int start = Convert.ToInt32(this.order_start.Text.Trim());
            int end = Convert.ToInt32(this.order_end.Text.Trim());
            CommonClick(this.Lab1_02.Tag.ToString(), 2, sno, sname, sdept, this.dateTimePicker1, start, end, this.commonBox1);

        }

        private void Lab1_06_Click(object sender, EventArgs e)
        {
            String sno = this.Sno.Text.ToString();
            String sname = this.Sname.Text.ToString();
            String sdept = this.Sdept.Text.ToString();
            int start = Convert.ToInt32(this.order_start.Text.Trim());
            int end = Convert.ToInt32(this.order_end.Text.Trim());
            CommonClick(this.Lab1_06.Tag.ToString(), 6, sno, sname, sdept, this.dateTimePicker1, start, end, this.commonBox1);

        }

        private void Lab1_07_Click(object sender, EventArgs e)
        {
            String sno = this.Sno.Text.ToString();
            String sname = this.Sname.Text.ToString();
            String sdept = this.Sdept.Text.ToString();
            int start = Convert.ToInt32(this.order_start.Text.Trim());
            int end = Convert.ToInt32(this.order_end.Text.Trim());
            CommonClick(this.Lab1_07.Tag.ToString(), 7, sno, sname, sdept, this.dateTimePicker1, start, end, this.commonBox1);

        }

        private void Lab1_08_Click(object sender, EventArgs e)
        {
            String sno = this.Sno.Text.ToString();
            String sname = this.Sname.Text.ToString();
            String sdept = this.Sdept.Text.ToString();
            int start = Convert.ToInt32(this.order_start.Text.Trim());
            int end = Convert.ToInt32(this.order_end.Text.Trim());
            CommonClick(this.Lab1_08.Tag.ToString(), 8, sno, sname, sdept, this.dateTimePicker1, start, end, this.commonBox1);

        }

        private void Lab1_09_Click(object sender, EventArgs e)
        {
            String sno = this.Sno.Text.ToString();
            String sname = this.Sname.Text.ToString();
            String sdept = this.Sdept.Text.ToString();
            int start = Convert.ToInt32(this.order_start.Text.Trim());
            int end = Convert.ToInt32(this.order_end.Text.Trim());
            CommonClick(this.Lab1_09.Tag.ToString(), 9, sno, sname, sdept, this.dateTimePicker1, start, end, this.commonBox1);

        }

        private void Lab1_10_Click(object sender, EventArgs e)
        {
            String sno = this.Sno.Text.ToString();
            String sname = this.Sname.Text.ToString();
            String sdept = this.Sdept.Text.ToString();
            int start = Convert.ToInt32(this.order_start.Text.Trim());
            int end = Convert.ToInt32(this.order_end.Text.Trim());
            CommonClick(this.Lab1_10.Tag.ToString(), 10, sno, sname, sdept, this.dateTimePicker1, start, end, this.commonBox1);

        }

        private void Lab1_11_Click(object sender, EventArgs e)
        {
            String sno = this.Sno.Text.ToString();
            String sname = this.Sname.Text.ToString();
            String sdept = this.Sdept.Text.ToString();
            int start = Convert.ToInt32(this.order_start.Text.Trim());
            int end = Convert.ToInt32(this.order_end.Text.Trim());
            CommonClick(this.Lab1_11.Tag.ToString(), 11, sno, sname, sdept, this.dateTimePicker1, start, end, this.commonBox1);

        }

        private void Lab1_12_Click(object sender, EventArgs e)
        {
            String sno = this.Sno.Text.ToString();
            String sname = this.Sname.Text.ToString();
            String sdept = this.Sdept.Text.ToString();
            int start = Convert.ToInt32(this.order_start.Text.Trim());
            int end = Convert.ToInt32(this.order_end.Text.Trim());
            CommonClick(this.Lab1_12.Tag.ToString(), 12, sno, sname, sdept, this.dateTimePicker1, start, end, this.commonBox1);

        }

        private void Lab1_13_Click(object sender, EventArgs e)
        {
            String sno = this.Sno.Text.ToString();
            String sname = this.Sname.Text.ToString();
            String sdept = this.Sdept.Text.ToString();
            int start = Convert.ToInt32(this.order_start.Text.Trim());
            int end = Convert.ToInt32(this.order_end.Text.Trim());
            CommonClick(this.Lab1_13.Tag.ToString(), 13, sno, sname, sdept, this.dateTimePicker1, start, end, this.commonBox1);

        }

        private void Lab1_14_Click(object sender, EventArgs e)
        {
            String sno = this.Sno.Text.ToString();
            String sname = this.Sname.Text.ToString();
            String sdept = this.Sdept.Text.ToString();
            int start = Convert.ToInt32(this.order_start.Text.Trim());
            int end = Convert.ToInt32(this.order_end.Text.Trim());
            CommonClick(this.Lab1_14.Tag.ToString(), 14, sno, sname, sdept, this.dateTimePicker1, start, end, this.commonBox1);

        }

        private void Lab1_15_Click(object sender, EventArgs e)
        {
            String sno = this.Sno.Text.ToString();
            String sname = this.Sname.Text.ToString();
            String sdept = this.Sdept.Text.ToString();
            int start = Convert.ToInt32(this.order_start.Text.Trim());
            int end = Convert.ToInt32(this.order_end.Text.Trim());
            CommonClick(this.Lab1_15.Tag.ToString(), 15, sno, sname, sdept, this.dateTimePicker1, start, end, this.commonBox1);

        }

        private void Lab1_16_Click(object sender, EventArgs e)
        {
            String sno = this.Sno.Text.ToString();
            String sname = this.Sname.Text.ToString();
            String sdept = this.Sdept.Text.ToString();
            int start = Convert.ToInt32(this.order_start.Text.Trim());
            int end = Convert.ToInt32(this.order_end.Text.Trim());
            CommonClick(this.Lab1_16.Tag.ToString(), 16, sno, sname, sdept, this.dateTimePicker1, start, end, this.commonBox1);

        }

        private void Lab1_17_Click(object sender, EventArgs e)
        {
            String sno = this.Sno.Text.ToString();
            String sname = this.Sname.Text.ToString();
            String sdept = this.Sdept.Text.ToString();
            int start = Convert.ToInt32(this.order_start.Text.Trim());
            int end = Convert.ToInt32(this.order_end.Text.Trim());
            CommonClick(this.Lab1_17.Tag.ToString(), 17, sno, sname, sdept, this.dateTimePicker1, start, end, this.commonBox1);

        }

        private void Lab1_19_Click(object sender, EventArgs e)
        {
            String sno = this.Sno.Text.ToString();
            String sname = this.Sname.Text.ToString();
            String sdept = this.Sdept.Text.ToString();
            int start = Convert.ToInt32(this.order_start.Text.Trim());
            int end = Convert.ToInt32(this.order_end.Text.Trim());
            CommonClick(this.Lab1_19.Tag.ToString(), 19, sno, sname, sdept, this.dateTimePicker1, start, end, this.commonBox1);

        }

        private void Lab1_18_Click(object sender, EventArgs e)
        {
            String sno = this.Sno.Text.ToString();
            String sname = this.Sname.Text.ToString();
            String sdept = this.Sdept.Text.ToString();
            int start = Convert.ToInt32(this.order_start.Text.Trim());
            int end = Convert.ToInt32(this.order_end.Text.Trim());
            CommonClick(this.Lab1_18.Tag.ToString(), 18, sno, sname, sdept, this.dateTimePicker1, start, end, this.commonBox1);

        }

        private void Lab1_20_Click(object sender, EventArgs e)
        {
            String sno = this.Sno.Text.ToString();
            String sname = this.Sname.Text.ToString();
            String sdept = this.Sdept.Text.ToString();
            int start = Convert.ToInt32(this.order_start.Text.Trim());
            int end = Convert.ToInt32(this.order_end.Text.Trim());
            CommonClick(this.Lab1_20.Tag.ToString(), 20, sno, sname, sdept, this.dateTimePicker1, start, end, this.commonBox1);

        }

        private void Lab1_21_Click(object sender, EventArgs e)
        {
            String sno = this.Sno.Text.ToString();
            String sname = this.Sname.Text.ToString();
            String sdept = this.Sdept.Text.ToString();
            int start = Convert.ToInt32(this.order_start.Text.Trim());
            int end = Convert.ToInt32(this.order_end.Text.Trim());
            CommonClick(this.Lab1_21.Tag.ToString(), 21, sno, sname, sdept, this.dateTimePicker1, start, end, this.commonBox1);

        }

        private void Lab1_22_Click(object sender, EventArgs e)
        {
            String sno = this.Sno.Text.ToString();
            String sname = this.Sname.Text.ToString();
            String sdept = this.Sdept.Text.ToString();
            int start = Convert.ToInt32(this.order_start.Text.Trim());
            int end = Convert.ToInt32(this.order_end.Text.Trim());
            CommonClick(this.Lab1_22.Tag.ToString(), 22, sno, sname, sdept, this.dateTimePicker1, start, end, this.commonBox1);

        }

        private void Lab1_23_Click(object sender, EventArgs e)
        {
            String sno = this.Sno.Text.ToString();
            String sname = this.Sname.Text.ToString();
            String sdept = this.Sdept.Text.ToString();
            int start = Convert.ToInt32(this.order_start.Text.Trim());
            int end = Convert.ToInt32(this.order_end.Text.Trim());
            CommonClick(this.Lab1_23.Tag.ToString(), 23, sno, sname, sdept, this.dateTimePicker1, start, end, this.commonBox1);

        }

        private void Lab1_24_Click(object sender, EventArgs e)
        {
            String sno = this.Sno.Text.ToString();
            String sname = this.Sname.Text.ToString();
            String sdept = this.Sdept.Text.ToString();
            int start = Convert.ToInt32(this.order_start.Text.Trim());
            int end = Convert.ToInt32(this.order_end.Text.Trim());
            CommonClick(this.Lab1_24.Tag.ToString(), 24, sno, sname, sdept, this.dateTimePicker1, start, end, this.commonBox1);

        }

      
        private void Form1_Load_1(object sender, EventArgs e)
        {

        }

        private void InitButton_Click(object sender, EventArgs e)
        {
            this.commonBox1.Text = "初始化";
            this.order_end.Text = "2";
            this.order_start.Text = "1";
            this.Sno.Text = "201526204041";
            this.Sname.Text = "郭爱斌";
            this.Sdept.Text = "计算机信息工程学院";
            String sql = "select Lno, Lname, Addr from lab";
            mysqlConnect.Open();

            MySqlCommand mySqlCommand = new MySqlCommand(sql, mysqlConnect);
            MySqlDataReader reader = mySqlCommand.ExecuteReader();
            try
            {
                while (reader.Read())
                {
                    if (reader.GetString("Lno").Equals("lab1"))
                    {
                        this.lab1_name.Text = reader.GetString("Lname");
                        this.lab1_addr.Text = reader.GetString("Addr");
                    }
                    if (reader.GetString("Lno").Equals("lab2"))
                    {
                        this.lab2_name.Text = reader.GetString("Lname");
                        this.lab2_addr.Text = reader.GetString("Addr");
                    }

                }
            }
            catch (Exception)
            {
                Console.WriteLine("查询失败了！");
            }
            finally
            {
                reader.Close();
            }
            mysqlConnect.Close();
        }

        private void dateTimePicker1_ValueChanged(object sender, EventArgs e)
        {

        }

        private void Lab2_03_Click(object sender, EventArgs e)
        {
            String sno = this.Sno.Text.ToString();
            String sname = this.Sname.Text.ToString();
            String sdept = this.Sdept.Text.ToString();
            int start = Convert.ToInt32(this.order_start.Text.Trim());
            int end = Convert.ToInt32(this.order_end.Text.Trim());
            CommonClick(this.Lab2_03.Tag.ToString(), 3, sno, sname, sdept, this.dateTimePicker1, start, end, this.commonBox1);

        }

        private void Lab2_04_Click(object sender, EventArgs e)
        {
            String sno = this.Sno.Text.ToString();
            String sname = this.Sname.Text.ToString();
            String sdept = this.Sdept.Text.ToString();
            int start = Convert.ToInt32(this.order_start.Text.Trim());
            int end = Convert.ToInt32(this.order_end.Text.Trim());
            CommonClick(this.Lab2_04.Tag.ToString(), 4, sno, sname, sdept, this.dateTimePicker1, start, end, this.commonBox1);

        }

        private void Lab2_06_Click(object sender, EventArgs e)
        {
            String sno = this.Sno.Text.ToString();
            String sname = this.Sname.Text.ToString();
            String sdept = this.Sdept.Text.ToString();
            int start = Convert.ToInt32(this.order_start.Text.Trim());
            int end = Convert.ToInt32(this.order_end.Text.Trim());
            CommonClick(this.Lab2_06.Tag.ToString(), 6, sno, sname, sdept, this.dateTimePicker1, start, end, this.commonBox1);

        }

        private void Lab2_07_Click(object sender, EventArgs e)
        {
            String sno = this.Sno.Text.ToString();
            String sname = this.Sname.Text.ToString();
            String sdept = this.Sdept.Text.ToString();
            int start = Convert.ToInt32(this.order_start.Text.Trim());
            int end = Convert.ToInt32(this.order_end.Text.Trim());
            CommonClick(this.Lab2_07.Tag.ToString(), 7, sno, sname, sdept, this.dateTimePicker1, start, end, this.commonBox1);

        }

        private void Lab2_08_Click(object sender, EventArgs e)
        {
            String sno = this.Sno.Text.ToString();
            String sname = this.Sname.Text.ToString();
            String sdept = this.Sdept.Text.ToString();
            int start = Convert.ToInt32(this.order_start.Text.Trim());
            int end = Convert.ToInt32(this.order_end.Text.Trim());
            CommonClick(this.Lab2_08.Tag.ToString(), 8, sno, sname, sdept, this.dateTimePicker1, start, end, this.commonBox1);

        }

        private void Lab2_09_Click(object sender, EventArgs e)
        {
            String sno = this.Sno.Text.ToString();
            String sname = this.Sname.Text.ToString();
            String sdept = this.Sdept.Text.ToString();
            int start = Convert.ToInt32(this.order_start.Text.Trim());
            int end = Convert.ToInt32(this.order_end.Text.Trim());
            CommonClick(this.Lab2_09.Tag.ToString(), 9, sno, sname, sdept, this.dateTimePicker1, start, end, this.commonBox1);

        }

        private void Lab2_10_Click(object sender, EventArgs e)
        {
            String sno = this.Sno.Text.ToString();
            String sname = this.Sname.Text.ToString();
            String sdept = this.Sdept.Text.ToString();
            int start = Convert.ToInt32(this.order_start.Text.Trim());
            int end = Convert.ToInt32(this.order_end.Text.Trim());
            CommonClick(this.Lab2_10.Tag.ToString(), 10, sno, sname, sdept, this.dateTimePicker1, start, end, this.commonBox1);

        }

        private void Lab2_11_Click(object sender, EventArgs e)
        {
            String sno = this.Sno.Text.ToString();
            String sname = this.Sname.Text.ToString();
            String sdept = this.Sdept.Text.ToString();
            int start = Convert.ToInt32(this.order_start.Text.Trim());
            int end = Convert.ToInt32(this.order_end.Text.Trim());
            CommonClick(this.Lab2_11.Tag.ToString(), 11, sno, sname, sdept, this.dateTimePicker1, start, end, this.commonBox1);

        }

        private void Lab2_12_Click(object sender, EventArgs e)
        {
            String sno = this.Sno.Text.ToString();
            String sname = this.Sname.Text.ToString();
            String sdept = this.Sdept.Text.ToString();
            int start = Convert.ToInt32(this.order_start.Text.Trim());
            int end = Convert.ToInt32(this.order_end.Text.Trim());
            CommonClick(this.Lab2_12.Tag.ToString(), 12, sno, sname, sdept, this.dateTimePicker1, start, end, this.commonBox1);

        }

        private void Lab2_13_Click(object sender, EventArgs e)
        {
            String sno = this.Sno.Text.ToString();
            String sname = this.Sname.Text.ToString();
            String sdept = this.Sdept.Text.ToString();
            int start = Convert.ToInt32(this.order_start.Text.Trim());
            int end = Convert.ToInt32(this.order_end.Text.Trim());
            CommonClick(this.Lab2_13.Tag.ToString(), 13, sno, sname, sdept, this.dateTimePicker1, start, end, this.commonBox1);

        }

        private void Lab2_14_Click(object sender, EventArgs e)
        {
            String sno = this.Sno.Text.ToString();
            String sname = this.Sname.Text.ToString();
            String sdept = this.Sdept.Text.ToString();
            int start = Convert.ToInt32(this.order_start.Text.Trim());
            int end = Convert.ToInt32(this.order_end.Text.Trim());
            CommonClick(this.Lab2_14.Tag.ToString(), 14, sno, sname, sdept, this.dateTimePicker1, start, end, this.commonBox1);

        }

        private void Lab2_15_Click(object sender, EventArgs e)
        {
            String sno = this.Sno.Text.ToString();
            String sname = this.Sname.Text.ToString();
            String sdept = this.Sdept.Text.ToString();
            int start = Convert.ToInt32(this.order_start.Text.Trim());
            int end = Convert.ToInt32(this.order_end.Text.Trim());
            CommonClick(this.Lab2_15.Tag.ToString(), 15, sno, sname, sdept, this.dateTimePicker1, start, end, this.commonBox1);

        }

        private void Lab2_16_Click(object sender, EventArgs e)
        {
            String sno = this.Sno.Text.ToString();
            String sname = this.Sname.Text.ToString();
            String sdept = this.Sdept.Text.ToString();
            int start = Convert.ToInt32(this.order_start.Text.Trim());
            int end = Convert.ToInt32(this.order_end.Text.Trim());
            CommonClick(this.Lab2_16.Tag.ToString(), 16, sno, sname, sdept, this.dateTimePicker1, start, end, this.commonBox1);

        }

        private void Lab2_17_Click(object sender, EventArgs e)
        {
            String sno = this.Sno.Text.ToString();
            String sname = this.Sname.Text.ToString();
            String sdept = this.Sdept.Text.ToString();
            int start = Convert.ToInt32(this.order_start.Text.Trim());
            int end = Convert.ToInt32(this.order_end.Text.Trim());
            CommonClick(this.Lab2_17.Tag.ToString(), 17, sno, sname, sdept, this.dateTimePicker1, start, end, this.commonBox1);

        }

        private void Lab2_20_Click(object sender, EventArgs e)
        {
            String sno = this.Sno.Text.ToString();
            String sname = this.Sname.Text.ToString();
            String sdept = this.Sdept.Text.ToString();
            int start = Convert.ToInt32(this.order_start.Text.Trim());
            int end = Convert.ToInt32(this.order_end.Text.Trim());
            CommonClick(this.Lab2_20.Tag.ToString(), 20, sno, sname, sdept, this.dateTimePicker1, start, end, this.commonBox1);

        }

        private void Lab2_21_Click(object sender, EventArgs e)
        {
            String sno = this.Sno.Text.ToString();
            String sname = this.Sname.Text.ToString();
            String sdept = this.Sdept.Text.ToString();
            int start = Convert.ToInt32(this.order_start.Text.Trim());
            int end = Convert.ToInt32(this.order_end.Text.Trim());
            CommonClick(this.Lab2_21.Tag.ToString(), 21, sno, sname, sdept, this.dateTimePicker1, start, end, this.commonBox1);

        }

        private void Lab2_22_Click(object sender, EventArgs e)
        {
            String sno = this.Sno.Text.ToString();
            String sname = this.Sname.Text.ToString();
            String sdept = this.Sdept.Text.ToString();
            int start = Convert.ToInt32(this.order_start.Text.Trim());
            int end = Convert.ToInt32(this.order_end.Text.Trim());
            CommonClick(this.Lab2_22.Tag.ToString(), 22, sno, sname, sdept, this.dateTimePicker1, start, end, this.commonBox1);

        }

        private void Lab2_23_Click(object sender, EventArgs e)
        {
            String sno = this.Sno.Text.ToString();
            String sname = this.Sname.Text.ToString();
            String sdept = this.Sdept.Text.ToString();
            int start = Convert.ToInt32(this.order_start.Text.Trim());
            int end = Convert.ToInt32(this.order_end.Text.Trim());
            CommonClick(this.Lab2_23.Tag.ToString(), 23, sno, sname, sdept, this.dateTimePicker1, start, end, this.commonBox1);

        }

      

        private void Button_find_Click_1(object sender, EventArgs e)
        {
            mysqlConnect.Open();
            DateTime date_now = DateTime.Now;
            String sno = this.Sno.Text.ToString();
            String str = "select * from my_order where Sno like '{0}'";
            String cmd_str = String.Format(str,sno);
            MySqlCommand cmd_select = new MySqlCommand(cmd_str,mysqlConnect);
            MySqlDataReader reader1 = cmd_select.ExecuteReader();
            String text = "你的预约信息：\n实验室编号    设备号     日期      时间段\n";
            try
            {
                while (reader1.Read())
                {
                    DateTime dateTime = reader1.GetDateTime("Order_time");
                    int year = dateTime.Year;
                    int month = dateTime.Month;
                    int day = dateTime.Day;
                    int days = CalDays(year, month, day) - CalDays(date_now.Year, date_now.Month, date_now.Day);
                    if(days>=0)
                    {
                        String lab = reader1.GetString("Lno");
                        int dev_no = reader1.GetInt32("Dev_no");
                        int start_course = reader1.GetInt32("Start_course");
                        int end_course = reader1.GetInt32("End_course");
                        text += lab + "       " + dev_no + "  " + year + "年" + month + "月" + day + "日" + "  " + start_course + "-" + end_course + "节课"+"\n";

                    }
                }
                this.commonBox1.Text = text;
            }
            catch (Exception )
            {
            }
            finally
            {
                reader1.Close();
            }
            mysqlConnect.Close();
        }

        private void Statistics_Click(object sender, EventArgs e)
        {
            mysqlConnect.Open();
            MySqlConnection mysqlConnect2 = TestDatebase.GetMySqlCon();
            mysqlConnect2.Open();
            String str = "select * from statistics";
            String str2 = "select * from lab where Lno like '{0}'";
            MySqlCommand data_select = new MySqlCommand(str,mysqlConnect);
            
            MySqlDataReader reader = data_select.ExecuteReader();
            String text = "显示设备利用率信息：\n实验室名称    设备号     日期      利用率\n";

            try
            {
                while (reader.Read())
                {
                    DateTime dateTime = reader.GetDateTime("Order_time");
                    String lno = reader.GetString("Lno");
                    int dev_no = reader.GetInt32("Dev_no");
                    String efficient = reader.GetString("Efficient");
                    String format1 = String.Format(str2,lno);
                    String lname = TestDatebase.getResultset(new MySqlCommand(format1, mysqlConnect2),"Lname");
                    text += lname + "    " + dev_no+"号"+"   " + dateTime.Year + "年" + dateTime.Month + "月" + dateTime.Day + "日" + "    " + efficient+"\n";
                }
            }
            catch (Exception )
            {
            }
            finally
            {
                reader.Close();
            }
            this.commonBox1.Text = text;
            mysqlConnect.Close();
            mysqlConnect2.Close();
        }

        private void Sno_TextChanged(object sender, EventArgs e)
        {

        }

        private void tabPage3_Click(object sender, EventArgs e)
        {

        }

        private void tabPage4_Click(object sender, EventArgs e)
        {

        }

        private void Stu_Load_Click(object sender, EventArgs e)
        {
            mysqlConnect.Open();
            string sno = this.Stu_Sno.Text;
            string sPassword = this.Stu_Password.Text;
            string sname=null;
            string sdept=null;
            string format = "select * from student where Sno like '{0}' and Password like '{1}'";
            string strCmd = string.Format(format, sno, sPassword);
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
            if(isCorrect==true)
            {
                this.Sno.Text = sno;
                this.Sname.Text = sname;
                this.Sdept.Text = sdept;
                this.group_stu.Visible = false;
                this.group_Tea.Visible = false;
                this.group_Admin.Visible = false;
                this.stu_tag.Text = "你好！ " + sname;
                this.stu_tag.Visible = true;
                 this.Exit_Stu.Visible = true;
                this.stu_Interface.Visible = true;
                this.tea_Interface.Visible = false;
                this.admin_Interface.Visible = false;
                
               
                this.tabControl1.Visible = true;
                this.commonBox1.Text = null;
                this.commonBox1.Visible = true;
                
            }else
            {
                System.Windows.Forms.MessageBox.Show("帐号或密码错误,请检查后重新登录", "错误", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            mysqlConnect.Close();
        }

        private void groupBox3_Enter(object sender, EventArgs e)
        {

        }

        private void ExitButton2_Click(object sender, EventArgs e)
        {
            System.Windows.Forms.MessageBox.Show("期望您的再次登录", "温馨", MessageBoxButtons.OK, MessageBoxIcon.Information);
            this.group_Tea.Visible = true;
            this.group_Admin.Visible = true;
            this.group_stu.Visible = true;
            this.Exit_Tea.Visible = false;
            this.tea_Interface.Visible = false;
            this.stu_Interface.Visible = false;
            this.Tea_tag.Visible = false;
            
            this.commonBox1.Visible = false;
            this.commonBox1.Text = null;
            this.Tea_End.Text = null;
            this.Tea_Start.Text = null;
            this.Tea_New_Again.Text = null;
            this.Tea_New_Password.Text = null;

        }

        private void Exit_Stu_Click(object sender, EventArgs e)
        {
            System.Windows.Forms.MessageBox.Show("期望您的再次登录", "温馨", MessageBoxButtons.OK, MessageBoxIcon.Information);
            this.group_Admin.Visible = true;
            this.group_Tea.Visible = true;
            this.group_stu.Visible = true;
            this.Exit_Stu.Visible = false;
            this.stu_Interface.Visible = false;
            this.stu_tag.Visible = false;
            this.tabControl1.Visible = false;
            this.commonBox1.Visible = false;
            this.commonBox1.Text = "";
            this.New_Password.Text = "";
            this.Confirm_New.Text = "";
            this.order_end.Text = "";
            this.order_start.Text = "";
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
                
                this.group_stu.Visible = false;
                this.group_Tea.Visible = false;
                this.group_Admin.Visible = false;
                
                this.Tea_tag.Text = "你好！ " + Tea_name+"老师";
                this.Tea_tag.Visible = true;
                                                                              
                this.Exit_Tea.Visible = true;
                
                this.tabControl1.Visible = false;
                this.commonBox1.Text = "";
                this.commonBox1.Visible = true;
                this.stu_Interface.Visible = true;
                this.tea_Interface.Visible = true;
                this.admin_Interface.Visible = false;
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

                this.group_Admin.Visible = false;

                this.Admin_tag.Text = "你好！ " + Admin_name ;
                this.Admin_tag.Visible = true;
                this.stu_Interface.Visible = true;
                this.tea_Interface.Visible = true;
                this.admin_Interface.Visible = true;
                
                this.Exit_Admin.Visible = true;
                
                this.group_Tea.Visible = false;
                this.group_stu.Visible = false;
                
                

                this.tabControl1.Visible = false;
                this.commonBox1.Text = "";
                this.commonBox1.Visible = true;
            }
            else
            {
                System.Windows.Forms.MessageBox.Show("帐号或密码错误,请检查后重新登录", "错误", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            mysqlConnect.Close();
        }

        private void Exit_Admin_Click(object sender, EventArgs e)
        {
            System.Windows.Forms.MessageBox.Show("期望您的再次登录", "温馨", MessageBoxButtons.OK, MessageBoxIcon.Information);
            this.group_Admin.Visible = true;
            this.group_Tea.Visible = true;
            this.group_stu.Visible = true;
            this.Exit_Admin.Visible = false;
            this.admin_Interface.Visible = false;
            this.stu_Interface.Visible = false;
            this.tea_Interface.Visible = false;
            this.Admin_tag.Visible = false;
            //this.tabControl1.Visible = false;
            this.commonBox1.Visible = false;
            this.commonBox1.Text = "";
            this.Admin_New.Text = "";
            this.Admin_New_Again.Text = "";

            this.Admin_Sno.Text = "";
            this.Admin_Stu_Name.Text = "";
            this.Admin_Stu_Password.Text = "";
            this.Admin_Stu_Sdept.Text = "";
            this.Admin_tag.Text = "";
            this.Admin_Tea_Name.Text = "";
            this.Admin_Tea_no.Text = "";
            this.Admin_Tea_Password.Text = "";
            
        }

        private void Confirm_Click(object sender, EventArgs e)
        {
            mysqlConnect.Open();
            string sno = this.Sno.Text.ToString().Trim();
            String password_new = this.New_Password.Text.Trim();
            String password_confirm = this.Confirm_New.Text.Trim();

            string format = "update student set Password = '{0}' where Sno='{1}'";
            
            if (password_new.Equals(password_confirm)==true)
            {
                string str = string.Format(format, password_confirm, sno);
                MySqlCommand cmd = new MySqlCommand(str,mysqlConnect);
                try
                {
                    if (cmd.ExecuteNonQuery() > 0)
                    {
                        System.Windows.Forms.MessageBox.Show("密码修改成功", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }
                catch (Exception)
                {

                }
                finally
                {
                    mysqlConnect.Close();
                }
            }
            else
            {
                System.Windows.Forms.MessageBox.Show("请再次确认密码", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
        }

        private void Tea_Start_TextChanged(object sender, EventArgs e)
        {

        }

        private void Tea_Clock_Click(object sender, EventArgs e)
        {
            String Tno = this.Tea_no.Text.ToString().Trim();
            int start = Convert.ToInt32(this.Tea_Start.Text.Trim());
            int end = Convert.ToInt32(this.Tea_End.Text.Trim());
            DateTime date = DateTime.Now;
            String str2 = "select count(*) from student";
            mysqlConnect.Open();
            MySqlCommand cmd2 = new MySqlCommand(str2,mysqlConnect);
            MySqlDataReader reader2 = cmd2.ExecuteReader();
            int sum = 0;
            try
            {
                while(reader2.Read())
                {
                    sum = reader2.GetInt32(0);
                }
            }
            catch (Exception)
            {
            }
            finally
            {
                reader2.Close();
                mysqlConnect.Close();
            }
            mysqlConnect.Open();
            string str1 = "select count(Order_no)  from my_order  where 0 = (select DATEDIFF(Order_time, '{0}-{1}-{2}')) and Start_course={3} and End_course={4}";
            string cmdstr = String.Format(str1,date.Year,date.Month,date.Day, start, end);
            MySqlCommand cmd1 = new MySqlCommand(cmdstr,mysqlConnect);
            MySqlDataReader reader1 = cmd1.ExecuteReader();
            int realNums = 0;
            try
            {
                while (reader1.Read())
                {
                    realNums = reader1.GetInt32(0);
                }
            }
            catch (Exception)
            {
            }
            finally
            {
                reader1.Close();
                mysqlConnect.Close();
            }
            int sub = sum - realNums;
            mysqlConnect.Open();
            string format3 = "insert into mycheck values('{0}',{1},{2},'{3}',{4})";    
            string str3 = String.Format(format3, Tno,start,end,date,sub);
            MySqlCommand cmd3 = new MySqlCommand(str3,mysqlConnect);
            try
            {
                if (cmd3.ExecuteNonQuery()>0)
                {
                    String tip = "应到" + sum + "人\n" + "实到" + realNums + "人\n" + "缺勤" + sub + "人\n";
                    this.commonBox1.Text = tip;
                }
            }
            catch (Exception)
            {
            }
            finally
            {
                mysqlConnect.Close();
            }
        }

        private void Tea_Find_Click(object sender, EventArgs e)
        {
            mysqlConnect.Open();
            String cmd_str = "select * from student order by Sno";
            MySqlCommand cmd_select = new MySqlCommand(cmd_str, mysqlConnect);
            MySqlDataReader reader1 = cmd_select.ExecuteReader();
            String text = "学生名单：\n";
            text +="    "+"学号           姓名   系别\n";
            try
            {
                while (reader1.Read())
                {
                    string sno = reader1.GetString("Sno");
                    string sname = reader1.GetString("Sname");
                    string sdept = reader1.GetString("Sdept");
                        text += "   "+sno + "     " + sname + "   " + sdept + "\n";                   
                }
                this.commonBox1.Text = text;
            }
            catch (Exception)
            {
            }
            finally
            {
                reader1.Close();
            }
            mysqlConnect.Close();
        }

        private void Tea_Confirm_Click(object sender, EventArgs e)
        {
            mysqlConnect.Open();
            string T_no = this.Tea_no.Text.ToString().Trim();
            String password_new = this.Tea_New_Password.Text.Trim();
            String password_confirm = this.Tea_New_Again.Text.Trim();
            string format = "update teacher set Tea_password = '{0}' where Tea_no='{1}'";
            if (password_new.Equals(password_confirm) == true)
            {
                string str = string.Format(format, password_confirm, T_no);
                MySqlCommand cmd = new MySqlCommand(str, mysqlConnect);
                TestDatebase.Update(cmd);                          
            }
            else
            {
                System.Windows.Forms.MessageBox.Show("请再次确认密码", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            mysqlConnect.Close();
        }

        private void Admin_Register_Stu_Click(object sender, EventArgs e)
        {
            mysqlConnect.Open();
            string sno = this.Admin_Sno.Text;
            string sname = this.Admin_Stu_Name.Text;
            string sdept = this.Admin_Stu_Sdept.Text;
            string password = this.Admin_Stu_Password.Text;
            string format1 = "insert into student(Sno,Sname,Sdept,Password) values('{0}','{1}','{2}','{3}')";
            string cmdStr = String.Format(format1,sno,sname,sdept,password);
            MySqlCommand insertCmd = new MySqlCommand(cmdStr,mysqlConnect);
            TestDatebase.Register(insertCmd);
            mysqlConnect.Close();
            
        }

        private void Admin_Stu_Reset_Click(object sender, EventArgs e)
        {
            mysqlConnect.Open();
            string sno = this.Admin_Sno.Text.ToString().Trim();           
            string format = "update student set Password = '123456' where Sno='{0}'";
            string str = string.Format(format,sno);
            MySqlCommand cmd = new MySqlCommand(str, mysqlConnect);
            TestDatebase.Reset(cmd);
            mysqlConnect.Close();
        }

        private void Admin_Tea_Reset_Click(object sender, EventArgs e)
        {
            mysqlConnect.Open();
            string Tno = this.Admin_Tea_no.Text.ToString().Trim();
            string format = "update teacher set Tea_password = '123456' where Tea_no='{0}'";
            string str = string.Format(format,Tno);
            MySqlCommand cmd = new MySqlCommand(str, mysqlConnect);
            TestDatebase.Reset(cmd);
            mysqlConnect.Close();
        }

        private void Admin_Confirm_Click(object sender, EventArgs e)
        {
            mysqlConnect.Open();
            string Admin_no = this.Admin_no.Text.ToString().Trim();
            String password_new = this.Admin_New.Text.Trim();
            String password_confirm = this.Admin_New_Again.Text.Trim();
            string format = "update administer set Admin_password = '{0}' where Admin_no='{1}'";
            if (password_new.Equals(password_confirm) == true)
            {
                string str = string.Format(format, password_confirm, Admin_no);
                MySqlCommand cmd = new MySqlCommand(str, mysqlConnect);
                TestDatebase.Update(cmd);
            }
            else
            {
                System.Windows.Forms.MessageBox.Show("请再次确认密码", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            mysqlConnect.Close();
        }

        private void Admin_Register_Tea_Click(object sender, EventArgs e)
        {
            mysqlConnect.Open();
            string Tea_no = this.Admin_Tea_no.Text;
            string Tea_name = this.Admin_Tea_Name.Text;        
            string Tea_password = this.Admin_Tea_Password.Text;
            string format1 = "insert into values('{0}','{1}','{2}')";
            string cmdStr = String.Format(format1, Tea_no, Tea_name, Tea_password);
            MySqlCommand insertCmd = new MySqlCommand(cmdStr, mysqlConnect);
            TestDatebase.Register(insertCmd);
            mysqlConnect.Close();
            
        }
    }
}
