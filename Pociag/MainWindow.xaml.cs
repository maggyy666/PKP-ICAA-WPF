using System;
using System.Windows;
using System.Windows.Controls;
using System.IO;
using Microsoft.Win32;
using System.Windows.Input;

namespace Pociag
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            
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
        private void OpenLoginWindow_Click(object sender, RoutedEventArgs e)//visibility, buttons content
        {
                Login login = new Login();
                login.ShowDialog();
        }
        private void StartButton_Click(object sender, RoutedEventArgs e)//visibility
        {
          
            Search searchWindow = new Search();
            searchWindow.Show();
            Close();
        }
        private void CreditsButton_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show(" Autor: Jakub Czyż \n Programowanie IV \n Semestr letni 2023 \n 2 rok Informatyka \n WEAiI, Politechnika Opolska");
        }
    }
}
