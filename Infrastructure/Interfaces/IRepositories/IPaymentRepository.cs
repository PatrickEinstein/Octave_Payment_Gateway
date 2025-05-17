using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CentralPG.Core.Models.Entities;
using CentralPG.Models;

namespace OCPG.Infrastructure.Interfaces.IRepositories
{
    public interface IPaymentRepository
    {
        Task<bool> CreatePayment(PaymentTransactions paymentTransactions);
        Task<PaymentTransactions> GetPaymentByPaymentReference(string parameter);
        Task<PaymentTransactions> GetPaymentByAdviceReference(string parameter);
        Task<bool> UpdatePayment(PaymentTransactions paymentTransactions);
        Task<bool> UpdateChamsSwitchWebhook(WebHookRequestModel paymentTransactions);

    }
}