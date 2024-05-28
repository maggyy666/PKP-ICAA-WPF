using System;
using System.Collections.Generic;
using System.Globalization;
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
    /// Logika interakcji dla klasy Search.xaml
    /// </summary>
    public partial class Search : Window
    {
        public string[] Cities { get; set; }
        public string[] Status { get; set; }
        public Search()
        {
            InitializeComponent();
            Cities = new string[] {"Warszawa","Kraków","Wrocław","Poznań","Gdańsk","Gdynia","Rzeszów","Opole" };
            Status = new string[] {"Osoba dorosła","Dziecko","Student","Osoba niepełnosprawna","Emeryt"};
            DataContext = this;


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
            MessageBoxResult result = MessageBox.Show("Czy na pewno chcesz wyjść?", "Potwierdzenie wyjścia", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (result == MessageBoxResult.Yes)
            {
                Close();
            }
            //Window.GetWindow(this).Close();
        }
        private void PowrotButton_Click(object sender, RoutedEventArgs e)
        {
            MainWindow mainWindow = new MainWindow();
            mainWindow.Show();
            Close();
        }
        public void ZapiszButton_Click(object sender, RoutedEventArgs e)
        {
            DateTime? wybranaData = Kalendarz.SelectedDate;
            if (wybranaData != null)
            {
                // konwertujemy wartość na format string
                string data = wybranaData.Value.ToString("yyyy-MM-dd");

                // pobieramy wartości wybrane w ComboBoxach
                string poczatek = Poczatek.SelectedItem.ToString();
                string koniec = Koniec.SelectedItem.ToString();
                string status1 = Statusek.SelectedItem.ToString();

                if (string.IsNullOrEmpty(poczatek) || string.IsNullOrEmpty(koniec) || string.IsNullOrEmpty(status1))
                {
                    MessageBox.Show("Nie wybrano wszystkich pól!");
                    return;
                }

                // tworzymy ciąg tekstowy do zapisania
                string dane = poczatek + ";" + koniec + ";" + status1 + ";" + data + Environment.NewLine;

                // zapisujemy dane do pliku
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
        private void WczytajButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string[] lines = File.ReadAllLines("Podroz.txt");
                if (lines.Length > 0)
                {
                    string[] data = lines[lines.Length - 1].Split(';');
                    if (data.Length == 4)
                    {
                        Poczatek.SelectedItem = data[0];
                        Koniec.SelectedItem = data[1];
                        Statusek.SelectedItem = data[2];

                        DateTime date;
                        if (DateTime.TryParse(data[3], out date))
                        {
                            Kalendarz.SelectedDate = date;
                        }
                    }
                }
            }
            catch (FileNotFoundException)
            {
                MessageBox.Show("Nie znaleziono pliku Podroz.txt");
            }
        }
        private void ObliczButton_Click(object sender, RoutedEventArgs e)
        {
            string miastoPoczatkowe = (string)Poczatek.SelectedValue;
            string miastoKoncowe = (string)Koniec.SelectedValue;
            string status = (string)Statusek.SelectedValue;

            double cena = 0;

            // Wczytanie cennika z pliku tekstowego
            Dictionary<string, Dictionary<string, int>> cennik = new Dictionary<string, Dictionary<string, int>>();
            string[] linieCennika = File.ReadAllLines("Cennik.txt");
            foreach (string linia in linieCennika)
            {
                string[] pola = linia.Split('-');
                string miasto1 = pola[0];
                string[] pola2 = pola[1].Split(':');
                string miasto2 = pola2[0];
                int cenaBiletu = int.Parse(pola2[1]);

                if (!cennik.ContainsKey(miasto1))
                {
                    cennik[miasto1] = new Dictionary<string, int>();
                }
                cennik[miasto1][miasto2] = cenaBiletu;
            }

            // Wyliczenie ceny biletu na podstawie wybranych danych i cennika
            if (cennik.ContainsKey(miastoPoczatkowe) && cennik[miastoPoczatkowe].ContainsKey(miastoKoncowe))
            {
                cena = cennik[miastoPoczatkowe][miastoKoncowe];
            }
            else
            {
                MessageBox.Show("Nie ma połączenia między podanymi miastami w cenniku!");
                return;
            }

            switch (status)
            {
                case "Osoba dorosła":
                    break;
                case "Dziecko":
                    cena = cena * 0.3;
                    break;
                case "Student":
                    cena = cena * 0.50;
                        break;
                case "Osoba niepełnosprawna":
                    cena = cena * 0.4;
                    break;
                case "Emeryt":
                    cena = cena * 0.6;
                    break;
                default:
                    break;
            }

            MessageBox.Show($"Cena biletu: {cena} zł");
        }

        
    }
}
