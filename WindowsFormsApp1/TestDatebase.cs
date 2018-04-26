using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;
using MySql.Data.MySqlClient;
using System.Windows.Forms;

namespace WindowsFormsApp1
{
    class TestDatebase
    {
        public TestDatebase()
        {
            //
            // TODO: 在此处添加构造函数逻辑
            //
        }
        
        /// <summary>
        /// 建立mysql数据库链接
        /// </summary>
        /// <returns></returns>
        public static MySqlConnection GetMySqlCon()
        {
            String mysqlStr = "server=localhost;User Id=root;password=123456;Database=laboratory";
            MySqlConnection mysql = new MySqlConnection(mysqlStr);
            return mysql;
        }

        /// <summary>
        /// 建立执行命令语句对象
        /// </summary>
        /// <param name='sql'></param>
        /// <param name='mysql'></param>
        /// <returns></returns>
        public static MySqlCommand getSqlCommand(String sql, MySqlConnection mysql)
        {
            MySqlCommand mySqlCommand = new MySqlCommand(sql, mysql);
            return mySqlCommand;
        }
        /// <summary>
        /// 查询并获得结果集并遍历
        /// </summary>
        /// <param name='mySqlCommand'></param>
        public static String getResultset(MySqlCommand mySqlCommand,String agv)
        {
            MySqlDataReader reader = mySqlCommand.ExecuteReader();
            try
            {
                while (reader.Read())
                {
                    return reader.GetString(agv);    
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
            return null;
        }
        /// <summary>
        /// 添加数据
        /// </summary>
        /// <param name='mySqlCommand'></param>
        public static String GetInsert(MySqlCommand mySqlCommand)
        {
            try
            {
                if(mySqlCommand.ExecuteNonQuery()>0)
                    System.Windows.Forms.MessageBox.Show("插入成功", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);

            }
            catch (Exception ex)
            {
                String message = ex.Message;
                return message;
            }
            return null;
        }
        /// <summary>
        /// 修改数据
        /// </summary>
        /// <param name='mySqlCommand'></param>
        public static void getUpdate(MySqlCommand mySqlCommand)
        {
            try
            {
                mySqlCommand.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                String message = ex.Message;
                Console.WriteLine("修改数据失败了！" + message);
            }
        }
        /// <summary>
        /// 删除数据
        /// </summary>
        /// <param name='mySqlCommand'></param>
        public static void getDel(MySqlCommand mySqlCommand)
        {
            try
            {
                mySqlCommand.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                String message = ex.Message;
                Console.WriteLine("删除数据失败了！" + message);
            }
        }
    }
}
