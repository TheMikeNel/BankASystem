using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text;
using System.Xml.Serialization;

namespace BankASystem.Models
{
    public enum AccountType
    {
        Deposit = 0,
        NonDeposit = 1,
        None = 99
    }

    public interface IAccount<out T>
    {
        T ID { get; }
        float Balance { get; set; }
        void Deposit(float amount, string description);
    }

    public interface ITransferable<in T>
    {
        bool SendTransfer(T toAccount, float value);
    }

    [Serializable]
    [XmlInclude(typeof(AccountID))]
    public abstract class BankAccount<T> : IAccount<T>, INotifyPropertyChanged
    {
        public abstract AccountType AccountType { get; }

        [XmlIgnore]
        public Client Owner { get; }

        public T ID { get; }

        private float _balance;
        public float Balance
        {
            get => _balance;
            set
            {
                _balance = (float)Math.Round(value, 2);

                OnPropertyChanged(nameof(Balance));
            }
        }

        protected Dictionary<int, OperationData> Operations { get; private set; }

        public BankAccount(T id, Client owner, float balance = 0)
        {
            ID = id;
            Owner = owner;
            Balance = balance;
            Operations = new Dictionary<int, OperationData>();
            if (balance > 0) Operations.Add(Operations.Count + 1, new OperationData(balance, "Open Deposit"));
        }

        public void Deposit(float amount, string description)
        {
            Balance += amount;
            Operations.Add(Operations.Count + 1, new OperationData(amount, description));
        }

        public void Deposit(float amount) { this.Deposit(amount, "Unknown replenishment"); }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    [Serializable]
    public class NonDepositAccount<T> : BankAccount<T>, ITransferable<IAccount<T>>
    {
        public override AccountType AccountType { get; } = AccountType.NonDeposit;

        /// <summary>
        /// Открыть недепозитный счёт
        /// </summary>
        /// <param name="id">Идентификатор счёта</param>
        /// <param name="balance">Начальный баланс</param>
        public NonDepositAccount(T id, Client owner, float balance = 0) : base(id, owner, balance) { }

        public NonDepositAccount() : this(default, default) { }

        public bool SendTransfer(IAccount<T> toAccount, float amount)
        {
            if (this.Balance - amount >= 0)
            {
                toAccount.Deposit(amount, $"Transfer from {this.ID}");
                Operations.Add(Operations.Count + 1, new OperationData(-amount, $"Transfer to {toAccount.ID}"));
                return true;
            }
            return false;
        }
    }

    [Serializable]
    public class DepositAccount<T> : BankAccount<T>, ITransferable<IAccount<T>>
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

        public void AccrueInterest()
        {
            this.Deposit(this.Balance * (InterestRate / 100), $"Accrual of interest on deposit");
        }

        public bool SendTransfer(IAccount<T> toAccount, float amount)
        {
            if (this.Balance - amount >= 0)
            {
                toAccount.Deposit(amount, $"Transfer from {this.ID}");
                Operations.Add(Operations.Count + 1, new OperationData(-amount, $"Transfer to {toAccount.ID}"));
                return true;
            }
            return false;
        }
    }

    [Serializable]
    public struct AccountID
    {
        public int IntID { get; set; }

        public AccountID(int id) { IntID = id; }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder(20, 20);
            sb.Append(IntID);
            sb.Insert(0, "0", sb.Capacity - sb.Length);
            return sb.ToString();
        }
    }

    [Serializable]
    public struct OperationData
    {
        public DateTime OperationDateTime { get; }
        public float Amount { get; }
        public string Description { get; }

        public OperationData (float amount, string description)
        {
            this.OperationDateTime = DateTime.Now;
            this.Amount = amount;
            this.Description = description;
        }

        public override string ToString()
        {
            return $"Описание: {Description}\n" +
                $"Изменение баланса: {(Amount > 0 ? "+" : "")}{Amount}\n" +
                $"Дата и время: {OperationDateTime}";
        }
    }
}
