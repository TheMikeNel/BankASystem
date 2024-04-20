using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Xml.Serialization;

namespace BankASystem.Models
{
    [Serializable]
    public class Client : INotifyPropertyChanged
    {
        private string _fio;
        private string _phoneNumber;
        private string _passport;

        [XmlElement("DepositAccount")]
        public int DepositAccountSerialization
        {
            get
            { 
                if (DepositAccount != null) return DepositAccount.ID;
                else return -1;
            }
            set
            {
                if (value > -1 && BankAccountsRepository.HasAccountByID(value))
                {
                    BankAccount<int> account = BankAccountsRepository.GetAccountByID(value);
                    if (account is DepositAccount<int>) DepositAccount = (DepositAccount<int>)account;
                }                 
            }
        }
        [XmlIgnore]
        public DepositAccount<int> DepositAccount { get; private set; }

        [XmlElement("NonDepositAccount")]
        public int NonDepositAccountSerialization
        {
            get
            {
                if (NonDepositAccount != null) return NonDepositAccount.ID;
                else return -1;
            }
            set
            {
                if (value > -1 && BankAccountsRepository.HasAccountByID(value))
                {
                    BankAccount<int> account = BankAccountsRepository.GetAccountByID(value);
                    if (account is NonDepositAccount<int>) NonDepositAccount = (NonDepositAccount<int>)account;
                }
            }
        }
        [XmlIgnore]
        public NonDepositAccount<int> NonDepositAccount { get; private set; }

        public string FIO 
        {
            get => _fio;
            set
            {
                if (!string.IsNullOrEmpty(value))
                {
                    _fio = value;
                    OnPropertyChanged(nameof(FIO));
                }
            }
        }

        public string PhoneNumber
        {
            get => _phoneNumber;

            set
            {
                if (!string.IsNullOrEmpty(value))
                {
                    if (value.Replace(" ", "").Length >= 2)
                    {
                        _phoneNumber = value.Replace(" ", "");
                        OnPropertyChanged(nameof(PhoneNumber));
                    }
                }
            }
        }

        public string Passport
        {
            get
            {
                if (_passport != null)
                {
                    if (_passport.Contains(' '))
                        _passport = _passport.Replace(" ", "");

                    if (_passport.Length == 10)
                        return $"{_passport.Substring(0, 4)} {_passport.Substring(4, 6)}";
                }
                return _passport;
            }
            set
            {
                if (!string.IsNullOrEmpty(value))
                {
                    if (value.Length >= 10 && value.Length <= 11 && value.Count(x => x == ' ') <= 1)
                    {
                        _passport = value;
                        OnPropertyChanged(nameof(Passport));
                    }
                }
            }
        }

        public List<ChangesData> ChangesHistory { get; set; }

        public Client()
        {
            _fio = "NaN";
            _phoneNumber = "12345678901";
            _passport = "1234567890";
            PropertyChanged = null;
            ChangesHistory = new List<ChangesData>();
        }

        public Client(string fio, string phoneNumber, string passport, List<ChangesData> changesHistory)
        {
            FIO = fio;
            PhoneNumber = phoneNumber;
            Passport = passport;
            ChangesHistory = changesHistory;
        }

        public Client(string fio, string phoneNumber, string passport, ChangesData changesData)
            : this(fio, phoneNumber, passport, new List<ChangesData> { changesData }) { }

        public Client(string fio, string phoneNumber, string passport) 
            : this(fio, phoneNumber, passport, new ChangesData()) { }

        public void AddChangesData(ChangesData changesData)
        {
            ChangesHistory.Add(changesData);
        }

        public string GetLastChangeString()
        {
            return ChangesHistory.Last().ToString();
        }

        public bool AddNewBankAccount(IAccount<int> newAccount)
        {
            if (newAccount != null)
            {
                if (newAccount is DepositAccount<int> && DepositAccount == null)
                {
                    DepositAccount = (DepositAccount<int>)newAccount;
                    return true;
                }
                else if (newAccount is NonDepositAccount<int> && NonDepositAccount == null)
                {
                    NonDepositAccount = (NonDepositAccount<int>)newAccount;
                    return true;
                }
            }
            return false;
        }

        public static bool CanCreateNewClient(string fio, string phone, string passport, out StringBuilder errors)
        {
            StringBuilder errorsSB = new StringBuilder(32);
            if (string.IsNullOrEmpty(fio) || fio.Length < 1)
                errorsSB.Append("ФИО\n");
            if (string.IsNullOrEmpty(phone) || phone.Length < 2)
                errorsSB.Append("Номер телефона\n");
            if (string.IsNullOrEmpty(passport) || passport.Length > 11 || passport.Length < 10 || passport.Count(char.IsDigit) != 10 || passport.Count(x => x == ' ') > 1)
                errorsSB.Append("Паспорт\n");

            errors = errorsSB;

            if (errors.Length > 0)
            {
                return false;
            }
            return true;
        }

        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public override string ToString() => $"{FIO}\t{PhoneNumber}\t{Passport}";
    }
}
