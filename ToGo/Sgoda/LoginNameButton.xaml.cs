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
    /// LoginNameButton.xaml 的互動邏輯
    /// </summary>
    public partial class LoginNameButton : Button
    {
        public LoginNameButton()
        {
            InitializeComponent();
        }

        public string LoginNameButton_Content
        {
            set
            {
                this.loginNameLabel.Content = value;
            }
        }

        public char LoginNameButtonFirstChar_Content
        {
            set
            {
                this.loginNameFirstChar.Content = value;
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            StackPanel stackPanel1 = new StackPanel();
            stackPanel1.Background = new SolidColorBrush(Colors.White);
            Button button1 = new Button();
            stackPanel1.Children.Add(button1);
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            MemberOrderHistory w = new MemberOrderHistory();
            w.ShowDialog();
        }
    }
}
