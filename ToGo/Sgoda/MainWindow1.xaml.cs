using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace ToGo
{
    /// <summary>
    /// MainWindow.xaml 的互動邏輯
    /// </summary>

    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            this.WindowStartupLocation = WindowStartupLocation.CenterScreen;
        }

        public static string LoginFirstName; // 登入成功後存取名字
        public static string LoginLastName;
        public static string LoginEmail;
        public static string LoginPhone;

        public static string OwnerLoginEmail;

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            MemberLogin w = new MemberLogin();
            w.Owner = this;
            this.Background = new SolidColorBrush(Colors.Gray);
            w.ShowDialog();
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            NewMember w = new NewMember();
            w.Owner = this;
            this.Background = new SolidColorBrush(Colors.Gray);
            w.ShowDialog();
        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            OwnerLogin w = new OwnerLogin();
            w.Owner = this;
            this.Background = new SolidColorBrush(Colors.Gray);
            w.ShowDialog();
        }
    }
}
