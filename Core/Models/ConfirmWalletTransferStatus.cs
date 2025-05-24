using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OCPG.Core.Models
{
    public class ConfirmWalletTransferStatus
    {
        public string transfer_reference { get; set; }
        public string processor_reference { get; set; }
        public double amount { get; set; }
        public string currency { get; set; }
        public string processor_response { get; set; }
        public string status { get; set; }
        public string? narration { get; set; }
        public string? created_at { get; set; }
        public DateTime? updated_at { get; set; }

        public string? customer_name { get; set; }
        public string? phone_number { get; set; }
        public string? email { get; set; }
         public string? provider { get; set; }
 
    }
}