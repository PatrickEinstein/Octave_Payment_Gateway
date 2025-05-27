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
using OCPG.Core.Models;
using OCPG.Core.Enums;

namespace CentralPG.Interfaces.IProcessors
{
    public interface IPaymentProcessor
    {
        Task<serviceResponse<AuthTokens>> Login();
        Task<serviceResponse<GetAdviceModel>> GetAdvice(string adviceReference);
        Task<serviceResponse<AdviceResponseModel>> GenerateAdvice(AdviceModel advice);
        Task<ProcessCardResponseModel> ProcessCardPayment(CardPayment cardDeetails, string adviceReference);
        Task<CompletePaymentResponseModel> CompleteCardPayment(CompleteCardPayment cardDeetails);
        Task<CompletePaymentResponseModel> ValidateCardPayment(ValidatePayment cardDeetails);

        Task<ProcessBankPaymentResponseModel> ProcessBankPayment(BankPayment cardDetails, string adviceReference);
        Task<serviceResponse<AdviceResponseModel>> CompleteBankPayment(CompleteCardPayment cardDetails);
        Task<BankTransferResponseModel> GenerateNewDynamicAccount(string merchantReference);
        Task<string> WebHookNotification(string stream);



        ///////////////WALLET MODULE ///////////////////WALLET MODULE ///////////
       
        Task<WemaWalletGenerateAccountResponse> GenerateWalletAccount(WemaWalletGenerateAccountRequest payload);
        Task<string> ProcessInternalTransferFromWalletProviderToBankAccount(WithdrawFromWallet payload);
        
        Task<WemaWalletBankListRresponse> GetAllBanks();
        Task<NipCharges> GetNipCharges(ChannelCode channelCode, double amount,PaymentType payment_type, Currency currency);
  
    }
}