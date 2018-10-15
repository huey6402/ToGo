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
    /// NewOwner.xaml 的互動邏輯
    /// </summary>
    public partial class NewOwner : Window
    {
        public NewOwner()
        {
            InitializeComponent();
            Storyboard sb1 = (Storyboard)this.Resources["windowUp"];
            sb1.Begin();
        }

        private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            this.DragMove();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
            OwnerLogin w = new OwnerLogin();
            w.Owner = this.Owner;
            w.ShowDialog();
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(Settings.Default.ToGoConnectionString))
                {
                    using (SqlCommand cmd = new SqlCommand("Select Email from Owner where Email = @UserName", conn)) // 檢查帳號是否已有人使用
                    {
                        string userName = this.EmailTextBox.Text;

                        cmd.Parameters.Add("@UserName", SqlDbType.VarChar, 40).Value = userName;
                        conn.Open();
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.HasRows || EmailTextBox.Text == "" || PasswordTextBox.Password == "" || PasswordTextBox.Password.Length < 8 || NameTextBox.Text == "" || LineTextBox.Text == "" || PhoneTextBox.Text == "")
                            {
                                if (this.NameTextBox.Text == "") { this.label1.Content = "*請輸入姓名"; } else { this.label1.Content = ""; }
                                if (this.EmailTextBox.Text == "") { this.label2.Content = "*請輸入信箱"; } else { this.label2.Content = ""; }
                                if (this.PasswordTextBox.Password == "") { this.label3.Content = "*請輸入密碼"; }
                                else if (this.PasswordTextBox.Password.Length < 8) { this.label3.Content = "*請輸入8位數密碼"; this.PasswordTextBox.Clear(); }
                                else { this.label3.Content = ""; }
                                if (this.PhoneTextBox.Text == "") { this.label4.Content = "*請輸入手機號碼"; } else { this.label4.Content = ""; }
                                if (this.LineTextBox.Text == "") { this.label5.Content = "*請輸入LineID"; } else { this.label5.Content = ""; }
                                if (reader.HasRows) // 檢查如果會員資料表已有新用戶輸入的帳號
                                {
                                    MessageBox.Show("此信箱已使用，請重新輸入其他信箱", "提示");
                                    this.label2.Content = "*請輸入其他信箱";
                                    this.label3.Content = "*請重新輸入密碼";
                                    this.EmailTextBox.Clear();
                                    this.PasswordTextBox.Clear();
                                    this.EmailTextBox.Focus();
                                }
                            }
                            else
                            {
                                reader.Close();
                                using (SqlCommand cmd1 = new SqlCommand("Insert into Owner (Email,Password,OwnerName,PhoneNumber,Line) values (@Email,@Password,@OwnerName,@PhoneNumber,@Line)", conn))
                                {
                                    string password = this.PasswordTextBox.Password;
                                    string name = this.NameTextBox.Text;
                                    string line = this.LineTextBox.Text;
                                    string phone = this.PhoneTextBox.Text;

                                    cmd1.Parameters.Add("@Email", SqlDbType.VarChar, 40).Value = userName;
                                    cmd1.Parameters.Add("@Password", SqlDbType.VarChar, 20).Value = password;
                                    cmd1.Parameters.Add("@OwnerName", SqlDbType.NVarChar, 30).Value = name;
                                    cmd1.Parameters.Add("@PhoneNumber", SqlDbType.VarChar, 10).Value = phone;
                                    cmd1.Parameters.Add("@Line", SqlDbType.VarChar, 20).Value = line;

                                    cmd1.ExecuteNonQuery();
                                    ((MainWindow)this.Owner).Background = new SolidColorBrush(Colors.White);
                                    MessageBox.Show("新增用戶成功");

                                    this.Close();
                                }
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

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            ((MainWindow)this.Owner).Background = new SolidColorBrush(Colors.White);
            this.Close();
        }
    }
}
