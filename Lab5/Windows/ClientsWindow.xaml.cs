using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace Lab5.Windows
{
    public partial class ClientsWindow : Window
    {
        private Employee _currentUser;

        public ClientsWindow(Employee currentUser)
        {
            InitializeComponent();
            _currentUser = currentUser ?? throw new ArgumentNullException(nameof(currentUser), "Текущий пользователь не может быть null.");

            MessageBox.Show($"Текущий пользователь: {_currentUser?.FirstName}, Должность: {_currentUser?.Position}", "Отладка", MessageBoxButton.OK, MessageBoxImage.Information);

            LoadClients();
        }



        private void LoadClients()
        {
            using (var context = new CarSharingDB1Entities())
            {
                ClientsDataGrid.ItemsSource = context.Client.ToList();
            }
        }

        private void CreateClient_Click(object sender, RoutedEventArgs e)
        {
            if (_currentUser.Position == "Manager")
            {
                var clientEditWindow = new ClientEditWindow(_currentUser, null);
                clientEditWindow.Show();
                this.Close();
            }
            else
            {
                MessageBox.Show("Вы не имеете прав для создания клиента.");
            }
        }

        private void EditClient_Click(object sender, RoutedEventArgs e)
        {
            if (_currentUser.Position == "Manager" && ClientsDataGrid.SelectedItem is Client selectedClient)
            {
                var clientEditWindow = new ClientEditWindow(_currentUser,selectedClient);
                clientEditWindow.Show();
                this.Close();
            }
            else
            {
                MessageBox.Show("Выберите клиента для редактирования или у вас нет прав для редактирования.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void DeleteClient_Click(object sender, RoutedEventArgs e)
        {
            if (_currentUser.Position == "Manager" && ClientsDataGrid.SelectedItem is Client selectedClient)
            {
                if (MessageBox.Show("Вы уверены, что хотите удалить клиента?", "Подтверждение",
                                    MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                {
                    try
                    {
                        using (var context = new CarSharingDB1Entities())
                        {
                            var clientToRemove = context.Client.FirstOrDefault(c => c.Client_Id == selectedClient.Client_Id);
                            if (clientToRemove != null)
                            {
                                context.Client.Remove(clientToRemove);
                                context.SaveChanges();
                                MessageBox.Show("Клиент удалён.", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                                LoadClients(); 
                            }
                            else
                            {
                                MessageBox.Show("Клиент не найден в базе данных.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Ошибка при удалении: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }
            else
            {
                MessageBox.Show("Выберите клиента для удаления или у вас нет прав для удаления.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void ReverseBtn_Click(object sender, RoutedEventArgs e)
        {
            if (_currentUser == null)
            {
                MessageBox.Show("Ошибка: текущий пользователь отсутствует.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            new StartWindow(_currentUser).Show();
            this.Close();
        }

    }
}
