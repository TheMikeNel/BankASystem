using System.Windows;
using System.Windows.Controls;


namespace BankASystem
{
    /// <summary>
    /// Логика взаимодействия для PasswordWindow.xaml
    /// </summary>
    public partial class PasswordWindow : Window
    {
        private string _password;

        public string Password
        {
            get { return _password; }
            private set { _password = value; }
        }

        public PasswordWindow(string password = null)
        {
            Password = password;

            InitializeComponent();
        }

        private void Apply_Click(object sender, RoutedEventArgs e)
        {
            if (PasswordBox.Password == this.Password) this.DialogResult = true;
            else
            {
                MessageBox.Show("Пароль неверный", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                PasswordBox.Password = string.Empty;
            }
        }
    }
}
