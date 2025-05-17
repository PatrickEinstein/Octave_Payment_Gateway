using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Net.Http;
using Microsoft.AspNetCore.Mvc;
using CentralPG.Models;
using CentralPG.Interfaces;
using OCPG.Models;
using CentralPG.Core.Models.Entities;

namespace CentralPG.Interfaces.IProcessors
{
    public interface IPaymentProcessor
    {
        Task<serviceResponse<AuthTokens>> Login();
        Task<serviceResponse<GetAdviceModel>> GetAdvice(string adviceReference);
        Task<serviceResponse<AdviceResponseModel>> GenerateAdvice(AdviceModel advice);
        Task<ProcessCardResponseModel> ProcessCardPayment(CardPayment cardDeetails, string adviceReference);
        Task<CompletePaymentResponseModel> CompleteCardPayment(CompleteCardPayment cardDeetails);
        Task<ProcessBankPaymentResponseModel> ProcessBankPayment(BankPayment cardDetails, string adviceReference);
        Task<serviceResponse<AdviceResponseModel>> CompleteBankPayment(CompleteCardPayment cardDetails);
        Task<BankTransferResponseModel> GenerateNewDynamicAccount(string merchantReference);
        Task<string> WebHookNotification(WebHookRequestModel cardDetails);



        ///////////////WALLET MODULE ///////////////////WALLET MODULE ///////////
        Task<WalletAccountNameInquiryResponse> NameEnquiry(string accountNumber);
        Task<CreditWalletRequestResponse> ConfirmClientTransferStatus(string clientTransactionReference);
        Task<GetAccountDetails> GetAccountDetails(string accountNumber);
        Task<string> GetWalletTransactionHistory(WemaAccountTransactionHistoryRequest model);
        Task<WemaWalletGenerateAccountResponse> GenerateWalletAccount(WemaWalletGenerateAccountRequest payload);
        // Task<WemaWalletValidateOTPResponse> ValidateAccountwithOtp(WemaWalletValidateOTPRequest payload);
        Task<WemaWalletBankListRresponse> GetAllBanks();
        Task<NipCharges> GetNipCharges();
        Task<CreditWalletRequestResponse> ProcessClientTransfer(ClientTransferRequest model);

    }
}