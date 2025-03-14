﻿using System;
using System.Data.Entity;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace Lab5.Windows
{
    public partial class BookingEditWindow : Window
    {
        private Booking _booking;
        private Employee employee;
        private CarSharingDBEntities _context = new CarSharingDBEntities();

        public BookingEditWindow(Employee _employee, Booking booking)
        {
            InitializeComponent();
            employee = _employee;
            _booking = booking;

            var clients = _context.Client
                                  .ToList()
                                  .Select(c => new
                                  {
                                      c.Client_Id,
                                      FullName = c.LastName + " " + c.FirstName
                                  })
                                  .ToList();

            ClientComboBox.ItemsSource = clients;
            ClientComboBox.DisplayMemberPath = "FullName";
            ClientComboBox.SelectedValuePath = "Client_Id";

            var cars = _context.Car
                               .ToList()
                               .Select(c => new
                               {
                                   c.Car_Id,
                                   CarDetails = c.Brand + " " + c.Model
                               })
                               .ToList();

            CarComboBox.ItemsSource = cars;
            CarComboBox.DisplayMemberPath = "CarDetails";
            CarComboBox.SelectedValuePath = "Car_Id";

            if (_booking != null)
            {
                CarComboBox.SelectedValue = _booking.CarID;
                StartDatePicker.SelectedDate = _booking.StartDate;
                EndDatePicker.SelectedDate = _booking.EndDate;
                StatusComboBox.SelectedItem = _booking.Status;
                PriceTextBox.Text = _booking.Price.ToString("F2");
                ClientComboBox.SelectedValue = _booking.ClientID;
            }
            else
            {
                StatusComboBox.SelectedItem = "confirmed";
            }

            StatusComboBox.ItemsSource = null;
            StatusComboBox.ItemsSource = new[] { "confirmed", "canceled" };

            CarComboBox.SelectionChanged += CarComboBox_SelectionChanged;
        }

        public event EventHandler BookingUpdated;

        private void OnBookingUpdated()
        {
            BookingUpdated?.Invoke(this, EventArgs.Empty);
        }

        private void CarComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (_booking != null && CarComboBox.SelectedValue != null)
            {
                _booking.CarID = (long)CarComboBox.SelectedValue;
            }
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            if (_booking == null)
            {
                _booking = new Booking();
                _context.Booking.Add(_booking);
            }
            else
            {
                _booking = _context.Booking
                                    .FirstOrDefault(b => b.Booking_Id == _booking.Booking_Id);
            }


            if (StartDatePicker.SelectedDate == null || EndDatePicker.SelectedDate == null)
            {
                MessageBox.Show("Выберите даты бронирования.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (StartDatePicker.SelectedDate > EndDatePicker.SelectedDate)
            {
                MessageBox.Show("Дата окончания не может быть раньше даты начала.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            _booking.StartDate = StartDatePicker.SelectedDate.Value;
            _booking.EndDate = EndDatePicker.SelectedDate.Value;
            _booking.Status = StatusComboBox.SelectedItem?.ToString();

            if (decimal.TryParse(PriceTextBox.Text, out decimal price))
            {
                _booking.Price = price;
            }
            else
            {
                MessageBox.Show("Введите корректную цену.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (ClientComboBox.SelectedValue == null)
            {
                MessageBox.Show("Не выбран клиент для бронирования.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            _booking.ClientID = (long)ClientComboBox.SelectedValue;

            if (CarComboBox.SelectedValue == null)
            {
                MessageBox.Show("Не выбран автомобиль для бронирования.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            _booking.CarID = (long)CarComboBox.SelectedValue;

            try
            { 
                if (_booking.Booking_Id == 0)
                {                  
                    _context.Booking.Add(_booking);
                }
                else
                {
                    _context.Entry(_booking).State = EntityState.Modified;
                }
                _context.SaveChanges();
                MessageBox.Show("Бронирование успешно сохранено.", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                OnBookingUpdated();

                new BookingWindow(employee).Show();
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при сохранении: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            new BookingWindow(employee).Show();
            this.Close();
        }
    }
}
