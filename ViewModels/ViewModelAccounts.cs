using BankASystem.Models;
using BankASystem.Services;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.Runtime.CompilerServices;
using System.Windows;

namespace BankASystem.ViewModels
{
    public class ViewModelAccounts : INotifyPropertyChanged
    {
        private bool _isOpening = false;

        public static Window TransferWindow { get; private set; }

        public static Window TransactionsWindow { get; private set; }

        public static Client CurrentClient { get; set; }

        public string CurrentClientName
        {
            get
            {
                if (CurrentClient != null) return CurrentClient.FIO;
                else return "NoN";
            }
        }

        public static string AccountIDForTransactionsShown { get; private set; }

        public static ObservableCollection<OperationData> Transactions { get; private set; }

        #region Available Accounts 
        public string DepositAccountID
        {
            get
            {
                if (CurrentClient != null && CurrentClient.DepositAccount != null)
                {
                    return BankAccountsRepository.IntToStringID(CurrentClient.DepositAccount.ID);
                }
                else return "NoN";
            }
        }

        public string DepositAccountBalance
        {
            get
            {
                if (CurrentClient != null && CurrentClient.DepositAccount != null)
                {
                    return CurrentClient.DepositAccount.Balance.ToString("#.##");
                }
                else return "NoN";
            }
        }

        public string DepositAccountInterest
        {
            get
            {
                if (CurrentClient != null && CurrentClient.DepositAccount != null)
                {
                    return CurrentClient.DepositAccount.InterestRate.ToString("#.##");
                }
                else return "NoN";
            }
        }

        public string DepositAccountPeriod
        {
            get
            {
                if (CurrentClient != null && CurrentClient.DepositAccount != null)
                {
                    return CurrentClient.DepositAccount.InterestPeriod.ToString();
                }
                else return "NoN";
            }
        }

        public string NonDepositAccountID
        {
            get
            {
                if (CurrentClient != null && CurrentClient.NonDepositAccount != null)
                {
                    return BankAccountsRepository.IntToStringID(CurrentClient.NonDepositAccount.ID);
                }
                else return "NoN";
            }
        }

        public string NonDepositAccountBalance
        {
            get
            {
                if (CurrentClient != null && CurrentClient.NonDepositAccount != null)
                {
                    return CurrentClient.NonDepositAccount.Balance.ToString("#.##");
                }
                else return "NoN";
            }
        }
        #endregion


        #region Create New Account Properties
        private int _newAccountID;
        public string NewAccountID
        {
            get          
            { 
                _newAccountID = BankAccountsRepository.FreeID;
                return BankAccountsRepository.IntToStringID(_newAccountID);
            }
        }

        private AccountType _accountType = AccountType.NonDeposit;
        public int AccountTypeIndex 
        {
            get => (int)_accountType;
            set
            {
                switch (value)
                {
                    case 0:
                        _accountType = AccountType.Deposit;
                        break;
                    case 1:
                        _accountType = AccountType.NonDeposit;
                        break;
                    default: _accountType = AccountType.None; break;
                }

                OnPropertyChanged(nameof(DepositSettingsPanelState));
            }
        }

        private float _startBalance = 0f;
        public string StartBalance
        {
            get => _startBalance.ToString("#.##");
            set
            {
                if (double.TryParse(value, NumberStyles.Float, CultureInfo.InvariantCulture, out double temp))
                    _startBalance = (float)Math.Round(temp, 2);
                else _startBalance = 0f;

                OnPropertyChanged(nameof(StartBalance));
            }
        }

        private float _writeInterestRate = 0f;
        public string WriteInterestRate
        {
            get => _writeInterestRate.ToString("#.##");
            set
            {
                if (float.TryParse(value, NumberStyles.Float, CultureInfo.InvariantCulture, out float temp))
                {
                    if (temp < 0f) temp = 0f;
                    else if (temp > 100f) temp = 100f;
                    _writeInterestRate = (float)Math.Round(temp, 2);
                }
                else _writeInterestRate = 0f;

                OnPropertyChanged(nameof(WriteInterestRate));
            }
        }

        private string _writeInterestPeriod = "∞";
        public string WriteInterestPeriod 
        {  
            get => _writeInterestPeriod;
            set
            {
                if (!int.TryParse(value, out int temp) || temp <= 0) _writeInterestPeriod = "∞";

                else _writeInterestPeriod = temp.ToString();

                OnPropertyChanged(nameof(WriteInterestPeriod));
            }             
        }
        #endregion


