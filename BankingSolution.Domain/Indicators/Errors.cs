using BankingSolution.Domain.Entities;

namespace BankingSolution.Domain.Indicators;

public interface IDomainError;

public struct EnoughFunds(Account account, decimal amount):IDomainError;
public struct SelfTransfer(Account account):IDomainError;
public struct InvalidOperationAmount(Account account,  decimal amount):IDomainError;