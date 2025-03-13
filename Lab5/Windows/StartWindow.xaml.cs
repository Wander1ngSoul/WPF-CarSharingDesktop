using Lab5.Windows;
using System.Linq;
using System.Windows;

namespace Lab5
{
    public partial class StartWindow : Window
    {
        private Employee _currentUser;

        public StartWindow(Employee currentUser)
        {
            InitializeComponent();
            _currentUser = currentUser;
            if (_currentUser != null)
            {
                WelcomeTextBlock.Text = $"Добро пожаловать, {_currentUser.FirstName}";

               
            }
            else
            {
                MessageBox.Show("Ошибка: пользователь не найден.");
                this.Close();
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            new CarsWindow(_currentUser).Show();
            this.Close();
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            new ClientsWindow(_currentUser).Show();
            this.Close();
        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            new EmployeesWindow(_currentUser, null).Show();
            this.Close();
        }

        private void Button_Click_3(object sender, RoutedEventArgs e)
        {
            if (_currentUser != null)
            {
                new BookingWindow(_currentUser).Show();
                this.Close();
            }
            else
            {
                MessageBox.Show("Ошибка: текущий пользователь не был передан.");
            }
        }

        private void LogoutButton_Click(object sender, RoutedEventArgs e)
        {
            new MainWindow().Show();
            this.Close();

        }
    }
}
