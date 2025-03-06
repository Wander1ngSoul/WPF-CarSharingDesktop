using System;
using System.IO;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using Microsoft.Win32;
using System.Data.Entity.Validation;

namespace Lab5.Windows
{
    public partial class CarsEditWindow : Window
    {
        private Car _carToEdit;
        private Employee _employee;

        public CarsEditWindow(Employee employee, Car car = null)
        {
            InitializeComponent();
            InitializeBrandComboBox();
            InitializeStatusComboBox();

            _carToEdit = car ?? new Car();
            if (car != null)
            {
                PopulateFields(car);
            }
            _employee = employee;
        }

        private void InitializeBrandComboBox()
        {
            BrandComboBox.Items.Add("Toyota");
            BrandComboBox.Items.Add("BMW");
            BrandComboBox.Items.Add("Mercedes");
            BrandComboBox.Items.Add("Audi");
            BrandComboBox.Items.Add("Ford");
        }

        private void InitializeStatusComboBox()
        {
            StatusComboBox.Items.Add("available");
            StatusComboBox.Items.Add("reserved");
            StatusComboBox.Items.Add("in repair");
        }

        private void PopulateFields(Car car)
        {
            BrandComboBox.SelectedItem = car.Brand;
            ModelComboBox.SelectedItem = car.Model;
            YearDatePicker.SelectedDate = new DateTime(car.Year, 1, 1);
            LicensePlateTextBox.Text = car.LicensePlate;
            RentalPriceTextBox.Text = car.RentalPrice.ToString("F2");
            PhotoTextBox.Text = car.Photo;
            StatusComboBox.SelectedItem = car.Status;
        }

        private void SaveCar_Click(object sender, RoutedEventArgs e)
        {
            if (!ValidateInputs())
            {
                return;
            }

            try
            {
                _carToEdit.Brand = BrandComboBox.SelectedItem.ToString();
                _carToEdit.Model = ModelComboBox.SelectedItem.ToString();
                _carToEdit.Year = YearDatePicker.SelectedDate.Value.Year;
                _carToEdit.LicensePlate = LicensePlateTextBox.Text;
                _carToEdit.RentalPrice = decimal.Parse(RentalPriceTextBox.Text);
                _carToEdit.Photo = PhotoTextBox.Text;
                _carToEdit.Status = StatusComboBox.SelectedItem.ToString();

                
                using (var context = new CarSharingDB1Entities())
                {
                    if (_carToEdit.Car_Id == 0)
                    {
                        context.Car.Add(_carToEdit); 
                    }
                    else
                    {
                        context.Entry(_carToEdit).State = System.Data.Entity.EntityState.Modified; // Редактирование существующего
                    }

                    context.SaveChanges();
                }

                ShowSuccessMessage();
                NavigateToCarsWindow();
            }
            catch (DbEntityValidationException dbEx)
            {
                HandleValidationErrors(dbEx);
            }
            catch (Exception ex)
            {
                ShowErrorMessage($"Ошибка при сохранении данных: {ex.Message}");
            }
        }

        private bool ValidateInputs()
        {
            if (BrandComboBox.SelectedItem == null || ModelComboBox.SelectedItem == null ||
                YearDatePicker.SelectedDate == null || string.IsNullOrEmpty(LicensePlateTextBox.Text) ||
                string.IsNullOrEmpty(RentalPriceTextBox.Text) || string.IsNullOrEmpty(PhotoTextBox.Text))
            {
                ShowErrorMessage("Пожалуйста, заполните все поля.");
                return false;
            }

            if (!decimal.TryParse(RentalPriceTextBox.Text, out decimal rentalPrice))
            {
                ShowErrorMessage("Цена аренды должна быть числом.");
                return false;
            }

            if (!File.Exists(PhotoTextBox.Text))
            {
                ShowErrorMessage("Выбранное фото не существует.");
                return false;
            }

            string licensePlatePattern = @"^[АВЕКМНОРСТУХ]\d{3}[АВЕКМНОРСТУХ]{2}\s\d{2,3}$";
            if (!Regex.IsMatch(LicensePlateTextBox.Text, licensePlatePattern, RegexOptions.IgnoreCase))
            {
                ShowErrorMessage("Некорректный формат гос. номера. Пример: А123ВС 99");
                return false;
            }

            if (StatusComboBox.SelectedItem == null)
            {
                ShowErrorMessage("Выберите статус автомобиля.");
                return false;
            }

            var selectedStatus = StatusComboBox.SelectedItem.ToString();
            if (selectedStatus.Length > 200)
            {
                ShowErrorMessage("Статус не должен превышать 200 символов.");
                return false;
            }

            return true;
        }

        private void ShowSuccessMessage()
        {
            MessageBox.Show("Автомобиль успешно сохранен!", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void NavigateToCarsWindow()
        {
            new CarsWindow(_employee).Show();
            this.Close();
        }

        private void HandleValidationErrors(DbEntityValidationException dbEx)
        {
            foreach (var validationErrors in dbEx.EntityValidationErrors)
            {
                foreach (var validationError in validationErrors.ValidationErrors)
                {
                    ShowErrorMessage($"Свойство: {validationError.PropertyName} - Ошибка: {validationError.ErrorMessage}");
                }
            }
        }

        private void ShowErrorMessage(string message)
        {
            MessageBox.Show(message, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            NavigateToCarsWindow();
        }

        private void BrowsePhoto_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                Filter = "Image Files (*.jpg;*.jpeg;*.png)|*.jpg;*.jpeg;*.png"
            };

            if (openFileDialog.ShowDialog() == true)
            {
                PhotoTextBox.Text = openFileDialog.FileName;
            }
        }

        private void BrandComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ModelComboBox.Items.Clear();

            switch (BrandComboBox.SelectedItem.ToString())
            {
                case "Toyota":
                    ModelComboBox.Items.Add("Corolla");
                    ModelComboBox.Items.Add("Camry");
                    break;
                case "BMW":
                    ModelComboBox.Items.Add("X5");
                    ModelComboBox.Items.Add("M3");
                    break;
                case "Mercedes":
                    ModelComboBox.Items.Add("C-Class");
                    ModelComboBox.Items.Add("E-Class");
                    break;
                case "Audi":
                    ModelComboBox.Items.Add("A4");
                    ModelComboBox.Items.Add("Q7");
                    break;
                case "Ford":
                    ModelComboBox.Items.Add("Focus");
                    ModelComboBox.Items.Add("Mustang");
                    break;
            }
        }
    }
}
