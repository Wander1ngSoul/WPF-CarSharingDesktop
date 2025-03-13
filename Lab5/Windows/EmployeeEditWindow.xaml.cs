using System;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using Lab5;

namespace Lab5.Windows
{
    public partial class EmployeeEditWindow : Window
    {
        private Employee _employeeToEdit;
        private Employee _employeeToMain;

        public EmployeeEditWindow(Employee employeeEdit, Employee employeeMain)
        {
            InitializeComponent();
            _employeeToMain = employeeMain;

            if (employeeEdit != null)
            {
                _employeeToEdit = employeeEdit;
                FirstNameTextBox.Text = employeeEdit.FirstName;
                LastNameTextBox.Text = employeeEdit.LastName;
                PositionComboBox.SelectedItem = PositionComboBox.Items.Cast<ComboBoxItem>().FirstOrDefault(x => x.Content.ToString() == employeeEdit.Position);
                EmailTextBox.Text = employeeEdit.Email;
                PhoneTextBox.Text = employeeEdit.Phone;
                PasswordTextBox.Password = employeeEdit.Password;
            }
            else
            {
                _employeeToEdit = new Employee();
            }
        }

        private bool IsValidName(string name)
        {
            var nameRegex = new Regex(@"^[A-Za-zА-Яа-яЁё]{2,}$");
            return nameRegex.IsMatch(name);
        }

        private void SaveButton_Click(object sender, RoutedEventArgs eventArgs)
        {
            if (string.IsNullOrEmpty(FirstNameTextBox.Text) || string.IsNullOrEmpty(LastNameTextBox.Text) ||
                string.IsNullOrEmpty(EmailTextBox.Text) || string.IsNullOrEmpty(PhoneTextBox.Text) ||
                string.IsNullOrEmpty(PasswordTextBox.Password))
            {
                MessageBox.Show("Пожалуйста, заполните все поля.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (!IsValidName(FirstNameTextBox.Text) || !IsValidName(LastNameTextBox.Text))
            {
                MessageBox.Show("Имя и фамилия должны содержать только буквы и быть длиной не менее 2 символов.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (!IsValidEmail(EmailTextBox.Text))
            {
                MessageBox.Show("Пожалуйста, введите корректный Email.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (!IsValidPhone(PhoneTextBox.Text))
            {
                MessageBox.Show("Пожалуйста, введите корректный номер телефона.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (PasswordTextBox.Password.Length < 8)
            {
                MessageBox.Show("Пароль должен быть не менее 8 символов.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (PositionComboBox.SelectedItem == null)
            {
                MessageBox.Show("Пожалуйста, выберите должность сотрудника.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            try
            {
                using (var context = new CarSharingDBEntities())
                {
                  
                    var existingEmployee = context.Employee.FirstOrDefault(e => e.Phone == PhoneTextBox.Text && e.Employee_Id != _employeeToEdit.Employee_Id);
                    if (existingEmployee != null)
                    {
                        MessageBox.Show("Этот номер телефона уже используется другим сотрудником.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }

                    Employee employee;

                    if (_employeeToEdit.Employee_Id != 0)
                    {
                        employee = context.Employee.Find(_employeeToEdit.Employee_Id);
                        if (employee == null)
                        {
                            MessageBox.Show("Ошибка: сотрудник не найден в базе данных.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                            return;
                        }
                    }
                    else
                    {
                        employee = new Employee();
                        context.Employee.Add(employee);
                    }

                    employee.FirstName = FirstNameTextBox.Text;
                    employee.LastName = LastNameTextBox.Text;
                    employee.Position = ((ComboBoxItem)PositionComboBox.SelectedItem).Content.ToString();
                    employee.Email = EmailTextBox.Text;
                    employee.Phone = PhoneTextBox.Text;
                    employee.Password = PasswordTextBox.Password;

                    context.SaveChanges();
                }

                MessageBox.Show("Сотрудник успешно сохранен!", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                new EmployeesWindow(_employeeToMain, _employeeToEdit).Show();
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при сохранении данных: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void CancelButton_Click(object sender, RoutedEventArgs eventArgs)
        {
            new EmployeesWindow(_employeeToMain,_employeeToEdit).Show();
            this.Close();
        }

        private void DeleteButton_Click(object sender, RoutedEventArgs eventArgs)
        {
            var result = MessageBox.Show("Вы уверены, что хотите удалить этого сотрудника?", "Подтверждение", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (result == MessageBoxResult.Yes)
            {
                try
                {
                    using (var context = new CarSharingDBEntities())
                    {
                        var employeeToDelete = context.Employee.FirstOrDefault(e => e.Employee_Id == _employeeToEdit.Employee_Id);
                        if (employeeToDelete != null)
                        {
                            context.Employee.Remove(employeeToDelete);
                            context.SaveChanges();
                            MessageBox.Show("Сотрудник успешно удален.", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                            this.Close();
                        }
                        else
                        {
                            MessageBox.Show("Сотрудник не найден в базе данных.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка при удалении сотрудника: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private bool IsValidEmail(string email)
        {
            var emailRegex = new Regex(@"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$");
            return emailRegex.IsMatch(email);
        }

        private bool IsValidPhone(string phone)
        {
            var phoneRegex = new Regex(@"^(\+7|8)[0-9]{10}$");
            return phoneRegex.IsMatch(phone);
        }
    }
}
