using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.IO;
using System.Text.Json;
using System.Windows;
using System.Windows.Input;

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
        }

        private void LoadCitiesFromDatabase()
        {
            cities = new List<string>();
            using (SQLiteConnection connection = new SQLiteConnection($"Data Source={dbPath};Version=3;"))
            {
                connection.Open();
                string query = "SELECT CityName FROM Cities";
                using (SQLiteCommand command = new SQLiteCommand(query, connection))
                {
                    using (SQLiteDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            cities.Add(reader["CityName"].ToString());
                        }
                    }
                }
            }
        }

        private void LoadDiscountsFromDatabase()
        {
            discounts = new List<string>();
            using (SQLiteConnection connection = new SQLiteConnection($"Data Source={dbPath};Version=3;"))
            {
                connection.Open();
                string query = "SELECT Description FROM Discounts";
                using (SQLiteCommand command = new SQLiteCommand(query, connection))
                {
                    using (SQLiteDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            discounts.Add(reader["Description"].ToString());
                        }
                    }
                }
            }
        }

        private void LoadDistancesFromDatabase()
        {
            distances = new Dictionary<string, Dictionary<string, int>>();
            using (SQLiteConnection connection = new SQLiteConnection($"Data Source={dbPath};Version=3;"))
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

                using (SQLiteCommand command = new SQLiteCommand(query, connection))
                {
                    using (SQLiteDataReader reader = command.ExecuteReader())
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
            using (SQLiteConnection connection = new SQLiteConnection($"Data Source={dbPath};Version=3;"))
            {
                connection.Open();
                string query = "SELECT DiscountPercent FROM Discounts WHERE Description = @Description";
                using (SQLiteCommand command = new SQLiteCommand(query, connection))
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

        private void LoadButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (File.Exists("Travels.json"))
                {
                    string jsonString = File.ReadAllText("Travels.json");
                    List<Travel> travels = JsonSerializer.Deserialize<List<Travel>>(jsonString);

                    if (travels != null && travels.Count > 0)
                    {
                        Travel lastTravel = travels[travels.Count - 1];
                        Beginning.SelectedItem = lastTravel.Beginning;
                        Final.SelectedItem = lastTravel.Final;
                        _Calendar.SelectedDate = lastTravel.Date;
                        _Status.SelectedItem = lastTravel.Status;

                        MessageBox.Show("Last travel loaded successfully!");
                    }
                    else
                    {
                        MessageBox.Show("No saved travels found.");
                    }
                }
                else
                {
                    MessageBox.Show("No saved travels found.");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error while loading travels: {ex.Message}");
            }
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
            }
            Travel newTravel = new Travel
            {
                Beginning = startingCity,
                Final = finalCity,
                Date = selectedDate,
                Status = selectedDiscount
            };

            List<Travel> travels = new List<Travel>();

            if (File.Exists("Travels.json"))
            {
                try
                {
                    string jsonString = File.ReadAllText("Travels.json");
                    travels = JsonSerializer.Deserialize<List<Travel>>(jsonString);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error while loading existing travels: {ex.Message}");
                }
            }

            travels.Add(newTravel);

            try
            {
                string jsonString = JsonSerializer.Serialize(travels);
                File.WriteAllText("Travels.json", jsonString);
                MessageBox.Show("Travel saved successfully!");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error while saving travel: {ex.Message}");
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
