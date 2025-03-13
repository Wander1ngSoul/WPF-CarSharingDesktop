using System;
using System.Linq;
using System.Windows;
using static Lab5.Windows.CarsWindow;

namespace Lab5
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void LoginButton_Click(object sender, RoutedEventArgs args)
        {
            string email = EmailTextBox.Text;
            string password = PasswordBox.Password;

            using (var context = new CarSharingDBEntities())
            {
                try
                {
                    var employee = context.Employee
                                          .FirstOrDefault(emp => emp.Email == email && emp.Password == password);

                    if (employee != null)
                    {
                        MessageBox.Show("Успешный вход!", "Авторизация", MessageBoxButton.OK, MessageBoxImage.Information);

                        AuthService.SetCurrentUser(employee.Position);

                        this.Hide();
                        var startWindow = new StartWindow(employee);
                        startWindow.Show();
                        this.Close();
                    }
                    else
                    {
                        MessageBox.Show("Неверные данные!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка подключения: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

    }
}
