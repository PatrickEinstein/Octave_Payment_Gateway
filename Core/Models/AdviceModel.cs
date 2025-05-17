using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using OCPG.Core.Enums;
using OCPG.Core.Models;

namespace CentralPG.Models
{

    public class AdviceModel
    {

        public AdviceModel(AppUrl appUrl, AuthConfig authConfig)
        {
            Guid guid = Guid.NewGuid();
            adviceReference = guid.ToString();
            integrationKey = authConfig.integrationKey;
            MerchantCode = authConfig.merchantCode;
            notificationUrl = appUrl.notificationUrl;
        }
        public string merchantRef { get; set; }
        public string adviceReference { get; private set; }
        public double amount { get; set; }
        public string currency { get; set; }
        public string narration { get; set; }
        public string callBackUrl { get; set; }
        public string notificationUrl { get; private set; }
        public string splitCode { get; set; }
        public string integrationKey { get; private set; }
        public int mcc { get; set; }
        public string merchantDescriptor { get; set; }
        public Customer customer { get; set; }
        public string MerchantCode { get; set; }
        public ChannelCode channel { get; set; }
    }

    public class AdviceModelReq
    {
        public string merchantRef { get; set; }
        public double amount { get; set; }
        public string currency { get; set; }
        public string narration { get; set; }
        public Customer customer { get; set; }
        public ChannelCode channel { get; set; }
        public string callBackUrlDomain { get; set; }
        public string notificationUrl { get; set; }
    }


    public class Customer
    {
        public string CustomerId { get; set; } = "";
        public string CustomerLastName { get; set; } = "";
        public string CustomerFirstName { get; set; } = "";
        public string CustomerEmail { get; set; } = "";
        public string CustomerPhoneNumber { get; set; } = "";
        public string CustomerAddress { get; set; } = "";
        public string CustomerCity { get; set; } = "";
        public string CustomerStateCode { get; set; } = "";
        public string CustomerPostalCode { get; set; } = "";
        public string CustomerCountryCode { get; set; } = "";
    }

    public class CardPayment
    {
        public string cardNumber { get; set; }
        public string expiredMonth { get; set; }
        public string expiredYear { get; set; }
        public string cvv { get; set; }
        public string cardPin { get; set; }
        public bool shouldSaveCard { get; set; }
        public ChannelCode channel { get; set; }

    }

    public class BankPayment
    {
        public string bankCode { get; set; }
        public string accountNumber { get; set; }
        public string nameOnAccount { get; set; }
        public ChannelCode channel { get; set; }

    }

    public class GenerateBankAccount
    {
        public string adviceReference { get; set; }
        public ChannelCode channel { get; set; }
    }

    public class CompleteResponseModel
    {

    }
    public class AdviceResponseModel
    {
        // public bool? RequestSuccessful { get; set; }
        public bool? requestSuccessful { get; set; }

        // public AdviceData? ResponseData { get; set; }
        public AdviceData? responseData { get; set; }

        // public string? Message { get; set; }
        public string? message { get; set; }

        // public string? ResponseCode { get; set; }
        public string? responseCode { get; set; }

    }

    public class AdviceData
    {
        public string currency { get; set; }
        public string adviceReference { get; set; }
        public string merchantRef { get; set; }
        public double amount { get; set; }
        public string narration { get; set; }
        public string customerId { get; set; }
        public int charge { get; set; }
        public string status { get; set; }
        public string customerFullName { get; set; }
        public string merchantName { get; set; }
        public string merchantId { get; set; }
        public string paymentUrl { get; set; }
        public string message { get; set; }
        public string authorization_url { get; set; }
        public string access_code { get; set; }
        public string reference { get; set; }

        public List<string> channel { get; set; }
    }

    public class ProcessBankPaymentResponseModel
    {
        // public bool? RequestSuccessful { get; set; }
        public bool? requestSuccessful { get; set; }

        // public AdviceData? ResponseData { get; set; }
        public ProcessBankPaymentData? responseData { get; set; }

        // public string? Message { get; set; }
        public string? message { get; set; }

        // public string? ResponseCode { get; set; }
        public string? responseCode { get; set; }

    }

    public class ProcessBankPaymentData
    {
        public string currency { get; set; }
        public string adviceReference { get; set; }
        public string merchantRef { get; set; }
        public decimal amount { get; set; }
        public string narration { get; set; }
        public string customerId { get; set; }
        public int charge { get; set; }
        public string status { get; set; }
        public string customerFullName { get; set; }
        public string merchantName { get; set; }
        public string paymentUrl { get; set; }
        public string bankCode { get; set; }
        public string bankAccount { get; set; }
        public string paymentReference { get; set; }
        public string message { get; set; }
        public string callBackUrl { get; set; }

