using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace Pociag
{
    public partial class Search : Window
    {
        private Dictionary<string, Dictionary<string, int>> distances;
        private List<string> cities;
        private List<string> discounts;

        private readonly string dbPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "database.db");

        public Search()
        {
            InitializeComponent();
            LoadCitiesFromDatabase();
            LoadDiscountsFromDatabase();
            LoadDistancesFromDatabase();

            Beginning.ItemsSource = cities;
            Final.ItemsSource = cities;
            _Status.ItemsSource = discounts;

            SetUserDetails();
        }

        private void SetUserDetails()
        {
            UsernameTextBlock.Text = UserSession.Username;
            if (!string.IsNullOrEmpty(UserSession.SelectedDiscount))
            {
                _Status.SelectedItem = UserSession.SelectedDiscount;
            }
        }

        private void LoadCitiesFromDatabase()
        {
            cities = new List<string>();
            int retryCount = 5;
            while (retryCount > 0)
            {
                try
                {
                    using (var connection = new SQLiteConnection($"Data Source={dbPath};Version=3;"))
                    {
                        connection.Open();
                        string query = "SELECT CityName FROM Cities";
                        using (var command = new SQLiteCommand(query, connection))
                        {
                            using (var reader = command.ExecuteReader())
                            {
                                while (reader.Read())
                                {
                                    cities.Add(reader["CityName"].ToString());
                                }
                            }
                        }
                    }
                    return;
                }
                catch (SQLiteException ex) when (ex.Message.Contains("database is locked"))
                {
                    retryCount--;
                    System.Threading.Thread.Sleep(200);
                }
            }
            MessageBox.Show("Unable to load cities after multiple attempts due to database being locked.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }

        private void LoadDiscountsFromDatabase()
        {
            discounts = new List<string>();
            int retryCount = 5;
            while (retryCount > 0)
            {
                try
                {
                    using (var connection = new SQLiteConnection($"Data Source={dbPath};Version=3;"))
                    {
                        connection.Open();
                        string query = "SELECT Description FROM Discounts";
                        using (var command = new SQLiteCommand(query, connection))
                        {
                            using (var reader = command.ExecuteReader())
                            {
                                while (reader.Read())
                                {
                                    discounts.Add(reader["Description"].ToString());
                                }
                            }
                        }
                    }
                    return;
                }
                catch (SQLiteException ex) when (ex.Message.Contains("database is locked"))
                {
                    retryCount--;
                    System.Threading.Thread.Sleep(200);
                }
            }
            MessageBox.Show("Unable to load discounts after multiple attempts due to database being locked.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }

        private void LoadDistancesFromDatabase()
        {
            distances = new Dictionary<string, Dictionary<string, int>>();
            int retryCount = 5;
            while (retryCount > 0)
            {
                try
                {
                    using (var connection = new SQLiteConnection($"Data Source={dbPath};Version=3;"))
                    {
                        connection.Open();
                        string query = @"
                        SELECT 
                            c1.CityName AS CityFrom, 
                            c2.CityName AS CityTo, 
                            d.Distance 
                        FROM 
                            Distances d
                            JOIN Cities c1 ON d.CityFromId = c1.Id
                            JOIN Cities c2 ON d.CityToId = c2.Id";

                        using (var command = new SQLiteCommand(query, connection))
                        {
                            using (var reader = command.ExecuteReader())
                            {
                                while (reader.Read())
                                {
                                    string cityFrom = reader["CityFrom"].ToString();
                                    string cityTo = reader["CityTo"].ToString();
                                    int distance = int.Parse(reader["Distance"].ToString());

                                    if (!distances.ContainsKey(cityFrom))
                                    {
                                        distances[cityFrom] = new Dictionary<string, int>();
                                    }

                                    distances[cityFrom][cityTo] = distance;
                                }
                            }
                        }
                    }
                    return;
                }
                catch (SQLiteException ex) when (ex.Message.Contains("database is locked"))
                {
                    retryCount--;
                    System.Threading.Thread.Sleep(200);
                }
            }
            MessageBox.Show("Unable to load distances after multiple attempts due to database being locked.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }

        private void CalculateButton_Click(object sender, RoutedEventArgs e)
        {
            string startingCity = (string)Beginning.SelectedItem;
            string finalCity = (string)Final.SelectedItem;
            string selectedDiscount = (string)_Status.SelectedItem;

            if (string.IsNullOrEmpty(startingCity) || string.IsNullOrEmpty(finalCity) || string.IsNullOrEmpty(selectedDiscount))
            {
                MessageBox.Show("Please select all fields");
                return;
            }

            if (startingCity == finalCity)
            {
                MessageBox.Show("Starting city and final city cannot be the same!");
                return;
            }

            if (!distances.ContainsKey(startingCity) || !distances[startingCity].ContainsKey(finalCity))
            {
                MessageBox.Show("There is no connection between selected cities!");
                return;
            }

            int distance = distances[startingCity][finalCity];
            double basePrice = distance * 0.5;
            double discountPercentage = GetDiscountPercentageFromDatabase(selectedDiscount);
            double finalPrice = basePrice * (1 - (discountPercentage / 100));

            MessageBox.Show($"Ticket price: {finalPrice:F2} PLN");
        }

        private double GetDiscountPercentageFromDatabase(string discountDescription)
        {
            int retryCount = 5;
            while (retryCount > 0)
            {
                try
                {
                    using (var connection = new SQLiteConnection($"Data Source={dbPath};Version=3;"))
                    {
                        connection.Open();
                        string query = "SELECT DiscountPercent FROM Discounts WHERE Description = @Description";
                        using (var command = new SQLiteCommand(query, connection))
                        {
                            command.Parameters.AddWithValue("@Description", discountDescription);
                            var result = command.ExecuteScalar();
                            if (result != null)
                            {
                                return Convert.ToDouble(result);
                            }
                        }
                    }
                    return 0;
                }
                catch (SQLiteException ex) when (ex.Message.Contains("database is locked"))
                {
                    retryCount--;
                    System.Threading.Thread.Sleep(200);
                }
            }
            MessageBox.Show("Unable to retrieve discount percentage after multiple attempts due to database being locked.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            return 0;
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            string startingCity = (string)Beginning.SelectedItem;
            string finalCity = (string)Final.SelectedItem;
            DateTime selectedDate = _Calendar.SelectedDate ?? DateTime.Now;
            string selectedDiscount = (string)_Status.SelectedItem;

            if (startingCity == finalCity)
            {
                MessageBox.Show("Starting city and final city cannot be the same!");
                return;
            }

            SaveTravelToDatabase(startingCity, finalCity, selectedDate, selectedDiscount);
            MessageBox.Show("Travel saved successfully!");
        }

        private void SaveTravelToDatabase(string startingCity, string finalCity, DateTime date, string discount)
        {
            int retryCount = 5;
            while (retryCount > 0)
            {
                try
                {
                    using (var connection = new SQLiteConnection($"Data Source={dbPath};Version=3;"))
                    {
                        connection.Open();
                        using (SQLiteTransaction transaction = connection.BeginTransaction())
                        {
                            int userId = UserSession.UserId;
                            int discountId = GetDiscountIdFromDatabase(discount);
                            int cityFromId = GetCityIdFromDatabase(startingCity);
                            int cityToId = GetCityIdFromDatabase(finalCity);
                            int distance = GetDistanceFromDatabase(cityFromId, cityToId);
                            double discountPercentage = GetDiscountPercentageFromDatabase(discount);
                            double ticketPrice = CalculateTicketPrice(distance, discountPercentage);

                            string query = "INSERT INTO Travels (UserId, CityFromId, CityToId, TravelDate, DiscountId, TicketPrice) VALUES (@UserId, @CityFromId, @CityToId, @TravelDate, @DiscountId, @TicketPrice)";
                            using (var command = new SQLiteCommand(query, connection, transaction))
                            {
                                command.Parameters.AddWithValue("@UserId", userId);
                                command.Parameters.AddWithValue("@CityFromId", cityFromId);
                                command.Parameters.AddWithValue("@CityToId", cityToId);
                                command.Parameters.AddWithValue("@TravelDate", date.ToString("yyyy-MM-dd"));
                                command.Parameters.AddWithValue("@DiscountId", discountId);
                                command.Parameters.AddWithValue("@TicketPrice", ticketPrice);

                                command.ExecuteNonQuery();
                            }
                            transaction.Commit();
                        }
                    }
                    return;
                }
                catch (SQLiteException ex) when (ex.Message.Contains("database is locked"))
                {
                    retryCount--;
                    System.Threading.Thread.Sleep(200);
                }
                catch (SQLiteException ex)
                {
                    MessageBox.Show($"An error occurred: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
            }
            MessageBox.Show("Operation failed after multiple attempts due to database being locked.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }

        private int GetDiscountIdFromDatabase(string discountDescription)
        {
            using (var connection = new SQLiteConnection($"Data Source={dbPath};Version=3;"))
            {
                connection.Open();
                string query = "SELECT Id FROM Discounts WHERE Description = @Description";
                using (var command = new SQLiteCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Description", discountDescription);
                    var result = command.ExecuteScalar();
                    if (result != null)
                    {
                        return Convert.ToInt32(result);
                    }
                }
            }
            return -1;
        }

        private int GetCityIdFromDatabase(string cityName)
        {
            using (var connection = new SQLiteConnection($"Data Source={dbPath};Version=3;"))
            {
                connection.Open();
                string query = "SELECT Id FROM Cities WHERE CityName = @CityName";
                using (var command = new SQLiteCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@CityName", cityName);
                    var result = command.ExecuteScalar();
                    if (result != null)
                    {
                        return Convert.ToInt32(result);
                    }
                }
            }
            return -1;
        }

        private int GetDistanceFromDatabase(int cityFromId, int cityToId)
        {
            using (var connection = new SQLiteConnection($"Data Source={dbPath};Version=3;"))
            {
                connection.Open();
                string query = "SELECT Distance FROM Distances WHERE CityFromId = @CityFromId AND CityToId = @CityToId";
                using (var command = new SQLiteCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@CityFromId", cityFromId);
                    command.Parameters.AddWithValue("@CityToId", cityToId);
                    var result = command.ExecuteScalar();
                    if (result != null)
                    {
                        return Convert.ToInt32(result);
                    }
                }
            }
            return 0; // Możesz zwrócić wartość domyślną lub rzucić wyjątek, jeśli nie ma odległości
        }

        private double CalculateTicketPrice(int distance, double discountPercentage)
        {
            double basePricePerKm = 0.5; // Ustalona cena bazowa za kilometr
            double basePrice = distance * basePricePerKm;
            double finalPrice = basePrice * (1 - discountPercentage / 100);
            return finalPrice;
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult result = MessageBox.Show("Are you sure you want to exit?", "Exit confirmation", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (result == MessageBoxResult.Yes)
            {
                Close();
            }
        }

        private void Window_OnMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                DragMove();
            }
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            MainWindow mainWindow = new MainWindow();
            mainWindow.Show();
            Close();
        }
    }
}
