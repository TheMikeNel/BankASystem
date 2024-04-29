using BankASystem.Services;
using BankASystem.Models;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text;
using System.Windows;
using BankASystem.Views;

namespace BankASystem.ViewModels
{
    public class ViewModelBase : INotifyPropertyChanged
    {
        #region Main Properties
        public static Window AccountsWindow { get; private set; }

        private Manager empManager = new Manager("1111");
        internal static Employee Emp { get; private set; } = new Consultant(); // Используется в PassportValueCodingConverter

        private ObservableCollection<Client> _clientsCollection;
        public ObservableCollection<Client> ClientsCollection
        {
            get => _clientsCollection;
            set
            {
                _clientsCollection = value;
                OnPropertyChanged(nameof(ClientsCollection));
            }
        }

        private Client _selectedClient;
        public Client SelectedClient
        {
            get => _selectedClient;
            set
            {
                if (value != null)
                {
                    _selectedClient = value;
                    TBFioText = _selectedClient.FIO;
                    TBPhoneText = _selectedClient.PhoneNumber;
                    TBPassportText = _selectedClient.Passport;
                }
                OnPropertyChanged(nameof(SelectedClient));
                OnPropertyChanged(nameof(ClientIsSelected));
                OnPropertyChanged(nameof(ChangeOrSaveBtnState));
                OnPropertyChanged(nameof(OpenBankAccountsState));
            }
        }

        public bool ClientIsSelected
        {
            get
            {
                if (SelectedClient != null) return true;
                return false;
            }
        } // Используется в свойстве IsEnabled кнопок "Изменить" и "Посмотреть последние изменения"

        private bool _isChangeClient = false;
        public bool IsChangeClient // Происходит изменение данных клиента
        {
            get => _isChangeClient;
            set
            {
                if (_isChangeClient != value)
                {
                    _isChangeClient = value;
                    OnPropertyChanged(nameof(IsChangeClient));
                    OnPropertyChanged(nameof(LVIsEnabled));
                    ChangeButtonsProperties();
                }
            }
        }

        private bool _isAddClient = false;
        public bool IsAddClient // Происходит добавление нового клиента
        {
            get => _isAddClient;
            set
            {
                if (_isAddClient != value)
                {
                    _isAddClient = value;
                    OnPropertyChanged(nameof(IsAddClient));
                    OnPropertyChanged(nameof(LVIsEnabled));
                    ChangeButtonsProperties();
                }
            }
        }

        private bool _isManager = false;
        public bool IsManager
        {
            get => _isManager;
            private set
            {
                if (_isManager != value)
                {
                    if (value) // Если устанавливается в true - необходим ввод пароля менеджера
                    {
                        PasswordWindow passW = new PasswordWindow(empManager.ManagerPassword);

                        if (passW.ShowDialog() == true)
                        {
                            _isManager = true;
                            Emp = empManager;
                        }
                    }

                    else
                    {
                        _isManager = false;
                        Emp = new Consultant();
                    }

                    OnPropertyChanged(nameof(IsManager));
                    OnPropertyChanged(nameof(EmployeeButtonContent));
                    OnPropertyChanged(nameof(AddOrCancelBtnState));
                    OnPropertyChanged(nameof(Emp));
                    ClientsCollection = Emp.GetAllClients();
                    OnPropertyChanged(nameof(TBPassportText));
                }
            }
        }

        #endregion

        #region TextBox Properties

        private bool _tbFioRO = true; // RO - IsReadOnly
        public bool TBFioRO
        {
            get => _tbFioRO;
            private set
            {
                _tbFioRO = value;
                OnPropertyChanged(nameof(TBFioRO));
            }
        }

        private bool _tbPhoneRO = true;
        public bool TBPhoneRO
        {
            get => _tbPhoneRO;
            private set
            {
                _tbPhoneRO = value;
                OnPropertyChanged(nameof(TBPhoneRO));
            }
        }

        private bool _tbPassportRO = true;
        public bool TBPassportRO
        {
            get => _tbPassportRO;
            private set
            {
                _tbPassportRO = value;
                OnPropertyChanged(nameof(TBPassportRO));
            }
        }

        private string _tbFioText;
        public string TBFioText
        {
            get => _tbFioText;
            set
            {
                _tbFioText = value;
                OnPropertyChanged(nameof(TBFioText));
            }
        }

        private string _tbPhoneText;
        public string TBPhoneText
        {
            get => _tbPhoneText;
            set
            {
                _tbPhoneText = value;
                OnPropertyChanged(nameof(TBPhoneText));
            }
        }

        private string _tbPassportText;
        public string TBPassportText
        {
            get => _tbPassportText;
            set
            {
                _tbPassportText = value;
                OnPropertyChanged(nameof(TBPassportText));
            }
        }

        private string _tbFioSearch;
        public string TBFioSearch
        {
            get => _tbFioSearch;
            set
            {
                _tbFioSearch = value;
            }
        }

        private string _tbPhoneSearch;
        public string TBPhoneSearch
        {
            get => _tbPhoneSearch;
            set
            {
                _tbPhoneSearch = value;
            }
        }

        #endregion

        #region Button and Other Properties

        public string EmployeeButtonContent
        {
            get => IsManager ? "Войти как клиент" : "Войти как менеджер";
        }

        public string ChangeOrSaveBtnContent
        {
            get => IsChangeClient || IsAddClient ? "Сохранить" : "Изменить";
        }

        public bool ChangeOrSaveBtnState // Состояние кнопки "Изменить/Сохранить" (вкл. - выкл.)
        {
            get
            {
                if (IsChangeClient || IsAddClient)
                {
                    return true;
                }

                return ClientIsSelected;
            }
        }

