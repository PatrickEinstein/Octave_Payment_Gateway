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
using OCPG.Core.Models;
using OCPG.Infrastructure.Interfaces.ICryptographies;
using OCPG.Infrastructure.Interfaces.IRepositories;
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
        private readonly IFlutterCryptography flutterCryptography;

        private readonly FlutterAuthConfig authConfig;
        private readonly IPaymentRepository paymentRepository;
        private readonly DataBaseContext dataBaseContext;
        public FlutterWave(FlutterWaveAppUrls appUrl, IApiCaller apiCaller, FlutterAuthConfig authConfig, IPaymentRepository paymentRepository, DataBaseContext dataBaseContext)
        {
            this.ApiCaller = apiCaller;
            this.authConfig = authConfig;
            this.paymentRepository = paymentRepository;
            this.dataBaseContext = dataBaseContext;
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
                        adviceReference = new Guid().ToString(),
                        amount = (double)advice.amount,
                        currency = advice.currency,
                        customerFullName = $"{advice.customer.CustomerFirstName ?? ""} {advice.customer.CustomerLastName ?? ""}",
                        merchantRef = advice.merchantRef,
                        status = "Pending",
                        narration = advice.narration
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
                var customer = JsonSerializer.Deserialize<AdviceModel>(payment.requestPayload);
                var newPayload = new
                {
                    card_number = cardDeetails.cardNumber,
                    expiry_month = cardDeetails.expiredMonth,
                    expiry_year = cardDeetails.expiredYear,
                    cvv = cardDeetails.cvv,
                    currency = "NGN",
                    amount = payment.amount,
                    email = customer.customer.CustomerEmail,
                    fullname = $"{customer.customer.CustomerFirstName} {customer.customer.CustomerLastName}",
                    phone_number = customer.customer.CustomerPhoneNumber,
                    tx_ref = "GB/" + new Guid(),
                    redirect_url = "https://example_company.com/success"
                };
                var Load = new StringContent(JsonSerializer.Serialize(new
                {
                    client = flutterCryptography.EncryptFlutter3DESAlgo(JsonSerializer.Serialize(newPayload), authConfig.encryptionKey),
                }), Encoding.UTF8, "application/json");

                var res = await ApiCaller.POST(Load, apiUrl, authConfig.clientSecret, headers);
                resApi = JsonSerializer.Deserialize<ProcessCardResponseModel>(res);

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