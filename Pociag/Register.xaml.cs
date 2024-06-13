using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace Pociag
{
    public partial class Register : Window
    {
        private readonly string _filePath = "users.json";
        private List<User> _users;

        public Register()
        {
            InitializeComponent();
            LoadOrCreateUsers();
        }

        private void LoadOrCreateUsers()
        {
            if (File.Exists(_filePath))
            {
                LoadUsers();
            }
            else
            {
                _users = new List<User>();
                SaveUsers();
            }
        }

        private void LoadUsers()
        {
            var jsonService = new JsonService<User>(_filePath);
            var loadedUsers = jsonService.Load();
            _users = loadedUsers ?? new List<User>();
        }

        private void SaveUsers()
        {
            var jsonService = new JsonService<User>(_filePath);
            jsonService.Save(_users);
        }

        private void RegisterButton_Click(object sender, RoutedEventArgs e)
        {
            string username = UsernameTextBox.Text == "Username" ? "" : UsernameTextBox.Text;
            string email = EmailTextBox.Text == "Email" ? "" : EmailTextBox.Text;
            string password = PasswordBox.Password == "Password" ? "" : PasswordBox.Password;

            if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
            {
                SetMessage("All fields must be filled in.", Colors.Red);
                return;
            }

            if (!IsValidEmail(email))
            {
                SetMessage("Invalid email format. Email must contain '@'.", Colors.Red);
                return;
            }

            if (_users.Any(u => u.Username == username))
            {
                SetMessage("The provided username already exists!", Colors.Red);
            }
            else
            {
                _users.Add(new User { Username = username, Email = email, Password = password });
                SaveUsers();
                SetMessage("Registration completed successfully!", Colors.Green);
            }
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            Login loginWindow = new Login();
            loginWindow.Show();
            this.Close();
        }

        private void Window_OnMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                DragMove();
            }
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            Login loginWindow = new Login();
            loginWindow.ShowDialog();
        }

        private void RemoveText(object sender, EventArgs e)
        {
            TextBox textBox = sender as TextBox;
            if (textBox != null)
            {
                if (textBox.Text == "Username" || textBox.Text == "Email")
                {
                    textBox.Text = "";
                    textBox.Foreground = new SolidColorBrush(Colors.White); // Zmieniamy kolor tekstu na biały
                    textBox.Opacity = 1.0; // Pełna przezroczystość tekstu
                }
            }
        }

        private void AddText(object sender, EventArgs e)
        {
            TextBox textBox = sender as TextBox;
            if (textBox != null && string.IsNullOrWhiteSpace(textBox.Text))
            {
                if (textBox.Name == "UsernameTextBox")
                {
                    textBox.Text = "Username";
                }
                else if (textBox.Name == "EmailTextBox")
                {
                    textBox.Text = "Email";
                }
                textBox.Foreground = new SolidColorBrush(Colors.Gray); // Placeholder color
                textBox.Opacity = 0.5; // Połowiczna przezroczystość dla placeholdera
            }
        }

        private void RemovePasswordText(object sender, EventArgs e)
        {
            PasswordBox passwordBox = sender as PasswordBox;
            if (passwordBox != null && passwordBox.Password == "Password")
            {
                passwordBox.Password = "";
                passwordBox.Foreground = new SolidColorBrush(Colors.White);
                passwordBox.Opacity = 1.0;
            }
        }

        private void AddPasswordText(object sender, EventArgs e)
        {
            PasswordBox passwordBox = sender as PasswordBox;
            if (passwordBox != null && string.IsNullOrWhiteSpace(passwordBox.Password))
            {
                passwordBox.Password = "Password";
                passwordBox.Foreground = new SolidColorBrush(Colors.Gray);
                passwordBox.Opacity = 0.5;
            }
        }

        private bool IsValidEmail(string email)
        {
            // Prosta walidacja e-mail
            return email.Contains("@");
        }

        // Funkcja do ustawiania komunikatów z kolorem
        private void SetMessage(string message, Color color)
        {
            MessageTextBlock.Text = message;
            MessageTextBlock.Foreground = new SolidColorBrush(color);
        }
    }
}
