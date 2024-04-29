using System;
using System.Xml.Serialization;

namespace BankASystem.Models
{
    [Serializable]
    [XmlInclude(typeof(int))]
    public class DepositAccount<T> : BankAccount<T>
    {
        public override AccountType AccountType { get; } = AccountType.Deposit;

        private float _interestRate = 0;
        public float InterestRate
        {
            get
            {
                return _interestRate;
            }
            set
            {
                if (value < 0) value = 0;
                if (value > 100) value = 100;
                _interestRate = (float)Math.Round(value, 2);

                OnPropertyChanged(nameof(InterestRate));
            }
        }

        public string InterestPeriod { get; private set; }

        /// <summary>
        /// Открыть депозитный счёт
        /// </summary>
        /// <param name="id">Идентификатор счёта</param>
        /// <param name="balance">Начальный баланс</param>
        /// <param name="interestRate">Процентная ставка (0...100%)</param>
        public DepositAccount(T id, Client owner, float balance = 0, float interestRate = 12, string interestPeriod = "∞") : base(id, owner, balance)
        {
            InterestRate = interestRate;
            InterestPeriod = interestPeriod;
        }

        public DepositAccount() : this(default, default) { }

        public void AddInterest()
        {
            this.Deposit(this.Balance * (InterestRate / 100), $"Accrual of interest on deposit");
        }
    }

}
