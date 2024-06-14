using Newtonsoft.Json.Bson;
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
        private void Window_Loadded(object sender, RoutedEventArgs e)
        {
            UpdateUsernameDisplay();
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
                        Password TEXT NOT NULL
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
                SetMessage("You have logged in successfully!", Colors.Green);
                // Przejdź do następnego okna lub wykonaj inne czynności
            }
            else
            {
                SetMessage("Incorrect username or password.", Colors.Red);
            }
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
            Window.GetWindow(this).Close();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            MainWindow mainWindow = new MainWindow();
            if (mainWindow != null)
            {
                mainWindow.Close();
            }
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            // MainWindow mainwindow = new MainWindow();
            // mainwindow.Show();
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
            if(UsernameTextBlock.Text != null)
            {
                UsernameTextBlock.Text = UserSession.Username;
            }
        }
    }
}
