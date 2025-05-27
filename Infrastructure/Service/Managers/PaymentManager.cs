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
using OCPG.Core.Models.Entities;
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
        private readonly IWalletRepository walletRepository;

        public PaymentManager(ICardSwitcher cardSwitcher,
        AppUrl appUrl,
        AuthConfig authConfig,
        IPaymentRepository paymentRepository,
        IWalletRepository walletRepository
        )
        {
            this.cardSwitcher = cardSwitcher;
            this.appUrl = appUrl;
            this.authConfig = authConfig;
            this.paymentRepository = paymentRepository;
            this.walletRepository = walletRepository;
        }

        public async Task<serviceResponse<PaymentTransactions>> GetTransactionStatus(string adviceReference)
        {
            serviceResponse<PaymentTransactions> res = new serviceResponse<PaymentTransactions>();

            var payres = await paymentRepository.GetPaymentByAdviceReference(adviceReference);
            res.Data = payres;

            return res;
        }
        public async Task<serviceResponse<PaymentTransactions>> GetTransactionStatusByPaymentReference(string paymentReference)
        {
            serviceResponse<PaymentTransactions> res = new serviceResponse<PaymentTransactions>();

            var payres = await paymentRepository.GetPaymentByPaymentReference(paymentReference);
            res.Data = payres;

            return res;
        }

        public async Task<serviceResponse<AdviceResponseModel>> InitiateTransaction(AdviceModelReq advice, ChannelCode channelCode)
        {
            var existingTransaction = await paymentRepository.GetPaymentByMerchantference(advice.merchantRef);
            if (existingTransaction != null)
            {
                return new serviceResponse<AdviceResponseModel>
                {
                    Message = "Transaction already exists",
                    Data = null
                };
            }
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
                        narration = resp.Data.responseData.narration,
                        processor = resp.Data.responseData.processor,
                    };
                    var isCreatedPayment = await paymentRepository.CreatePayment(payment);
                }
                catch (Exception e)
                {
                    resp.Message = $"{e.Message}";
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
                        authMode = resp.responseData.authMode,
                        authFields = JsonSerializer.Serialize(resp.responseData.authFields),
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
                        paymentReference = resp.responseData.paymentReference != null ? resp.responseData.paymentReference : "",
                        adviceReference = resp.responseData.adviceReference,
                        amountCollected = (double)resp.responseData.amountCollected,
                        transactionStatus = resp.responseData.transactionStatus,
                        responsePayload = JsonSerializer.Serialize(resp),
                        accountNumberMasked = resp.responseData.accountNumberMasked,
                        merchantCode = resp.responseData.merchantCode,
                        processor_message = resp.responseData.processor_message ?? "",
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

        public async Task<CompletePaymentResponseModel> ValidateCardPayment(ValidatePayment cardDeetails, ValidateCardPaymentChannelCode cc)
        {
            CompletePaymentResponseModel res = new CompletePaymentResponseModel();

            ChannelCode channel = (ChannelCode)cc;

            var processor = cardSwitcher.SwitchCardProcessor(channel);
            if (processor == null)
            {
                res.message = "There is no such operation for the selected channel";
                return res;
            }
            var resp = await processor.ValidateCardPayment(cardDeetails);
            if (resp.requestSuccessful == true)
            {
                try
                {
                    PaymentTransactions payment = new PaymentTransactions
                    {
                        paymentReference = resp.responseData.paymentReference != null ? resp.responseData.paymentReference : "",
                        adviceReference = resp.responseData.adviceReference,
                        amountCollected = (double)resp.responseData.amountCollected,
                        transactionStatus = resp.responseData.transactionStatus,
                        responsePayload = JsonSerializer.Serialize(resp),
                        accountNumberMasked = resp.responseData.accountNumberMasked,
                        merchantCode = resp.responseData.merchantCode,
                        processor_message = resp.responseData.processor_message ?? "",
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


        ///////////////WALLET MODULE ///////////////////WALLET MODULE ///////////


       




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


        public async Task<WalletAccountNameInquiryResponse> NameEnquiry(string accountNumber)
        {
            WalletAccountNameInquiryResponse response = new WalletAccountNameInquiryResponse();
            var wallet = await walletRepository.GetWalletByAccountNumber(accountNumber);
            if (wallet != null)
            {
                response.result = new NameInquiryResponseResult
                {
                    accountName = wallet.account_name,
                    accountNumber = wallet.account_number,
                };

            }
            return response;

        }

        public async Task<GetAccountDetails> GetAccountDetails(string accountNumber)
        {
            var response = new GetAccountDetails();
            var wallet = await walletRepository.GetWalletByAccountNumber(accountNumber);
            if (wallet != null)
            {
                response.result = new GetAccountDetailsResult
                {
                    accountName = wallet.account_name,
                    accountNumber = wallet.account_number,
                    accountBalance = wallet.account_balance,
                    accountMandate = wallet.account_mandate,
                    accountType = wallet.account_type,
                    walletProvider = wallet.wallet_provider,
                    accountTrackerId = wallet.acccount_trackerId,
                    accountTrackerRef = wallet.acccount_trackerRef,
                    createdAt = wallet.created_at.ToString("yyyy-MM-dd HH:mm:ss")

                };
            }
            return response;
        }

 
        public async Task<serviceResponse<List<WalletTransactionHistory>>> GetWalletTransactionHistory(WemaAccountTransactionHistoryRequest model)
        {
            return await walletRepository.GetWalletTransactionHistory(model);
        }

        public async Task<serviceResponse<string>> LayMandateOnAccount(string accountNumber, double mandateAmount)
        {
            var res = new serviceResponse<string>();
            try
            {
                var response = await walletRepository.LayMandateOnWallet(accountNumber, mandateAmount);
                res.Message = response.Message;
            }
            catch (Exception e)
            {
                res.Message = $"An error occurred while laying mandate on account: {e.Message}";
            }
            return res;
        }

         public async Task<serviceResponse<string>> SubtractMandate(string accountNumber, double mandateAmount)
        {
            var res = new serviceResponse<string>();
            try
            {
                var response = await walletRepository.SubtractMandate(accountNumber, mandateAmount);
                res.Message = response.Message;
            }
            catch (Exception e)
            {
                res.Message = $"An error occurred while laying mandate on account: {e.Message}";
            }
            return res;
        }

        public async Task<serviceResponse<string>> InitiateWithrawals(WithdrawFromWallet payload)
        {
            return await walletRepository.InitiateWithrawals(payload);
        }


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
        public async Task<NipCharges> GetNipCharges(ChannelCode channelCode, double amount,PaymentType payment_type, Currency currency)
        {
            var processor = cardSwitcher.SwitchCardProcessor(channelCode);
            if (processor == null)
            {
                throw new BadHttpRequestException("There is no such operation for the selected channel");
            }
            var updatedTransaction = await processor.GetNipCharges(channelCode, amount,payment_type, currency);
            return updatedTransaction;
        }
        public async Task<CreditWalletRequestResponse> ProcessClientTransfer(ClientTransferRequest model)
        {
            return await walletRepository.ProcessWalletToWalletTransfer(model);
        }

        public async Task<string> ProcessInternalTransferFromWalletProviderToBankAccount(WithdrawFromWallet payload, ChannelCode channelCode)
        {
         var processor = cardSwitcher.SwitchCardProcessor(channelCode);
            if (processor == null)
            {
                throw new BadHttpRequestException("There is no such operation for the selected channel");
            }
            var updatedTransaction = await processor.ProcessInternalTransferFromWalletProviderToBankAccount(payload);
            return updatedTransaction;
        }


        ///////////////WALLET MODULE ///////////////////WALLET MODULE ///////////

        /// WEBHOOK
        public async Task<string> WebHookNotification(string stream, ChannelCode channel)
        {
            var processor = cardSwitcher.SwitchCardProcessor(channel);
            if (processor == null)
            {
                return $"There is no such operation for the selected channel";
            }
            return await processor.WebHookNotification(stream);
        }


    }
}