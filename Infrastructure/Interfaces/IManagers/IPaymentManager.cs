using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CentralPG.Core.Models.Entities;
using CentralPG.Models;
using OCPG.Core.Enums;
using OCPG.Core.Models;
using OCPG.Core.Models.Entities;
using OCPG.Models;

namespace OCPG.Infrastructure.Interfaces.IManagers
{
    public interface IPaymentManager
    {
        Task<serviceResponse<PaymentTransactions>> GetTransactionStatus(string adviceReference);
        Task<serviceResponse<PaymentTransactions>> GetTransactionStatusByPaymentReference(string paymentReference);
        Task<serviceResponse<AdviceResponseModel>> InitiateTransaction(AdviceModelReq advice, ChannelCode channel);
        Task<ProcessCardResponseModel> ProcessCardPayment(CardPayment cardDeetails, string adviceReference, ChannelCode channel);
        Task<CompletePaymentResponseModel> CompleteCardPayment(CompleteCardPayment cardDeetails, ChannelCode channelCode);
        Task<CompletePaymentResponseModel> ValidateCardPayment(ValidatePayment cardDeetails, ValidateCardPaymentChannelCode channelCode);
        Task<ProcessBankPaymentResponseModel> ProcessBankPayment(BankPayment cardDetails, string adviceReference, ChannelCode channelCode);
        Task<serviceResponse<AdviceResponseModel>> CompleteBankPayment(CompleteCardPayment cardDetails, ChannelCode channelCode);
        Task<BankTransferResponseModel> GenerateBankAccount(GenerateBankAccount model, ChannelCode channelCode);

        ///////////////WALLET MODULE ///////////////////WALLET MODULE ///////////
        Task<serviceResponse<List<WalletTransactionHistory>>> GetWalletTransactionHistory(WemaAccountTransactionHistoryRequest model);
        Task<WemaWalletGenerateAccountResponse> GenerateWalletAccount(WemaWalletGenerateAccountRequest payload, ChannelCode channelCode);
        // Task<WemaWalletValidateOTPResponse> ValidateAccountwithOtp(WemaWalletValidateOTPRequest payload);

        Task<serviceResponse<string>> LayMandateOnAccount(string accountNumber, double mandateAmount);
          Task<serviceResponse<string>> SubtractMandate(string accountNumber, double mandateAmount);
        Task<serviceResponse<string>> InitiateWithrawals(WithdrawFromWallet payload);
        Task<WemaWalletBankListRresponse> GetAllBanks(ChannelCode channelCode);
        Task<NipCharges> GetNipCharges(ChannelCode channelCode, double amount,PaymentType payment_type, Currency currency);
        Task<WalletAccountNameInquiryResponse> NameEnquiry(string accountNumber);
        
        Task<GetAccountDetails> GetAccountDetails(string accountNumber);
        Task<CreditWalletRequestResponse> ProcessClientTransfer(ClientTransferRequest model);
Task<string> ProcessInternalTransferFromWalletProviderToBankAccount(WithdrawFromWallet payload, ChannelCode channelCode);
        Task<string> WebHookNotification(string stream, ChannelCode channelCode);

    }
}