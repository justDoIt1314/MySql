using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WindowsFormsApp1
{
    static class Program
    {
        /// <summary>
        /// 应用程序的主入口点。
        /// </summary>
        [STAThread]

        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Thread thread1 = new Thread(new ThreadStart(Thread1));
            thread1.Start();           
            Application.Run(new Form2());

            //thread1.Start();
        }
        
        static void Thread1()
        {
            DateTime date_now = DateTime.Now;
            // if (date_now.Hour == 11 && date_now.Minute == 7 && date_now.Second == 0)
            //  {
            int year = date_now.Year;
            int month = date_now.Month;
            int day = date_now.Day;
            String str1 = "select Lno, Dev_no, Order_time,Start_course,End_course  from my_order  where 0 = (select DATEDIFF(Order_time, '{0}-{1}-{2}')) group by Lno,Dev_no";
            String strCmd = String.Format(str1, year, month, day);

            String format2 = "insert into statistics(Order_time,Lno,Dev_no,Efficient) values('{0}-{1}-{2}','{3}',{4},'{5}')";

            MySqlConnection mySqlConnection1 = TestDatebase.GetMySqlCon();
            mySqlConnection1.Open();
            MySqlConnection mySqlConnection2 = TestDatebase.GetMySqlCon();
            mySqlConnection2.Open();
            MySqlCommand myCmd = new MySqlCommand(strCmd, mySqlConnection1);
            MySqlDataReader reader = myCmd.ExecuteReader();
            try
            {
                while (reader.Read())
                {
                    String lno = (String)reader[0];
                    int dev_no = (int)reader[1];
                    DateTime dateTime = (DateTime)reader[2];
                    int b1 = Convert.ToInt32(reader[3].ToString());
                    int b2 = Convert.ToInt32(reader[4].ToString());
                    int b = b2 - b1;
                    double a = (double)(b * 1.0 / 9.0);

                    double effi = (double)Math.Round(a, 2) * 100;
                    String efficient = effi.ToString() + "%";

                    String str_insert = String.Format(format2, dateTime.Year, dateTime.Month, dateTime.Day, lno, dev_no, efficient);
                    MySqlCommand mycmd_insert = new MySqlCommand(str_insert, mySqlConnection2);
                    //this.commonBox1.Text = 
                    TestDatebase.GetInsert(mycmd_insert);
                }
            }
            catch (Exception)
            {

            }
            finally
            {
                reader.Close();
            }
            mySqlConnection1.Close();
            mySqlConnection2.Close();

        }
    }
}