        public string AddOrCancelBtnContent
        {
            get =>IsChangeClient || IsAddClient ? "Отмена" : "Добавить";
        }

        public bool AddOrCancelBtnState // Состояние кнопки "Добавить/Отменить" (вкл. - выкл.)
        {
            get
            {
                if (IsChangeClient || IsAddClient)
                {
                    return true;
                }

                return IsManager;
            }
        }

        public bool LVIsEnabled
        {
            get => !(IsAddClient || IsChangeClient);
        } // Свойство ListView IsEnabled 

        public bool OpenBankAccountsState
        {
            get
            {
                return SelectedClient != null && !IsAddClient && !IsChangeClient;
            }
        }
        #endregion


        #region Commands and Events
        private RelayCommand _switchEmployee;
        public RelayCommand SwitchEmployee
        {
            get
            {
                return _switchEmployee ?? (_switchEmployee = new RelayCommand(obj =>
                {
                    this.IsManager = !IsManager;
                    SwitchTextBoxesReadOnly(true);
                    SelectedClient = _selectedClient;
                    IsAddClient = false;
                    IsChangeClient = false;
                }));
            }
        }

        private RelayCommand _changeOrSaveButtonClick;
        public RelayCommand ChangeOrSaveButtonClick
        {
            get
            {
                return _changeOrSaveButtonClick ?? 
                (_changeOrSaveButtonClick = new RelayCommand(obj =>
                {
                    if (IsChangeClient || IsAddClient) // Кнопка выполняет функционал "Сохранить"
                    {
                        SaveChanges();
                    }
                    else // Иначе, "Изменить"
                    {
                        IsChangeClient = true;
                        SwitchTextBoxesReadOnly(false);
                    }
                    OnPropertyChanged(nameof(OpenBankAccountsState));
                }));
            }
        }

        private RelayCommand _addOrCancelButtonClick;
        public RelayCommand AddOrCancelButtonClick
        {
            get
            {
                return _addOrCancelButtonClick ?? 
                (_addOrCancelButtonClick = new RelayCommand(obj =>
                {
                    if (IsAddClient || IsChangeClient) // Кнопка выполняет функционал "Отменить"
                    {                     
                        CancelChanges();
                    }
                    else // Иначе, "Добавить"
                    {
                        IsAddClient = true;
                        TBFioText = "";
                        TBPhoneText = "";
                        TBPassportText = "";
                        SwitchTextBoxesReadOnly(false);
                    }
                    OnPropertyChanged(nameof(OpenBankAccountsState));
                }));
            }
        }

        private RelayCommand _search;
        public RelayCommand Search
        {
            get
            {
                return _search ??
                (_search = new RelayCommand(obj =>
                {
                    ClientsCollection = Emp.FindClients(TBFioSearch, TBPhoneSearch);
                }));
            }
        }

        private RelayCommand _getLastChanges;
        public RelayCommand GetLastChanges
        {
            get
            {
                return _getLastChanges ??
                (_getLastChanges = new RelayCommand(obj =>
                {
                    MessageBox.Show(SelectedClient.GetLastChangeString());
                }));
            }
        }

        private RelayCommand _openBankAccountWindow;
        public RelayCommand OpenBankAccountWindow
        {
            get
            {
                return _openBankAccountWindow ??
                (_openBankAccountWindow = new RelayCommand(obj =>
                {
                    ViewModelAccounts.CurrentClient = SelectedClient;
                    AccountsWindow = new BankAccountWindow();
                    AccountsWindow.ShowDialog();
                }));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion


        #region Methods
        public ViewModelBase()
        {
            ClientsCollection = Emp.GetAllClients();
        }

        private void CancelChanges()
        {
            IsAddClient = false;
            IsChangeClient = false;
            SelectedClient = _selectedClient;
            SwitchTextBoxesReadOnly(true);
        }

        private void SaveChanges()
        {
            // Проверка на ошибки ввода при сохранении
            if (!Client.CanCreateNewClient(TBFioText, TBPhoneText, TBPassportText, out StringBuilder errors))
            {
                MessageBox.Show($"Клиент не сохранён!\nВ пользовательском вводе присутствуют ошибки:\n{errors}", "Error",
                    MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            Client client = new Client(_tbFioText, _tbPhoneText, _tbPassportText);

            if (Emp is Manager && IsAddClient) // Добавление нового клиента
            {
                if ((Emp as Manager).AddClient(client))
                {
                    ClientsCollection = Emp.GetAllClients();
                    SelectedClient = client;
                }
                else
                {
                    MessageBox.Show($"Клиент не сохранён!\nКлиент с таким номером телефона / паспортом уже есть!", "Error",
                    MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
            }

            if (IsChangeClient) // Сохранение данных
            {
                if (!Emp.ChangeClient(SelectedClient, client))
                {
                    MessageBox.Show($"Клиент не сохранён!\nКлиент с таким номером телефона / паспортом уже есть!", "Error",
                        MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                OnPropertyChanged(nameof(SelectedClient));
            }

            IsAddClient = false;
            IsChangeClient = false;
            SwitchTextBoxesReadOnly(true);
        }

        private void ChangeButtonsProperties()
        {
            OnPropertyChanged(nameof(ChangeOrSaveBtnState));
            OnPropertyChanged(nameof(ChangeOrSaveBtnContent));
            OnPropertyChanged(nameof(AddOrCancelBtnState));
            OnPropertyChanged(nameof(AddOrCancelBtnContent));
        }

        private void SwitchTextBoxesReadOnly(bool isReadOnly)
        {
            TBPhoneRO = isReadOnly;

            if (IsManager || isReadOnly)
            {
                TBFioRO = isReadOnly;
                TBPassportRO = isReadOnly;
            }
        }

        #endregion
    }
}
