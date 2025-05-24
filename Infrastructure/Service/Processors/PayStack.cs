using System;
using System.Collections.Generic;
using System.ComponentModel;
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
using OCPG.Core.Models;
using OCPG.Infrastructure.Interfaces.IRepositories;
using OCPG.Models;

namespace OCPG.Infrastructure.Service.Processors
{
    public class PayStack : IPaymentProcessor
    {
        public IApiCaller ApiCaller { get; set; }

        private IDictionary<string, string> headers = new Dictionary<string, string>
        {
            { "Accept", "application/json" },
        };
        private readonly PayStackAppUrls appUrl;


        private readonly PaystackAuthConfig authConfig;
        private readonly IPaymentRepository paymentRepository;
        private readonly DataBaseContext dataBaseContext;
        public PayStack(PayStackAppUrls appUrl, IApiCaller apiCaller, PaystackAuthConfig authConfig, IPaymentRepository paymentRepository, DataBaseContext dataBaseContext)
        {
            this.ApiCaller = apiCaller;
            this.authConfig = authConfig;
            this.paymentRepository = paymentRepository;
            this.dataBaseContext = dataBaseContext;
            this.appUrl = appUrl;
        }

        Task<serviceResponse<AuthTokens>> IPaymentProcessor.Login()
        {
            throw new NotImplementedException();
        }

        public async Task<serviceResponse<GetAdviceModel>> GetAdvice(string adviceReference)
        {
            serviceResponse<GetAdviceModel> serviceResponse = new serviceResponse<GetAdviceModel>();
            try
            {
                headers.Add("Authorization", $"Bearer {authConfig.clientSecret}");
                string apiUrl = $"{appUrl.BaseUrl}/transaction/verify/{adviceReference}";
                var res = await ApiCaller.GET(apiUrl, null, headers);
                var resApi = JsonSerializer.Deserialize<PaystackBaseModel<verifyPaymentData>>(res);

                serviceResponse.Data = new GetAdviceModel
                {
                    requestSuccessful = resApi.status,
                    responseData = new ResponseData
                    {
                        currency = resApi.data.currency,
                        adviceReference = resApi.data.reference,
                        amount = resApi.data.amount / 100,
                        customerId = resApi.data.customer.id.ToString(),
                        customerFullName = $"{resApi.data.customer.firstName} {resApi.data.customer.lastName}",
                        status = resApi.data.status,
                        channels = new List<string> { "Card", "Bank" },
                        channel = new List<string> { "Card", "Bank" },
                        customerCharge = 0,
                        merchantCharge = 0,
                        catCharge = 0
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

                string apiUrl = $"{appUrl.BaseUrl}/transaction/transaction/initialize";
                var payload = new StringContent(JsonSerializer.Serialize(new
                {
                    email = advice.customer.CustomerEmail,
                    amount = advice.amount,

                }), System.Text.Encoding.UTF8, "application/json");
                var res = await ApiCaller.POST(payload, apiUrl, authConfig.clientSecret, headers);
                var resApi = JsonSerializer.Deserialize<PaystackBaseModel<initializePaymentData>>(res);


                serviceResponse.Data = new AdviceResponseModel
                {
                    requestSuccessful = resApi.status,
                    message = resApi.message,
                    responseData = new AdviceData
                    {
                        access_code = resApi.data.access_code,
                        reference = resApi.data.reference,
                        authorization_url = resApi.data.authorization_url
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
            throw new NotImplementedException("");
        }

        public async Task<CompletePaymentResponseModel> CompleteCardPayment(CompleteCardPayment cardDeetails)
        {
            throw new NotImplementedException();
        }

        public async Task<CompletePaymentResponseModel> ValidateCardPayment(ValidatePayment cardDeetails)
        {
            throw new NotImplementedException();
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


        public async Task<WalletAccountNameInquiryResponse> NameEnquiry(string accountNumber)
        {
            throw new NotImplementedException();
        }

        public async Task<CreditWalletRequestResponse> ConfirmClientTransferStatus(ConfirmWalletTransferStatus clientTransactionReference)
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
            var Load = new StringContent(JsonSerializer.Serialize(new
            {
                email = payload.email,
                first_name = payload.first_name,
                middle_name = payload.middle_name,
                last_name = payload.last_name,
                phone = payload.phoneNumber,
                preferred_bank = payload.preferred_bank,
                country = "NG"
            }), Encoding.UTF8, "application/json");

            var res = await ApiCaller.POST(Load, apiUrl, authConfig.clientSecret, headers);
            var PaystackRes = JsonSerializer.Deserialize<PaystackBaseModel<CreateWalletData>>(res);

            response.status = PaystackRes.status;
            response.message = PaystackRes.message;
            return response;
        }

        public async Task<WemaWalletBankListRresponse> GetAllBanks()
        {
            WemaWalletBankListRresponse _ = new WemaWalletBankListRresponse();
            var apiUrl = $"https://api.paystack.co/dedicated_account/available_providers";
            var res = await ApiCaller.GET(apiUrl, authConfig.clientSecret, headers);
            var resApi = JsonSerializer.Deserialize<PaystackBaseModel<List<BankProvidersData>>>(res);

            if (resApi.data != null) for (int i = 0; i < resApi.data.Count; i++)
                {
                    _.result.Add(new ResultList
                    {
                        bankName = resApi.data[i].bank_name,
                        provider_slug = resApi.data[i].provider_slug,
                    });
                }
            ;
            _.message = resApi.message;
            _.status = resApi.status;

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
        public async Task<string> WebHookNotification(string stream)
        {
            throw new NotImplementedException();
        }
    }
}