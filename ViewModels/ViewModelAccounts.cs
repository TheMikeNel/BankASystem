using BankASystem.Models;
using BankASystem.Services;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.Runtime.CompilerServices;
using System.Text;
using System.Windows;

namespace BankASystem
{
    public class ViewModelAccounts : INotifyPropertyChanged
    {
        private bool _isOpening = false;

        public static Client CurrentClient { get; set; }

        public string CurrentClientName
        {
            get
            {
                if (CurrentClient != null) return CurrentClient.FIO;
                else return "NoN";
            }
        }


        #region Available Accounts 
        public string DepositAccountID
        {
            get
            {
                if (CurrentClient != null && CurrentClient.DepositAccount != null)
                {
                    return CurrentClient.DepositAccount.ID.ToString();
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
                    return CurrentClient.DepositAccount.Balance.ToString();
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
                    return CurrentClient.DepositAccount.InterestRate.ToString();
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
                    return CurrentClient.NonDepositAccount.ID.ToString();
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
                    return CurrentClient.NonDepositAccount.Balance.ToString();
                }
                else return "NoN";
            }
        }
        #endregion


        #region Create New Account Properties
        private AccountID _newAccountID;
        public string NewAccountID
        {
            get          
            { 
                _newAccountID = BankAccountsRepository.GetFreeID();
                return _newAccountID.ToString();
            }
        }

        private AccountType _accountType = AccountType.None;
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
            get => _startBalance.ToString();
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
            get => _writeInterestRate.ToString();
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
                        _isOpening = false;
                        WriteAccountPanelState = Visibility.Collapsed;
                        ResetWritePanel();
                        UpdateAccountsPanel();
                        OnPropertyChanged(nameof(WriteAccountPanelState));
                        OnPropertyChanged(nameof(OpenNewAccountButtonState));
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
                    _isOpening = false;
                    WriteAccountPanelState = Visibility.Collapsed;
                    OnPropertyChanged(nameof(WriteAccountPanelState));
                    OnPropertyChanged(nameof(OpenNewAccountButtonState));
                }));
            }
        }
        #endregion

        private bool TryCreateAccount()
        {
            if (CurrentClient == null) return false;

            bool typeError = (CurrentClient.DepositAccount != null && _accountType == AccountType.Deposit)
                || (CurrentClient.NonDepositAccount != null && _accountType == AccountType.NonDeposit);

            bool isError = false;

            if (typeError) MessageBox.Show("У клиента уже есть счёт такого типа");
            else
            {
                if (_accountType == AccountType.Deposit)
                {                   
                    isError = !BankAccountsRepository.OpenDepositAccount(_newAccountID, CurrentClient, _startBalance, _writeInterestRate, _writeInterestPeriod);
                }
                else if (_accountType == AccountType.NonDeposit)
                {
                    isError = !BankAccountsRepository.OpenNonDepositAccount(_newAccountID, CurrentClient, _startBalance);
                }
            }

            return !(typeError || isError);
        }

        private void ResetWritePanel()
        {
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
