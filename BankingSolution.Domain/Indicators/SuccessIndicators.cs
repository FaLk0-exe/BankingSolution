﻿using BankingSolution.Domain.Entities;

namespace BankingSolution.Domain.Indicators;

public struct AccountReplenishSucceeded(ReplenishAccountTransaction replenishAccountTransaction);
public struct AccountWithdrawSucceeded(WithdrawAccountTransaction withdrawAccountTransaction);
public struct TransferSucceeded(Account From, Account To, WithdrawAccountTransaction accountTransaction);