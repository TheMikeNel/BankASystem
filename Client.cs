using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BankASystem
{
    [Serializable]
    public struct Client
    {
        private string _phoneNumber;
        private string _passport;

        public string FIO { get; set; }

        public string PhoneNumber
        {
            get => _phoneNumber;

            set
            {
                if (value.Length >= 2) // Устанавливается только если передается значение, больше 2 символов
                {
                    _phoneNumber = value;
                }
            }
        }

        public string Passport
        {
            get => _passport;
            set
            {
                if (value.Length >= 10 && value.Length < 11)
                {
                    _passport = value;
                }
            }
        }

        public Client(string fio, string phoneNumber, string passport)
        {
            _phoneNumber = "00";
            _passport = "0000 000000";

            FIO = fio;
            PhoneNumber = phoneNumber;
            Passport = passport;
        }
        public override string ToString() => $"{FIO}\t{PhoneNumber}\t{Passport}";
    }
}
