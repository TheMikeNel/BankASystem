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

        // Параметры для взаимодействия с выбранным клиентом
        private Client _selectedClient;
        private int _selectedClientIndex = -1;

        private bool _isManager = false;
        private bool _isAddClient = false;
        private bool _isChanges = false;
        private bool _clientIsSelected = false;

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

                    if (_isAddClient) DeselectClient(); // Если добавляется новый клиент, необходимо очистить поля ввода данных о клиенте

                    AddClientButton.IsEnabled = !_isAddClient;
                    ClientIsSelected = _isAddClient;
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
                    CheckLastChanges.IsEnabled = !_isChanges;
                    ClientsListBox.IsEnabled = !_isChanges;

                    if (_isChanges) // Если пользователь вносит новые данные, кнопка "Изменить" принимает функционал кнопки "Сохранить",
                                        // появляется кнопка отмены изменений.
                    {
                        AddClientButton.IsEnabled = false;
                        CancelChangesButton.Visibility = Visibility.Visible;
                        ChangesButton.Content = "Сохранить";
                    }
                    else
                    {
                        CancelChangesButton.Visibility = Visibility.Hidden;
                        ChangesButton.Content = "Изменить";
                    }
                    // Поля данных о клиенте принимают состояния: Чтение и запись / Только чтение
                    SwitchClientDataTextBoxes(_isChanges);                                                                                                                            
                }
            }
        }

        // Выбран ли какой-либо клиент для взаимодействия.
        public bool ClientIsSelected
        {
            get => _clientIsSelected;
            set
            {
                if ( _clientIsSelected != value)
                {
                    _clientIsSelected = value;

                    // Может ли пользователь изменять и просматривать данные о клиенте.
                    ChangesButton.IsEnabled = _clientIsSelected;
                    CheckLastChanges.IsEnabled = _clientIsSelected;
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
            SelectClient(_selectedClientIndex);
        }

        // Кнопка "Добавить".
        private void AddClientButton_Click(object sender, RoutedEventArgs e) 
        {
            if (employee is Manager)
                IsAddClient = true;
        }

        // Кнопка "Поиск" (Поиск сотрудников по ФИО и номеру)
        private void Search_Click(object sender, RoutedEventArgs e)
        {            
            UpdateClientsCollection(employee.FindClients(FIOSearchTB.Text, PhoneSearchTB.Text));

            if (clientsCollection.Count < 1) MessageBox.Show("Ни один клиент не найден.");
        }

        // Кнопка "Посмотреть последние изменения"
        private void CheckLastChanges_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show(_selectedClient.GetLastChangeString(), "Изменение");
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
                SelectClient(_selectedClientIndex);
            }

            else DeselectClient();
        }


        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            DataRepository.SerializeClientsList();
        }
        #endregion

        #region Custom Methods
        /// <summary>
        /// Поменять аккаунт текущего пользователя на менеджера (true) / консультанта (falce)
        /// </summary>
        /// <param name="isManager">Менеджер (true) / Консультант (falce)</param>
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

            // Необходимо обновлять коллекцию клиентов, т.к. у консультанта и менеджера
            // разные отображения клиентов (у консультанта нет доступа к паспортным данным)
            UpdateClientsCollection(employee.GetAllClients());
        }

        /// <summary>
        /// Проверить на ошибки пользовательский ввод данных о клиенте. Если есть ошибки, вернет true и выведет окно с ошибками.
        /// </summary>
        /// <returns></returns>
        private bool GetErrorsInChanges()
        {
            StringBuilder errors = new StringBuilder(null);

            if (ClientTextBoxes[0].Text.Replace(" ", "") == string.Empty) errors.Append(" >>ФИО<< ");
            if (ClientTextBoxes[1].Text.Replace(" ", "").Length < 2) errors.Append(" >>Номер телефона<< ");
            if (ClientTextBoxes[2].Text.Replace(" ", "").Length != 10) errors.Append(" >>Паспортные данные<< ");

            bool isError = errors.Length > 0;

            if (isError)
            {
                MessageBox.Show($"Поля: {errors} заполнены неверно / не заполнены! Смотри подсказки к вводу данных", "Ошибка",
                            MessageBoxButton.OK, MessageBoxImage.Error);
            }

            return isError;
        }

        /// <summary>
        /// Сохранить изменения клиента / Подтвердить добавление нового.
        /// </summary>
        private void SaveChanges()
        {
            if (!GetErrorsInChanges()) // Если в пользовательском вводе данных нет ошибок. Иначе выведет окно об ошибках.
            {
                if (IsAddClient) // Если добавляется клиент.
                {
                    Client client = new Client(ClientTextBoxes[0].Text, ClientTextBoxes[1].Text, ClientTextBoxes[2].Text);

                    if (!(employee as Manager).AddClient(client)) // Добавление происходит через аккаунт менеджера (Класс Manager).
                    {
                        MessageBoxAboutRepeatError();
                        return;
                    }
                    clientsCollection.Add(client);
                    IsAddClient = false;
                    SelectClient(clientsCollection.Count - 1);

                    MessageBox.Show("Клиент успешно добавлен в базу данных", "Добавление",
                        MessageBoxButton.OK, MessageBoxImage.Information);
                }
                else // Иначе, изменяются данные конкретного клиента.
                {
                    Client client = new Client(ClientTextBoxes[0].Text, ClientTextBoxes[1].Text, ClientTextBoxes[2].Text);

                    bool isChanged = false; // Переменная, с помощью которой происходит уведомление об успешном изменении / ошибке.

                    if (employee is Manager) 
                        isChanged = (employee as Manager).ChangeClient(_selectedClient, client);

                    if (employee is Consultant) 
                        isChanged = (employee as Consultant).ChangeClient(_selectedClient, client);

                    if (isChanged)
                    {
                        MessageBox.Show("Клиент успешно изменён", "Изменение",
                            MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                    else
                    {
                        MessageBoxAboutRepeatError();
                        return;
                    }

                    clientsCollection[_selectedClientIndex] = client; // Изменяемый клиент также обновляется в отображении ListBox
                    SelectClient(_selectedClientIndex);
                    IsChanges = false;
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
            else if (!IsManager && isEnabled) // Иначе, если изменения вносит консультант, включается только поле "Номер телефона" клиента
            {
                TextBox phoneTB = ClientTextBoxes[1];
                phoneTB.IsReadOnly = !isEnabled;
                phoneTB.Background = (Brush)TryFindResource(enabledRes);
            }
        }

        private void SelectClient(int clientIndex)
        {
            _selectedClient = clientsCollection[clientIndex];
            ClientIsSelected = true;

            FIOTB.Text = _selectedClient.FIO;
            PhoneNumberTB.Text = _selectedClient.PhoneNumber;
            PassportTB.Text = _selectedClient.Passport;
        }

        private void DeselectClient()
        {
            ClientIsSelected = false;

            foreach (TextBox textBox in ClientTextBoxes)
            {
                textBox.Text = "";
            }
        }

        /// <summary>
        /// Обновление коллекции клиентов, отображаемой в ListBox, в соответствии с новой передаваемой коллекцией.
        /// </summary>
        /// <param name="newCollection"></param>
        private void UpdateClientsCollection(ObservableCollection<Client> newCollection)
        {
            clientsCollection.Clear();

            foreach (Client client in newCollection)
            {
                clientsCollection.Add(client);
            }
        }

        private void MessageBoxAboutRepeatError()
        {
            string operation = "выполнить операцию";
            if (IsAddClient) operation = "добавить клиента";
            else if (IsChanges) operation = "изменить данные клиента";

            MessageBox.Show($"Не удалось {operation}! Сотрудник с таким же паспортом / номером телефона уже есть.",
                "Повторение данных", MessageBoxButton.OK, MessageBoxImage.Error);
        }

        public string GetManagerPassword() => manager.ManagerPassword;
        #endregion
    }
}
