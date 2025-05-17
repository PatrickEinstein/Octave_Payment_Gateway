using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Channels;
using System.Threading.Tasks;
using CentralPG.Core.Models.Entities;
using CentralPG.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using OCPG.Core.Enums;
using OCPG.Core.Models;
using OCPG.Infrastructure.Interfaces.IManagers;
using OCPG.Infrastructure.Interfaces.IRepositories;
using OCPG.Infrastructure.Interfaces.ISwitches;
using OCPG.Infrastructure.Service.Repositories;
using OCPG.Models;

namespace OCPG.Infrastructure.Service.Managers
{
    public class PaymentManager : IPaymentManager
    {
        private readonly ICardSwitcher cardSwitcher;
        private readonly AppUrl appUrl;
        private readonly AuthConfig authConfig;
        private readonly IPaymentRepository paymentRepository;

        public PaymentManager(ICardSwitcher cardSwitcher, AppUrl appUrl, AuthConfig authConfig, IPaymentRepository paymentRepository)
        {
            this.cardSwitcher = cardSwitcher;
            this.appUrl = appUrl;
            this.authConfig = authConfig;
            this.paymentRepository = paymentRepository;
        }

        public async Task<serviceResponse<PaymentTransactions>> GetTransactionStatus(string adviceReference, ChannelCode channel)
        {
            serviceResponse<PaymentTransactions> res = new serviceResponse<PaymentTransactions>();

            var payres = await paymentRepository.GetPaymentByAdviceReference(adviceReference);
            res.Data = payres;

            return res;
        }

        public async Task<serviceResponse<AdviceResponseModel>> InitiateTransaction(AdviceModelReq advice, ChannelCode channelCode)
        {
            serviceResponse<AdviceResponseModel> res = new serviceResponse<AdviceResponseModel>();

            AdviceModel load = new AdviceModel(appUrl, authConfig)
            {
                merchantRef = advice.merchantRef,
                amount = advice.amount,
                currency = advice.currency,
                narration = advice.narration,
                customer = advice.customer,
                channel = advice.channel,
                callBackUrl = advice.callBackUrlDomain + "status/",
            };
            var processor = cardSwitcher.SwitchCardProcessor(channelCode);
            if (processor == null)
            {
                res.Message = "There is no such operation for the selected channel";
                return res;
            }
            var resp = await processor.GenerateAdvice(load);
            if (resp.Data != null && resp.Data.requestSuccessful == true)
            {
                try
                {
                    PaymentTransactions payment = new PaymentTransactions
                    {
                        adviceReference = resp.Data.responseData.adviceReference,
                        amount = resp.Data.responseData.amount,
                        currencyCode = resp.Data.responseData.currency,
                        customerName = resp.Data.responseData.customerFullName,
                        merchantReference = resp.Data.responseData.merchantRef,
                        transactionStatus = "Pending",
                        requestPayload = JsonSerializer.Serialize(advice),
                        responsePayload = JsonSerializer.Serialize(resp),
                        notificationUrl = advice.notificationUrl,
                        callbackUrl = advice.callBackUrlDomain + "status/",
                        paymentDate = DateTime.Now.ToString(),
                        narration = resp.Data.responseData.narration
                    };
                    var isCreatedPayment = await paymentRepository.CreatePayment(payment);
                }
                catch (Exception e)
                {
                    Console.WriteLine($"saving payment failed ==> {e.Message}");
                }
            }
            return resp;
        }



