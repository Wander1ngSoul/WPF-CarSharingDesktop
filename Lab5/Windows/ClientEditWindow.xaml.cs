using System;
using System.Data.Entity.Migrations.Infrastructure;
using System.Data.Entity.Validation;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;

namespace Lab5.Windows
{
    public partial class ClientEditWindow : Window
    {
        private Client _clientToEdit;
        private Employee _employee;


        public ClientEditWindow(Employee employee,Client client = null)
        {
            InitializeComponent();
            _employee = employee;

            if (client != null)
            {
                _clientToEdit = client;
                FirstNameTextBox.Text = client.FirstName;
                LastNameTextBox.Text = client.LastName;
                MiddleNameTextBox.Text = client.MiddleName;
                EmailTextBox.Text = client.Email;
                PhoneTextBox.Text = client.Phone;

                PasswordPanel.Visibility = Visibility.Collapsed;
            }
            else
            {
                _clientToEdit = new Client();
            }
        }

        private void SaveClient_Click(object sender, RoutedEventArgs e)
        {
            string firstName = FirstNameTextBox.Text.Trim();
            string lastName = LastNameTextBox.Text.Trim();
            string middleName = MiddleNameTextBox.Text.Trim();
            string email = EmailTextBox.Text.Trim();
            string phone = PhoneTextBox.Text.Trim();
            string password = "DefaultPassword123";

            if (!ValidateFields(firstName, lastName, middleName, email, phone, password))
                return;

            try
            {
                _clientToEdit.FirstName = firstName;
                _clientToEdit.LastName = lastName;
                _clientToEdit.MiddleName = middleName;
                _clientToEdit.Email = email;
                _clientToEdit.Phone = phone;
                _clientToEdit.Password = password;

                using (var context = new CarSharingDBEntities())
                {
                    if (_clientToEdit.Client_Id == 0)
                    {
                        if (context.Client.Any(c => c.Email == email))
                        {
                            MessageBox.Show("Клиент с таким Email уже существует.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                            return;
                        }

                        if (context.Client.Any(c => c.Phone == phone))
                        {
                            MessageBox.Show("Клиент с таким номером телефона уже существует.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                            return;
                        }

                        context.Client.Add(_clientToEdit);
                    }
                    else
                    {
                        var existingClient = context.Client.FirstOrDefault(c => c.Client_Id == _clientToEdit.Client_Id);
                        if (existingClient != null)
                        {
                            if (existingClient.Email != email && context.Client.Any(c => c.Email == email))
                            {
                                MessageBox.Show("Клиент с таким Email уже существует.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                                return;
                            }

                            if (existingClient.Phone != phone && context.Client.Any(c => c.Phone == phone))
                            {
                                MessageBox.Show("Клиент с таким номером телефона уже существует.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                                return;
                            }

                            context.Entry(existingClient).CurrentValues.SetValues(_clientToEdit);
                        }
                    }

                    context.SaveChanges();
                }

                MessageBox.Show("Клиент успешно сохранен!", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                new ClientsWindow(_employee).Show();
                this.Close();
            }
            catch (DbEntityValidationException ex)
            {
                string errorMessage = "Ошибка валидации:\n";
                foreach (var validationErrors in ex.EntityValidationErrors)
                {
                    foreach (var validationError in validationErrors.ValidationErrors)
                    {
                        errorMessage += $"- {validationError.PropertyName}: {validationError.ErrorMessage}\n";
                    }
                }
                MessageBox.Show(errorMessage, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            
        }

        private bool ValidateFields(string firstName, string lastName, string middleName, string email, string phone, string password)
        {
            if (string.IsNullOrEmpty(firstName) || string.IsNullOrEmpty(lastName) ||
                string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
            {
                MessageBox.Show("Заполните все обязательные поля.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }

            if (!Regex.IsMatch(firstName, @"^[А-Яа-яA-Za-z]{2,50}$") ||
                !Regex.IsMatch(lastName, @"^[А-Яа-яA-Za-z]{2,50}$")
                )
            {
                MessageBox.Show("Имя, фамилия должны содержать только буквы и быть от 2 до 50 символов.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }

            if (middleName == null) { middleName = "Неизвестно"; }

            if (!Regex.IsMatch(email, @"^[^@\s]+@[^@\s]+\.[^@\s]+$"))
            {
                MessageBox.Show("Введите корректный Email.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }

            if (!string.IsNullOrEmpty(phone) && !Regex.IsMatch(phone, @"^\+?\d{11,11}$"))
            {
                MessageBox.Show("Телефон должен содержать 11 цифр и может начинаться с '+'.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }

            if (password.Length < 8)
            {
                MessageBox.Show("Пароль должен содержать минимум 8 символов.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }

            return true;
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            new ClientsWindow(_employee).Show();
            this.Close();
        }
    }
}
