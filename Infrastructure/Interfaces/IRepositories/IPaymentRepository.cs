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
         Task<PaymentTransactions> GetPayment(string parameter);
        Task<bool> UpdatePayment(PaymentTransactions paymentTransactions);
        Task<bool> UpdateChamsSwitchWebhook(WebHookRequestModel paymentTransactions);

    }
}