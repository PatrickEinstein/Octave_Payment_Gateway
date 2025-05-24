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


        public async Task<PaymentTransactions> GetPaymentByPaymentReference(string parameter)
        {
            var gottenPayment = await dataBaseContext.Payment.FirstOrDefaultAsync(c => c.paymentReference == parameter);
            return gottenPayment;
        }

        public async Task<PaymentTransactions> GetPaymentByAdviceReference(string parameter)
        {
            var gottenPayment = await dataBaseContext.Payment.FirstOrDefaultAsync(c => c.adviceReference == parameter);
            return gottenPayment;
        }

          public async Task<PaymentTransactions> GetPaymentByMerchantference(string parameter)
        {
            var gottenPayment = await dataBaseContext.Payment.FirstOrDefaultAsync(c => c.merchantReference == parameter);
            return gottenPayment;
        }


        public async Task<List<PaymentTransactions>> GetAllPaymentsAsync()
        {
            var gottenPayments = dataBaseContext.Payment.ToListAsync();
            return await gottenPayments;
        }

        public async Task<bool> UpdatePayment(PaymentTransactions paymentTransactions)
        {
            try
            {
                var paymentToUpdate = await dataBaseContext.Payment.FirstOrDefaultAsync(x => x.adviceReference == paymentTransactions.adviceReference || x.paymentReference == paymentTransactions.paymentReference);
                paymentToUpdate.transactionStatus = paymentTransactions.transactionStatus ?? paymentToUpdate.transactionStatus;
                paymentToUpdate.paymentReference = paymentToUpdate.paymentReference ?? paymentToUpdate.paymentReference;
                paymentToUpdate.amountCollected = paymentTransactions.amountCollected;
                paymentToUpdate.accountNumberMasked = paymentTransactions.accountNumberMasked ?? paymentToUpdate.accountNumberMasked;
                paymentToUpdate.merchantCode = paymentTransactions.merchantCode ?? paymentToUpdate.merchantCode;
                paymentToUpdate.responsePayload = paymentTransactions.responsePayload ?? paymentToUpdate.responsePayload;
                paymentToUpdate.authMode = paymentTransactions.authMode ?? paymentToUpdate.authMode ;
                paymentToUpdate.authFields = paymentTransactions.authFields ?? paymentToUpdate.authFields;
                paymentToUpdate.processor_message = paymentTransactions.processor_message ?? paymentToUpdate.processor_message;
                await dataBaseContext.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

  
    }
}