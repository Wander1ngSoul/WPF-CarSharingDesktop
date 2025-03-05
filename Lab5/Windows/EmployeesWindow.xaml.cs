using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace Lab5.Windows
{
    public partial class EmployeesWindow : Window
    {
        private readonly CarSharingDB1Entities _context;
        private readonly Employee _currentUser;

        public EmployeesWindow(Employee currentUser)
        {
            InitializeComponent();
            _context = new CarSharingDB1Entities();
            _currentUser = currentUser;
            LoadEmployees();
        }

        public void LoadEmployees()
        {
            var employees = _context.Employee.ToList();
            EmployeesListBox.ItemsSource = employees;
        }

        private void EditEmployee_Click(object sender, RoutedEventArgs e)
        {
            if (_currentUser.Position != "Manager")
            {
                MessageBox.Show("У вас нет прав для редактирования сотрудников.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            var button = sender as Button;
            var employee = button?.DataContext as Employee;

            if (employee != null)
            {
                var employeeEditWindow = new EmployeeEditWindow(employee);  
                employeeEditWindow.Show();
                this.Close();
            }
        }

        private void DeleteEmployee_Click(object sender, RoutedEventArgs eventArgs)
        {
            if (_currentUser.Position != "Manager")
            {
                MessageBox.Show("У вас нет прав для удаления сотрудников.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            var button = sender as Button;
            var employee = button?.DataContext as Employee;

            if (employee != null)
            {
                var result = MessageBox.Show($"Вы уверены, что хотите удалить сотрудника {employee.FirstName} {employee.LastName}?", "Удаление", MessageBoxButton.YesNo);
                if (result == MessageBoxResult.Yes)
                {
                    try
                    {
                        var employeeToRemove = _context.Employee.FirstOrDefault(e => e.Employee_Id == employee.Employee_Id);
                        if (employeeToRemove != null)
                        {
                            _context.Employee.Remove(employeeToRemove);
                            _context.SaveChanges();
                            MessageBox.Show("Сотрудник удалён.", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                            LoadEmployees();
                        }
                        else
                        {
                            MessageBox.Show("Сотрудник не найден в базе данных.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Ошибка при удалении: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }
        }

        private void CreateEmployee_Click(object sender, RoutedEventArgs e)
        {
            if (_currentUser.Position != "Manager")
            {
                MessageBox.Show("У вас нет прав для создания сотрудников.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            var employeeEditWindow = new EmployeeEditWindow(); 
            employeeEditWindow.Show();
            this.Close();
        }

        private void ReverseBtn_Click(object sender, RoutedEventArgs e)
        {
            new StartWindow(_currentUser).Show();
            this.Close();
        }
    }
}