        public List<string> channel { get; set; }
    }
    public class BankTransferResponseModel
    {
        // public bool? RequestSuccessful { get; set; }
        public bool? requestSuccessful { get; set; }

        // public AdviceData? ResponseData { get; set; }
        public BankTransferData? responseData { get; set; }

        // public string? Message { get; set; }
        public string? message { get; set; }

        // public string? ResponseCode { get; set; }
        public string? responseCode { get; set; }

    }

    public class BankTransferData
    {
        public string bankName { get; set; }
        public string bankCode { get; set; }
        public string bankAccount { get; set; }
        public string paymentReference { get; set; }
        public string status { get; set; }
        public string message { get; set; }
        public decimal amount { get; set; }
        public int amountPaid { get; set; }
        public string callBackUrl { get; set; }
        public string customerName { get; set; }
    }

    public class ProcessCardResponseModel
    {
        // public bool? RequestSuccessful { get; set; }
        public bool? requestSuccessful { get; set; }

        // public AdviceData? ResponseData { get; set; }
        public ProcessCardData? responseData { get; set; }

        // public string? Message { get; set; }
        public string? message { get; set; }

        // public string? ResponseCode { get; set; }
        public string? responseCode { get; set; }

    }
    public class ProcessCardData
    {
        public string merchantCode { get; set; }
        public string paymentReference { get; set; }
        public string merchantReference { get; set; }
        public decimal amountCollected { get; set; }
        public decimal amount { get; set; }
        public string callBackUrl { get; set; }
        public string transactionStatus { get; set; }
        public string currencyCode { get; set; }
        public string accountNumber { get; set; }
        public string accountNumberMasked { get; set; }
        public string narration { get; set; }
        public string env { get; set; }
        public string message { get; set; }
        public FormData formData { get; set; }
        public string paymentLink { get; set; }

    }
    public class FormData
    {
        public string url { get; set; }
        public NestedFormData formData { get; set; }
    }
    public class NestedFormData
    {
        public string JWT { get; set; }
    }

    public class CompleteCardPayment
    {
        public string paymentreference { get; set; }
        public string value { get; set; }
        public ChannelCode channel { get; set; }
    }


    public class FormDataDetailsCompletePayment
    {

        public string JWT { get; set; }


        public string PaymentReference { get; set; }
    }

    public class FormDataCompletePayment
    {

        public string url { get; set; }


        public FormDataDetailsCompletePayment formData { get; set; }


        public string formString { get; set; }
    }

    public class ResponseDataCompletePayment
    {

        public string merchantCode { get; set; }


        public string paymentReference { get; set; }


        public string merchantReference { get; set; }


        public double amountCollected { get; set; }


        public double amount { get; set; }


        public string callBackUrl { get; set; }


        public string processorCode { get; set; }


        public string transactionStatus { get; set; }


        public string currencyCode { get; set; }


        public string accountNumber { get; set; }


        public string accountNumberMasked { get; set; }


        public string narration { get; set; }


        public string env { get; set; }


        public string message { get; set; }


        public FormDataCompletePayment formData { get; set; }


        public string returnUrl { get; set; }


        public string customerName { get; set; }


        public string paymentDate { get; set; }
        public string paymentLink { get; set; }

    }

    public class CompletePaymentResponseModel
    {

        public bool requestSuccessful { get; set; }


        public ResponseDataCompletePayment responseData { get; set; }


        public string message { get; set; }


        public string responseCode { get; set; }
    }

    public class WebHookRequestModel
    {
        public string MerchantCode { get; set; }
        public string PaymentReference { get; set; }
        public string MerchantReference { get; set; }
        public decimal AmountCollected { get; set; }
        public decimal Amount { get; set; }
        public string TransactionStatus { get; set; }
        public string CurrencyCode { get; set; }
        public string AccountNumberMasked { get; set; }
        public string Narration { get; set; }
        public string CustomerName { get; set; }
        public string PaymentDate { get; set; }
    }


    public class WemaWalletServiceModels
    {

    }

    public class WemaWalletAuthorizeTransactionWebhook
    {
        public string transactionReference { get; set; }
        public string securityInfo { get; set; }

    }


    public class WemaWalletAuthorizeTransactionWebhookResponse
    {
        public string transactionReference { get; set; }
        public bool authorized { get; set; }

    }

    public class WemaSecurityInfoPayload
    {
        public string transactionReference { get; set; }


    }


    public class WemaWalletBankListRresponse
    {
        public List<ResultList> result { get; set; }
        public string message { get; set; }
        public bool status { get; set; }
    }


    public class TransactionsCallbackModelRequest
    {
        public string message { get; set; }
        public string title { get; set; }
        public string requestType { get; set; }
        public TransactionCallbackModelData data { get; set; }
    }

