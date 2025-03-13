using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Lab5.Windows
{
    public partial class CarsWindow : Window
    {
        private Employee _currentUser;
        public CarsWindow()
        {
            InitializeComponent();
            if (AuthService.CurrentUserRole != "Manager")
            {
                AddCarButton.Visibility = Visibility.Collapsed;
            }
            LoadCars();
        }
        public CarsWindow(Employee currentUser) 
        {
            InitializeComponent();
            _currentUser = currentUser;

            if (AuthService.CurrentUserRole != "Manager")
            {
                AddCarButton.Visibility = Visibility.Collapsed;
            }
            LoadCars();
        }



        private void LoadCars()
        {
            CarsPanel.Children.Clear();
            using (var context = new CarSharingDBEntities())
            {
                var cars = context.Car.ToList();
                foreach (var car in cars)
                {
                    CarsPanel.Children.Add(CreateCarCard(car));
                }
            }
        }

        public static class AuthService
        {
           
            public static string CurrentUserRole { get; set; }

            public static void SetCurrentUser(string role)
            {
                CurrentUserRole = role;
            }
        }

        private UIElement CreateCarCard(Car car)
        {
            Border cardBorder = new Border
            {
                Background = Brushes.White,
                CornerRadius = new CornerRadius(10),
                BorderThickness = new Thickness(1),
                BorderBrush = Brushes.Gray,
                Width = 250,
                Height = 300,
                Margin = new Thickness(15),
                Effect = new System.Windows.Media.Effects.DropShadowEffect
                {
                    Color = Colors.Black,
                    Opacity = 0.2,
                    ShadowDepth = 4
                }
            };

            Grid cardGrid = new Grid();

            ImageBrush backgroundImage = new ImageBrush { Stretch = Stretch.UniformToFill };
            try
            {
                backgroundImage.ImageSource = new BitmapImage(new Uri(car.Photo, UriKind.RelativeOrAbsolute));
            }
            catch
            {
                backgroundImage.ImageSource = new BitmapImage(new Uri("pack://application:,,,/Resources/no-image.png"));
            }
            cardBorder.Background = backgroundImage;

            Border overlay = new Border
            {
                Background = new SolidColorBrush(Color.FromArgb(100, 0, 0, 0)),
                CornerRadius = new CornerRadius(10)
            };
            Grid.SetZIndex(overlay, 1);
            cardGrid.Children.Add(overlay);

            StackPanel textPanel = new StackPanel
            {
                VerticalAlignment = VerticalAlignment.Bottom,
                Margin = new Thickness(10)
            };

            TextBlock carTitle = new TextBlock
            {
                Text = $"{car.Brand} {car.Model}",
                FontWeight = FontWeights.Bold,
                Foreground = Brushes.White,
                FontSize = 16,
                TextAlignment = TextAlignment.Center
            };

            TextBlock carDetails = new TextBlock
            {
                Text = $"Год: {car.Year}\nЦена: {car.RentalPrice}₽/день",
                Foreground = Brushes.LightGray,
                FontSize = 14,
                TextAlignment = TextAlignment.Center
            };

            textPanel.Children.Add(carTitle);
            textPanel.Children.Add(carDetails);

            StackPanel buttonPanel = new StackPanel
            {
                Orientation = Orientation.Horizontal,
                HorizontalAlignment = HorizontalAlignment.Center,
                Margin = new Thickness(5)
            };

            if (AuthService.CurrentUserRole == "Manager")
            {
                Button editButton = new Button
                {
                    Content = "Изменить",
                    Width = 90,
                    Background = Brushes.Orange,
                    Foreground = Brushes.White,
                    Margin = new Thickness(5)
                };
                editButton.Click += (s, e) => EditCar(car);

                Button deleteButton = new Button
                {
                    Content = "Удалить",
                    Width = 90,
                    Background = Brushes.Red,
                    Foreground = Brushes.White,
                    Margin = new Thickness(5)
                };
                deleteButton.Click += (s, e) => DeleteCar(car);

                buttonPanel.Children.Add(editButton);
                buttonPanel.Children.Add(deleteButton);
            }


            textPanel.Children.Add(buttonPanel);

            Grid.SetZIndex(textPanel, 2);
            cardGrid.Children.Add(textPanel);

            cardBorder.Child = cardGrid;
            return cardBorder;
        }


        private void EditCar(Car car)
        {
            var carEditWindow = new CarsEditWindow(_currentUser,car);
            carEditWindow.Show();
            this.Close();
        }

        private void DeleteCar(Car car)
        {
            if (MessageBox.Show($"Удалить {car.Brand} {car.Model}?", "Удаление", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes)
            {
                using (var context = new CarSharingDBEntities())
                {
                    var carToDelete = context.Car.FirstOrDefault(c => c.Car_Id == car.Car_Id);
                    if (carToDelete != null)
                    {
                        context.Car.Remove(carToDelete);
                        context.SaveChanges();
                        LoadCars();
                    }
                }
            }
        }

        private void AddCar_Click(object sender, RoutedEventArgs e)
        {
            new CarsEditWindow(_currentUser, null).Show();
            this.Close();
        }

        private void Back_Click(object sender, RoutedEventArgs e)
        {
            new StartWindow(_currentUser).Show();
            this.Close();
        }
    }
}
