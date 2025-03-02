using Lab5.Windows;
using System.Windows;

namespace Lab5
{
    public partial class StartWindow : Window
    {
        public StartWindow(string employeeName)
        {
            InitializeComponent();
            WelcomeTextBlock.Text = $"Добро пожаловать, {employeeName}";
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            new CarsWindow().Show();
            this.Close();
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            new ClientsWindow().Show();
            this.Close();
        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            new EmployeesWindow().Show();
            this.Close();
        }

        private void Button_Click_3(object sender, RoutedEventArgs e)
        {
            new BookingWindow().Show();
            this.Close();
        }
    }
}
