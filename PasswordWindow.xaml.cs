using System;
using System.Collections.Generic;
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

namespace BankASystem
{
    /// <summary>
    /// Логика взаимодействия для PasswordWindow.xaml
    /// </summary>
    public partial class PasswordWindow : Window
    {
        private string Password { get { return PasswordBox.Text; } }

        public PasswordWindow()
        {
            InitializeComponent();
        }

        private void Apply_Click(object sender, RoutedEventArgs e)
        {
            if (Password == Employee.password)
            {
                this.DialogResult = true;
            }
            else
            {
                MessageBox.Show("Пароль неверный", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                PasswordBox.Text = "";
            }
        }
    }
}
