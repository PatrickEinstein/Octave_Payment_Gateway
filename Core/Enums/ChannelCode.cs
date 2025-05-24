using System.Text.Json.Serialization;

namespace OCPG.Core.Enums
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum ChannelCode
    {
        chamsSwitch, paystack, flutterWave
    }
    [JsonConverter(typeof(JsonStringEnumConverter))]
 public enum Currency
    {
        NGN, USD, EUR, GBP, JPY, CAD, AUD, CNY, INR, ZAR
    }
[JsonConverter(typeof(JsonStringEnumConverter))]
 public enum PaymentType
    {
        card, debit_ng_account, mobilemoney, bank_transfer, ach_payment
    }
[JsonConverter(typeof(JsonStringEnumConverter))]
    public enum ValidateCardPaymentChannelCode
    {
        chamsSwitch, paystack, flutterWave
    }
}