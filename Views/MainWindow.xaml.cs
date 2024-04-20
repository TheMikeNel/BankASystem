using BankASystem.Models;
using System.Windows;
using System.Windows.Documents;

namespace BankASystem
{
    
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Window_Closed(object sender, System.EventArgs e)
        {
            ClientsDataRepository.SaveClientsList();
            BankAccountsRepository.SaveAccounts();
        }
    }
}
