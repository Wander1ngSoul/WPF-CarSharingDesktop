using System;
using System.IO;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using Microsoft.Win32;

namespace Lab5.Windows
{
    public partial class CarsEditWindow : Window
    {
        private Car _carToEdit;

        public CarsEditWindow(Car car = null)
        {
            InitializeComponent();
            InitializeBrandComboBox();

            if (car != null)
            {
                _carToEdit = car;
                BrandComboBox.SelectedItem = car.Brand;
                ModelComboBox.SelectedItem = car.Model;
                YearDatePicker.SelectedDate = new DateTime(car.Year, 1, 1);
                LicensePlateTextBox.Text = car.LicensePlate;
                RentalPriceTextBox.Text = car.RentalPrice.ToString();
                PhotoTextBox.Text = car.Photo;
                StatusComboBox.SelectedItem = car.Status;
            }
            else
            {
                _carToEdit = new Car();
            }
        }

        private void InitializeBrandComboBox()
        {
            BrandComboBox.Items.Add("Toyota");
            BrandComboBox.Items.Add("BMW");
            BrandComboBox.Items.Add("Mercedes");
            BrandComboBox.Items.Add("Audi");
            BrandComboBox.Items.Add("Ford");
        }

        private void SaveCar_Click(object sender, RoutedEventArgs e)
        {
            if (BrandComboBox.SelectedItem == null || ModelComboBox.SelectedItem == null ||
                YearDatePicker.SelectedDate == null || string.IsNullOrEmpty(LicensePlateTextBox.Text) ||
                string.IsNullOrEmpty(RentalPriceTextBox.Text) || string.IsNullOrEmpty(PhotoTextBox.Text))
            {
                MessageBox.Show("Пожалуйста, заполните все поля.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (!decimal.TryParse(RentalPriceTextBox.Text, out decimal rentalPrice))
            {
                MessageBox.Show("Цена аренды должна быть числом.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (!File.Exists(PhotoTextBox.Text))
            {
                MessageBox.Show("Выбранное фото не существует.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            // Валидация гос. номера РФ
            string licensePlatePattern = @"^[АВЕКМНОРСТУХ]\d{3}[АВЕКМНОРСТУХ]{2}\s\d{2,3}$";
            if (!Regex.IsMatch(LicensePlateTextBox.Text, licensePlatePattern, RegexOptions.IgnoreCase))
            {
                MessageBox.Show("Некорректный формат гос. номера. Пример: А123ВС 99", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var selectedStatus = ((ComboBoxItem)StatusComboBox.SelectedItem).Content.ToString();

            try
            {
                _carToEdit.Brand = BrandComboBox.SelectedItem.ToString();
                _carToEdit.Model = ModelComboBox.SelectedItem.ToString();
                _carToEdit.Year = YearDatePicker.SelectedDate.Value.Year;
                _carToEdit.LicensePlate = LicensePlateTextBox.Text;
                _carToEdit.RentalPrice = rentalPrice;
                _carToEdit.Photo = PhotoTextBox.Text;
                _carToEdit.Status = selectedStatus;

                using (var context = new CarSharingDBEntities2())
                {
                    if (_carToEdit.Car_Id == 0)
                    {
                        context.Car.Add(_carToEdit);
                    }
                    else
                    {
                        context.Entry(_carToEdit).State = System.Data.Entity.EntityState.Modified;
                    }
                    context.SaveChanges();
                }

                MessageBox.Show("Автомобиль успешно сохранен!", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                new CarsWindow().Show();
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при сохранении данных: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            new CarsWindow().Show();
            this.Close();
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
