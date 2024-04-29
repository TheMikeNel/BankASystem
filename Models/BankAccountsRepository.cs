using BankASystem.Services;
using System;
using System.Collections.Generic;
using System.Security.Principal;
using System.Text;

namespace BankASystem.Models
{
    public static class BankAccountsRepository
    {
        private static readonly string _xmlFreeIDPath = "FreeID.xml";
        private static readonly string _xmlPath = "AccountsStorage.xml";
        private static Serializer<string> _freeIDSerializer = new Serializer<string>();
        private static Serializer<HashSet<BankAccount<int>>> _hashSerializer = new Serializer<HashSet<BankAccount<int>>>();

        private static int _freeID = -1;
        public static int FreeID
        {
            get
            {
                if (_freeID < 1)
                {
                    if (!(_freeIDSerializer.TryDeserializeData(out string freeID, _xmlFreeIDPath) && int.TryParse(freeID, out _freeID)))
                        _freeID = 1;
                }

                if (IDs.Contains(_freeID))
                {
                    while (IDs.Contains(++_freeID))
                    {
                        _freeID = new Random().Next(_freeID, int.MaxValue);
                    }
                }

                return _freeID;
            }
            private set { _freeID = value; }
        }

        private static HashSet<BankAccount<int>> _accounts;
        public static HashSet<BankAccount<int>> Accounts
        {
            get
            {
                if (_accounts == null)
                {
                    if (!_hashSerializer.TryDeserializeData(out _accounts, _xmlPath) || _accounts == null)
                       _accounts = new HashSet<BankAccount<int>>();
                }
                return _accounts;
            }

            private set { _accounts = value; }
        }

        public static HashSet<int> _IDs;
        public static HashSet<int> IDs
        {
            get
            {
                if (_IDs == null)
                {
                    _IDs = new HashSet<int>();

                    foreach (BankAccount<int> account in Accounts)
                    {
                        if (!_IDs.Add(account.ID))
                        {
                            throw new Exception("Обнаружены повторения ID десериализованных счетов!");
                        }
                    }
                }
                return _IDs;
            }

            private set { _IDs = value; }
        }

        public static bool OpenNonDepositAccount(Client owner, float initialBalance)
        {
            BankAccount<int> account = new NonDepositAccount<int>(FreeID, owner, initialBalance);
            if (owner.AddNewBankAccount(account) && Accounts.Add(account) && IDs.Add(FreeID))
            {
                _freeID++;
                return true;
            }
            return false;
        }

        public static bool OpenDepositAccount(Client owner, float initialBalance, float interestRate, string interestPeriod)
        {
            BankAccount<int> account = new DepositAccount<int>(FreeID, owner, initialBalance, interestRate, interestPeriod);
            if (owner.AddNewBankAccount(account) && Accounts.Add(account) && IDs.Add(FreeID))
            {
                _freeID++;
                return true;
            }
            return false;
        }

        public static bool Transfer(ITransferable<IAccount<int>> fromAccount, IAccount<int> toAccount, float amount, string message = "")
        {
            return fromAccount.SendTransfer(toAccount, amount, message);
        }

        public static bool CloseAccount(Client owner, AccountType accountType)
        {
            if (owner.HasAccountByType(accountType))
            {
                if (accountType == AccountType.Deposit)
                {
                    if (Accounts.Remove(owner.DepositAccount)) 
                        IDs.Remove(owner.DepositAccount.ID);
                }
                else if (accountType == AccountType.Deposit)
                {
                    if (Accounts.Remove(owner.NonDepositAccount))
                        IDs.Remove(owner.NonDepositAccount.ID);
                }

                owner.CloseBankAccount(accountType);
                return true;
            }
            else return false;
        }

        public static BankAccount<int> GetAccountByID(int id)
        {
            if (IDs.Contains(id))
            {
                foreach(BankAccount<int> account in Accounts)
                {
                    if (account.ID == id) return account;
                }
                throw new Exception("Ошибка несовпадения множества 'IDs' с множеством 'Accounts'!");
            }
            else return null;
        }

        public static bool HasAccountID(int id)
        {
            return IDs.Contains(id);
        }

        public static string IntToStringID(int id)
        {
            string strID = id.ToString();
            if (strID.Length < 20)
            {
                StringBuilder sb = new StringBuilder(20, 20);
                sb.Append(strID);
                sb.Insert(0, "0", sb.Capacity - sb.Length);
                return sb.ToString();
            }
            else return strID;
        }

        public static BankAccount<int>[] GetAllAccounts()
        {
            BankAccount<int>[] currentList = new BankAccount<int>[Accounts.Count];
            Accounts.CopyTo(currentList);
            return currentList;
        }

        public static void SaveAccounts()
        {
            _freeIDSerializer.SerializeData(FreeID.ToString(), _xmlFreeIDPath);
            //_hashSerializer.SerializeData(Accounts, _xmlPath); Не сериализуются счета ввиду наличия generics!!!
        }
    }
}
