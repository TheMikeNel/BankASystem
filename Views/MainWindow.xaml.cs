using BankASystem.Models;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace BankASystem
{
    
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        ObservableCollection<Client> ClientsCollection = new ObservableCollection<Client>() { new Client("Niga", "12431", "1111000333")};
        public MainWindow()
        {
            InitializeComponent();

            //ClientsListView.ItemsSource = new ObservableCollection<Client>() { new Client("Niga", "12431", "1111000333")};
        }
    }
}
