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
    /// Interaction logic for CMmaker.xaml
    /// </summary>
    public partial class NewMember : Window
    {
        public NewMember()
        {
            InitializeComponent();
            this.comboBox1.ItemsSource = dbContext.Countries.ToList();
            Storyboard sb1 = (Storyboard)this.Resources["windowDown"];
            sb1.Begin();
        }

        ToGoEntities dbContext = new ToGoEntities();
        int countryID;

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
            try
            {
                using (SqlConnection conn = new SqlConnection(Settings.Default.ToGoConnectionString))
                {
                    using (SqlCommand cmd = new SqlCommand("Select Email from Member where Email = @UserName", conn)) // 檢查帳號是否已有人使用
                    {
                        string userName = this.EmailTextBox.Text;

                        cmd.Parameters.Add("@UserName", SqlDbType.VarChar, 40).Value = userName;
                        conn.Open();
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.HasRows || EmailTextBox.Text == "" || PasswordTextBox.Password == "" || PasswordTextBox.Password.Length < 8 || FirstNameTextBox.Text == ""
                            || LastNameTextBox.Text == "" || PhoneTextBox.Text == "" || comboBox1.Text == "")
                            {
                                if (this.EmailTextBox.Text == "") { this.label1.Content = "*請輸入信箱"; } else { this.label1.Content = ""; }
                                if (this.PasswordTextBox.Password == "") { this.label2.Content = "*請輸入密碼"; }
                                else if (this.PasswordTextBox.Password.Length < 8) { this.label2.Content = "*請輸入8位數密碼"; this.PasswordTextBox.Clear(); }
                                else { this.label2.Content = ""; }
                                if (this.FirstNameTextBox.Text == "") { this.label3.Content = "*請輸入名字"; } else { this.label3.Content = ""; }
                                if (this.LastNameTextBox.Text == "") { this.label4.Content = "*請輸入姓"; } else { this.label4.Content = ""; }
                                if (this.PhoneTextBox.Text == "") { this.label5.Content = "*請輸入手機號碼"; } else { this.label5.Content = ""; }
                                if (this.comboBox1.Text == "") { this.label6.Content = "*請選擇國家"; } else { this.label6.Content = ""; }
                                if (reader.HasRows) // 檢查如果會員資料表已有新用戶輸入的帳號
                                {
                                    MessageBox.Show("此信箱已使用，請重新輸入其他信箱", "提示");
                                    this.label1.Content = "*請輸入其他信箱";
                                    this.label2.Content = "*請重新輸入密碼";
                                    this.EmailTextBox.Clear();
                                    this.PasswordTextBox.Clear();
                                    this.EmailTextBox.Focus();
                                }
                            }
                            else
                            {
                                reader.Close();
                                using (SqlCommand cmd1 = new SqlCommand("Insert into Member (Email,Password,FirstName,LastName,PhoneNumber,CountryID) values (@UserName,@Password,@FirstName,@LastName,@PhoneNumber,@Country)", conn))
                                {

                                    string password = this.PasswordTextBox.Password;
                                    string firstName = this.FirstNameTextBox.Text;
                                    string lastName = this.LastNameTextBox.Text;
                                    string phone = this.PhoneTextBox.Text;

                                    cmd1.Parameters.Add("@UserName", SqlDbType.VarChar, 40).Value = userName;
                                    cmd1.Parameters.Add("@Password", SqlDbType.VarChar, 10).Value = password;
                                    cmd1.Parameters.Add("@FirstName", SqlDbType.VarChar, 20).Value = firstName;
                                    cmd1.Parameters.Add("@LastName", SqlDbType.VarChar, 10).Value = lastName;
                                    cmd1.Parameters.Add("@PhoneNumber", SqlDbType.VarChar, 10).Value = phone;
                                    cmd1.Parameters.Add("@Country", SqlDbType.Int).Value = countryID;

                                    cmd1.ExecuteNonQuery();

                                    MessageBox.Show("新增用戶成功");
                                    (this.Owner as MainWindow).Background = new SolidColorBrush(Colors.White);
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

        private void comboBox1_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var c = comboBox1.SelectedValue as Country;
            countryID = c.CountryID;
        }
    }
}
