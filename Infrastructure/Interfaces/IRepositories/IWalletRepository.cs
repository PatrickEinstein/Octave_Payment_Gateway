using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CentralPG.Models;
using OCPG.Core.Models;
using OCPG.Core.Models.Entities;

namespace OCPG.Infrastructure.Interfaces.IRepositories
{
    public interface IWalletRepository
    {
        Task<bool> CreateWallet(Wallets wallet);
        Task<Wallets> GetWalletByAccountNumber(string parameter);
        void CreditWallet(ConfirmWalletTransferStatus payload);
        Task<Wallets> Debit();
        Task<Wallets> LayMandateOnWallet();
        Task CreateWalletTransactionHistory(WalletTransactionHistory payload);
        Task<WalletTransactionHistory> UpdateWalletTransactionHistoryStatus(string transaction_reference, CentralPG.Enums.OrderStatus status);
        Task<serviceResponse<List<WalletTransactionHistory>>> GetWalletTransactionHistory(WemaAccountTransactionHistoryRequest payload);

        Task<CreditWalletRequestResponse> ProcessWalletToWalletTransfer(ClientTransferRequest model);
        Task<serviceResponse<string>> LayMandateOnWallet(string accountNumber, double mandateAmount);
        Task<serviceResponse<string>> SubtractMandate(string accountNumber, double mandateAmount);
        Task<serviceResponse<string>> InitiateWithrawals(WithdrawFromWallet payload);
        Task<List<Withdrawals>> GetAllPendingWithdrawals();
        Task<bool> UpdateWithdrawal(Withdrawals withdrawal);
    }
}