        //CARD PAYMENT
        public async Task<ProcessCardResponseModel> ProcessCardPayment(CardPayment cardDeetails, string adviceReference, ChannelCode channelCode)
        {
            ProcessCardResponseModel res = new ProcessCardResponseModel();

            var processor = cardSwitcher.SwitchCardProcessor(channelCode);
            if (processor == null)
            {
                res.message = "There is no such operation for the selected channel";
                return res;
            }
            var resp = await processor.ProcessCardPayment(cardDeetails, adviceReference);
            if (resp.requestSuccessful == true)
            {
                try
                {
                    PaymentTransactions payment = new PaymentTransactions
                    {
                        paymentReference = resp.responseData.paymentReference,
                        amountCollected = (double)resp.responseData.amountCollected,
                        transactionStatus = resp.responseData.transactionStatus,
                        responsePayload = JsonSerializer.Serialize(resp),
                        accountNumberMasked = resp.responseData.accountNumberMasked,
                        merchantCode = resp.responseData.merchantCode,
                        adviceReference = adviceReference,
                    };
                    var isCreatedPayment = await paymentRepository.UpdatePayment(payment);
                }
                catch (Exception e)
                {
                    Console.WriteLine($"saving payment failed ==> {e.Message}");
                }
            }
            return resp;
        }

        public async Task<CompletePaymentResponseModel> CompleteCardPayment(CompleteCardPayment cardDeetails, ChannelCode channelCode)
        {
            CompletePaymentResponseModel res = new CompletePaymentResponseModel();

            var processor = cardSwitcher.SwitchCardProcessor(channelCode);
            if (processor == null)
            {
                res.message = "There is no such operation for the selected channel";
                return res;
            }
            var resp = await processor.CompleteCardPayment(cardDeetails);
            if (resp.requestSuccessful == true)
            {
                try
                {
                    PaymentTransactions payment = new PaymentTransactions
                    {
                        paymentReference = resp.responseData.paymentReference,
                        amountCollected = (double)resp.responseData.amountCollected,
                        transactionStatus = resp.responseData.transactionStatus,
                        responsePayload = JsonSerializer.Serialize(resp),
                        accountNumberMasked = resp.responseData.accountNumberMasked,
                        merchantCode = resp.responseData.merchantCode,
                        // adviceReference = cardDeetails.a,
                    };
                    var isCreatedPayment = await paymentRepository.UpdatePayment(payment);
                }
                catch (Exception e)
                {
                    Console.WriteLine($"saving payment failed ==> {e.Message}");
                }
            }
            return resp;
        }



        // BANK PAYMENT
        public async Task<serviceResponse<AdviceResponseModel>> CompleteBankPayment(CompleteCardPayment cardDetails, ChannelCode channelCode)
        {
            serviceResponse<AdviceResponseModel> res = new serviceResponse<AdviceResponseModel>();
            var processor = cardSwitcher.SwitchCardProcessor(channelCode);
            if (processor == null)
            {
                res.Message = "There is no such operation for the selected channel";
                return res;
            }
            var resp = await processor.CompleteBankPayment(cardDetails);
            return resp;
        }


        public async Task<ProcessBankPaymentResponseModel> ProcessBankPayment(BankPayment cardDetails, string adviceReference, ChannelCode channelCode)
        {
            ProcessBankPaymentResponseModel res = new ProcessBankPaymentResponseModel();
            var processor = cardSwitcher.SwitchCardProcessor(channelCode);
            if (processor == null)
            {
                res.message = "There is no such operation for the selected channel";
                return res;
            }
            var resp = await processor.ProcessBankPayment(cardDetails, adviceReference);
            return resp;
        }
        public async Task<BankTransferResponseModel> GenerateBankAccount(GenerateBankAccount model, ChannelCode channelCode)
        {
            BankTransferResponseModel _ = new BankTransferResponseModel();
            var processor = cardSwitcher.SwitchCardProcessor(channelCode);
            if (processor == null)
            {
                _.message = "There is no such operation for the selected channel";
                return _;
            }
            var resp = await processor.GenerateNewDynamicAccount(model.adviceReference);
            return resp;
        }

        public async Task<string> WebHookNotification(WebHookRequestModel cardDetails, ChannelCode channel)
        {
            var processor = cardSwitcher.SwitchCardProcessor(channel);
            if (processor == null)
            {
                return $"There is no such operation for the selected channel";
            }
            var updatedTransaction = await paymentRepository.UpdateChamsSwitchWebhook(cardDetails);
            return await processor.WebHookNotification(cardDetails);
        }

