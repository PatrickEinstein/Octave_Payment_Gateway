using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CentralPG.Core.Models.Entities;
using CentralPG.Data;
using CentralPG.Models;
using Microsoft.EntityFrameworkCore;
using OCPG.Infrastructure.Interfaces.IRepositories;

namespace OCPG.Infrastructure.Service.Repositories
{
    public class PaymentRepository : IPaymentRepository
    {
        private readonly DataBaseContext dataBaseContext;

        public PaymentRepository(DataBaseContext dataBaseContext)
        {
            this.dataBaseContext = dataBaseContext;
        }
        public async Task<bool> CreatePayment(PaymentTransactions paymentTransactions)
        {
            try
            {
                var savedPayment = dataBaseContext.Add(paymentTransactions);
                await dataBaseContext.SaveChangesAsync();
                return true;
            }
            catch (Exception e)
            {
                return false;
            }
        }


        public async Task<PaymentTransactions> GetPayment(string parameter)
        {
            var gottenPayment = await dataBaseContext.payment.FirstOrDefaultAsync(c => c.paymentReference == parameter || c.adviceReference == parameter);
            return gottenPayment;
        }

        public async Task<List<PaymentTransactions>> GetAllPaymentsAsync()
        {
            var gottenPayments = dataBaseContext.payment.ToListAsync();
            return await gottenPayments;
        }

        public async Task<bool> UpdatePayment(PaymentTransactions paymentTransactions)
        {
            try
            {
                var paymentToUpdate = await dataBaseContext.payment.FirstOrDefaultAsync(x => x.adviceReference == paymentTransactions.adviceReference || x.paymentReference == paymentTransactions.paymentReference);
                paymentToUpdate.transactionStatus = paymentTransactions.transactionStatus;
                paymentToUpdate.paymentReference = paymentTransactions.paymentReference;
                paymentToUpdate.amountCollected = paymentTransactions.amountCollected;
                paymentToUpdate.accountNumberMasked = paymentTransactions.accountNumberMasked;
                paymentToUpdate.merchantCode = paymentTransactions.merchantCode;
                paymentToUpdate.responsePayload = paymentTransactions.responsePayload;
                await dataBaseContext.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public async Task<bool> UpdateChamsSwitchWebhook(WebHookRequestModel paymentTransactions)
        {
            try
            {
                var paymentToUpdate = await dataBaseContext.payment.FirstOrDefaultAsync(x => x.paymentReference == paymentTransactions.PaymentReference);
                if (paymentToUpdate == null)
                {
                    return false;
                }
                else
                {
                    paymentToUpdate.transactionStatus = paymentTransactions.TransactionStatus;
                    await dataBaseContext.SaveChangesAsync();
                    return true;
                }
            }
            catch (Exception ex)
            {
                return false;
            }
        }


    }
}