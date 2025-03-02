using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using System.Windows.Media;
using System;

namespace Lab5.Windows
{
    public partial class BookingWindow : Window
    {
        private CarSharingDBEntities2 _context = new CarSharingDBEntities2();

        public BookingWindow()
        {
            InitializeComponent();
            LoadBookings();
        }

        private void LoadBookings()
        {
            BookingPanel.Children.Clear();

            var bookings = _context.Booking
                .Where(b => b.Client != null && b.Car != null)
                .ToList();

            foreach (var booking in bookings)
            {
                Border card = new Border
                {
                    Width = 250,
                    Height = 350,
                    Margin = new Thickness(10),
                    Padding = new Thickness(10),
                    VerticalAlignment = VerticalAlignment.Top,
                    Background = new LinearGradientBrush(
                        Color.FromRgb(150, 150, 150),
                        Color.FromRgb(100, 100, 100),
                        45)
                };

                card.CornerRadius = new CornerRadius(10);
                card.BorderBrush = Brushes.Transparent;
                card.BorderThickness = new Thickness(1);

                StackPanel stack = new StackPanel();

                Image carImage = new Image
                {
                    Width = 230,
                    Height = 150,
                    Source = new BitmapImage(new Uri(booking.Car.Photo, UriKind.RelativeOrAbsolute)),
                    Margin = new Thickness(0, 0, 0, 10),
                    Stretch = Stretch.Fill
                };

                stack.Children.Add(carImage);

                stack.Children.Add(new TextBlock
                {
                    Text = $"Клиент: {booking.Client.FirstName} {booking.Client.LastName}",
                    FontWeight = FontWeights.Bold
                });

                stack.Children.Add(new TextBlock
                {
                    Text = $"Авто: {booking.Car.Brand} {booking.Car.Model}",
                    Margin = new Thickness(0, 5, 0, 0)
                });

                stack.Children.Add(new TextBlock
                {
                    Text = $"С {booking.StartDate:dd.MM.yyyy} по {booking.EndDate:dd.MM.yyyy}",
                    Margin = new Thickness(0, 5, 0, 0)
                });

                stack.Children.Add(new TextBlock
                {
                    Text = $"Статус: {booking.Status}",
                    Foreground = booking.Status == "Active" ? Brushes.Green : Brushes.Red,
                    Margin = new Thickness(0, 5, 0, 0)
                });

                stack.Children.Add(new TextBlock
                {
                    Text = $"Цена: {booking.Price} ₽",
                    FontWeight = FontWeights.Bold,
                    Margin = new Thickness(0, 5, 0, 0)
                });

                StackPanel buttonsStack = new StackPanel
                {
                    Orientation = Orientation.Horizontal,
                    HorizontalAlignment = HorizontalAlignment.Center,
                    Margin = new Thickness(0, 10, 0, 0)
                };

                Button editButton = new Button
                {
                    Content = "Изменить",
                    Background = Brushes.Orange,
                    Foreground = Brushes.White,
                    Width = 100,
                    Height = 30,
                    Margin = new Thickness(5)
                };
                editButton.Click += (sender, e) => EditBooking(booking);

                Button deleteButton = new Button
                {
                    Content = "Удалить",
                    Background = Brushes.Red,
                    Foreground = Brushes.White,
                    Width = 100,
                    Height = 30,
                    Margin = new Thickness(5)
                };
                deleteButton.Click += (sender, e) => DeleteBooking(booking);

                buttonsStack.Children.Add(editButton);
                buttonsStack.Children.Add(deleteButton);

                stack.Children.Add(buttonsStack);
                card.Child = stack;
                BookingPanel.Children.Add(card);
            }
        }

        private void EditBooking(Booking booking)
        {
            var bookingEditWindow = new BookingEditWindow(booking);
            bookingEditWindow.BookingUpdated += (sender, e) => LoadBookings();  // Подписка на событие обновления
            bookingEditWindow.Show();
            this.Close();
        }

        private void AddBooking_Click(object sendering, RoutedEventArgs eventArgs)
        {
            var bookingEditWindow = new BookingEditWindow(null);
            bookingEditWindow.BookingUpdated += (sender, e) => LoadBookings();  // Подписка на событие обновления
            bookingEditWindow.Show();
            this.Close();
        }

        private void DeleteBooking(Booking booking)
        {
            var result = MessageBox.Show("Вы уверены, что хотите удалить это бронирование?", "Удалить", MessageBoxButton.YesNo);
            if (result == MessageBoxResult.Yes)
            {
                _context.Booking.Remove(booking);
                _context.SaveChanges();
                LoadBookings(); 
            }
        }

        private void Back_Click(object sender, RoutedEventArgs e)
        {
            new StartWindow(null).Show();
            this.Close();
        }
    }
}