    public class TransactionCallbackModelData
    {
        public string status { get; set; }
        public string message { get; set; }
        public string narration { get; set; }
        public string transactionReference { get; set; }
        public string platformTransactionReference { get; set; }
        public string transactionStan { get; set; }
        public string orinalTxnTransactionDate { get; set; }
    }
    public class WemaWalletNotificationModel
    {

        public string accountNumber { get; set; }
        public string transactionType { get; set; }
        public decimal amount { get; set; }
        public string narration { get; set; }
        public string transactionDate { get; set; }


    }

    public class ResultList
    {
        public string bankName { get; set; }
        public string bankCode { get; set; }
        public string bankLogo { get; set; }
        public string provider_slug { get; set; }
    }


    public class WemaWalletCreationNotificationModel
    {

        public string title { get; set; }
        public string message { get; set; }
        public Data data { get; set; }
        public int requestType { get; set; }
    }

    public class Data
    {
        public string email { get; set; }
        public string nuban { get; set; }
        public string nubanName { get; set; }
        public int type { get; set; }
        public string nubanStatus { get; set; }
        public string phoneNumber { get; set; }
    }



    public class WemaWalletGenerateAccountRequest
    {
        public string phoneNumber { get; set; }
        public string email { get; set; }
        public string nin { get; set; }
        public string preferred_bank { get; set; }
        public string first_name { get; set; }
        public string middle_name { get; set; }
        public string last_name { get; set; }

    }

    public class WemaWalletGenerateAccountResponse
    {
        public WemaWalletGenerateAccountResponseData data { get; set; }
        public string? message { get; set; }
        public bool? status { get; set; }
        public int? code { get; set; }
        public int? statusCode { get; set; }
        public List<string>? errors { get; set; }

    }

    public class WemaWalletGenerateAccountResponseData
    {
        public string trackingId { get; set; }
    }
    public class WemaGenerateAccountCallbackData
    {
        public string email { get; set; }
        public string nuban { get; set; }
        public string nubanName { get; set; }
        public int type { get; set; }
        public string nubanStatus { get; set; }
        public string phoneNumber { get; set; }
    }

    public class WemaGenerateAccountCallback
    {
        public string title { get; set; }
        public string message { get; set; }
        public WemaGenerateAccountCallbackData data { get; set; }
        public int requestType { get; set; }
    }



    public class WemaWalletValidateOTPRequest
    {
        public string phoneNumber { get; set; }
        public string otp { get; set; }
        public string trackingId { get; set; }
    }

    public class WemaWalletValidateOTPResponse
    {
        public string? message { get; set; }
        public bool? status { get; set; }
        public int? code { get; set; }
        public object? statusCode { get; set; }
        public List<string>? errors { get; set; }
        public WemaWalletValidateOTPResponseData data { get; set; }
    }

    public class WemaWalletValidateOTPResponseData
    {
        public string accountGenerationStatus { get; set; }
    }
    public class GetPartnerShipAccountDetails
    {
        public GetPartnerShipAccountDetailsData? data { get; set; }
        public string? message { get; set; }
        public bool? status { get; set; }
        public int? code { get; set; }
        public int? statusCode { get; set; }
    }

    public class GetPartnerShipAccountDetailsData
    {
        public string accountNumber { get; set; }
        public string firstName { get; set; }
        public string lastName { get; set; }
        public string email { get; set; }
        public string phoneNumber { get; set; }
    }



    public class WemaWalletCallback
    {
        public string title { get; set; }
        public string message { get; set; }
        public WemaWalletCallbackData data { get; set; }
        public int requestType { get; set; }
    }

    public class WemaWalletCallbackData
    {
        public string email { get; set; }
        public string nuban { get; set; }
        public string nubanName { get; set; }
        public int type { get; set; }
        public string nubanStatus { get; set; }
        public string phoneNumber { get; set; }
    }



    public class WemaAccountBalanceResponseData
    {
        public string WalletNumber { get; set; }
        public string AvailableBalance { get; set; }
        public string WalletStatus { get; set; }
        public string AccountType { get; set; }
    }

    public class WemaAccountBalanceResponse
    {
        public WemaAccountBalanceResponseData Result { get; set; }
        public bool Successful { get; set; }
        public string Message { get; set; }
    }

    public class WemaAccountTransactionHistoryRequest

    {
        public string accountNumber { get; set; }
        public string from { get; set; }
        public string to { get; set; }
        public string keyWord { get; set; }

    }

    public class WemaAccountTransactionHistoryResponseResult
    {
        public string title { get; set; }
        public decimal amount { get; set; }
        public string type { get; set; }
        public string date { get; set; }
        public string transactionDate { get; set; }
        public string narration { get; set; }
        public string status { get; set; }
        public string creditType { get; set; }
        public string sender { get; set; }
        public string senderAccountNumber { get; set; }
        public string destinationBank { get; set; }
        public string destinationAccountNumber { get; set; }
        public string recieverName { get; set; }
        public string referenceId { get; set; }
        public bool isViewReceiptEnabled { get; set; }
        public string tranId { get; set; }
    }