        #region Elements States
        public bool OpenNewAccountButtonState { get => CurrentClient.CanAddNewBankAccount() && !_isOpening; }

        public Visibility WriteAccountPanelState { get; private set; } = Visibility.Collapsed;

        public Visibility DepositSettingsPanelState { get => _accountType == AccountType.Deposit ? Visibility.Visible : Visibility.Collapsed; }

        public Visibility DepositAccountPanelState { get => CurrentClient != null && CurrentClient.DepositAccount != null ? Visibility.Visible : Visibility.Collapsed; }

        public Visibility NonDepositAccountPanelState { get => CurrentClient != null && CurrentClient.NonDepositAccount != null ? Visibility.Visible : Visibility.Collapsed; }
        #endregion


        #region Create New Account Commands
        private RelayCommand _openNewAccount;
        public RelayCommand OpenNewAccount
        {
            get
            {
                return _openNewAccount ?? (_openNewAccount = new RelayCommand(obj =>
                {
                    _isOpening = true;
                    WriteAccountPanelState = Visibility.Visible;
                    OnPropertyChanged(nameof(OpenNewAccountButtonState));
                    OnPropertyChanged(nameof(WriteAccountPanelState));
                    OnPropertyChanged(nameof(NewAccountID));
                }));
            }
        }

        private RelayCommand _openNewAccountApply;
        public RelayCommand OpenNewAccountApply
        {
            get
            {
                return _openNewAccountApply ?? (_openNewAccountApply = new RelayCommand(obj =>
                {
                    if (TryCreateAccount())
                    {
                        ResetWritePanel();
                        UpdateAccountsPanel();
                        MessageBox.Show("Новый аккаунт успешно создан!");
                    }
                    else
                    {
                        MessageBox.Show("Ошибка при открытии счёта");
                    }
                }));
            }
        }

        private RelayCommand _openNewAccountCancel;
        public RelayCommand OpenNewAccountCancel
        {
            get
            {
                return _openNewAccountCancel ?? (_openNewAccountCancel = new RelayCommand(obj =>
                {
                    ResetWritePanel();
                }));
            }
        }
        #endregion

        private RelayCommand _addInterest;
        public RelayCommand AddInterest
        {
            get
            {
                return _addInterest ?? (_addInterest = new RelayCommand(obj =>
                {
                    if (CurrentClient != null && CurrentClient.DepositAccount != null)
                    {
                        CurrentClient.DepositAccount.AddInterest();
                        UpdateAccountsPanel();
                    }
                }));
            }
        }

        private RelayCommand _accountTransactions;
        public RelayCommand AccountTransactions
        {
            get => _accountTransactions ?? (_accountTransactions = new RelayCommand(ExecuteAccountTransactions));
        }

        private RelayCommand _closeTransactions;
        public RelayCommand CloseTransactions
        {
            get => _closeTransactions ?? (_closeTransactions = new RelayCommand(obj => TransactionsWindow.Close()));
        }

        private RelayCommand _goToTransfer;
        public RelayCommand GoToTransfer
        {
            get
            {
                return _goToTransfer ?? (_goToTransfer = new RelayCommand(ExecuteTransfer));
            }
        }

        private RelayCommand _closeAccountWindow;
        public RelayCommand CloseAccountWindow
        {
            get
            {
                return _closeAccountWindow ?? (_closeAccountWindow = new RelayCommand(obj =>
                {
                    ResetWritePanel();
                    ViewModelBase.AccountsWindow.Close();
                }));
            }
        }

        private RelayCommand _deleteAccount;
        public RelayCommand DeleteAccount
        {
            get
            {
                return _deleteAccount ?? (_deleteAccount = new RelayCommand(ExecuteDeleteAccount));
            }
        }

