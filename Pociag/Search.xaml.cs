using System;
using System.Collections.Generic;
using System.IO;
using System.Net.NetworkInformation;
using System.Text.Json;
using System.Windows;
using System.Windows.Input;

namespace Pociag
{
    public partial class Search : Window
    {
        private Dictionary<string, Dictionary<string, int>> priceList;
        private List<string> cities;
        private List<string> discounts;

        public Search()
        {
            InitializeComponent();
            LoadCitiesAndDiscounts();
            InitializePriceList();

            Beginning.ItemsSource = cities;
            Final.ItemsSource = cities;
            _Status.ItemsSource = discounts;
        }

        private void LoadCitiesAndDiscounts()
        {
            cities = new List<string> { "Warsaw", "Krakow", "Wroclaw", "Poznan", "Gdansk", "Gdynia", "Rzeszow", "Opole", "Lodz", "Szczecin" };
            discounts = new List<string> { "None", "Student", "Senior", "Disabled", "Guide", "BlindNonSelfSufficient", "ChildrenWithDisability", "DisabledStudents", "ParentGuardian", "NonProfessionalMilitaryService", "CivilianWarBlindVictims", "Students", "AbroadStudents", "ForeignLanguageTeachers", "SocialServiceWorkers", "DoctoralStudents", "LargeFamilyCard", "Blind", "NonSelfSufficient", "ChildrenUntil24", "PolishLanguageStudents", "EuropeanSchoolStudents", "Teachers", "Other" };
        }

        private void InitializePriceList()
        {
            priceList = new Dictionary<string, Dictionary<string, int>>();

            // Definicja cen połączeń między miastami
            priceList["Warsaw"] = new Dictionary<string, int>
{
    { "Krakow", 250 },
    { "Wroclaw", 300 },
    { "Poznan", 200 },
    { "Gdansk", 400 },
    { "Gdynia", 450 },
    { "Rzeszow", 350 },
    { "Opole", 280 },
    { "Lodz", 150 },
    { "Szczecin", 400 }
};

            priceList["Krakow"] = new Dictionary<string, int>
{
    { "Warsaw", 250 },
    { "Wroclaw", 200 },
    { "Poznan", 300 },
    { "Gdansk", 500 },
    { "Gdynia", 450 },
    { "Rzeszow", 120 },
    { "Opole", 180 },
    { "Lodz", 250 },
    { "Szczecin", 550 }
};

            priceList["Wroclaw"] = new Dictionary<string, int>
{
    { "Warsaw", 300 },
    { "Krakow", 200 },
    { "Poznan", 100 },
    { "Gdansk", 450 },
    { "Gdynia", 500 },
    { "Rzeszow", 300 },
    { "Opole", 100 },
    { "Lodz", 200 },
    { "Szczecin", 400 }
};

            priceList["Poznan"] = new Dictionary<string, int>
{
    { "Warsaw", 200 },
    { "Krakow", 300 },
    { "Wroclaw", 100 },
    { "Gdansk", 350 },
    { "Gdynia", 400 },
    { "Rzeszow", 220 },
    { "Opole", 150 },
    { "Lodz", 150 },
    { "Szczecin", 300 }
};

            priceList["Gdansk"] = new Dictionary<string, int>
{
    { "Warsaw", 400 },
    { "Krakow", 500 },
    { "Wroclaw", 450 },
    { "Poznan", 350 },
    { "Gdynia", 50 },
    { "Rzeszow", 450 },
    { "Opole", 420 },
    { "Lodz", 400 },
    { "Szczecin", 250 }
};

            priceList["Gdynia"] = new Dictionary<string, int>
{
    { "Warsaw", 450 },
    { "Krakow", 450 },
    { "Wroclaw", 500 },
    { "Poznan", 400 },
    { "Gdansk", 50 },
    { "Rzeszow", 500 },
    { "Opole", 450 },
    { "Lodz", 450 },
    { "Szczecin", 300 }
};

            priceList["Rzeszow"] = new Dictionary<string, int>
{
    { "Warsaw", 350 },
    { "Krakow", 120 },
    { "Wroclaw", 300 },
    { "Poznan", 220 },
    { "Gdansk", 450 },
    { "Gdynia", 500 },
    { "Opole", 280 },
    { "Lodz", 320 },
    { "Szczecin", 550 }
};

            priceList["Opole"] = new Dictionary<string, int>
{
    { "Warsaw", 280 },
    { "Krakow", 180 },
    { "Wroclaw", 100 },
    { "Poznan", 150 },
    { "Gdansk", 420 },
    { "Gdynia", 450 },
    { "Rzeszow", 280 },
    { "Lodz", 200 },
    { "Szczecin", 400 }
};

            priceList["Lodz"] = new Dictionary<string, int>
{
    { "Warsaw", 150 },
    { "Krakow", 250 },
    { "Wroclaw", 200 },
    { "Poznan", 150 },
    { "Gdansk", 400 },
    { "Gdynia", 450 },
    { "Rzeszow", 320 },
    { "Opole", 200 },
    { "Szczecin", 400 }
};

            priceList["Szczecin"] = new Dictionary<string, int>
{
    { "Warsaw", 400 },
    { "Krakow", 550 },
    { "Wroclaw", 400 },
    { "Poznan", 300 },
    { "Gdansk", 250 },
    { "Gdynia", 300 },
    { "Rzeszow", 550 },
    { "Opole", 400 },
    { "Lodz", 400 }
};



            // Dodaj ceny dla pozostałych miast
            // ...

            // Symetryczne dodanie cen dla każdego połączenia
            foreach (var city in cities)
            {
                foreach (var otherCity in cities)
                {
                    if (city != otherCity && !priceList[city].ContainsKey(otherCity))
                    {
                        priceList[city][otherCity] = priceList[otherCity][city];
                    }
                }
            }
        }

