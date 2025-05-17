using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OCPG.Models
{
    public class GetAdviceModel
    {

        public bool requestSuccessful { get; set; }
        public ResponseData responseData { get; set; }

    }
    public class ResponseData
    {
        public string currency { get; set; }
        public string adviceReference { get; set; }
        public string merchantRef { get; set; }
        public decimal amount { get; set; }
        public string narration { get; set; }
        public string customerId { get; set; }
        public decimal charge { get; set; }
        public string status { get; set; }
        public string customerFullName { get; set; }
        public string merchantName { get; set; }
        public string merchantCode { get; set; }
        public string merchantId { get; set; }
        public string paymentUrl { get; set; }
        public List<string> channels { get; set; }
        public List<string> channel { get; set; }
        public decimal customerCharge { get; set; }
        public decimal merchantCharge { get; set; }
        public decimal catCharge { get; set; }
    }
}