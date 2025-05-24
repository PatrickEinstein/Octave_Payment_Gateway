using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using CentralPG.Enums;
using OCPG.Core.Enums;

namespace OCPG.Core.Models.Entities
{
    public class Wallets
    {
        [Key]
        public int id { get; set; }
        public string account_number { get; set; }
        public string account_name { get; set; }
        public double account_balance { get; set; }
        public double account_mandate { get; set; }
        public string account_type { get; set; }
        public string acccount_trackerRef { get; set; }
         public string acccount_trackerId { get; set; }
        public string wallet_provider { get; set; }
        public string phone_number { get; set; }
        public string email { get; set; }
        public DateTime created_at { get; set; } = DateTime.Now;   
    }


    
}