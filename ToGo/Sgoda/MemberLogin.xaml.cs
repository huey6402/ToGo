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
    /// Interaction logic for CMLogin.xaml
    /// </summary>
    public partial class MemberLogin : Window
    {
        List<LogonFailUser> logonFailUsers = new List<LogonFailUser>(); //紀錄登入錯誤的帳號及次數
        bool haveThisLogonFailUser = false;
        int thisLogonFailUser;

        public MemberLogin()
        {
            InitializeComponent();
            
            Storyboard sb1 = (Storyboard)this.Resources["windowDown"];
            sb1.Begin();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (this.UsernameTextBox.Text == "")
            {
                this.EmailLabel.Content = "*請輸入E-mail";
                this.UsernameTextBox.Focus();
                return;
            }
            else
            {
                this.EmailLabel.Content = "";
            }
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
                        cmd.CommandText = "Select Email,IsLocking from Member where Email = @UserName";
                        conn.Open();
                        SqlDataReader reader = cmd.ExecuteReader();
                        if (reader.Read() && Convert.ToInt32(reader["IsLocking"]) == 1) // 檢查是否被鎖
                        {
                            reader.Close();
                            MessageBox.Show("此帳號已被鎖定，請重設密碼");
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
                                cmd.CommandText = "Select Email,Password,FirstName,LastName,PhoneNumber from Member where Email = @UserName and Password = @Password";
                                using (SqlDataReader reader2 = cmd.ExecuteReader())
                                {
                                    if (reader2.HasRows) // 檢查如果會員資料表有使用者輸入的帳密
                                    {
                                        while (reader2.Read())
                                        {
                                            // 主視窗所需資訊-------------------------------------------↓
                                            MainWindow.LoginFirstName = $"{reader2["FirstName"]}";
                                            MainWindow.LoginLastName = $"{reader2["LastName"]}";
                                            MainWindow.LoginEmail = $"{reader2["Email"]}";
                                            MainWindow.LoginPhone = $"{reader2["PhoneNumber"]}";
                                            // 主視窗右上顯示會員名字---------------------------------------↓
                                            string Name = $"{reader2["FirstName"]} {reader2["LastName"]}";
                                            LoginNameButton LNB = new LoginNameButton();
                                            LNB.LoginNameButton_Content = Name;
                                            LNB.LoginNameButtonFirstChar_Content = Name[0];
                                            LNB.Margin = new Thickness(1650,10,0,0);
                                            (this.Owner as MainWindow).stackPanel1.Children.Add(LNB); // 子視窗回傳父視窗
                                            (this.Owner as MainWindow).LoginButton.Visibility = Visibility.Hidden;
                                            (this.Owner as MainWindow).NewMemberButton.Visibility = Visibility.Hidden;
                                            (this.Owner as MainWindow).NewMemberButton1.Visibility = Visibility.Hidden;
                                            (this.Owner as MainWindow).GreetLabel.Content = Name + " 您好，歡迎回來！";

                                            Button Logout = new Button();
                                            Logout.Content = "登出";
                                            Logout.Foreground = new SolidColorBrush(Colors.White);
                                            Logout.Background = new SolidColorBrush(Colors.Gray);
                                            Logout.Click += Logout_Click;
                                            Logout.Width = 50;
                                            Logout.Height = 30;
                                            Logout.FontSize = 20;
                                            Logout.BorderThickness = new Thickness(0);
                                            Logout.Margin = new Thickness(1300, -40, 0, 0);
                                            (this.Owner as MainWindow).stackPanel1.Children.Add(Logout); // 子視窗回傳父視窗
                                        }
                                        reader2.Close();
                                        (this.Owner as MainWindow).Background = new SolidColorBrush(Colors.White);
                                        MessageBox.Show(MainWindow.LoginEmail + "登入成功");

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

                                        if (logonFailUsers[thisLogonFailUser].failTimes == 3) // 輸入錯誤第4次時就鎖帳號
                                        {
                                            cmd.CommandText = "Update Member set IsLocking = 'true' where Email = '" + logonFailUsers[thisLogonFailUser].userName + "'";
                                            SqlDataReader reader3 = cmd.ExecuteReader();
                                            reader3.Close();
                                            MessageBox.Show("密碼錯誤3次，帳號已被鎖定，請重設密碼","登入失敗");
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

        private void Logout_Click(object sender, RoutedEventArgs e)
        {
            
        }

        private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            this.DragMove();
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            (this.Owner as MainWindow).Background = new SolidColorBrush(Colors.White);
            this.Close();
        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            this.Close();
            NewMember w = new NewMember();
            w.Owner = this.Owner;
            w.ShowDialog();
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

        private void Button_Click_3(object sender, RoutedEventArgs e)
        {
            this.Close();
            ForgetPassword w = new ForgetPassword();
            w.Owner = this.Owner;
            w.ShowDialog();
        }

    }
}
