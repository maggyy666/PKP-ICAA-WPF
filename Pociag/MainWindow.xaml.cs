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
            MessageBoxResult result = MessageBox.Show("Are you sure you want to exit?", "Exit confirmation", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (result == MessageBoxResult.Yes)
            {
                Application.Current.Shutdown();
            }
            //Window.GetWindow(this).Close();
        }
        private void OpenLoginWindow_Click(object sender, RoutedEventArgs e)//visibility, buttons content
        {
            Login login = new Login();
            login.Show();
            this.Close();
        }
        private void StartButton_Click(object sender, RoutedEventArgs e)//visibility
        {
          
            Search searchWindow = new Search();
            searchWindow.Show();
            this.Close();
        }
        private void CreditsButton_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show(" Author: Jakub Czyż \n Programming IV \n Summer semester 2023 \n 2nd year Computer science \n WEAiI, Opole University of Technology");
        }
   
    }
}
