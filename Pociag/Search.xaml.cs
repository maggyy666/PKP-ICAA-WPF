using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json.Serialization.Metadata;
using System.Windows;
using System.Windows.Input;

namespace Pociag
{
    /// <summary>
    /// Logika interakcji dla klasy Search.xaml
    /// </summary>
    public partial class Search : Window
    {
        public string[] Cities { get; set; }
        public string[] Status { get; set; }
        public Search()
        {
            InitializeComponent();
            Cities = new string[] { "Warsaw", "Kraków", "Wrocław", "Poznań", "Gdańsk", "Gdynia", "Rzeszów", "Opole" };
            Status = new string[] { "Adult", "Child", "Student", "Disabled", "Retired" };
            DataContext = this;

            //_travelService = new JsonMetadataServices<Travel>("JSON/travels.json");

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
                Close();
            }
            //Window.GetWindow(this).Close();
        }
        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            MainWindow mainWindow = new MainWindow();
            mainWindow.Show();
            Close();
        }
        public void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            DateTime? selectedDate = _Calendar.SelectedDate;
            if (selectedDate != null)
            {
                string data = selectedDate.Value.ToString("yyyy-MM-dd");

                string beginning = Beginning.SelectedItem.ToString();
                string final = Final.SelectedItem.ToString();
                string _status = _Status.SelectedItem.ToString();

                if (string.IsNullOrEmpty(beginning) || string.IsNullOrEmpty(final) || string.IsNullOrEmpty(_status))
                {
                    MessageBox.Show("Nie wybrano wszystkich pól!");
                    return;
                }

                string dane = beginning + ";" + final + ";" + _status + ";" + data + Environment.NewLine;

                if (!File.Exists("Podroz.txt"))
                {
                    File.WriteAllText("Podroz.txt", dane);
                }
                else
                {
                    File.AppendAllText("Podroz.txt", dane);
                }
            }
            else
            {
                MessageBox.Show("Nie wybrano daty!");
            }

        }
        private void LoadButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string[] lines = File.ReadAllLines("Podroz.txt");
                if (lines.Length > 0)
                {
                    string[] data = lines[lines.Length - 1].Split(';');
                    if (data.Length == 4)
                    {
                        Beginning.SelectedItem = data[0];
                        Final.SelectedItem = data[1];
                        _Status.SelectedItem = data[2];

                        DateTime date;
                        if (DateTime.TryParse(data[3], out date))
                        {
                           _Calendar.SelectedDate = date;
                        }
                    }
                }
            }
            catch (FileNotFoundException)
            {
                MessageBox.Show("Nie znaleziono pliku Podroz.txt");
            }
        }
        private void CalculateButton_Click(object sender, RoutedEventArgs e)
        {
            string startingCity = (string)Beginning.SelectedValue;
            string finalCity = (string)Final.SelectedValue;
            string status = (string)_Status.SelectedValue;

            double price = 0;

            Dictionary<string, Dictionary<string, int>> pricelist = new Dictionary<string, Dictionary<string, int>>();
            string[] pricelistLines = File.ReadAllLines("Cennik.txt");
            foreach (string line in pricelistLines)
            {
                string[] field = line.Split('-');
                string city1 = field[0];
                string[] field2 = field[1].Split(':');
                string city2 = field2[0];
                int ticketPrice = int.Parse(field2[1]);

                if (!pricelist.ContainsKey(city1))
                {
                    pricelist[city1] = new Dictionary<string, int>();
                }
                pricelist[city1][city2] = ticketPrice;
            }

            if (pricelist.ContainsKey(startingCity) && pricelist[startingCity].ContainsKey(finalCity))
            {
                price = pricelist[startingCity][finalCity];
            }
            else
            {
                MessageBox.Show("There is no connection between the cities listed in the price list!");
                return;
            }

            switch (status)
            {
                case "Adult":
                    break;
                case "Child":
                    price = price * 0.3;
                    break;
                case "Student":
                    price = price * 0.50;
                    break;
                case "Disabled person":
                    price = price * 0.4;
                    break;
                case "Pensioner":
                    price = price * 0.6;
                    break;
                default:
                    break;
            }

            MessageBox.Show($"Ticket price: PLN {price}");
        }

        
    }
}
