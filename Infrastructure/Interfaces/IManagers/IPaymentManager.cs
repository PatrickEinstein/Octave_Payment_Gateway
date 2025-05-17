using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CentralPG.Core.Models.Entities;
using CentralPG.Models;
using OCPG.Core.Enums;
using OCPG.Models;

namespace OCPG.Infrastructure.Interfaces.IManagers
{
    public interface IPaymentManager
    {
        Task<serviceResponse<PaymentTransactions>> GetTransactionStatus(string adviceReference, ChannelCode channel);
        Task<serviceResponse<AdviceResponseModel>> InitiateTransaction(AdviceModelReq advice, ChannelCode channel);
        Task<ProcessCardResponseModel> ProcessCardPayment(CardPayment cardDeetails, string adviceReference,ChannelCode channel);
        Task<CompletePaymentResponseModel> CompleteCardPayment(CompleteCardPayment cardDeetails, ChannelCode channelCode);
        Task<ProcessBankPaymentResponseModel> ProcessBankPayment(BankPayment cardDetails, string adviceReference, ChannelCode channelCode);
        Task<serviceResponse<AdviceResponseModel>> CompleteBankPayment(CompleteCardPayment cardDetails, ChannelCode channelCode);
        Task<BankTransferResponseModel> GenerateBankAccount(GenerateBankAccount model, ChannelCode channelCode);
        Task<string> WebHookNotification(WebHookRequestModel cardDetails, ChannelCode channelCode);

        ///////////////WALLET MODULE ///////////////////WALLET MODULE ///////////
        Task<WalletAccountNameInquiryResponse> NameEnquiry(string accountNumber, ChannelCode channelCode);
        Task<CreditWalletRequestResponse> ConfirmClientTransferStatus(string clientTransactionReference, ChannelCode channelCode);
        Task<GetAccountDetails> GetAccountDetails(string accountNumber, ChannelCode channelCode);
        Task<string> GetWalletTransactionHistory(WemaAccountTransactionHistoryRequest model, ChannelCode channelCode);
        Task<WemaWalletGenerateAccountResponse> GenerateWalletAccount(WemaWalletGenerateAccountRequest payload, ChannelCode channelCode);
        // Task<WemaWalletValidateOTPResponse> ValidateAccountwithOtp(WemaWalletValidateOTPRequest payload);
        Task<WemaWalletBankListRresponse> GetAllBanks(ChannelCode channelCode);
        Task<NipCharges> GetNipCharges(ChannelCode channelCode);
        Task<CreditWalletRequestResponse> ProcessClientTransfer(ClientTransferRequest model, ChannelCode channelCode);
    }
}