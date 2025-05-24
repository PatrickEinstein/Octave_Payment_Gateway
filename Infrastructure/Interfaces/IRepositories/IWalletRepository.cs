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
        void CreateWalletTransactionHistory(WalletTransactionHistory payload);
        Task<WalletTransactionHistory> UpdateWalletTransactionHistoryStatus(string transaction_reference);
        Task<List<WalletTransactionHistory>> GetWalletTransactionHistory(WemaAccountTransactionHistoryRequest payload);

        Task<CreditWalletRequestResponse> ProcessWalletToWalletTransfer(ClientTransferRequest model);
       
    }
}