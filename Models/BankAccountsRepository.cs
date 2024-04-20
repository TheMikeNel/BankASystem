using BankASystem.Services;
using System.Collections.Generic;

namespace BankASystem.Models
{
    public static class BankAccountsRepository
    {
        private static string _xmlPath = "AccountsStorage";
        private static Serializer<SerializableDictionary<int, BankAccount<int>>> _serializer = new Serializer<SerializableDictionary<int, BankAccount<int>>>();

        private static SerializableDictionary<int, BankAccount<int>> _accounts;
        public static SerializableDictionary<int, BankAccount<int>> Accounts
        {
            get
            {
                if (_accounts == null)
                {
                    if (!_serializer.TryDeserializeData(out _accounts, _xmlPath) || _accounts == null)
                       _accounts = new SerializableDictionary<int, BankAccount<int>>();
                }
                return _accounts;
            }

            private set { _accounts = value; }
        }

        public static bool OpenNonDepositAccount(int id, Client owner, double initialBalance)
        {
            BankAccount<int> account = new NonDepositAccount<int>(id, owner, initialBalance);
            if (owner.AddNewBankAccount(account))
            {
                Accounts.Add(id, account);
                return true;
            }
            return false;          
        }

        public static bool OpenDepositAccount(int id, Client owner, double initialBalance, float interestRate)
        {
            BankAccount<int> account = new DepositAccount<int>(id, owner, initialBalance, interestRate);
            if (owner.AddNewBankAccount(account))
            {
                Accounts.Add(id, account);
                return true;
            }
            return false;
        }

        public static bool Transfer(ITransferable<IAccount<int>> fromAccount, IAccount<int> toAccount, double amount)
        {
            return fromAccount.SendTransfer(toAccount, amount);
        }

        public static BankAccount<int> GetAccountByID(int id)
        {
            if (Accounts.ContainsKey(id)) return Accounts[id];
            else return null;
        }

        public static bool HasAccountByID(int id)
        {
            return Accounts.ContainsKey(id);
        }

        public static void SaveAccounts()
        {
            _serializer.SerializeData(Accounts, _xmlPath);
        }
    }
}
