using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CentralPG.Data;
using CentralPG.Infrasturcture.Interfaces.Utilities;
using CentralPG.Models;
using Dapper;
using Microsoft.EntityFrameworkCore;
using OCPG.Core.Models;
using OCPG.Core.Models.Entities;
using OCPG.Infrastructure.Interfaces.IRepositories;

namespace OCPG.Infrastructure.Service.Repositories
{
    public class WalletRepository : IWalletRepository
    {
        private readonly DataBaseContext dataBaseContext;
        private readonly IDapperContext dapperContext;

        public WalletRepository(
            DataBaseContext dataBaseContext,
            IDapperContext dapperContext
            )
        {
            this.dataBaseContext = dataBaseContext;
            this.dapperContext = dapperContext;
        }



        public async Task<bool> CreateWallet(Wallets wallet)
        {
            try
            {
                var savedWallet = dataBaseContext.Add(wallet);
                await dataBaseContext.SaveChangesAsync();
                return true;
            }
            catch (Exception e)
            {
                return false;
            }
        }

        public async Task<Wallets> GetWalletByAccountNumber(string parameter)
        {
            var gottenWallet = await dataBaseContext.Wallets.FirstOrDefaultAsync(c => c.account_number == parameter);
            return gottenWallet;
        }

        public async void CreditWallet(ConfirmWalletTransferStatus payload)
        {
            try
            {

                var gottenWallets = await dataBaseContext.Wallets.FirstOrDefaultAsync(c => c.email == payload.email &&
                c.phone_number == payload.phone_number &&
                c.wallet_provider == payload.provider);
                if (gottenWallets == null)
                    throw new Exception("Wallet not found");
                gottenWallets.account_balance += payload.amount;
                dataBaseContext.Wallets.Update(gottenWallets);
                await dataBaseContext.SaveChangesAsync();
            }
            catch (Exception e)
            {
                throw new Exception("Error while crediting wallet: " + e.Message);
            }

        }

        public async Task<Wallets> Debit()
        {
            var gottenWallets = await dataBaseContext.Wallets.FirstOrDefaultAsync(c => c.account_number == "1234567890");
            return gottenWallets;
        }

        public async Task<Wallets> LayMandateOnWallet()
        {
            var gottenWallets = await dataBaseContext.Wallets.FirstOrDefaultAsync(c => c.account_number == "1234567890");
            return gottenWallets;
        }

        public async void CreateWalletTransactionHistory(WalletTransactionHistory payload)
        {
            try
            {
                dataBaseContext.walletTransactionHistory.Add(payload);
                await dataBaseContext.SaveChangesAsync();


            }
            catch (Exception e)
            {
                throw new Exception("Error while creating wallet transaction history: " + e.Message);
            }
        }

        public async Task<WalletTransactionHistory> UpdateWalletTransactionHistoryStatus(string transaction_reference)
        {
            var transactionToupdate = await dataBaseContext.walletTransactionHistory
                .FirstOrDefaultAsync(c => c.transaction_reference == transaction_reference);
                
            if (transactionToupdate == null) throw new Exception("Transaction history not found");
           var onlyHistory =  transactionToupdate.status = CentralPG.Enums.OrderStatus.Successful; // Update the status to Successful

            dataBaseContext.walletTransactionHistory.Update(transactionToupdate);
            await dataBaseContext.SaveChangesAsync();
            return transactionToupdate;
        }

    public async Task<List<WalletTransactionHistory>> GetWalletTransactionHistory(WemaAccountTransactionHistoryRequest payload)
{
    var query = "SELECT * FROM WalletTransactionHistory WHERE 1=1";

    if (!string.IsNullOrEmpty(payload.accountNumber))
    {
        query += " AND account_number = @accountNumber";
    }

    if (!string.IsNullOrEmpty(payload.transaction_reference))
    {
        query += " AND transaction_reference = @transactionReference";
    }

    query += " AND created_at >= @startDate AND created_at <= @endDate ORDER BY transaction_date DESC";

    var startDate = DateTime.TryParse(payload.from, out var fromDate)
        ? fromDate
        : new DateTime(1970, 1, 1);

    var endDate = DateTime.TryParse(payload.to, out var toDate)
        ? toDate
        : DateTime.Now;

    using (var connection = dapperContext.GetDbConnection())
    {
        var result = await connection.QueryAsync<WalletTransactionHistory>(query, new
        {
            accountNumber = payload.accountNumber,
            transactionReference = payload.transaction_reference,
            startDate,
            endDate
        });

        return result.ToList();
    }
}



        public async Task<CreditWalletRequestResponse> ProcessWalletToWalletTransfer(ClientTransferRequest model)
        {
           CreditWalletRequestResponse res = new CreditWalletRequestResponse();
            try
            {
                var originatorWallet = await dataBaseContext.Wallets.FirstOrDefaultAsync(c => c.account_number == model.sourceAccountNumber);
                if (originatorWallet == null)
                {
                    res.errorMessage = "Source wallet not found";
                    return res;
                }

                var destinationWallet = await dataBaseContext.Wallets.FirstOrDefaultAsync(c => c.account_number == model.destinationAccountNumber);
                if (destinationWallet == null)
                {
                    res.errorMessage = "Destination wallet not found";
                    return res;
                }

                if (originatorWallet.account_balance < model.amount)
                {
                    res.errorMessage = "Insufficient balance in originator wallet";
                    return res;
                }

                originatorWallet.account_balance -= model.amount;
                destinationWallet.account_balance += model.amount;

                dataBaseContext.Wallets.Update(originatorWallet);
                dataBaseContext.Wallets.Update(destinationWallet);

                await dataBaseContext.SaveChangesAsync();
              
                res.message = "Transfer successful";
            }
            catch (Exception e)
            {
                res.errorMessage = $"Error processing transfer: {e.Message}";
            }
            return res;
        }
    }
}