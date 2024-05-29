using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Input;

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

            if (CheckLoginAndPassword(login, password))
            {
                MessageBox.Show("You have logged in successfully!");
                this.Close();
            }
            else
            {
                MessageBox.Show("Incorrect login or password");
            }
        }

        private void RegisterButton_Click(object sender, RoutedEventArgs e)
        {
            string login = LoginTextBox.Text;
            string password = PasswordBox.Password;

            if (_users.Any(u => u.Username == login))
            {
                MessageBox.Show("The provided username already exists!");
            }
            else
            {
                _users.Add(new User { Username = login, Password = password });
                SaveUsers();
                MessageBox.Show("Registration completed successfully!");
            }
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
            Window.GetWindow(this).Close();
        }
    }
}
