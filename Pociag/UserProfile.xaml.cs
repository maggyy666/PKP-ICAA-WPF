using System;
using System.Data.SQLite;
using System.IO;
using System.Windows;

namespace Pociag
{
    public partial class UserProfile : Window
    {
        private readonly string dbPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "database.db");

        public UserProfile()
        {
            InitializeComponent();
            LoadUserData();
            UpdateUsernameDisplay();
        }

        private void LoadUserData()
        {
            NicknameTextBlock.Text = UserSession.Username;
            EmailTextBlock.Text = UserSession.Email;
            PasswordTextBlock.Text = new string('*', UserSession.Password.Length);
            DiscountTextBlock.Text = UserSession.SelectedDiscount;
            LoadRecentTrips();
        }

        private void LoadRecentTrips()
        {
            RecentTripsListBox.Items.Clear();

            using (var connection = new SQLiteConnection($"Data Source={dbPath};Version=3;"))
            {
                connection.Open();
                string query = @"
                    SELECT 
                        Travels.TravelDate,
                        CitiesFrom.CityName AS CityFromName,
                        CitiesTo.CityName AS CityToName,
                        Discounts.Description AS DiscountDescription,
                        Travels.TicketPrice
                    FROM Travels
                    JOIN Cities AS CitiesFrom ON Travels.CityFromId = CitiesFrom.Id
                    JOIN Cities AS CitiesTo ON Travels.CityToId = CitiesTo.Id
                    JOIN Discounts ON Travels.DiscountId = Discounts.Id
                    WHERE Travels.UserId = @UserId";

                using (var command = new SQLiteCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@UserId", UserSession.UserId);
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            string travelDate = Convert.ToDateTime(reader["TravelDate"]).ToString("yyyy-MM-dd");
                            string cityFromName = reader["CityFromName"].ToString();
                            string cityToName = reader["CityToName"].ToString();
                            string discountDescription = reader["DiscountDescription"].ToString();
                            double ticketPrice = Convert.ToDouble(reader["TicketPrice"]);

                            string travelDetails = $"{cityFromName} - {cityToName} - {travelDate} - {discountDescription} - {ticketPrice:F2} PLN";
                            RecentTripsListBox.Items.Add(travelDetails);
                        }
                    }
                }
            }
        }

        private void TogglePasswordButton_Click(object sender, RoutedEventArgs e)
        {
            PasswordTextBlock.Text = PasswordTextBlock.Text == "********" ? UserSession.Password : "********";
            TogglePasswordButton.Content = TogglePasswordButton.Content.ToString() == "Show" ? "Hide" : "Show";
        }

        private void EditProfileButton_Click(object sender, RoutedEventArgs e)
        {
            EditUser editUserWindow = new EditUser();
            editUserWindow.Show();
            this.Close();
        }

        private void BackToMainButton_Click(object sender, RoutedEventArgs e)
        {
            MainWindow mainWindow = new MainWindow();
            mainWindow.Show();
            this.Close();
        }

        private void HomeButton_Click(object sender, RoutedEventArgs e)
        {
            MainWindow mainWindow = new MainWindow();
            mainWindow.Show();
            this.Close();
        }

        private void LogoutButton_Click(object sender, RoutedEventArgs e)
        {
            UserSession.Logout();
            MainWindow mainWindow = new MainWindow();
            mainWindow.Show();
            this.Close();
        }

        private void Window_OnMouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (e.LeftButton == System.Windows.Input.MouseButtonState.Pressed)
            {
                DragMove();
            }
        }

        private void UpdateUsernameDisplay()
        {
            UsernameTextBlock.Text = UserSession.Username;
        }
    }
}
