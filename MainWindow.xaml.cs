using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Converters;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace BankASystem
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        ObservableCollection<Client> clientsCollection = Employee.ClientsCollection;

        Client selectedClient;
        List<TextBox> ClientTextBoxes = new List<TextBox>();

        public MainWindow()
        {
            InitializeComponent();

            ClientsListBox.ItemsSource = clientsCollection;

            ClientTextBoxes.Add(FIOTB);
            ClientTextBoxes.Add(PhoneNumberTB);
            ClientTextBoxes.Add(PassportTB);
        }

        private bool _isManager = false;
        private bool _isAddClient = false;
        private bool _isSaveChanges = false;

        // Цвета включенных / выключенных элементов
        static string enabledRes = "EnabledRColor";
        static string disabledRes = "DisabledColor";

        public bool IsManager
        {
            get => _isManager;

            set
            {
                if (_isManager != value)
                {
                    _isManager = value;

                    if (_isManager)
                    {
                        Employee.IsManager = true;
                        EmployeeButton.Content = "Войти как консультант";
                    }
                    else
                    {
                        Employee.IsManager = false;
                        EmployeeButton.Content = "Войти как менеджер";
                    }

                    SwitchEmployee(_isManager);
                }
            }
        }

        public bool IsAddClient
        {
            get => _isAddClient;
            set
            {
                if (_isAddClient != value)
                {
                    _isAddClient = value;
                    IsSaveChanges = _isAddClient;
                }
            }
        }

        public bool IsSaveChanges 
        {
            get => _isSaveChanges;

            set // При нажатии на кнопку "Изменить", появляется новая кнопка "Отменить",
                // а кнопка "Изменить" меняется на "Сохранить" и приобретает соответствующий функционал.
            {
                if (_isSaveChanges != value)
                {
                    _isSaveChanges = value;

                    if (_isSaveChanges)
                    {
                        CancelChangesButton.Visibility = Visibility.Visible;
                        ChangesButton.Content = "Сохранить";
                    }
                    else
                    {
                        CancelChangesButton.Visibility = Visibility.Hidden;
                        ChangesButton.Content = "Изменить";
                    }
                }
            }
        }


        #region Buttons Events

        // Событие нажатия на кнопку "Войти как..."
        private void Employee_Click(object sender, RoutedEventArgs e)
        {
            if (!IsManager)
            {
                PasswordWindow passW = new PasswordWindow();
                passW.Owner = this;
                
                if (passW.ShowDialog() == true) IsManager = true;
            }
            else IsManager = false;
        }

        private void ChangesButton_Click(object sender, RoutedEventArgs e)
        {
            if (!IsSaveChanges)
            {
                IsSaveChanges = true;
                SwitchClientDataTextBoxes(true);
            }
            else
            {
                SaveChanges();
            }
        }

        private void CancelChangesButton_Click(object sender, RoutedEventArgs e)
        {
            IsSaveChanges = false;
            IsAddClient = false;
            SwitchClientDataTextBoxes(false);
        }

        private void AddClientButton_Click(object sender, RoutedEventArgs e) // При нажатии на кнопку "Добавить",
                                                                             // разблокируются для изменений поля данных клиента
        {
            SwitchClientDataTextBoxes(true);
            IsAddClient = true;
            AddClientButton.IsEnabled = false;
        }

        private void Search_Click(object sender, RoutedEventArgs e)
        {
            clientsCollection.Clear();

            foreach (Client client in ClientRepository.FindClients(FIOSearchTB.Text, PhoneSearchTB.Text))
            {
                clientsCollection.Add(client);
            }
            if (clientsCollection.Count < 1) MessageBox.Show("Ни один клиент не найден.");
        }
        #endregion

        #region Other Events

        // Ограничение пользовательского ввода букв (В поля должны быть введены только цифры).
        private void CheckDigit_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key.ToString().Length == 2 && Char.IsDigit(e.Key.ToString()[1])) return;
            else e.Handled = true;
        }


        private void ClientsListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            selectedClient = (Client)ClientsListBox.SelectedItem;

            FIOTB.Text = selectedClient.FIO;
            PhoneNumberTB.Text = selectedClient.PhoneNumber;
            PassportTB.Text = selectedClient.Passport;
        }
        #endregion

        #region Custom Methods

        private void SwitchEmployee(bool isManager)
        {
            if (isManager)
            AddClientButton.IsEnabled = isManager;
        }

        private void SaveChanges()
        {
            if (IsAddClient)
            {
                StringBuilder errors = new StringBuilder(null);

                if (ClientTextBoxes[0].Text == string.Empty) errors.Append(" >>ФИО<< ");
                if (ClientTextBoxes[1].Text.Length < 2) errors.Append(" >>Номер телефона<< ");
                if (ClientTextBoxes[2].Text.Length < 10) errors.Append(" >>Паспортные данные<< ");

                if (errors.Length > 1)
                {
                    MessageBox.Show($"Поля: {errors} заполнены неверно! Смотри подсказку", "Ошибка", 
                        MessageBoxButton.OK, MessageBoxImage.Error);
                    errors = null;
                }
                else
                {
                    IsAddClient = false;
                    SwitchClientDataTextBoxes(false);
                    Client client = new Client(ClientTextBoxes[0].Text, ClientTextBoxes[1].Text, ClientTextBoxes[2].Text);
                    ClientRepository.AddClient(client);
                    MessageBox.Show("Клиент успешно добавлен в базу данных", "Успешно", MessageBoxButton.OK, MessageBoxImage.Information);
                    allClients.Add(client);
                    AddClientButton.IsEnabled = true;

                    foreach (TextBox textBox in ClientTextBoxes)
                    {
                        textBox.Text = "";
                    }
                }
            }
        }

        /// <summary>
        /// Изменение доступности внесения данных в поля данных о клиенте (Иземенение цвета, IsReadOnly)
        /// </summary>
        /// <param name="isEnabled"></param>
        private void SwitchClientDataTextBoxes(bool isEnabled)
        {
            if (IsManager || !isEnabled) // Если изменения вносит менеджер или изменения отменяются / сохраняются,
                                         // изменяются все TextBox клиента
            {
                foreach (TextBox textBox in ClientTextBoxes)
                {
                    textBox.IsReadOnly = !isEnabled;
                    textBox.Background = (Brush)TryFindResource(isEnabled ? enabledRes : disabledRes);
                }
            }
            else if (!IsManager && isEnabled) // Иначе, если изменения вносит консультант, включается только поле номера телефона клиента
            {
                TextBox phoneTB = ClientTextBoxes[1];
                phoneTB.IsReadOnly = !isEnabled;
                phoneTB.Background = (Brush)TryFindResource(enabledRes);
                
            }
        }

        #endregion



    }
}