    public class WemaAccountTransactionHistoryResponse
    {
        public List<WemaAccountTransactionHistoryResponseResult> result { get; set; }
        public bool successful { get; set; }
        public string message { get; set; }
    }


    public class DebitWalletRequest
    {

    }

    public class DebitRestrictionRequest
    {
        // LiftPnd, PlacePnd
        public string pndType { get; set; }
        public string accountNumber { get; set; }
    }


    public class WalletAccountNameInquiryResponse
    {
        public NameInquiryResponseResult? result { get; set; }
        public string? errorMessage { get; set; }
        public List<string>? errorMessages { get; set; }
        public bool? hasError { get; set; }
        public DateTime? TimeGenerated { get; set; }
    }

    public class NameInquiryResponseResult
    {
        public string? bankCode { get; set; }
        public string? accountName { get; set; }
        public string? accountNumber { get; set; }
        public string? currency { get; set; }
        public string? channelId { get; set; }
        public string? callbackUrl { get; set; }
        public string? termsAndConditions { get; set; }
        public string? termsAndConditionsUrl { get; set; }
        public List<string>? chargeFee { get; set; }
    }


    public class CreditWalletRequestDto
    {
        public string destinationAccountNumber { get; set; }
        public int amount { get; set; }
        public string narration { get; set; }
        public string transactionReference { get; set; }
        public bool useCustomNarration { get; set; }
    }

    public class CreditWalletRequest
    {
        public string? securityInfo { get; set; }
        public string destinationAccountNumber { get; set; }
        public double amount { get; set; }
        public string narration { get; set; }
        public string transactionReference { get; set; }
        public bool useCustomNarration { get; set; }
    }
    public class ClientTransferRequest : CreditWalletRequest
    {
        public string destinationBankName { get; set; }
        public string destinationAccountName { get; set; }
        public string sourceAccountNumber { get; set; }
        public string destinationBankCode { get; set; }
        public ChannelCode channelCode { get; set; }
    }



    public class CreditWalletRequestResponse
    {
        public CreditWalletResult? result { get; set; }
        public string? errorMessage { get; set; }
        public List<string>? errorMessages { get; set; }
        public bool? hasError { get; set; }
        public string? timeGenerated { get; set; }
        public object statusCode { get; set; }
        public object message { get; set; }

    }
    public class CreditWalletResult
    {
        public string status { get; set; }
        public string message { get; set; }
        public string narration { get; set; }
        public string transactionReference { get; set; }
        public string platformTransactionReference { get; set; }
        public string transactionStan { get; set; }
        public string orinalTxnTransactionDate { get; set; }
    }

    public class ConfirmCreditWalletTransactionResponse
    {
        public ConfirmCreditWalletTransactionResult result { get; set; }
        public string errorMessage { get; set; }
        public List<string> errorMessages { get; set; }
        public bool hasError { get; set; }
        public DateTime timeGenerated { get; set; }
    }

    public class ConfirmCreditWalletTransactionData
    {
        public string status { get; set; }
        public string message { get; set; }
        public string narration { get; set; }
        public string transactionReference { get; set; }
        public string platformTransactionReference { get; set; }
        public string transactionStan { get; set; }
        public DateTime orinalTxnTransactionDate { get; set; }
    }

    public class ConfirmCreditWalletTransactionResult
    {
        public string title { get; set; }
        public string message { get; set; }
        public ConfirmCreditWalletTransactionData data { get; set; }
        public int request { get; set; }
    }

    public class NipCharges
    {
        public NipChargesResult result { get; set; }
        public string errorMessage { get; set; }
        public List<string> errorMessages { get; set; }
        public bool hasError { get; set; }
        public DateTime timeGenerated { get; set; }
    }

    public class NipChargesResult
    {
        public List<NipChargesResultFee> chargeFees { get; set; }
        public string termsAndConditions { get; set; }
        public string termsAndConditionsUrl { get; set; }
    }

    public class NipChargesResultFee
    {
        public int id { get; set; }
        public string chargeFeeName { get; set; }
        public int transactionType { get; set; }
        public double charge { get; set; }
        public double lower { get; set; }
        public double upper { get; set; }
    }


    public class GetAccountDetails
    {
        public GetAccountDetailsResult? result { get; set; }
        public bool? successful { get; set; }
        public string? message { get; set; }
    }

    public class GetAccountDetailsResult
    {
        public string? walletNumber { get; set; }
        public string? availableBalance { get; set; }
        public string? accountType { get; set; }
    }
}
