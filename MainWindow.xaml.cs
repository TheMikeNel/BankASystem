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
        Employee employee = new Consultant(); // Режим пользователя. По умолчанию - консультант.
        Manager manager = new Manager("1111"); // Аккаунт менеджера. Передается в режим пользователя при успешном вхождении.
        ObservableCollection<Client> clientsCollection; // Коллекция клиентов, отображаемая в ListBox.
        List<TextBox> ClientTextBoxes = new List<TextBox>(); // Поля ввода / вывода данных о выбранном клиенте (ФИО, номер телефона, паспорт)

        public MainWindow()
        {
            InitializeComponent();

            clientsCollection = new ObservableCollection<Client>(employee.GetAllClients());
            ClientsListBox.ItemsSource = clientsCollection;

            ClientTextBoxes.Add(FIOTB);
            ClientTextBoxes.Add(PhoneNumberTB);
            ClientTextBoxes.Add(PassportTB);
        }

        // Ресурсы цветов включенных / выключенных элементов TextBox.
        static string enabledRes = "EnabledRColor";
        static string disabledRes = "DisabledColor";

        private Client _selectedClient;
        private int _selectedClientIndex = -1;

        private bool _isManager = false;
        private bool _isAddClient = false;
        private bool _isChanges = false;
        private bool _isCanChanges = false;

        // Является ли пользователь менеджером. При изменении, вызывает метод SwitchEmployee(bool)
        public bool IsManager
        {
            get => _isManager;

            set
            {
                if (_isManager != value)
                {
                    _isManager = value;
                    SwitchEmployee(_isManager);
                }
            }
        }

        // Добавляет ли пользователь нового клиента. Если "Да", то кнопка "Добавить" выключается,
        // добавляются функции "Сохранить" и "Отменить". И наоборот.
        public bool IsAddClient
        {
            get => _isAddClient;
            set
            {
                if (_isAddClient != value)
                {
                    _isAddClient = value;
                    if (_isAddClient) DeselectClient();
                    AddClientButton.IsEnabled = !_isAddClient;
                    IsCanChanges = _isAddClient;
                    IsChanges = _isAddClient;
                }
            }
        }

        // Вносит ли пользователь новые данные.
        public bool IsChanges 
        {
            get => _isChanges;

            set
            {
                if (_isChanges != value)
                {
                    _isChanges = value;

                    if (_isChanges) // Если пользователь вносит новые данные, кнопка "Изменить" принимает функционал кнопки "Сохранить",
                                        // появляется кнопка отмены изменений.
                    {
                        CancelChangesButton.Visibility = Visibility.Visible;
                        ChangesButton.Content = "Сохранить";
                    }
                    else
                    {
                        CancelChangesButton.Visibility = Visibility.Hidden;
                        ChangesButton.Content = "Изменить";
                    }

                    SwitchClientDataTextBoxes(_isChanges); // Поля данных о клиенте принимают состояния:
                                                               // Чтение и запись;
                                                               // Только чтение
                }
            }
        }

        // Может ли пользователь изменять данные о клиенте.
        public bool IsCanChanges
        {
            get => _isCanChanges;
            set
            {
                if ( _isCanChanges != value)
                {
                    _isCanChanges = value;

                    ChangesButton.IsEnabled = _isCanChanges;
                }
            }
        }

        #region Buttons Events
        // Кнопка "Войти как..."
        private void Employee_Click(object sender, RoutedEventArgs e)
        {
            if (!IsManager) // Если "Войти как менеджер", требуется ввести пароль.
            {
                PasswordWindow passW = new PasswordWindow();
                passW.Owner = this;
                
                if (passW.ShowDialog() == true) IsManager = true;
            }
            else IsManager = false;
        }

        // Кнопка "Изменить" (Также, принимает функционал кнопки "Сохранить").
        private void ChangesButton_Click(object sender, RoutedEventArgs e) 
        {
            if (!IsChanges) IsChanges = true;
            else SaveChanges();
        }

        // Кнопка "Отменить" (изменения).
        private void CancelChangesButton_Click(object sender, RoutedEventArgs e)
        {
            IsAddClient = false;
            IsChanges = false;
            SelectClient(_selectedClient);
        }

        // Кнопка "Добавить".
        private void AddClientButton_Click(object sender, RoutedEventArgs e) 
        {
            IsAddClient = true;
        }

        // Кнопка "Поиск" (Поиск сотрудников по ФИО и номеру)
        private void Search_Click(object sender, RoutedEventArgs e)
        {
            clientsCollection = employee.FindClients(FIOSearchTB.Text, PhoneSearchTB.Text);

            if (clientsCollection.Count < 1) MessageBox.Show("Ни один клиент не найден.");
        }
        #endregion

        #region Other Events
        // Ограничение пользовательского ввода букв (В поля с данным событием должны быть введены только цифры).
        private void CheckDigit_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key >= Key.D0 && e.Key <= Key.D9) return;
            else e.Handled = true;
        }

        // Выбор клиента из списка найденных
        private void ClientsListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (ClientsListBox.SelectedItem != null)
            {
                _selectedClientIndex = ClientsListBox.SelectedIndex;
                SelectClient((Client)ClientsListBox.SelectedItem);
            }

            else DeselectClient();
        }
        #endregion

        #region Custom Methods

        private void SwitchEmployee(bool isManager)
        {
            IsChanges = false;
            AddClientButton.IsEnabled = isManager;
            DeselectClient();

            if (isManager)
            {
                employee = manager;
                EmployeeButton.Content = "Войти как консультант";
            }
            else
            {
                employee = new Consultant();
                EmployeeButton.Content = "Войти как менеджер";
            }

            clientsCollection = employee.GetAllClients(); // Необходимо заново получить коллекцию клиентов, т.к. у консультанта и менеджера
                                                          // разные отображения клиентов (у консультанта нет доступа к паспортным данным)
            ClientsListBox.ItemsSource = clientsCollection;
        }

        /// <summary>
        /// Проверить на ошибки пользовательский ввод данных о клиенте. Если есть ошибки, вернет true и выведет окно с ошибками.
        /// </summary>
        /// <returns></returns>
        private bool GetErrorsInChanges()
        {
            StringBuilder errors = new StringBuilder(null);

            if (ClientTextBoxes[0].Text == string.Empty) errors.Append(" >>ФИО<< ");
            if (ClientTextBoxes[1].Text.Length < 2) errors.Append(" >>Номер телефона<< ");
            if (ClientTextBoxes[2].Text.Length < 10) errors.Append(" >>Паспортные данные<< ");

            bool isError = errors.Length > 0;

            if (isError)
            {
                MessageBox.Show($"Поля: {errors} заполнены неверно! Смотри подсказки к вводу данных", "Ошибка",
                            MessageBoxButton.OK, MessageBoxImage.Error);
            }

            return isError;
        }

        private void SaveChanges()
        {
            if (!GetErrorsInChanges())
            {
                if (IsAddClient)
                {
                    Client client = new Client(ClientTextBoxes[0].Text, ClientTextBoxes[1].Text, ClientTextBoxes[2].Text);
                    (employee as Manager)?.AddClient(client);
                    clientsCollection.Add(client);
                    IsAddClient = false;

                    MessageBox.Show("Клиент успешно добавлен в базу данных", "Добавление",
                        MessageBoxButton.OK, MessageBoxImage.Information);
                }
                else
                {
                    Client client = new Client(ClientTextBoxes[0].Text, ClientTextBoxes[1].Text, ClientTextBoxes[2].Text);
                    clientsCollection[_selectedClientIndex] = client;
                    bool isChanged = employee.ChangeClient(_selectedClient, client);
                    IsChanges = false;

                    MessageBox.Show(isChanged ? "Клиент успешно изменён" : "Ошибка", "Изменение",
                        MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
        }

        /// <summary>
        /// Изменение состояния полей данных о клиенте - Чтение и запись / Только чтение (Иземенение цвета, изменение IsReadOnly)
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

        private void SelectClient(Client client)
        {
            _selectedClient = client;
            IsCanChanges = true;

            FIOTB.Text = _selectedClient.FIO;
            PhoneNumberTB.Text = _selectedClient.PhoneNumber;
            PassportTB.Text = _selectedClient.Passport;
        }

        private void DeselectClient()
        {
            IsCanChanges = false;

            foreach (TextBox textBox in ClientTextBoxes)
            {
                textBox.Text = "";
            }
        }

        public string GetManagerPassword() => manager.ManagerPassword;
        #endregion
    }
}
