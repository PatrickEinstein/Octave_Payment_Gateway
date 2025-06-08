using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace OCPG.Core.Models
{


    public class FlutterBaseModel<T, M>
    {
        public string status { get; set; }
        public string? message { get; set; }
        public T? data { get; set; }
        public M? meta { get; set; }
    }



    public class meta
    {
        public auth authorization { get; set; }
    }

    public class auth
    {
        public string mode { get; set; }
        public string? redirect { get; set; }
        public string? endpoint { get; set; }
        public List<string> fields { get; set; }
    }

    public class validatPaymentAuth
    {
        public string mode { get; set; }
        public string? pin { get; set; }
        public string? city { get; set; }
        public string? address { get; set; }
        public string? state { get; set; }
        public string? country { get; set; }
        public string? zipcode { get; set; }

    }

    public class Card
    {
        public string first_6digits { get; set; }
        public string last_4digits { get; set; }
        public string issuer { get; set; }
        public string country { get; set; }
        public string type { get; set; }
        public string expiry { get; set; }
    }

    public class cardToken
    {
        public string card_number { get; set; }
        public string expiry_month { get; set; }
        public string expiry_year { get; set; }
        public string cvv { get; set; }
        public string currency { get; set; }
        public double amount { get; set; }
        public string email { get; set; }
        public string fullname { get; set; }
        public string phone_number { get; set; }
        public string tx_ref { get; set; }
        public string redirect_url { get; set; }
        public validatPaymentAuth authorization { get; set; }
    }

    public class Customer
    {
        public int id { get; set; }
        public string phone_number { get; set; }
        public string name { get; set; }
        public string email { get; set; }
        public DateTime created_at { get; set; }
    }

    public class FlutterChargeResponse
    {
        public int id { get; set; }
        public string tx_ref { get; set; }
        public string flw_ref { get; set; }
        public string device_fingerprint { get; set; }
        public decimal amount { get; set; }
        public decimal charged_amount { get; set; }
        public decimal app_fee { get; set; }
        public decimal merchant_fee { get; set; }
        public string processor_response { get; set; }
        public string auth_model { get; set; }
        public string currency { get; set; }
        public string ip { get; set; }
        public string narration { get; set; }
        public string status { get; set; }
        public string payment_type { get; set; }
        public string fraud_status { get; set; }
        public string charge_type { get; set; }
        public DateTime created_at { get; set; }
        public int account_id { get; set; }
        public Customer customer { get; set; }
        public Card card { get; set; }
    }


    public class FlutterWallet
    {

        public string response_code { get; set; }
        public string response_message { get; set; }
        public string flw_ref { get; set; }
        public string order_ref { get; set; }
        public string account_number { get; set; }
        public string account_status { get; set; }
        public int frequency { get; set; }
        public string bank_name { get; set; }
        public string created_at { get; set; }
        public string expiry_date { get; set; }
        public string note { get; set; }
        public string amount { get; set; }

    }

public class WalletWithdrawTransferResponse
{
    public long id { get; set; }
    public string account_number { get; set; }
    public string bank_code { get; set; }
    public string full_name { get; set; }
    public string created_at { get; set; }
    public string currency { get; set; }
    public string debit_currency { get; set; }
    public double amount { get; set; }
    public double fee { get; set; }
    public string status { get; set; }
    public string reference { get; set; }
    public object meta { get; set; }
    public string narration { get; set; }
    public string complete_message { get; set; }
    public int requires_approval { get; set; }
    public int is_approved { get; set; }
    public string bank_name { get; set; }
}




    public class FlutterWebhook
    {
        [JsonPropertyName("event")]
        public string? Event { get; set; }
        public WebHookData? data { get; set; }


        public WebHookCard? card { get; set; }
        [JsonPropertyName("event.type")]
        public string? EventType { get; set; }
    }

    public class WebHookData
    {
        public long? id { get; set; }
        public string? tx_ref { get; set; }
        public string? flw_ref { get; set; }
        public string? device_fingerprint { get; set; }
        public double? amount { get; set; }
        public string? currency { get; set; }
        public long? charged_amount { get; set; }
        public double? app_fee { get; set; }
        public string? merchant_fee { get; set; }
        public string? processor_response { get; set; }
        public string? auth_model { get; set; }
        public string? ip { get; set; }
        public string? narration { get; set; }
        public string? status { get; set; }
        public string? payment_type { get; set; }
        public string? created_at { get; set; }
        public string? account_id { get; set; }
        public string? account_number { get; set; }
        public string bank_name { get; set; }
        public string bank_code { get; set; }
        public string fullname { get; set; }
        public string debit_currency { get; set; }
        public decimal fee { get; set; }
        public string reference { get; set; }
        public object meta { get; set; }
        public string approver { get; set; }
        public string complete_message { get; set; }
        public int requires_approval { get; set; }
        public int is_approved { get; set; }
        public WebHookCustomer? customer { get; set; }
    }
    public class WebHookCustomer
    {
        public string? id { get; set; }
        public string? name { get; set; }
        public string? phone_number { get; set; }
        public string? email { get; set; }
        public string? created_at { get; set; }
    }
    public class WebHookCard
    {
        public string? first_6digits { get; set; }
        public string? last_4digits { get; set; }
        public string? issuer { get; set; }
        public string? country { get; set; }
        public string? type { get; set; }
        public string? expiry { get; set; }
    }
    

    public class FlutterBankList
    {
        
        public string? code { get; set; }
        public string name { get; set; }
    }


   public class ChargeData
{
    public int chargeAmount { get; set; }   
    public int fee { get; set; }   
    public int merchantFee { get; set; }   
    public int flutterwaveFee { get; set; }
    public int stampDutyFee { get; set; }   
    public string currency { get; set; }
}

}

