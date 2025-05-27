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
                c.wallet_provider.ToString() == payload.provider);
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

        public async Task CreateWalletTransactionHistory(WalletTransactionHistory payload)
        {
            try
            {
                await dataBaseContext.WalletTransactionHistory.AddAsync(payload);
                await dataBaseContext.SaveChangesAsync();
            }
            catch (Exception e)
            {
                throw new Exception("Error while creating wallet transaction history: " + e.Message);
            }
        }

        public async Task<WalletTransactionHistory> UpdateWalletTransactionHistoryStatus(string transaction_reference, CentralPG.Enums.OrderStatus status)
        {
            var transactionToupdate = await dataBaseContext.WalletTransactionHistory
                .FirstOrDefaultAsync(c => c.transaction_reference == transaction_reference);

            if (transactionToupdate == null) throw new Exception("Transaction history not found");
            var onlyHistory = transactionToupdate.status = status;

            dataBaseContext.WalletTransactionHistory.Update(transactionToupdate);
            await dataBaseContext.SaveChangesAsync();
            return transactionToupdate;
        }

        public async Task<serviceResponse<List<WalletTransactionHistory>>> GetWalletTransactionHistory(WemaAccountTransactionHistoryRequest payload)
        {
            var response = new serviceResponse<List<WalletTransactionHistory>>();
            try
            {
                var query = "SELECT * FROM \"walletTransactionHistory\" WHERE 1=1";

                if (!string.IsNullOrEmpty(payload.accountNumber))
                {
                    query += " AND \"destination_accountNumber\" = @accountNumber OR \"originator_accountNumber\" = @accountNumber";
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

                    var res = result.ToList();

                    foreach (var item in res)
                    {

                        if (item.originator_accountNumber == payload.accountNumber)
                        {
                            item.transaction_type = "Debit";
                        }
                        else if (item.destination_accountNumber == payload.accountNumber)
                        {
                            item.transaction_type = "Credit";
                        }
                        else
                        {
                            item.transaction_type = "Unknown";
                        }
                    }
                    response.Data = res;
                    response.Message = "Transaction history fetched successfully";

                }
            }
            catch (Exception e)
            {
                response.Data = null;
                response.Message = "Error fetching transaction history: " + e.Message;

            }
            return response;
        }



        public async Task<CreditWalletRequestResponse> ProcessWalletToWalletTransfer(ClientTransferRequest model)
        {
            CreditWalletRequestResponse res = new CreditWalletRequestResponse();
            WalletTransactionHistory transactionHistory = new WalletTransactionHistory();
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


                transactionHistory = new WalletTransactionHistory
                {
                    originator_accountNumber = model.sourceAccountNumber,
                    transaction_reference = model.transactionReference,
                    destination_accountNumber = model.destinationAccountNumber,
                    originator_accountName = originatorWallet.account_name,
                    destination_accountName = destinationWallet.account_name,
                    processor_reference = Guid.NewGuid().ToString(),
                    status = CentralPG.Enums.OrderStatus.Pending,
                    narration = !string.IsNullOrWhiteSpace(model.narration) ? model.narration : $"Transfer from {originatorWallet.account_name} to {destinationWallet.account_name}",
                    transaction_type = "Wallet Transfer",
                    amount = model.amount,
                    created_at = DateTime.UtcNow,
                    transaction_date = DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss"),
                    provider = Core.Enums.ChannelCode.flutterWave,
                };

                await this.CreateWalletTransactionHistory(transactionHistory);

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

                await this.UpdateWalletTransactionHistoryStatus(transactionHistory.transaction_reference, CentralPG.Enums.OrderStatus.Successful);

                res.message = "Transfer successful";
            }
            catch (Exception e)
            {
                await this.UpdateWalletTransactionHistoryStatus(transactionHistory.transaction_reference, CentralPG.Enums.OrderStatus.Failed);
                res.errorMessage = $"Error processing transfer: {e.Message}";

            }
            return res;
        }


        public async Task<serviceResponse<string>> LayMandateOnWallet(string accountNumber, double mandateAmount)
        {
            var response = new serviceResponse<string>();
            try
            {

                var wallet = await dataBaseContext.Wallets.FirstOrDefaultAsync(c => c.account_number == accountNumber);
                if (wallet == null)
                {
                    response.Data = null;
                    response.Message = "Wallet not found";
                    response.code = "99";
                    return response;
                }
                if ((wallet.account_balance - wallet.account_mandate) < mandateAmount)
                {
                    response.Data = null;
                    response.Message = "Insufficient balance in wallet";
                    response.code = "99";
                    return response;
                }

                wallet.account_mandate += mandateAmount;
                dataBaseContext.Wallets.Update(wallet);
                dataBaseContext.SaveChanges();
                response.Data = $"Mandate laid successfully on wallet {accountNumber} with amount {mandateAmount}";
                response.Message = $"Mandate laid successfully on wallet {accountNumber} with amount {mandateAmount}";
                response.code = "00";

            }
            catch (Exception e)
            {
                response.Data = null;
                response.Message = "Error laying mandate on wallet: " + e.Message;
                response.code = "99";

                return response;
            }

            return response;
        }


        public async Task<serviceResponse<string>> SubtractMandate(string accountNumber, double mandateAmount)
        {
            var response = new serviceResponse<string>();
            try
            {

                var wallet = await dataBaseContext.Wallets.FirstOrDefaultAsync(c => c.account_number == accountNumber);
                if (wallet == null)
                {
                    response.Data = null;
                    response.Message = "Wallet not found";
                    response.code = "99";
                    return response;
                }
                if (wallet.account_mandate - mandateAmount < 0)
                {
                    response.Data = null;
                    response.Message = "Insufficient balance in wallet";
                    response.code = "99";
                    return response;
                }

                wallet.account_mandate -= mandateAmount;
                dataBaseContext.Wallets.Update(wallet);
                dataBaseContext.SaveChanges();
                response.Data = $"Mandate removed successfully on wallet {accountNumber} with amount {mandateAmount}";
                response.Message = $"Mandate removed successfully on wallet {accountNumber} with amount {mandateAmount}";
                  response.code = "00";
            }
            catch (Exception e)
            {
                response.Data = null;
                response.Message = "Error removing mandate on wallet: " + e.Message;
                response.code = "99";
                return response;
            }

            return response;
        }


        public async Task<serviceResponse<string>> InitiateWithrawals(WithdrawFromWallet payload)
        {
            var response = new serviceResponse<string>();
            try
            {
                var wallet = await dataBaseContext.Wallets.FirstOrDefaultAsync(c => c.account_number == payload.wallet_accountNumber);
                if (wallet == null)
                {
                    response.Data = null;
                    response.Message = "Wallet not found";
                    return response;
                }

                if ((wallet.account_balance - wallet.account_mandate) < payload.amount)
                {
                    response.Data = null;
                    response.Message = "Insufficient balance in wallet";
                    return response;
                }
                wallet.account_balance -= payload.amount;
                var withdrawal = new Withdrawals
                {
                    wallet_accountNumber = payload.wallet_accountNumber,
                    bank_accountNumber = payload.bank_accountNumber,
                    amount = payload.amount,
                    narration = payload.narration,
                    transactionReference = payload.transactionReference,
                    currency = payload.currency,
                    channelCode = wallet.wallet_provider,
                    status = CentralPG.Enums.OrderStatus.Pending,
                    createdAt = DateTimeOffset.UtcNow
                };

                await dataBaseContext.Withdrawals.AddAsync(withdrawal);
                dataBaseContext.Wallets.Update(wallet);
                await dataBaseContext.SaveChangesAsync();
                response.Data = $"Withdrawal of {payload.amount} from wallet {payload.bank_accountNumber} is pending";
                response.Message = $"Withdrawal of {payload.amount} from wallet {payload.bank_accountNumber} is pending";
            }
            catch (Exception e)
            {
                response.Data = null;
                response.Message = "Error initiating withdrawal: " + e.Message;
            }

            return response;

        }

        public async Task<List<Withdrawals>> GetAllPendingWithdrawals()
        {
            List<Withdrawals> withdrawal = new List<Withdrawals>();
            withdrawal = await dataBaseContext.Withdrawals.Where(c => c.status == CentralPG.Enums.OrderStatus.Pending).ToListAsync();
            return withdrawal;
        }

         public async Task<bool> UpdateWithdrawal(Withdrawals withdrawal)
        {
           var withdrawalToUpdate = await dataBaseContext.Withdrawals.FirstOrDefaultAsync(c => c.transactionReference == withdrawal.transactionReference);
            if (withdrawalToUpdate == null)
            {
                throw new Exception("Withdrawal not found");
            }
            withdrawalToUpdate.status = withdrawal.status;
            dataBaseContext.Withdrawals.Update(withdrawalToUpdate);

            // if the withrawal status is failed, we need to credit the wallet back
            if (withdrawal.status == CentralPG.Enums.OrderStatus.Failed)
            {
                var wallet = await dataBaseContext.Wallets.FirstOrDefaultAsync(c => c.account_number == withdrawal.wallet_accountNumber);
                if (wallet == null)
                {
                    throw new Exception("Wallet not found");
                }
                wallet.account_balance += withdrawal.amount;
                dataBaseContext.Wallets.Update(wallet);
            }
            await dataBaseContext.SaveChangesAsync();
            return true;
        }
    }
}