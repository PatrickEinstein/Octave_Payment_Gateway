using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace CentralPG.Core.Models.Entities
{
    public class PaymentTransactions
    {
        [Key]
        public int id { get; set; }
        public string merchantCode { get; set; }
        public string adviceReference { get; set; }
        public string paymentReference { get; set; }
        public string merchantReference { get; set; }
        public double amountCollected { get; set; }
        public double amount { get; set; }
        public string transactionStatus { get; set; }
        public string currencyCode { get; set; }
        public string accountNumberMasked { get; set; }
        public string narration { get; set; }
        public string customerName { get; set; }
        public string paymentDate { get; set; }
        public string requestPayload { get; set; }
        public string responsePayload { get; set; }
        public string channel { get; set; }
        public string notificationUrl { get; set; }
        public string callbackUrl { get; set; }
        public bool isNotified { get; set; } = false;
    }
}