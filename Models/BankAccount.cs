using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace BankASystem.Models
{
    public interface IAccount<out T>
    {
        T ID { get; }
        double Balance { get; set; }
        void Deposit(double amount, string description);
    }

    public interface ITransferable<in T>
    {
        bool SendTransfer(T toAccount, double value);
    }

    public abstract class BankAccount<T> : IAccount<T>, INotifyPropertyChanged
    {
        Dictionary<int, OperationData> operations = new Dictionary<int, OperationData>();
        public Client Owner { get; }

        public T ID { get; }

        private double _balance;
        public double Balance
        {
            get => _balance;
            set
            {
                _balance = value;

                OnPropertyChanged(nameof(Balance));
            }
        }

        public BankAccount(T id, Client owner, double balance = 0)
        {
            ID = id;
            Owner = owner;
            Balance = balance;
        }

        public void Deposit(double amount, string description)
        {
            Balance += amount;
            operations.Add(operations.Count + 1, new OperationData(amount, description));
        }

        public void Deposit(double amount) { this.Deposit(amount, "Unknown replenishment"); }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    [Serializable]
    public class NonDepositAccount<T> : BankAccount<T>, ITransferable<IAccount<T>>
    {
        /// <summary>
        /// Открыть недепозитный счёт
        /// </summary>
        /// <param name="id">Идентификатор счёта</param>
        /// <param name="balance">Начальный баланс</param>
        public NonDepositAccount(T id, Client owner, double balance = 0) : base(id, owner, balance) { }

        public NonDepositAccount() : this(default, default) { }

        public bool SendTransfer(IAccount<T> toAccount, double amount)
        {
            if (this.Balance - amount >= 0)
            {
                toAccount.Deposit(amount, $"Transfer from {this.ID}");
                return true;
            }
            return false;
        }
    }

    [Serializable]
    public class DepositAccount<T> : BankAccount<T>, ITransferable<IAccount<T>>
    {
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
                _interestRate = value;

                OnPropertyChanged(nameof(InterestRate));
            }
        }

        /// <summary>
        /// Открыть депозитный счёт
        /// </summary>
        /// <param name="id">Идентификатор счёта</param>
        /// <param name="balance">Начальный баланс</param>
        /// <param name="interestRate">Процентная ставка (0...100%)</param>
        public DepositAccount(T id, Client owner, double balance = 0, float interestRate = 12) : base(id, owner, balance) 
        { 
            InterestRate = interestRate;
        }

        public DepositAccount() : this(default, default) { }

        public void AccrueInterest()
        {
            this.Deposit(this.Balance * (InterestRate / 100), $"Accrual of interest on deposit");
        }

        public bool SendTransfer(IAccount<T> toAccount, double amount)
        {
            if (this.Balance - amount >= 0)
            {
                toAccount.Deposit(amount, $"Transfer from {this.ID}");
                return true;
            }
            return false;
        }
    }

    [Serializable]
    public struct OperationData
    {
        public DateTime DateTime { get; }
        public double Amount { get; }
        public string Description { get; }

        public OperationData (double amount, string description)
        {
            this.DateTime = DateTime.Now;
            this.Amount = amount;
            this.Description = description;
        }
    }
}
