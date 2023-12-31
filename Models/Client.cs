﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Xml.Serialization;

namespace BankASystem.Models
{
    [Serializable]
    public struct Client : INotifyPropertyChanged, IComparable
    {
        private string _fio;
        private string _phoneNumber;
        private string _passport;

        public event PropertyChangedEventHandler PropertyChanged;

        public string FIO 
        {
            get => _fio;
            set
            {
                _fio = value;
                OnPropertyChanged("FIO");
            }
        }

        public string PhoneNumber
        {
            get => _phoneNumber;

            set
            {
                if (value.Replace(" ", "").Length >= 2)
                {
                    _phoneNumber = value.Replace(" ", "");
                    OnPropertyChanged("PhoneNumber");
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
                if (value.Length >= 10 && value.Length <= 11 && value.Count(x => x == ' ') <= 1)
                {
                    _passport = value;
                    OnPropertyChanged("Passport");
                }
            }
        }

        public List<ChangesData> ChangesHistory { get; set; }

        public Client(string fio, string phoneNumber, string passport, ChangesData changesData)
        {
            _fio = "NaN";
            _phoneNumber = "12345678901";
            _passport = "1234567890";
            PropertyChanged = null;
            ChangesHistory = new List<ChangesData>();

            FIO = fio;
            PhoneNumber = phoneNumber;
            Passport = passport;
            if (changesData.TypesOfChanges != string.Empty) ChangesHistory.Add(changesData);
        }

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

        public void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public int CompareTo(object clientObj)
        {
            Client client = (Client)clientObj;
            return String.Compare(this.FIO, client.FIO, true);
        }

        public override string ToString() => $"{FIO}\t {PhoneNumber}\t {Passport}";
    }
}
