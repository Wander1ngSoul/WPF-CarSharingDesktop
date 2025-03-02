using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace Lab5.Windows
{
    public partial class ClientsWindow : Window
    {
        public ClientsWindow()
        {
            InitializeComponent();
            LoadClients();
        }

        private void LoadClients()
        {
            using (var context = new CarSharingDBEntities2())
            {
                ClientsDataGrid.ItemsSource = context.Client.ToList();
            }
        }

        private void CreateClient_Click(object sender, RoutedEventArgs e)
        {
            var clientEditWindow = new ClientEditWindow();
            clientEditWindow.Show();
            LoadClients();
            this.Close();
        }

        private void EditClient_Click(object sender, RoutedEventArgs e)
        {
            if (ClientsDataGrid.SelectedItem is Client selectedClient)
            {
                var clientEditWindow = new ClientEditWindow(selectedClient);
                clientEditWindow.Show();
                LoadClients();
                this.Close();
            }
            else
            {
                MessageBox.Show("Выберите клиента для редактирования.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void DeleteClient_Click(object sender, RoutedEventArgs e)
        {
            if (ClientsDataGrid.SelectedItem is Client selectedClient)
            {
                if (MessageBox.Show("Вы уверены, что хотите удалить клиента?", "Подтверждение",
                                    MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                {
                    try
                    {
                        using (var context = new CarSharingDBEntities2())
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
                MessageBox.Show("Выберите клиента для удаления.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void ReverseBtn_Click(object sender, RoutedEventArgs e)
        {
            new StartWindow(null).Show();
            this.Close();
        }
    }
}
