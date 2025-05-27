using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using CentralPG.Enums;
using OCPG.Core.Enums;

namespace OCPG.Core.Models.Entities
{

    public class Withdrawals
    {
        [Key]
        public int id { get; set; }
        public string wallet_accountNumber { get; set; }
        public string bank_accountNumber { get; set; }
        public double amount { get; set; }
        public string narration { get; set; }
        public string transactionReference { get; set; }
        public string processorRef { get; set; }
        public string processorMsg { get; set; }
        public string currency { get; set; }
        public ChannelCode channelCode { get; set; }
        public OrderStatus status { get; set; }
        public DateTimeOffset createdAt { get; set; }
    }
    
}