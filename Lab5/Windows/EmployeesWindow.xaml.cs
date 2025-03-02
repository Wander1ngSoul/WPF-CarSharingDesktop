using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace Lab5.Windows
{
    public partial class EmployeesWindow : Window
    {
        private readonly CarSharingDBEntities2 _context;

        public EmployeesWindow()
        {
            InitializeComponent();
            _context = new CarSharingDBEntities2();
            LoadEmployees();
        }

        public void LoadEmployees()
        {
            var employees = _context.Employee.ToList();
            EmployeesListBox.ItemsSource = employees;
        }

        private void EditEmployee_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            var employee = button?.DataContext as Employee;

            if (employee != null)
            {
                var employeeEditWindow = new EmployeeEditWindow(employee);  // Окно редактирования сотрудника
                employeeEditWindow.Show();
                this.Close();
            }
        }

        private void DeleteEmployee_Click(object sender, RoutedEventArgs eventArgs)
        {
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
            var employeeEditWindow = new EmployeeEditWindow(); 
            employeeEditWindow.Show();
            this.Close();
        }

        private void ReverseBtn_Click(object sender, RoutedEventArgs e)
        {
            new StartWindow(null).Show();
            this.Close();
            

        }
    }
}
