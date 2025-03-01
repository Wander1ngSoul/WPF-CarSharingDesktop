using System;
using System.Data.SqlClient;
using System.Windows;

namespace Lab5
{
    public partial class MainWindow : Window
    {
        private readonly string connectionString = "Server=your_server;Database=CarSharingDB;User Id=your_user;Password=your_password;";

        public MainWindow()
        {
            InitializeComponent();
        }

        private void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            string email = EmailTextBox.Text;
            string password = PasswordBox.Password;

            if (AuthenticateUser(email, password))
            {
                MessageBox.Show("Успешный вход!", "Авторизация", MessageBoxButton.OK, MessageBoxImage.Information);
                // Открыть следующую страницу
                //this.Hide();
                //var mainAppWindow = new MainAppWindow(); // Подставьте имя окна приложения
                //mainAppWindow.Show();
                //this.Close();
            }
            else
            {
                MessageBox.Show("Неверные данные!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private bool AuthenticateUser(string email, string password)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                try
                {
                    connection.Open();
                    string query = "SELECT COUNT(1) FROM Client WHERE Email = @Email AND Password = @Password";
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@Email", email);
                        command.Parameters.AddWithValue("@Password", password);

                        int count = Convert.ToInt32(command.ExecuteScalar());
                        return count > 0;
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка подключения: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    return false;
                }
            }
        }
    }
}
