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

namespace BankASystem
{
    public class ViewModelBase : INotifyPropertyChanged
    {
        #region Fields and Properties

        Employee employee = new Consultant();

        public ObservableCollection<Client> ClientsCollection { get; set; }

        private string _clientFIO;
        public string ClientFIO
        {
            get => _clientFIO;
            set
            {
                _clientFIO = value;
                OnPropertyChanged(nameof(_clientFIO));
            }
        }

        private string _clientPhone;
        public string ClientPhone
        {
            get => _clientPhone;
            set
            {
                _clientPhone = value;
                OnPropertyChanged(nameof(_clientPhone));
            }
        }

        private string _clientPassport;
        public string ClientPassport
        {
            get => _clientPassport;
            set
            {
                _clientPassport = value;
                OnPropertyChanged(nameof(_clientPassport));
            }
        }

        private Client _selectedClient;
        public Client SelectedClient
        {
            get => _selectedClient;
            set
            {
                _selectedClient = value;
                OnPropertyChanged(nameof(SelectedClient));
                OnPropertyChanged(nameof(ChangeOrSaveBtnState));
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
                        PasswordWindow passW = new PasswordWindow("1111");

                        if (passW.ShowDialog() == true)
                            _isManager = true;

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

        public bool ChangeOrSaveBtnState // Состояние кнопки "Изменить" / "Сохранить" (вкл. - выкл.)
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

        public bool AddOrCancelBtnState // Состояние кнопки "Добавить" / "Отменить" (вкл. - выкл.)
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
                if (SelectedClient.FIO != null) return true;
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
                (_switchEmployee = new RelayCommand(obj => this.IsManager = !IsManager));
            }
        }

        private RelayCommand _changeOrSaveButtonClick;
        public RelayCommand ChangeOrSaveButtonClick
        {
            get
            {
                return _changeOrSaveButtonClick ?? (_changeOrSaveButtonClick = new RelayCommand(obj =>
                {
                    if (IsChangeClient || IsAddClient) // Кнопка выполняет функционал "Сохранить"
                    {
                        IsAddClient = false; 
                        IsChangeClient = false;
                        ChangeButtonsProperties();
                        MessageBox.Show("Save");
                    }
                    else IsChangeClient = true;
                }));
            }
        }

        private RelayCommand _addOrCancelButtonClick;
        public RelayCommand AddOrCancelButtonClick
        {
            get
            {
                return _addOrCancelButtonClick ?? (_addOrCancelButtonClick = new RelayCommand(obj =>
                {
                    if (IsAddClient || IsChangeClient) // Кнопка выполняет функционал "Отменить"
                    {
                        IsAddClient = false; 
                        IsChangeClient = false;
                        ChangeButtonsProperties();
                        MessageBox.Show("Cancel");
                    }
                    else IsAddClient = true;
                }));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion

        public ViewModelBase()
        {
            ClientsCollection = new ObservableCollection<Client>() { 
                new Client("Name", "1234", "1111222333"), new Client("Fame2", "1234", "1111222333"), new Client("agme", "1234", "1111222333") };
        }

        private void ChangeButtonsProperties()
        {
            OnPropertyChanged(nameof(ChangeOrSaveBtnState));
            OnPropertyChanged(nameof(AddOrCancelBtnState));
            OnPropertyChanged(nameof(ChangeOrSaveBtnContent));
            OnPropertyChanged(nameof(AddOrCancelBtnContent));
        }
    }
}
