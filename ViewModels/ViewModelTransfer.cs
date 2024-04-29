using BankASystem.Models;
using BankASystem.Services;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;


namespace BankASystem.ViewModels
{
    internal class ViewModelTransfer : INotifyPropertyChanged
    {
        public static BankAccount<int> FromAccount { get; set; }

        public string FromID
        {
            get
            {
                if (FromAccount != null)
                    return BankAccountsRepository.IntToStringID(FromAccount.ID);
                return "NoN";
            }
        }

        public string FromBalance
        {
            get
            {
                if (FromAccount != null)
                    return FromAccount.Balance.ToString();
                return "NoN";
            }
        }

        private float _amount;
        public string AmountField
        {
            get => _amount.ToString("#.##");
            set
            {
                if (float.TryParse(value, out float parsed))
                {
                    if (parsed < 0) parsed *= -1;

                    _amount = (float)Math.Round(parsed, 2);
                }
                else _amount = 0;

                OnPropertyChanged(nameof(AmountField));
            }
        }

        public string Message { private get; set; } = string.Empty;

        private ObservableCollection<BankAccount<int>> _allAccounts;
        public ObservableCollection<BankAccount<int>> AllAccounts
        {
            get
            {
                if (_allAccounts == null)
                    _allAccounts = new ObservableCollection<BankAccount<int>>(BankAccountsRepository.GetAllAccounts());
                return _allAccounts;
            }
        }

        private ObservableCollection<BankAccount<int>> _filteredAccounts;
        public ObservableCollection<BankAccount<int>> FilteredAccounts
        {
            get
            {
                if (_filteredAccounts == null)
                {
                    _filteredAccounts = new ObservableCollection<BankAccount<int>>(AllAccounts);

                    if (_filteredAccounts.Contains(FromAccount)) _filteredAccounts.Remove(FromAccount);
                }
                return _filteredAccounts;
            }

            private set
            {
                _filteredAccounts = value;
            }
        }

        public int SelectedSearchType { private get; set; } = 0;

        public string SearchField {  private get; set; }

        private BankAccount<int> _selectedAccount;
        public BankAccount<int> SelectedAccount 
        {
            get => _selectedAccount;
            set
            {
                _selectedAccount = value;
                OnPropertyChanged(nameof(ToID));
            }
        }

        public string ToID
        {
            get
            {
                if (SelectedAccount != null) return BankAccountsRepository.IntToStringID(SelectedAccount.ID);
                else return string.Empty;
            }
        }

        private RelayCommand _searchCommand;
        public RelayCommand SearchCommand
        { get => _searchCommand ?? (_searchCommand = new RelayCommand(FilterAccounts)); }

        private RelayCommand _applyTransfer;
        public RelayCommand ApplyTransfer
        { get => _applyTransfer ?? (_applyTransfer = new RelayCommand(TryApplyTransfer)); }

        private RelayCommand _cancelTransfer;
        public RelayCommand CancelTransfer
        { 
            get => _cancelTransfer ?? (_cancelTransfer = new RelayCommand(obj => ViewModelAccounts.TransferWindow.Close()));
        }

        private void FilterAccounts(object param)
        {
            ObservableCollection<BankAccount<int>> tempFiltered = new ObservableCollection<BankAccount<int>>();

            foreach(BankAccount<int> account in AllAccounts)
            {
                if (SelectedSearchType == 0) // Search by ID
                {
                    if (account.ID.ToString().ToLower().Contains(SearchField.ToLower()))
                    {
                        tempFiltered.Add(account);
                    }
                }
                else // Search by FIO
                {
                    if (account.Owner.FIO.ToLower().Contains(SearchField.ToLower()))
                    {
                        tempFiltered.Add(account);
                    }
                }
            }

            FilteredAccounts = tempFiltered;
            OnPropertyChanged(nameof(FilteredAccounts));
        }

        private void TryApplyTransfer(object param)
        {
            if (FromAccount != null && SelectedAccount != null && BankAccountsRepository.Transfer(FromAccount, SelectedAccount, _amount, Message)) 
            {
                OnPropertyChanged(nameof(FromBalance));
                MessageBox.Show($"Перевод выполнен! Сумма перевода: {_amount:#.##} руб.\nНа счёт: ID: {BankAccountsRepository.IntToStringID(SelectedAccount.ID)}; " +
                    $"ФИО: {SelectedAccount.Owner.FIO};\nОстаток на счету: {FromAccount.Balance:#.##} руб.");
                ViewModelAccounts.TransferWindow.Close();
            }
            else
            {
                MessageBox.Show("Произошла ошибка при переводе! Возможно, недостаточно средств для совершения перевода.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
