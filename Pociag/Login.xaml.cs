using System;
using System.Data.SQLite;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace Pociag
{
    public partial class Login : Window
    {
        private readonly string _dbPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "database.db");
        private SQLiteConnection _connection;

        public Login()
        {
            InitializeComponent();
            InitializeDatabase();
            UpdateUsernameDisplay();
        }

        private void InitializeDatabase()
        {
            if (!File.Exists(_dbPath))
            {
                CreateDatabase();
            }

            _connection = new SQLiteConnection($"Data Source={_dbPath};Version=3;");
            _connection.Open();
        }

        private void CreateDatabase()
        {
            SQLiteConnection.CreateFile(_dbPath);
            using (var connection = new SQLiteConnection($"Data Source={_dbPath};Version=3;"))
            {
                connection.Open();
                string createUserTable = @"
                    CREATE TABLE Users (
                        Id INTEGER PRIMARY KEY AUTOINCREMENT,
                        Username TEXT NOT NULL,
                        Email TEXT NOT NULL,
                        Password TEXT NOT NULL,
                        DiscountId INTEGER,
                        FOREIGN KEY (DiscountId) REFERENCES Discounts(Id)
                    )";
                using (var command = new SQLiteCommand(createUserTable, connection))
                {
                    command.ExecuteNonQuery();
                }
            }
        }

        private bool CheckLoginAndPassword(string username, string password)
        {
            string query = "SELECT COUNT(1) FROM Users WHERE Username = @Username AND Password = @Password";
            using (var command = new SQLiteCommand(query, _connection))
            {
                command.Parameters.AddWithValue("@Username", username);
                command.Parameters.AddWithValue("@Password", password);
                int count = Convert.ToInt32(command.ExecuteScalar());
                return count > 0;
            }
        }

        private void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            string username = LoginTextBox.Text;
            string password = PasswordBox.Password;

            if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
            {
                SetMessage("Username and password cannot be empty.", Colors.Red);
                return;
            }

            if (CheckLoginAndPassword(username, password))
            {
                int userId = GetUserIdFromDatabase(username);
                string email = GetEmailFromDatabase(username);
                var (discountId, selectedDiscount) = GetSelectedDiscountFromDatabase(username);

                // Przekazanie wszystkich wymaganych parametrów do UserSession.Login
                UserSession.Login(userId, username, email, password, discountId, selectedDiscount);
                SetMessage("You have logged in successfully!", Colors.Green);
                UpdateUsernameDisplay();
            }
            else
            {
                SetMessage("Incorrect username or password.", Colors.Red);
            }
        }
        private int GetUserIdFromDatabase(string username)
        {
            string query = "SELECT Id FROM Users WHERE Username = @Username";
            using (var command = new SQLiteCommand(query, _connection))
            {
                command.Parameters.AddWithValue("@Username", username);
                return Convert.ToInt32(command.ExecuteScalar());
            }
        }




        private string GetEmailFromDatabase(string username)
        {
            string query = "SELECT Email FROM Users WHERE Username = @Username";
            using (var command = new SQLiteCommand(query, _connection))
            {
                command.Parameters.AddWithValue("@Username", username);
                return command.ExecuteScalar()?.ToString() ?? string.Empty;
            }
        }

        private (int, string) GetSelectedDiscountFromDatabase(string username)
        {
            string query = "SELECT DiscountId FROM Users WHERE Username = @Username";
            using (var command = new SQLiteCommand(query, _connection))
            {
                command.Parameters.AddWithValue("@Username", username);
                var discountId = Convert.ToInt32(command.ExecuteScalar());

                if (discountId == 0)
                    return (0, string.Empty);

                string discountQuery = "SELECT Id, Description FROM Discounts WHERE Id = @Id";
                using (var discountCommand = new SQLiteCommand(discountQuery, _connection))
                {
                    discountCommand.Parameters.AddWithValue("@Id", discountId);
                    using (var reader = discountCommand.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            int id = reader.GetInt32(0);
                            string description = reader.GetString(1);
                            return (id, description);
                        }
                    }
                }
            }
            return (0, string.Empty);
        }


        private void Register_Click(object sender, RoutedEventArgs e)
        {
            Register registerWindow = new Register();
            registerWindow.Show();
            this.Close();
        }

        private void Window_OnMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                DragMove();
            }
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
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

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            // Nie zamykaj MainWindow przy ładowaniu okna logowania
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            // Nie zamykaj MainWindow przy zamykaniu okna logowania
        }

        private void RemoveText(object sender, RoutedEventArgs e)
        {
            TextBox textBox = sender as TextBox;
            if (textBox != null && textBox.Text == "Username")
            {
                textBox.Text = "";
                textBox.Foreground = new SolidColorBrush(Colors.White);
                textBox.Opacity = 1.0;
            }
        }

        private void AddText(object sender, RoutedEventArgs e)
        {
            TextBox textBox = sender as TextBox;
            if (textBox != null && string.IsNullOrWhiteSpace(textBox.Text))
            {
                textBox.Text = "Username";
                textBox.Foreground = new SolidColorBrush(Colors.Gray);
                textBox.Opacity = 0.5;
            }
        }

        private void RemovePasswordText(object sender, RoutedEventArgs e)
        {
            PasswordBox passwordBox = sender as PasswordBox;
            if (passwordBox != null && passwordBox.Password == "Password")
            {
                passwordBox.Password = "";
                passwordBox.Foreground = new SolidColorBrush(Colors.White);
                passwordBox.Opacity = 1.0;
            }
        }

        private void AddPasswordText(object sender, RoutedEventArgs e)
        {
            PasswordBox passwordBox = sender as PasswordBox;
            if (passwordBox != null && string.IsNullOrWhiteSpace(passwordBox.Password))
            {
                passwordBox.Password = "Password";
                passwordBox.Foreground = new SolidColorBrush(Colors.Gray);
                passwordBox.Opacity = 0.5;
            }
        }

        private void SetMessage(string message, Color color)
        {
            MessageTextBlock.Text = message;
            MessageTextBlock.Foreground = new SolidColorBrush(color);
        }

        private void UpdateUsernameDisplay()
        {
            if (UserSession.IsLoggedIn && !string.IsNullOrEmpty(UserSession.Username))
            {
                UsernameTextBlock.Text = UserSession.Username;
            }
            else
            {
                UsernameTextBlock.Text = "Guest";
            }
        }

        private void UsernameTextBlock_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            UserProfile userProfileWindow = new UserProfile();
            userProfileWindow.Show();
            this.Close();
        }
    }
}
