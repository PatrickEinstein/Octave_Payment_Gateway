using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using CentralPG.Core.Models.Entities;
using CentralPG.Data;
using CentralPG.Infrasturcture.Interfaces.Utilities;
using CentralPG.Interfaces.IProcessors;
using CentralPG.Models;
using Microsoft.AspNetCore.Authorization;
using OCPG.Core.Models;
using OCPG.Infrastructure.Interfaces.ICryptographies;
using OCPG.Infrastructure.Interfaces.IRepositories;
using OCPG.Infrastructure.Service.Repositories;
using OCPG.Models;

namespace OCPG.Infrastructure.Service.Processors
{
    public class FlutterWave : IPaymentProcessor
    {
        public IApiCaller ApiCaller { get; set; }

        private IDictionary<string, string> headers = new Dictionary<string, string>
        {
            { "Accept", "application/json" },
        };
        private readonly FlutterWaveAppUrls appUrl;

        private readonly FlutterAuthConfig authConfig;
        private readonly IPaymentRepository paymentRepository;
        private readonly DataBaseContext dataBaseContext;
        private readonly IFlutterCryptography flutterCryptography;
        private readonly ICardRepository cardRepository;

        public FlutterWave(FlutterWaveAppUrls appUrl,
        IApiCaller apiCaller,
        FlutterAuthConfig authConfig,
        IPaymentRepository paymentRepository,
        DataBaseContext dataBaseContext,
        IFlutterCryptography flutterCryptography,
        ICardRepository cardRepository

        )
        {
            this.ApiCaller = apiCaller;
            this.authConfig = authConfig;
            this.paymentRepository = paymentRepository;
            this.dataBaseContext = dataBaseContext;
            this.flutterCryptography = flutterCryptography;
            this.cardRepository = cardRepository;
            this.appUrl = appUrl;
        }


        public async Task<serviceResponse<AuthTokens>> Login()
        {
            throw new NotImplementedException();
        }

        public async Task<serviceResponse<GetAdviceModel>> GetAdvice(string adviceReference)
        {
            serviceResponse<GetAdviceModel> serviceResponse = new serviceResponse<GetAdviceModel>();
            try
            {
                var payment = await paymentRepository.GetPaymentByAdviceReference(adviceReference);
                var customer = JsonSerializer.Deserialize<AdviceModel>(payment.requestPayload);
                serviceResponse.Data = new GetAdviceModel
                {
                    requestSuccessful = true,
                    responseData = new ResponseData
                    {
                        adviceReference = payment.adviceReference,
                        merchantRef = payment.merchantReference,
                        amount = (decimal)payment.amount,
                        currency = payment.currencyCode,
                        customerFullName = $"{customer.customer.CustomerFirstName ?? ""} {customer.customer.CustomerLastName ?? ""}",
                        status = payment.transactionStatus,
                        narration = payment.narration,
                        charge = 0,
                        processor = payment.processor,
                    }

                };
                return serviceResponse;
            }
            catch (Exception ex)
            {
                serviceResponse.Message = $"{ex.Message}";
                return serviceResponse;
            }
        }

        public async Task<serviceResponse<AdviceResponseModel>> GenerateAdvice(AdviceModel advice)
        {
            serviceResponse<AdviceResponseModel> serviceResponse = new serviceResponse<AdviceResponseModel>();

            try
            {
                serviceResponse.Data = new AdviceResponseModel
                {
                    requestSuccessful = true,
                    responseData = new AdviceData
                    {
                        adviceReference = Guid.NewGuid().ToString(),

                        amount = (double)advice.amount,
                        currency = advice.currency,
                        customerFullName = $"{advice.customer.CustomerFirstName ?? ""} {advice.customer.CustomerLastName ?? ""}",
                        merchantRef = advice.merchantRef,
                        status = "Pending",
                        narration = advice.narration,
                        channel = ["Card", "BankTransfer", "USSD"],
                        processor = "Flutterwave",
                    }
                };
                return serviceResponse;
            }
            catch (Exception ex)
            {
                serviceResponse.Message = $"{ex.Message}";
                return serviceResponse;
            }
        }

