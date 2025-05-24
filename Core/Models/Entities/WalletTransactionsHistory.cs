using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using CentralPG.Enums;
using OCPG.Core.Enums;

namespace OCPG.Core.Models.Entities
{
  
        

        public class WalletTransactionHistory
    {
        [Key]
        public int id { get; set; }
        public string originator_accountNumber { get; set; }
        public string destination_accountNumber { get; set; }
        public string originator_accountName { get; set; }
        public string destination_accountName { get; set; }

        public string processor_reference { get; set; }
        public string transaction_reference { get; set; }
        public OrderStatus status { get; set; }
        public string narration { get; set; }
        public string transaction_type { get; set; }
        public string direction { get; set; } = "NGN";
        public double amount { get; set; }
         public DateTime created_at { get; set; } = DateTime.Now;  
        public string transaction_date { get; set; }
        public ChannelCode provider { get; set; }
    }
    
}