using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Pociag
{
    /// <summary>
    /// Logika interakcji dla klasy Login.xaml
    /// </summary>
    public partial class Login : Window
    {
        public Login()
        {
            InitializeComponent();
        }

        private void Window_OnMouseDown(object sender, MouseButtonEventArgs e)
        {
            if(e.LeftButton == MouseButtonState.Pressed)
            {
                DragMove();
            }
        }
        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            Window.GetWindow(this).Close();
        }
        private void SaveLoginAndPassword(string login, string password) 
        {
            string filePath = "loginData.txt";

            if (!File.Exists(filePath))//function that checks if there's any loginData.txt file in this project, if not it creates one
            {
                File.CreateText(filePath).Close();
            }
            string dataToSave = $"Login: {login}, Hasło: {password}{Environment.NewLine}"; //it saves data in following: "Login:" login "Haslo:" password

            try
            {
                File.AppendAllText(filePath, dataToSave);
                MessageBox.Show("Zarejestrowano pomyslnie!");
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Błąd podczas zapisywania danych: {ex.Message}");
            }
        }

        private void Rejestracja(object sender, RoutedEventArgs e)//actual process of saving user's data to loginData.txt
        {
            string login = LoginTextBox.Text;
            string password = PasswordBox.Password;

            if (CheckRegister(login, password))
            {
                MessageBox.Show("Podane dane już istnieją!");
            }
            else
            {
                SaveLoginAndPassword(login, password);
                MessageBox.Show("Rejestracja zakończona pomyślnie!");
            }
        }

        private bool CheckRegister(string login, string password)
        {
            string loginDataPath = "loginData.txt";
            bool isDuplicate = false;

            if (File.Exists(loginDataPath))
            {
                string[] loginData = File.ReadAllLines(loginDataPath);

                foreach (string line in loginData)
                {
                    if (line.Contains("Login: " + login) && line.Contains("Hasło: " + password))
                    {
                        isDuplicate = true;
                        break;
                    }
                }
            }

            return isDuplicate;
        }

        private bool CheckLoginAndPassword(string login, string password)
        {
            string filePath = "loginData.txt";

            try
            {
                using (StreamReader sr = new StreamReader(filePath))
                {
                    string line;
                    while ((line = sr.ReadLine()) != null)
                    {
                        if (line.Contains($"Login: {login}, Hasło: {password}"))
                        {
                            return true;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Błąd podczas odczytu pliku: {ex.Message}");
            }

            return false;
        }
        private void Logowanie(object sender, RoutedEventArgs e)//checks if there is login and password in loginData
        {
            string login = LoginTextBox.Text;
            string password = PasswordBox.Password;

            if (CheckLoginAndPassword(login, password))
            {
                MessageBox.Show("Zalogowano pomyślnie.");
                this.Close();
            }
            else
            {
                MessageBox.Show("Nieprawidłowy login lub hasło.");
            }
        }

    }
}
