using System;

namespace BankASystem.Models
{
    [Serializable]
    public class NonDepositAccount<T> : BankAccount<T>
    {
        public override AccountType AccountType { get; } = AccountType.NonDeposit;

        /// <summary>
        /// Открыть недепозитный счёт
        /// </summary>
        /// <param name="id">Идентификатор счёта</param>
        /// <param name="balance">Начальный баланс</param>
        public NonDepositAccount(T id, Client owner, float balance = 0) : base(id, owner, balance) { }
        public NonDepositAccount() : this(default, default) { }
    }
}
