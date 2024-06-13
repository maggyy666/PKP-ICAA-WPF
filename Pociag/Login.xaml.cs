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
    public partial class Login : Window
    {
        private readonly string _filePath = "users.json";
        private List<User> _users;

        public Login()
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
            _users = jsonService.Load();
        }

        private void SaveUsers()
        {
            var jsonService = new JsonService<User>(_filePath);
            jsonService.Save(_users);
        }

        private bool CheckLoginAndPassword(string login, string password)
        {
            var user = _users.FirstOrDefault(u => u.Username == login && u.Password == password);
            return user != null;
        }

        private void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            string login = LoginTextBox.Text;
            string password = PasswordBox.Password;

            if (string.IsNullOrWhiteSpace(login) || string.IsNullOrWhiteSpace(password))
            {
                SetMessage("Username and password cannot be empty.", Colors.Red);
                return;
            }

            if (CheckLoginAndPassword(login, password))
            {
                SetMessage("You have logged in successfully!", Colors.Red);
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

        private void RemoveText(object sender, EventArgs e)
        {
            TextBox textBox = sender as TextBox;
            if (textBox != null)
            {
                if (textBox.Text == "Username")
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
                if (textBox.Name == "LoginTextBox")
                {
                    textBox.Text = "Username";
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

        private void SetMessage(string message, Color color)
        {
            MessageTextBlock.Text = message;
            MessageTextBlock.Foreground = new SolidColorBrush(color); 
        }
    }
}