        private void CalculateButton_Click(object sender, RoutedEventArgs e)
        {
            string startingCity = (string)Beginning.SelectedItem;
            string finalCity = (string)Final.SelectedItem;
            string selectedDiscount = (string)_Status.SelectedItem;

            if (string.IsNullOrEmpty(startingCity) || string.IsNullOrEmpty(finalCity) || string.IsNullOrEmpty(selectedDiscount))
            {
                MessageBox.Show("Please select all fields");
                return;
            }

            if (!priceList.ContainsKey(startingCity) || !priceList[startingCity].ContainsKey(finalCity))
            {
                MessageBox.Show("There is no connection between selected cities!");
                return;
            }
            if (startingCity == finalCity)
            {
                MessageBox.Show("Starting city and final city cannot be the same!");
                return;
            }

            int basePrice = priceList[startingCity][finalCity];
            double discountPercentage = GetDiscountPercentage(selectedDiscount);
            double finalPrice = basePrice * (1 - (discountPercentage / 100));

            MessageBox.Show($"Ticket price: {finalPrice} PLN");
        }

        private double GetDiscountPercentage(string selectedDiscount)
        {
            switch (selectedDiscount)
            {
                case "None":
                    return 0;
                case "Student":
                    return 50;
                case "Senior":
                    return 30;
                case "Disabled":
                    return 20;
                case "Guide":
                    return 95; // 95% ulga dla przewodnika lub opiekuna
                case "BlindNonSelfSufficient":
                    return 93; // 93% ulga dla osób niewidomych uznanych za niezdolne do samodzielnej egzystencji
                case "ChildrenWithDisability":
                case "DisabledStudents":
                case "ParentGuardian":
                    return 78; // 78% ulga dla dzieci i młodzieży dotkniętych inwalidztwem, niepełnosprawnych studentów oraz jednego z rodziców lub opiekuna
                case "NonProfessionalMilitaryService":
                case "CivilianWarBlindVictims":
                    return 78; // 78% ulga dla żołnierzy odbywających niezawodową służbę wojskową, cywilnych niewidomych ofiar działań wojennych
                case "Students":
                case "AbroadStudents":
                case "ForeignLanguageTeachers":
                case "SocialServiceWorkers":
                case "DoctoralStudents":
                    return 51; // 51% ulga dla studentów do 26 roku życia, słuchaczy kolegiów nauczycielskich, nauczycieli języka obcego, doktorantów
                case "LargeFamilyCard":
                    return 49; // 49% ulga dla posiadaczy Karty Dużej Rodziny
                case "Blind":
                case "NonSelfSufficient":
                case "ChildrenUntil24":
                case "PolishLanguageStudents":
                case "EuropeanSchoolStudents":
                    return 37; // 37% ulga dla osób niewidomych, osób niezdolnych do samodzielnej egzystencji, dzieci i młodzieży do 24 roku życia, posiadaczy Karty Polaka, dzieci uczących się języka polskiego za granicą, uczniów szkół europejskich
                case "Teachers":
                    return 33; // 33% ulga dla nauczycieli przedszkoli, szkół podstawowych i ponadpodstawowych, nauczycieli akademickich
                default:
                    return 0;
            }
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult result = MessageBox.Show("Are you sure you want to exit?", "Exit confirmation", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (result == MessageBoxResult.Yes)
            {
                Close();
            }
        }
        private void Window_OnMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                DragMove();
            }
        }

        private void LoadButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (File.Exists("Travels.json"))
                {
                    string jsonString = File.ReadAllText("Travels.json");
                    List<Travel> travels = JsonSerializer.Deserialize<List<Travel>>(jsonString);

                    if (travels != null && travels.Count > 0)
                    {
                        Travel lastTravel = travels[travels.Count - 1];
                        Beginning.SelectedItem = lastTravel.Beginning;
                        Final.SelectedItem = lastTravel.Final;
                        _Calendar.SelectedDate = lastTravel.Date;
                        _Status.SelectedItem = lastTravel.Status;

                        MessageBox.Show("Last travel loaded successfully!");
                    }
                    else
                    {
                        MessageBox.Show("No saved travels found.");
                    }
                }
                else
                {
                    MessageBox.Show("No saved travels found.");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error while loading travels: {ex.Message}");
            }
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            string startingCity = (string)Beginning.SelectedItem;
            string finalCity = (string)Final.SelectedItem;
            DateTime selectedDate = _Calendar.SelectedDate ?? DateTime.Now;
            string selectedDiscount = (string)_Status.SelectedItem;
            if (startingCity == finalCity)
            {
                MessageBox.Show("Starting city and final city cannot be the same!");
            }
            Travel newTravel = new Travel
            {
                Beginning = startingCity,
                Final = finalCity,
                Date = selectedDate,
                Status = selectedDiscount
            };

            List<Travel> travels = new List<Travel>();

            if (File.Exists("Travels.json"))
            {
                try
                {
                    string jsonString = File.ReadAllText("Travels.json");
                    travels = JsonSerializer.Deserialize<List<Travel>>(jsonString);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error while loading existing travels: {ex.Message}");
                }
            }

            travels.Add(newTravel);

            try
            {
                string jsonString = JsonSerializer.Serialize(travels);
                File.WriteAllText("Travels.json", jsonString);
                MessageBox.Show("Travel saved successfully!");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error while saving travel: {ex.Message}");
            }
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            MainWindow mainWindow = new MainWindow();
            mainWindow.Show();
            Close();
        }
    }
}




