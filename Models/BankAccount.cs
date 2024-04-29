using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
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
        bool SendTransfer(T toAccount, float value, string message = "");
    }

    [Serializable]
    [XmlInclude(typeof(int))]
    [XmlInclude(typeof(string))]
    public abstract class BankAccount<T> : IAccount<T>, ITransferable<IAccount<T>>, INotifyPropertyChanged
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

        protected List<OperationData> Operations { get; private set; }

        public BankAccount(T id, Client owner, float balance = 0)
        {
            ID = id;
            Owner = owner;
            Balance = balance;
            Operations = new List<OperationData>();
            if (balance > 0) Operations.Add(new OperationData(Balance, "Пополнение при открытии счёта"));
        }

        public void Deposit(float amount, string description)
        {
            Balance += amount;
            Operations.Add(new OperationData(amount, description));
        }

        public void Deposit(float amount) { this.Deposit(amount, "Unknown replenishment"); }

        public string GetStringID()
        {
            if (ID is int intId) return BankAccountsRepository.IntToStringID(intId);
            else return ID.ToString();
        }

        public bool SendTransfer(IAccount<T> toAccount, float amount, string message = "")
        {
            if (toAccount != null && this.Balance - amount >= 0)
            {
                toAccount.Deposit(amount, $"Перевод от: {this.ID}; \nСообщение: {message}");
                Balance -= amount;
                Operations.Add(new OperationData(-amount, $"Перевод на счёт: {toAccount.ID}; \nСообщение: {message}"));
                return true;
            }
            return false;
        }

        public List<OperationData> GetOperationsData()
        {
            return new List<OperationData>(Operations);
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }


    //[Serializable]
    //public struct AccountID
    //{
    //    public int IntID { get; set; }

    //    public AccountID(int id) { IntID = id; }

    //    public override string ToString()
    //    {
    //        StringBuilder sb = new StringBuilder(20, 20);
    //        sb.Append(IntID);
    //        sb.Insert(0, "0", sb.Capacity - sb.Length);
    //        return sb.ToString();
    //    }
    //}

    [Serializable]
    public readonly struct OperationData
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
            return $"Описание: {Description} \n" +
                $"Изменение баланса: {(Amount > 0 ? "+" : "")}{Amount} \n" +
                $"Дата и время: {OperationDateTime}";
        }
    }
}
