using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OCPG.Core.Models
{

    public class PaystackBaseModel<T>
    {
        public bool status { get; set; }
        public string message { get; set; }
        public T data { get; set; }
    }


    public class CreateWalletData
    {
        public string type { get; set; }
        public string code { get; set; }
    }


    public class BankProvidersData
    {
        public string provider_slug { get; set; }
        public string bank_id { get; set; }
        public string bank_name { get; set; }
        public string bankid_code { get; set; }
    }

    public class initializePaymentData
    {
        public string authorization_url { get; set; }
        public string access_code { get; set; }
        public string reference { get; set; }
    }


    public class verifyPaymentData
    {
        public long id { get; set; }
        public string domain { get; set; }
        public string status { get; set; }
        public string reference { get; set; }
        public string receiptNumber { get; set; }
        public int amount { get; set; }
        public string message { get; set; }
        public string gatewayResponse { get; set; }
        public DateTime paidAt { get; set; }
        public DateTime createdAt { get; set; }
        public string channel { get; set; }
        public string currency { get; set; }
        public string ipAddress { get; set; }
        public string metadata { get; set; }
        public logData log { get; set; }
        public int fees { get; set; }
        public string feesSplit { get; set; }
        public authorization authorization { get; set; }
        public customer customer { get; set; }
        public string plan { get; set; }
        public object split { get; set; }
        public string orderId { get; set; }
        public DateTime paidAtLocal { get; set; }
        public DateTime createdAtLocal { get; set; }
        public int requestedAmount { get; set; }
        public object posTransactionData { get; set; }
        public object source { get; set; }
        public object feesBreakdown { get; set; }
        public object connect { get; set; }
        public DateTime transactionDate { get; set; }
        public object planObject { get; set; }
        public object subaccount { get; set; }
    }

    public class logData
    {
        public int startTime { get; set; }
        public int timeSpent { get; set; }
        public int attempts { get; set; }
        public int errors { get; set; }
        public bool success { get; set; }
        public bool mobile { get; set; }
        public List<history> history { get; set; }
    }

    public class history
    {
        public string type { get; set; }
        public string message { get; set; }
        public int time { get; set; }
    }

    public class authorization
    {
        public string authorizationCode { get; set; }
        public string bin { get; set; }
        public string last4 { get; set; }
        public string expMonth { get; set; }
        public string expYear { get; set; }
        public string channel { get; set; }
        public string cardType { get; set; }
        public string bank { get; set; }
        public string countryCode { get; set; }
        public string brand { get; set; }
        public bool reusable { get; set; }
        public string signature { get; set; }
        public string accountName { get; set; }
    }

    public class customer
    {
        public long id { get; set; }
        public string firstName { get; set; }
        public string lastName { get; set; }
        public string email { get; set; }
        public string customerCode { get; set; }
        public string phone { get; set; }
        public string metadata { get; set; }
        public string riskAction { get; set; }
        public string internationalFormatPhone { get; set; }
    }

}