        ///////////////WALLET MODULE ///////////////////WALLET MODULE ///////////


        //////////// QUERY MODULE //////////////////

        public async Task<WalletAccountNameInquiryResponse> NameEnquiry(string accountNumber, ChannelCode channelCode)
        {
            var processor = cardSwitcher.SwitchCardProcessor(channelCode);
            if (processor == null)
            {
                throw new BadHttpRequestException("There is no such operation for the selected channel");
            }
            var updatedTransaction = await processor.NameEnquiry(accountNumber);
            return updatedTransaction;
        }

        public async Task<CreditWalletRequestResponse> ConfirmClientTransferStatus(string clientTransactionReference, ChannelCode channelCode)
        {
            var processor = cardSwitcher.SwitchCardProcessor(channelCode);
            if (processor == null)
            {
                throw new BadHttpRequestException("There is no such operation for the selected channel");
            }
            var updatedTransaction = await processor.ConfirmClientTransferStatus(clientTransactionReference);
            return updatedTransaction;
        }
        public async Task<GetAccountDetails> GetAccountDetails(string accountNumber, ChannelCode channelCode)
        {
            var processor = cardSwitcher.SwitchCardProcessor(channelCode);
            if (processor == null)
            {
                throw new BadHttpRequestException("There is no such operation for the selected channel");
            }
            var updatedTransaction = await processor.GetAccountDetails(accountNumber);
            return updatedTransaction;
        }
        public async Task<string> GetWalletTransactionHistory(WemaAccountTransactionHistoryRequest model, ChannelCode channelCode)
        {
            var processor = cardSwitcher.SwitchCardProcessor(channelCode);
            if (processor == null)
            {
                throw new BadHttpRequestException("There is no such operation for the selected channel");
            }
            var updatedTransaction = await processor.GetWalletTransactionHistory(model);
            return updatedTransaction;
        }


        //////////// CREATE WALLET MODULE //////////////////



        public async Task<WemaWalletGenerateAccountResponse> GenerateWalletAccount(WemaWalletGenerateAccountRequest payload, ChannelCode channelCode)
        {
            var processor = cardSwitcher.SwitchCardProcessor(channelCode);
            if (processor == null)
            {
                throw new BadHttpRequestException("There is no such operation for the selected channel");
            }
            var updatedTransaction = await processor.GenerateWalletAccount(payload);
            return updatedTransaction;
        }

        //////////// CREATE WALLET MODULE //////////////////


        //////////// DEBIT WALLET MODULE //////////////////
        public async Task<WemaWalletBankListRresponse> GetAllBanks(ChannelCode channelCode)
        {
            var processor = cardSwitcher.SwitchCardProcessor(channelCode);
            if (processor == null)
            {
                throw new BadHttpRequestException("There is no such operation for the selected channel");
            }
            var updatedTransaction = await processor.GetAllBanks();
            return updatedTransaction;
        }
        public async Task<NipCharges> GetNipCharges(ChannelCode channelCode)
        {
            var processor = cardSwitcher.SwitchCardProcessor(channelCode);
            if (processor == null)
            {
                throw new BadHttpRequestException("There is no such operation for the selected channel");
            }
            var updatedTransaction = await processor.GetNipCharges();
            return updatedTransaction;
        }
        public async Task<CreditWalletRequestResponse> ProcessClientTransfer(ClientTransferRequest model, ChannelCode channelCode)
        {
            var processor = cardSwitcher.SwitchCardProcessor(channelCode);
            if (processor == null)
            {
                throw new BadHttpRequestException("There is no such operation for the selected channel");
            }
            var updatedTransaction = await processor.ProcessClientTransfer(model);
            return updatedTransaction;
        }

        ///////////////WALLET MODULE ///////////////////WALLET MODULE ///////////

    }
}