using System.Windows;
using System.Windows.Controls;


namespace BankASystem
{
    /// <summary>
    /// Логика взаимодействия для PasswordWindow.xaml
    /// </summary>
    public partial class PasswordWindow : Window
    {

        public PasswordWindow()
        {
            InitializeComponent();
        }

        private void Apply_Click(object sender, RoutedEventArgs e)
        {
            if (PasswordBox.Password == (Owner as MainWindow)?.GetManagerPassword())
            {
                this.DialogResult = true;
            }
            else
            {
                MessageBox.Show("Пароль неверный", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                PasswordBox.Password = string.Empty;
            }
        }
    }
}
