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

            this.CalendarSearch.DisplayDateStart = DateTime.Today;
            this.CalendarSearch.DisplayDateEnd = DateTime.Today.AddMonths(3);
        }

        public static string LoginFirstName; // 登入成功後存取名字
        public static string LoginLastName;
        public static string LoginEmail;
        public static string LoginPhone;

        private void AddDataToTextBox()
        {
            //===========================================
            // Todo:首頁國家、城市搜尋TextBox自動帶入城市名
            //===========================================
        }

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

        private void Button_Click_3(object sender, RoutedEventArgs e)
        {
            OwnerLogin w = new OwnerLogin();
            w.Owner = this;
            this.Background = new SolidColorBrush(Colors.Gray);
            w.ShowDialog();
        }

        global::ToGo.ToGoEntities dbContext = new ToGo.ToGoEntities();

        public static DateTime _StartDate;
        public static DateTime _EndDate;
        public static string _City;
        public static int _RoomType;

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            if (TextBox_City.Text == "")
            {
                City_Require.Opacity = 1;
            }
            else if (SearchSDate.Content.ToString()=="請選擇")
            {
                Date_Require.Opacity = 1;
            }
            else if(TextBox_Room.Text=="")
            {
                Room_Require.Opacity = 1;
            }
            else if(TxtRoomType.Text=="")
            {
                People_Require.Opacity = 1;
            }
            else
            {
                List<int> RoomIdCollection = new List<int>();
                List<int> IsOrderedRoom = new List<int>();

                _StartDate = CalendarSearch.SelectedDates[0]; //(DateTime)SearchSDate.Content;
                _EndDate = CalendarSearch.SelectedDates[CalendarSearch.SelectedDates.Count - 1]; //(DateTime)SearchEDate.Content;
                _City = TextBox_City.Text;
                _RoomType = int.Parse(TxtRoomType.Text);

                using (SqlConnection conn = new SqlConnection("Data Source=.;Initial Catalog=ToGo;Integrated Security=True"))
                {
                    using (SqlCommand cmd = new SqlCommand())
                    {
                        cmd.Connection = conn;

                        cmd.CommandText = "select  r1.CityCHName,r1.HotelNameCN,r1.GoogleMapUri,min(r1.UnitPrice) from V_Room r1 where r1.CityCHName=@CityCHName and  not exists (select 1 from( select r.CityCHName,r.HotelID, r.RoomID from V_Room r left join v_order o on r.HotelID= o.HotelID and r.RoomID=o.RoomID where r.CityCHName=@CityCHName and r.RoomType=@RoomType and  @SearchStartDate  between ISNULL( o.StartDate, '2099-11-05') and ISNULL( dateadd( day,-1,o.EndDate),'2099-11-05')or  @SearchEndDate   between ISNULL( o.StartDate, '2099-11-05') and ISNULL(  o.EndDate,'2099-11-05')or o.StartDate between @SearchStartDate and @SearchEndDate or DATEADD(day,-1 ,o.EndDate) between @SearchStartDate and @SearchEndDate) r2 where r1.CityCHName =r2.CityCHName and r1.HotelID =r2.HotelID and r1.RoomID  =r2.RoomID)group by r1.CityCHName,r1.HotelNameCN,r1.GoogleMapUri";
                        cmd.Parameters.Add("@RoomType", SqlDbType.Int).Value = _RoomType;
                        cmd.Parameters.Add("@CityCHName", SqlDbType.NVarChar).Value = _City;
                        cmd.Parameters.Add("@SearchStartDate", SqlDbType.Date).Value = _StartDate;
                        cmd.Parameters.Add("@SearchEndDate", SqlDbType.Date).Value = _EndDate;
                        
                        conn.Open();
                        //Hotel_Search ww = new Hotel_Search();
                        //using (SqlDataReader dr = cmd.ExecuteReader())
                        //{
                        //    int a = 0;//搜尋出的資料筆數
                        //    while (dr.Read())
                        //    {
                        //        Window_HotelButton xx = new Window_HotelButton();
                        //        xx.City = dr[0];//CityCHName
                        //        xx.DESC = dr[1];//HotelNameCN
                        //        xx.TempUrl = dr[2].ToString(); //URL
                        //        xx.price = dr[3];//UnitPrice
                        //        xx.Btn_Map.Click += Btn_Map_Click;
                        //        ww.StackPanel_ShowHotel.Children.Add(xx);
                        //        a++;
                        //    }
                        //    ww.Label_HotelCount.Content = a;
                        //}
                        this.Close();
                        //ww.Show();
                    }
                }
            }
        }

        private void Btn_Map_Click(object sender, RoutedEventArgs e)
        {            
            
        }

        public static DateTime _SearchSDate;
        public static DateTime _SearchEDate;
        
        private void searchDate_SelectedDatesChanged(object sender, SelectionChangedEventArgs e)
        {
            this.SearchSDate.Content = this.CalendarSearch.SelectedDates[0].ToString("yyyy/MM/dd");
            _SearchSDate= this.CalendarSearch.SelectedDates[0];
            this.SearchEDate.Content = this.CalendarSearch.SelectedDates[CalendarSearch.SelectedDates.Count - 1].ToString("yyyy/MM/dd");
            _SearchEDate= this.CalendarSearch.SelectedDates[CalendarSearch.SelectedDates.Count - 1];
        }

        
    }
}
