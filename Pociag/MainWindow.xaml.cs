using System;
using System.Windows;
using System.Windows.Controls;
using System.IO;
using Microsoft.Win32;
using System.Windows.Input;
using System.Data.SQLite;

namespace Pociag
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly string dbPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "database.db");

        public MainWindow()
        {
            InitializeComponent();
            this.Loaded += MainWindow_Loaded;
            
        }
        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            UpdateUsernameDisplay();
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
            MessageBoxResult result = MessageBox.Show("Are you sure you want to exit?", "Exit confirmation", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (result == MessageBoxResult.Yes)
            {
                Application.Current.Shutdown();
            }
            //Window.GetWindow(this).Close();
        }
        private void OpenLoginWindow_Click(object sender, RoutedEventArgs e)
        {
            Login login = new Login();
            login.Show();
            this.Close();
        }
        private void StartButton_Click(object sender, RoutedEventArgs e)
        {
            if (UserSession.IsLoggedIn)
            {
                Search searchWindow = new Search();
                searchWindow.Show();
            }
            else
            {
                Login loginWindow = new Login();
                loginWindow.Show();
            }
            this.Close();
        }
        private void UpdateUsernameDisplay()
        {
            UsernameTextBlock.Text = UserSession.Username;
        }
        private void UsernameTextBlock_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (UserSession.IsLoggedIn)
            {
                UserProfile userProfileWindow = new UserProfile();
                userProfileWindow.Show();
            }
            else
            {
                Login loginWindow  = new Login();  
                loginWindow.Show();
            }
            this.Close();
        }

        private void CreateTablesIfNotExists()
        {
            using (SQLiteConnection conn = new SQLiteConnection($"Data Source={dbPath};Version=3;"))
            {
                conn.Open();
                string checkCitiesTableQuery = "SELECT name FROM sqlite_master WHERE type='table' AND name='Cities';";
                using (SQLiteCommand checkCitiesCmd = new SQLiteCommand(checkCitiesTableQuery, conn))
                {
                    var result = checkCitiesCmd.ExecuteScalar();
                    if(result == null)
                    {
                        string createCitiesTableQuery = @"
                            CREATE TABLE Cities (
                                Id INTEGER PRIMARY KEY AUTOINCREMENT,
                                CityName TEXT NOT NULL
                            );";
                    }
                }

            }
        }

    }
}
