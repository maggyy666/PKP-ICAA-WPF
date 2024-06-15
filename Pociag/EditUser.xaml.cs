using System;
using System.Data.SQLite;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace Pociag
{
    public partial class EditUser : Window
    {
        private readonly string _dbPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "database.db");

        public EditUser()
        {
            InitializeComponent();
            LoadUserData();
            LoadDiscounts();
        }

        private void LoadUserData()
        {
            UsernameTextBox.Text = UserSession.Username;
            EmailTextBox.Text = UserSession.Email;
            PasswordBox.Password = UserSession.Password;
            DiscountComboBox.SelectedItem = UserSession.SelectedDiscount;
        }

        private void LoadDiscounts()
        {
            using (var connection = new SQLiteConnection($"Data Source={_dbPath};Version=3;"))
            {
                connection.Open();
                string query = "SELECT Description FROM Discounts";
                using (var command = new SQLiteCommand(query, connection))
                {
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            DiscountComboBox.Items.Add(reader["Description"].ToString());
                        }
                    }
                }
            }
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            string username = UsernameTextBox.Text;
            string email = EmailTextBox.Text;
            string password = PasswordBox.Password;
            string selectedDiscount = DiscountComboBox.SelectedItem as string;

            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password) || string.IsNullOrEmpty(selectedDiscount))
            {
                SetMessage("All fields must be filled in", Colors.Red);
                return;
            }

            if (!IsValidEmail(email))
            {
                SetMessage("Invalid email format.", Colors.Red);
                return;
            }

            UpdateUserInDatabase(UserSession.UserId, username, email, password, selectedDiscount);
            SetMessage("User details updated successfully", Colors.Green);
            UserSession.Username = username;
            UserSession.Email = email;
            UserSession.Password = password;
            UserSession.SelectedDiscount = selectedDiscount;
        }

        private void UpdateUserInDatabase(int userId, string username, string email, string password, string discountDescription)
        {
            using (var connection = new SQLiteConnection($"Data Source={_dbPath};Version=3;"))
            {
                connection.Open();
                int discountId = GetDiscountIdByDescription(connection, discountDescription);

                string query = "UPDATE Users SET Username = @Username, Email = @Email, Password = @Password, DiscountId = @DiscountId WHERE Id = @Id";
                using (var command = new SQLiteCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Id", userId);
                    command.Parameters.AddWithValue("@Username", username);
                    command.Parameters.AddWithValue("@Email", email);
                    command.Parameters.AddWithValue("@Password", password);
                    command.Parameters.AddWithValue("@DiscountId", discountId);
                    command.ExecuteNonQuery();
                }
            }
        }

        private int GetDiscountIdByDescription(SQLiteConnection connection, string description)
        {
            string query = "SELECT Id FROM Discounts WHERE Description = @Description";
            using (var command = new SQLiteCommand(query, connection))
            {
                command.Parameters.AddWithValue("@Description", description);
                return Convert.ToInt32(command.ExecuteScalar());
            }
        }

        private bool IsValidEmail(string email)
        {
            try
            {
                var addr = new System.Net.Mail.MailAddress(email);
                return addr.Address == email;
            }
            catch
            {
                return false;
            }
        }

        private void SetMessage(string message, Color color)
        {
            MessageTextBlock.Text = message;
            MessageTextBlock.Foreground = new SolidColorBrush(color);
        }

        private void HomeButton_Click(object sender, RoutedEventArgs e)
        {
            MainWindow mainWindow = new MainWindow();
            mainWindow.Show();
            this.Close();
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            UserProfile userProfile = new UserProfile();
            userProfile.Show();
            this.Close();
        }

        private void Window_OnMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                DragMove();
            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            UsernameTextBlock.Text = UserSession.Username;
        }
    }
}