        private void ExecuteAccountTransactions(object parameter)
        {
            if (TryParseCommandParameter(parameter, out int accType) && CurrentClient != null)
            {
                if (accType == (int)AccountType.Deposit && CurrentClient.DepositAccount != null)
                {
                    AccountIDForTransactionsShown = BankAccountsRepository.IntToStringID(CurrentClient.DepositAccount.ID);
                    Transactions = new ObservableCollection<OperationData>(CurrentClient.DepositAccount.GetOperationsData());
                }
                else if (accType == (int)AccountType.NonDeposit && CurrentClient.NonDepositAccount != null)
                {
                    AccountIDForTransactionsShown = BankAccountsRepository.IntToStringID(CurrentClient.NonDepositAccount.ID);
                    Transactions = new ObservableCollection<OperationData>(CurrentClient.NonDepositAccount.GetOperationsData());
                }
                else return;

                TransactionsWindow = new Views.TransactionsWindow();
                TransactionsWindow.ShowDialog();
                OnPropertyChanged(nameof(AccountIDForTransactionsShown));
                OnPropertyChanged(nameof(Transactions));
            }
        }

        private void ExecuteTransfer(object parameter)
        {
            if (TryParseCommandParameter(parameter, out int accType) && CurrentClient != null)
            {

                if (accType == (int)AccountType.Deposit)
                {
                    ViewModelTransfer.FromAccount = CurrentClient.DepositAccount;
                }
                else if (accType == 1)
                {
                    ViewModelTransfer.FromAccount = CurrentClient.NonDepositAccount;
                }
                else return;

                TransferWindow = new Views.TransferWindow();
                bool? isClose = TransferWindow.ShowDialog();
                if (!isClose != null) UpdateAccountsPanel();
            }
        }

        private void ExecuteDeleteAccount(object parameter)
        {
            if (TryParseCommandParameter(parameter, out int accType))
            {
                if (MessageBox.Show("Вы уверены, что хотите удалить выбранный счёт?", "Удаление счёта",
                    MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                {
                    if (!TryDeleteAccount(accType))
                        MessageBox.Show("Не удалось удалить счёт, так как на счету есть деньги!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);

                    UpdateAccountsPanel();
                }
            }
        }

        private bool TryDeleteAccount(int accountType)
        {
            if (accountType == 0 && CurrentClient.DepositAccount != null && CurrentClient.DepositAccount.Balance == 0)
            {
                BankAccountsRepository.CloseAccount(CurrentClient, AccountType.Deposit);
                return true;
            }
            else if (accountType == 1 && CurrentClient.NonDepositAccount != null && CurrentClient.NonDepositAccount.Balance == 0)
            {
                BankAccountsRepository.CloseAccount(CurrentClient, AccountType.NonDeposit);
                return true;
            }
            return false;
        }

        private bool TryCreateAccount()
        {
            if (CurrentClient == null || _accountType == AccountType.None) return false;

            bool typeError = (CurrentClient.DepositAccount != null && _accountType == AccountType.Deposit)
                || (CurrentClient.NonDepositAccount != null && _accountType == AccountType.NonDeposit);

            bool isError = false;

            if (typeError) MessageBox.Show("У клиента уже есть счёт такого типа");
            else
            {
                if (_accountType == AccountType.Deposit)
                {                   
                    isError = !BankAccountsRepository.OpenDepositAccount(CurrentClient, _startBalance, _writeInterestRate, _writeInterestPeriod);
                }
                else if (_accountType == AccountType.NonDeposit)
                {
                    isError = !BankAccountsRepository.OpenNonDepositAccount(CurrentClient, _startBalance);
                }
            }
            return !(typeError || isError);
        }


        private bool TryParseCommandParameter(object parameter, out int outInt)
        {
            if (parameter != null && parameter is string paramStr && int.TryParse(paramStr, out outInt)) 
                return true;

            outInt = default;
            return false;
        }

        private void ResetWritePanel()
        {
            _isOpening = false;
            WriteAccountPanelState = Visibility.Collapsed;
            OnPropertyChanged(nameof(WriteAccountPanelState));
            OnPropertyChanged(nameof(OpenNewAccountButtonState));
            OnPropertyChanged(nameof(NewAccountID));
            StartBalance = "0";
            WriteInterestRate = "0";
            WriteInterestPeriod = "0";
        }

        private void UpdateAccountsPanel()
        {
            OnPropertyChanged(nameof(DepositAccountPanelState));
            OnPropertyChanged(nameof(DepositAccountID));
            OnPropertyChanged(nameof(DepositAccountBalance));
            OnPropertyChanged(nameof(DepositAccountInterest));
            OnPropertyChanged(nameof(DepositAccountPeriod));

            OnPropertyChanged(nameof(NonDepositAccountPanelState));
            OnPropertyChanged(nameof(NonDepositAccountID));
            OnPropertyChanged(nameof(NonDepositAccountBalance));
        }

        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }       
    }
}
