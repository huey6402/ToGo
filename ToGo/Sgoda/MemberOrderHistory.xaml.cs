using System;
using System.Collections.Generic;
using System.Data.Entity;
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
using System.Windows.Shapes;


namespace ToGo
{
    /// <summary>
    /// MemberOrderHistory.xaml 的互動邏輯
    /// </summary>
    public partial class MemberOrderHistory : Window
    {
        public MemberOrderHistory()
        {
            InitializeComponent();
            this.Title = $"{MainWindow.LoginFirstName} {MainWindow.LoginLastName} 的訂單查詢";
        }

        global::ToGo.ToGoEntities dbContext = new ToGoEntities();
        //CollectionViewSource OrderViewSource;

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            //this.dbContext.Orders.Load();
            //OrderViewSource = ((CollectionViewSource)(this.FindResource("OrderViewSource")));
            //OrderViewSource.Source = this.dbContext.Orders.Local;
            // 透過設定 CollectionViewSource.Source 屬性載入資料: 
            // orderViewSource.Source = [泛用資料來源]
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            DateTime orderDate = DateTime.Today;
            if(this.datePicker1.SelectedDate.HasValue)
            {
                orderDate = this.datePicker1.SelectedDate.Value;
            }
            
            this.dataGrid1.ItemsSource = this.dbContext.Orders.Where(d => d.OrderDate == orderDate && d.Email == MainWindow.LoginEmail).
                Select(o => new { o.HotelName,o.RoomName, o.OrderDate,o.StartDate,o.EndDate,o.TotalPrice,o.OrderState1.OrderStateName,o.IsPay }).ToList();
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            DateTime orderDate1 = DateTime.Today;
            DateTime orderDate2 = DateTime.Today;
            if (this.datePicker2.SelectedDate.HasValue && this.datePicker3.SelectedDate.HasValue)
            {
                orderDate1 = this.datePicker2.SelectedDate.Value;
                orderDate2 = this.datePicker3.SelectedDate.Value;
            }
            else
            {
                MessageBox.Show("請輸入入住日期與退房日期");
            }

            this.dataGrid1.ItemsSource = this.dbContext.Orders.Where(d => d.StartDate >= orderDate1 && d.EndDate <= orderDate2 && d.Email == MainWindow.LoginEmail).
                Select(o => new { o.HotelName, o.RoomName, o.OrderDate, o.StartDate, o.EndDate, o.TotalPrice, o.OrderState1.OrderStateName, o.IsPay }).ToList();
        }
    }
}