        public async Task<ProcessCardResponseModel> ProcessCardPayment(CardPayment cardDeetails, string adviceReference)
        {
            ProcessCardResponseModel resApi = new ProcessCardResponseModel();
            try
            {
                string apiUrl = "https://api.flutterwave.com/v3/charges?type=card";
                var payment = await paymentRepository.GetPaymentByAdviceReference(adviceReference);
                var customer = JsonSerializer.Deserialize<AdviceModelReq>(payment.requestPayload);
                var newPayload = new
                {
                    card_number = cardDeetails.cardNumber,
                    expiry_month = cardDeetails.expiredMonth,
                    expiry_year = cardDeetails.expiredYear,
                    cvv = cardDeetails.cvv,
                    currency = "NGN",
                    amount = payment.amount,
                    email = customer.customer.CustomerEmail,
                    fullname = payment.customerName,
                    phone_number = customer.customer.CustomerPhoneNumber,
                    tx_ref = adviceReference,
                    redirect_url = payment.callbackUrl,

                };
                var Load = new StringContent(JsonSerializer.Serialize(new
                {
                    client = flutterCryptography.EncryptFlutter3DESAlgo(JsonSerializer.Serialize(newPayload), authConfig.encryptionKey),
                }), Encoding.UTF8, "application/json");

                var res = await ApiCaller.POST(Load, apiUrl, authConfig.clientSecret, headers);
                var flutterResponse = JsonSerializer.Deserialize<FlutterBaseModel<FlutterChargeResponse, meta>>(res);

                resApi = new ProcessCardResponseModel
                {
                    requestSuccessful = flutterResponse.status == "success" ? true : false,
                    message = flutterResponse.message,
                    responseData = new ProcessCardData
                    {
                        amount = (decimal)payment.amount,
                        transactionStatus = $"{flutterResponse.meta.authorization.mode}",
                        accountNumberMasked = cardDeetails.cardNumber.Substring(0, 2) + "********" + cardDeetails.cardNumber.Substring(8),
                        authMode = flutterResponse.meta.authorization.mode,
                        authFields = flutterResponse.meta.authorization.fields,
                        paymentLink = flutterResponse.meta.authorization.redirect != null ? flutterResponse.meta.authorization.redirect : "",
                        merchantCode = "",
                        paymentReference = adviceReference,
                    }
                };
                var cardPayload = new
                {
                    card_number = cardDeetails.cardNumber,
                    expiry_month = cardDeetails.expiredMonth,
                    expiry_year = cardDeetails.expiredYear,
                    cvv = cardDeetails.cvv,
                    currency = "NGN",
                    amount = payment.amount,
                    email = customer.customer.CustomerEmail,
                    fullname = payment.customerName,
                    phone_number = customer.customer.CustomerPhoneNumber,
                    tx_ref = adviceReference,
                    redirect_url = payment.callbackUrl,
                    authorization = new validatPaymentAuth
                    {
                        mode = flutterResponse.meta.authorization.mode,
                        pin = "",
                        city = "",
                        address = "",
                        state = "",
                        country = "",
                        zipcode = ""
                    }
                };
               
                await cardRepository.CreateCard(adviceReference, Convert.ToBase64String(flutterCryptography.EncryptAES(JsonSerializer.Serialize(cardPayload))));

                return resApi;
            }
            catch (Exception ex)
            {
                resApi.message = $"{ex.Message}";
                return resApi;
            }
        }

        public async Task<CompletePaymentResponseModel> CompleteCardPayment(CompleteCardPayment cardDeetails)
        {
            CompletePaymentResponseModel serviceResponse = new CompletePaymentResponseModel();

            string apiUrl = "https://api.flutterwave.com/v3/charges?type=card";
            var cardToken = await cardRepository.GetCardByAdviceReference(cardDeetails.paymentreference);
            var decryptedCard = flutterCryptography.DecryptAES(Convert.FromBase64String(cardToken.token));
            var card = JsonSerializer.Deserialize<cardToken>(decryptedCard);
            var newPayload = new
            {
                card_number = card.card_number,
                expiry_month = card.expiry_month,
                expiry_year = card.expiry_year,
                cvv = card.cvv,
                currency = "NGN",
                amount = card.amount,
                email = card.email,
                fullname = card.fullname,
                phone_number = card.phone_number,
                tx_ref = card.tx_ref,
                redirect_url = card.redirect_url,
                authorization = new validatPaymentAuth
                {
                    mode = card.authorization.mode,
                    pin = cardDeetails.value,
                    city = cardDeetails.city,
                    address = cardDeetails.address,
                    state = cardDeetails.state,
                    country = cardDeetails.country,
                    zipcode = cardDeetails.zipcode
                }
            };
            try
            {
                var Load = new StringContent(JsonSerializer.Serialize(new
                {
                    client = flutterCryptography.EncryptFlutter3DESAlgo(JsonSerializer.Serialize(newPayload), authConfig.encryptionKey),
                }), Encoding.UTF8, "application/json");

                var res = await ApiCaller.POST(Load, apiUrl, authConfig.clientSecret, headers);
                var flutterResponse = JsonSerializer.Deserialize<FlutterBaseModel<FlutterChargeResponse, meta>>(res);
                return serviceResponse;

            }
            catch (Exception ex)
            {
                serviceResponse.message = $"{ex.Message}";
                return serviceResponse;
            }

        }

        public async Task<ProcessBankPaymentResponseModel> ProcessBankPayment(BankPayment cardDetails, string adviceReference)
        {
            throw new NotImplementedException();
        }

        public async Task<serviceResponse<AdviceResponseModel>> CompleteBankPayment(CompleteCardPayment cardDetails)
        {
            throw new NotImplementedException();
        }

        public async Task<BankTransferResponseModel> GenerateNewDynamicAccount(string merchantReference)
        {
            throw new NotImplementedException();
        }

        public async Task<string> WebHookNotification(WebHookRequestModel cardDetails)
        {
            throw new NotImplementedException();
        }

        public async Task<WalletAccountNameInquiryResponse> NameEnquiry(string accountNumber)
        {
            throw new NotImplementedException();
        }

        public async Task<CreditWalletRequestResponse> ConfirmClientTransferStatus(string clientTransactionReference)
        {
            throw new NotImplementedException();
        }

        public async Task<GetAccountDetails> GetAccountDetails(string accountNumber)
        {
            throw new NotImplementedException();
        }

        public async Task<string> GetWalletTransactionHistory(WemaAccountTransactionHistoryRequest model)
        {
            throw new NotImplementedException();
        }

        public async Task<WemaWalletGenerateAccountResponse> GenerateWalletAccount(WemaWalletGenerateAccountRequest payload)
        {
            WemaWalletGenerateAccountResponse response = new WemaWalletGenerateAccountResponse();
            var apiUrl = $"https://api.paystack.co/dedicated_account";

            return response;
        }

        public async Task<WemaWalletBankListRresponse> GetAllBanks()
        {
            WemaWalletBankListRresponse _ = new WemaWalletBankListRresponse();
            var apiUrl = $"https://api.paystack.co/dedicated_account/available_providers";


            return _;
        }

        public async Task<NipCharges> GetNipCharges()
        {
            throw new NotImplementedException();
        }

        public async Task<CreditWalletRequestResponse> ProcessClientTransfer(ClientTransferRequest model)
        {
            throw new NotImplementedException();
        }

    }
}