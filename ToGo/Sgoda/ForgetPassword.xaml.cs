using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using ToGo.Properties;

namespace ToGo
{
    /// <summary>
    /// ForgetPassword.xaml 的互動邏輯
    /// </summary>
    public partial class ForgetPassword : Window
    {
        public ForgetPassword()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            string userEmail = this.mailTextBox.Text;

            using (SqlConnection conn = new SqlConnection(Settings.Default.ToGoConnectionString))
            {
                using(SqlCommand cmd = new SqlCommand())
                {
                    cmd.Parameters.Add("@UserEmail", SqlDbType.VarChar, 40).Value = userEmail;
                    cmd.Connection = conn;
                    cmd.CommandText = "select FirstName,LastName,Email from Member where Email = @UserEmail";
                    conn.Open();
                    SqlDataReader dataReader = cmd.ExecuteReader();
                    if (dataReader.Read())
                    {
                        Random random = new Random();
                        MailMessage mail = new MailMessage("yhkcjo@gmail.com", Convert.ToString(dataReader["Email"]));
                        SmtpClient client = new SmtpClient("smtp.gmail.com", 587);
                        int newCode = random.Next(10000000, 100000000);
                        mail.Subject = "(重要信件)ToGo會員密碼變更信函";
                        mail.Body = $"親愛的 {dataReader["FirstName"]} {dataReader["LastName"]} 您好： \r\n您的密碼已暫時更改為：{newCode} ，請盡速更換您的密碼";
                        mail.IsBodyHtml = false;
                        client.Credentials = new NetworkCredential("yhkcjo@gmail.com", "y hk4cjo4");
                        client.DeliveryMethod = SmtpDeliveryMethod.Network;
                        client.EnableSsl = true;
                        client.Send(mail);

                        // 寄信後變更資料庫密碼
                        dataReader.Close(); // 一定要關
                        cmd.CommandText = "update member set Password = @NewPassword,IsLocking = 'false' where Email = @UserEmail";
                        cmd.Parameters.Add("@NewPassword", SqlDbType.VarChar, 10).Value = newCode;
                        cmd.ExecuteNonQuery();

                        MessageBox.Show("密碼變更信函已送出，請至信箱收取信件");
                        ((MainWindow)this.Owner).Background = new SolidColorBrush(Colors.White);
                        this.Close();
                    }
                    else
                    {
                        MessageBox.Show("似乎沒有ToGo帳號使用此電子郵件的紀錄。");
                    }
                }
            }
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            ((MainWindow)this.Owner).Background = new SolidColorBrush(Colors.White);
            this.Close();
        }

        private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            this.DragMove();
        }
    }
}
