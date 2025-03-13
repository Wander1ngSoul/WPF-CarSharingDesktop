using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace Lab5.Windows
{
    public partial class EmployeesWindow : Window
    {
        private Employee _currentUser;
        private Employee _mainemployee;

        public EmployeesWindow(Employee mainUser,Employee currentUser)
        {
            InitializeComponent();
            _mainemployee = mainUser;
            _currentUser = currentUser;
            LoadEmployees();
        }

        private void LoadEmployees()
        {
            using (var context = new CarSharingDBEntities())
            {
                EmployeesListBox.ItemsSource = context.Employee.ToList();
            }
        }

        private void CreateEmployee_Click(object sender, RoutedEventArgs e)
        {
            if (_mainemployee.Position == "Manager")
            {
                var createEmployeeWindow = new EmployeeEditWindow(null, _mainemployee);
                createEmployeeWindow.Show();
                this.Close();
            }
            else {MessageBox.Show("У вас нет прав.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning); }
        }

        private void EditEmployee_Click(object sender, RoutedEventArgs e)
        {
            if (_mainemployee.Position == "Manager")
            {
                if (EmployeesListBox.SelectedItem is Employee selectedEmployee)
                {
                    var editEmployeeWindow = new EmployeeEditWindow(selectedEmployee, _mainemployee);
                    editEmployeeWindow.Show();
                    this.Close();
                }
                else
                {
                    MessageBox.Show("Выберите сотрудника для редактирования.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
            }
            else
            {
                MessageBox.Show("У вас нет прав для редактирования.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }



        private void DeleteEmployee_Click(object sender, RoutedEventArgs e)
        {
            if (_mainemployee.Position == "Manager" && EmployeesListBox.SelectedItem is Employee selectedEmployee)
            {
                if (MessageBox.Show("Вы уверены, что хотите удалить сотрудника?", "Подтверждение",
                                    MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                {
                    try
                    {
                        using (var context = new CarSharingDBEntities())
                        {
                            var employeeToRemove = context.Employee.FirstOrDefault(c => c.Employee_Id == selectedEmployee.Employee_Id);
                            if (employeeToRemove != null)
                            {
                                context.Employee.Remove(employeeToRemove);
                                context.SaveChanges();
                                MessageBox.Show("Сотрудник удалён.", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                                LoadEmployees();
                            }
                            else
                            {
                                MessageBox.Show("Сотрудник не найден в базе данных.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Ошибка при удалении: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }
            else { MessageBox.Show("Выберите сотрудника для удаления или у вас нет прав.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning); }
        }


        private void ReverseBtn_Click(object sender, RoutedEventArgs e)
        {
            new StartWindow(_mainemployee).Show();
            this.Close();
        }
    }
}
