using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using ToGo.Properties;

namespace ToGo
{
    /// <summary>
    /// OwnerLogin.xaml 的互動邏輯
    /// </summary>
    public partial class OwnerLogin : Window
    {
        public OwnerLogin()
        {
            InitializeComponent();
            Storyboard sb1 = (Storyboard)this.Resources["windowUp"];
            sb1.Begin();
        }

        List<LogonFailUser> logonFailUsers = new List<LogonFailUser>(); //紀錄登入錯誤的帳號及次數
        bool haveThisLogonFailUser = false;
        int thisLogonFailUser;

        private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            this.DragMove();
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            (this.Owner as MainWindow).Background = new SolidColorBrush(Colors.White);
            this.Close();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            string userName = UsernameTextBox.Text;
            string password = this.PasswordTextBox.Password;
            
            try
            {
                using (SqlConnection conn = new SqlConnection(Settings.Default.ToGoConnectionString))
                {
                    using (SqlCommand cmd = new SqlCommand())
                    {
                        cmd.Parameters.Add("@UserName", SqlDbType.VarChar, 40).Value = userName;
                        cmd.Parameters.Add("@Password", SqlDbType.VarChar, 20).Value = password;
                        cmd.Connection = conn;
                        cmd.CommandText = "Select Email,IsLocking from Owner where Email = @UserName";
                        conn.Open();
                        SqlDataReader reader = cmd.ExecuteReader();
                        if (reader.Read() && Convert.ToInt32(reader["IsLocking"]) == 1) // 檢查是否被鎖
                        {
                            reader.Close();
                            MessageBox.Show("此帳號已被鎖定，請聯絡客服人員");
                            this.UsernameTextBox.Clear();
                            this.PasswordTextBox.Clear();
                            this.UsernameTextBox.Focus();
                            return;
                        }

                        reader.Close();
                        cmd.CommandText = "Select Email from Member where Email = @UserName";
                        using (SqlDataReader reader1 = cmd.ExecuteReader())
                        {
                            if (reader1.HasRows) // 帳號正確(先檢查有無此帳號)才開始計算錯誤次數
                            {
                                reader1.Close();
                                cmd.CommandText = "Select Email,Password,OwnerName from Owner where Email = @UserName and Password = @Password";
                                using (SqlDataReader reader2 = cmd.ExecuteReader())
                                {
                                    if (reader2.HasRows) // 檢查如果會員資料表有使用者輸入的帳密
                                    {
                                        while(reader2.Read())
                                        {
                                            MainWindow.OwnerLoginEmail = $"{reader2["Email"]}";
                                        }
                                        MessageBox.Show(MainWindow.OwnerLoginEmail + "登入成功");
                                        reader2.Close();
                                        (this.Owner as MainWindow).Background = new SolidColorBrush(Colors.White);
                                        this.Close();
                                    }
                                    else
                                    {
                                        reader2.Close();
                                        for (int i = 0; i < logonFailUsers.Count; i++) // 檢查此帳號是不是有密碼錯誤紀錄
                                        {
                                            if (userName == logonFailUsers[i].userName)
                                            {
                                                haveThisLogonFailUser = true;
                                                thisLogonFailUser = i; // 此帳號陣列位置
                                                break;   // 檢查到即停止檢查
                                            }
                                            else
                                            {
                                                haveThisLogonFailUser = false;
                                            }
                                        }

                                        if (haveThisLogonFailUser == false) // 沒有紀錄就新增帳號的錯誤紀錄
                                        {
                                            logonFailUsers.Add(new LogonFailUser(userName, 1));
                                            this.PasswordTextBox.Clear();
                                            MessageBox.Show("密碼錯誤(1)", "登入失敗");
                                            return; // 新增錯誤記錄完 結束
                                        }

                                        logonFailUsers[thisLogonFailUser].failTimes += 1; // 錯誤次數加1

                                        if (logonFailUsers[thisLogonFailUser].failTimes > 3) // 輸入錯誤超過3次時就鎖帳號
                                        {
                                            cmd.CommandText = "Update Owner set IsLocking = 'true' where Email = '" + logonFailUsers[thisLogonFailUser].userName + "'";
                                            SqlDataReader reader3 = cmd.ExecuteReader();
                                            reader3.Close();
                                            MessageBox.Show("此帳號已被鎖定，請聯絡客服人員","登入失敗");
                                            this.UsernameTextBox.Clear();
                                            this.PasswordTextBox.Clear();
                                            this.UsernameTextBox.Focus();
                                            return;
                                        }

                                        MessageBox.Show("密碼錯誤(" + logonFailUsers[thisLogonFailUser].failTimes + ")", "登入失敗");
                                        //this.UsernameTextBox.Clear();
                                        this.PasswordTextBox.Clear();
                                        this.UsernameTextBox.Focus();
                                    }
                                }
                            }
                            else
                            {
                                MessageBox.Show("登入失敗 - 無此帳號", "警告");
                                this.UsernameTextBox.Clear();
                                this.PasswordTextBox.Clear();
                                this.UsernameTextBox.Focus();
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        class LogonFailUser        //紀錄登入錯誤的帳號及次數、是否被鎖
        {
            internal string userName;
            internal int failTimes;

            internal LogonFailUser(string name, int failTime)
            {
                userName = name;
                failTimes = failTime;
            }
        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            this.Close();
            NewOwner w = new NewOwner();
            w.Owner = this.Owner;
            w.ShowDialog();
        }
    }
}
