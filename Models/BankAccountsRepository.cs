using BankASystem.Services;

namespace BankASystem.Models
{
    public static class BankAccountsRepository
    {
        private static string _xmlPath = "AccountsStorage";
        private static Serializer<SerializableDictionary<int, BankAccount<AccountID>>> _serializer = new Serializer<SerializableDictionary<int, BankAccount<AccountID>>>();

        private static SerializableDictionary<int, BankAccount<AccountID>> _accounts;
        public static SerializableDictionary<int, BankAccount<AccountID>> Accounts
        {
            get
            {
                if (_accounts == null)
                {
                    if (!_serializer.TryDeserializeData(out _accounts, _xmlPath) || _accounts == null)
                       _accounts = new SerializableDictionary<int, BankAccount<AccountID>>();
                }
                return _accounts;
            }

            private set { _accounts = value; }
        }

        public static AccountID GetFreeID()
        {
            return new AccountID(Accounts.Count + 1);
        }

        public static bool OpenNonDepositAccount(AccountID id, Client owner, float initialBalance)
        {
            BankAccount<AccountID> account = new NonDepositAccount<AccountID>(id, owner, initialBalance);
            if (!Accounts.ContainsKey(id.IntID) && owner.AddNewBankAccount(account))
            {
                Accounts.Add(id.IntID, account);
                return true;
            }
            return false;          
        }

        public static bool OpenDepositAccount(AccountID id, Client owner, float initialBalance, float interestRate, string interestPeriod)
        {
            BankAccount<AccountID> account = new DepositAccount<AccountID>(id, owner, initialBalance, interestRate, interestPeriod);
            if (!Accounts.ContainsKey(id.IntID) && owner.AddNewBankAccount(account))
            {
                Accounts.Add(id.IntID, account);
                return true;
            }
            return false;
        }

        public static bool Transfer(ITransferable<IAccount<AccountID>> fromAccount, IAccount<AccountID> toAccount, float amount)
        {
            return fromAccount.SendTransfer(toAccount, amount);
        }

        public static BankAccount<AccountID> GetAccountByID(int id)
        {
            if (HasAccountID(id)) return Accounts[id];
            else return null;
        }

        public static bool HasAccountID(int id)
        {
            return Accounts.ContainsKey(id);
        }

        public static void SaveAccounts()
        {
            _serializer.SerializeData(Accounts, _xmlPath);
        }
    }
}
