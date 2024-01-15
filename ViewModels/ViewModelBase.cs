using BankASystem.Services;
using BankASystem.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Documents;
using System.Windows;
using System.ComponentModel.Design;

namespace BankASystem
{
    public class ViewModelBase : INotifyPropertyChanged
    {
        #region Properties

        private Employee employee = new Consultant();
        private Manager empManager = new Manager("1111");
        private ObservableCollection<Client> _clientsCollection;
        public ObservableCollection<Client> ClientsCollection
        {
            get => _clientsCollection;
            set => _clientsCollection = value;
        }

        public int selectedIndex = 0;

        private Client _selectedClient;
        public Client SelectedClient
        {
            get => _selectedClient;
            set
            {
                _selectedClient = value;
                TBFioText = _selectedClient.FIO;
                TBPhoneText = _selectedClient.PhoneNumber;
                TBPassportText = _selectedClient.Passport;
                OnPropertyChanged(nameof(SelectedClient));
                OnPropertyChanged(nameof(ChangeOrSaveBtnState));
            }
        }

        private bool _tbFioRO = true;
        public bool TBFioRO
        {
            get => _tbFioRO;
            set
            {
                _tbFioRO = value;
                OnPropertyChanged(nameof(TBFioRO));
            }
        }

        private bool _tbPhoneRO = true;
        public bool TBPhoneRO
        {
            get => _tbPhoneRO;
            set
            {
                _tbPhoneRO = value;
                OnPropertyChanged(nameof(TBPhoneRO));
            }
        }

        private bool _tbPassportRO = true;
        public bool TBPassportRO
        {
            get => _tbPassportRO;
            set
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

        private bool _isManager = false;
        public bool IsManager
        {
            get => _isManager;
            set
            {
                if (_isManager != value)
                {
                    if (value)
                    {
                        PasswordWindow passW = new PasswordWindow(empManager.ManagerPassword);

                        if (passW.ShowDialog() == true)
                        {
                            _isManager = true;
                            employee = empManager;
                        }

                        else _isManager = false;
                    }
                    else _isManager = false;

                    OnPropertyChanged(nameof(IsManager));
                    OnPropertyChanged(nameof(EmployeeButtonContent));
                    OnPropertyChanged(nameof(AddOrCancelBtnState));
                }
            }
        }

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

                return CanChanges;
            }
        }

        public string AddOrCancelBtnContent
        {
            get
            {
                return IsChangeClient || IsAddClient ? "Отмена" : "Добавить";
            }
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

        public bool CanChanges
        {
            get
            {
                if (SelectedClient != null) return true;
                return false;
            }
        }

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
                    ChangeButtonsProperties();
                }
            }
        }
        #endregion

        #region Commands and Events

        private RelayCommand _switchEmployee;
        public RelayCommand SwitchEmployee
        {
            get
            {
                return _switchEmployee ??
                (_switchEmployee = new RelayCommand(obj => 
                { 
                    this.IsManager = !IsManager;
                    ClientsCollection = employee.GetAllClients();
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
                        ChangeTextBoxesReadOnly(false);
                    }
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
                        ChangeTextBoxesReadOnly(false);
                    }
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
            ClientsCollection = employee.GetAllClients();
        }

        private void CancelChanges()
        {
            IsAddClient = false;
            IsChangeClient = false;
            SelectedClient = _selectedClient;
            ChangeTextBoxesReadOnly(true);
        }

        private void SaveChanges()
        {

            if (!Client.CanCreateNewClient(TBFioText, TBPhoneText, TBPassportText, out StringBuilder errors))
            {
                MessageBox.Show($"Клиент не сохранён!\nВ пользовательском вводе присутствуют ошибки:\n{errors}", "Error",
                    MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            Client client = new Client(_tbFioText, _tbPhoneText, _tbPassportText);

            if (employee is Manager && IsAddClient)
            {
                MessageBox.Show("Произошло");
                if ((employee as Manager).AddClient(client))
                {
                    _clientsCollection.Add(client);
                    SelectedClient = client;
                    MessageBox.Show("Произошло норм");
                }
                else MessageBox.Show("Произошло дерьмо");
            }

            if (IsChangeClient)
            {
                _selectedClient.FIO = client.FIO;
                _selectedClient.PhoneNumber = client.PhoneNumber;
                _selectedClient.Passport = client.Passport;
                OnPropertyChanged(nameof(SelectedClient));
            }

            IsAddClient = false;
            IsChangeClient = false;

            ChangeTextBoxesReadOnly(true);
        }

        private void ChangeButtonsProperties()
        {
            OnPropertyChanged(nameof(ChangeOrSaveBtnState));
            OnPropertyChanged(nameof(ChangeOrSaveBtnContent));
            OnPropertyChanged(nameof(AddOrCancelBtnState));
            OnPropertyChanged(nameof(AddOrCancelBtnContent));
        }

        private void ChangeTextBoxesReadOnly(bool isReadOnly)
        {
            TBPhoneRO = isReadOnly;

            if (IsManager)
            {
                TBFioRO = isReadOnly;
                TBPassportRO = isReadOnly;
            }
        }
        #endregion
    }
}
