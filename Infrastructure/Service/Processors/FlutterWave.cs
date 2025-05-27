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
using OCPG.Core.Enums;
using OCPG.Core.Models;
using OCPG.Core.Models.Entities;
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
        private readonly IWalletRepository walletRepository;

        public FlutterWave(FlutterWaveAppUrls appUrl,
        IApiCaller apiCaller,
        FlutterAuthConfig authConfig,
        IPaymentRepository paymentRepository,
        DataBaseContext dataBaseContext,
        IFlutterCryptography flutterCryptography,
        ICardRepository cardRepository,
        IWalletRepository walletRepository
        )
        {
            this.ApiCaller = apiCaller;
            this.authConfig = authConfig;
            this.paymentRepository = paymentRepository;
            this.dataBaseContext = dataBaseContext;
            this.flutterCryptography = flutterCryptography;
            this.cardRepository = cardRepository;
            this.walletRepository = walletRepository;
            this.appUrl = appUrl;
        }


        public async Task<serviceResponse<AuthTokens>> Login()
        {
            throw new NotImplementedException();
        }




        //////////////////START ----------------///////////////////// INITIALIZE PAYMENTS  ///////////////////

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

        //////////////////END ----------------///////////////////// INITIALIZE PAYMENTS  ///////////////////


        //////////////////START ----------------///////////////////// CARD PROCESSING  ///////////////////

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

            string apiUrl = appUrl.BaseUrl + "/v3/charges?type=card";
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
                serviceResponse = new CompletePaymentResponseModel
                {
                    requestSuccessful = flutterResponse.status == "success" ? true : false,
                    message = $"{flutterResponse.message} {flutterResponse.data.processor_response}",
                    responseData = new ResponseDataCompletePayment
                    {
                        adviceReference = cardDeetails.paymentreference,
                        paymentReference = flutterResponse.data.flw_ref,
                        merchantReference = flutterResponse.data.tx_ref,
                        amountCollected = (double)flutterResponse.data.amount,
                        amount = (double)flutterResponse.data.amount + (double)flutterResponse.data.app_fee + (double)flutterResponse.data.merchant_fee,
                        callBackUrl = card.redirect_url,
                        processorCode = flutterResponse.data.processor_response,
                        transactionStatus = flutterResponse.data.status,
                        currencyCode = flutterResponse.data.currency,
                        accountNumber = "",
                        accountNumberMasked = card.card_number.Substring(0, 2) + "********" + card.card_number.Substring(8),
                        narration = flutterResponse.data.narration,
                        merchantCode = "",
                        message = flutterResponse.message,
                        customerName = flutterResponse.data.customer.name,
                        paymentDate = flutterResponse.data.created_at.ToString(),
                        paymentLink = flutterResponse.meta.authorization.mode == "otp" ? flutterResponse.meta.authorization.endpoint : flutterResponse.meta.authorization.redirect,
                    }
                };
                return serviceResponse;

            }
            catch (Exception ex)
            {
                serviceResponse.message = $"{ex.Message}";
                return serviceResponse;
            }

        }

        public async Task<CompletePaymentResponseModel> ValidateCardPayment(ValidatePayment cardDeetails)
        {
            CompletePaymentResponseModel serviceResponse = new CompletePaymentResponseModel();

            try
            {
                string apiUrl = "https://api.flutterwave.com/v3/validate-charge";
                var Load = new StringContent(JsonSerializer.Serialize(new
                {
                    otp = cardDeetails.otp,
                    flw_ref = cardDeetails.paymentReference,
                    type = "card",
                }), Encoding.UTF8, "application/json");
                var res = await ApiCaller.POST(Load, apiUrl, authConfig.clientSecret, headers);
                var flutterResponse = JsonSerializer.Deserialize<FlutterBaseModel<FlutterChargeResponse, meta>>(res);
                var payment = await paymentRepository.GetPaymentByPaymentReference(cardDeetails.paymentReference);
                serviceResponse = new CompletePaymentResponseModel
                {
                    requestSuccessful = flutterResponse.status == "success" ? true : false,
                    message = $"{flutterResponse.message} {flutterResponse.data.processor_response}",
                    responseData = new ResponseDataCompletePayment
                    {
                        adviceReference = payment.adviceReference,
                        paymentReference = flutterResponse.data.flw_ref,
                        merchantReference = flutterResponse.data.tx_ref,
                        amountCollected = (double)flutterResponse.data.amount,
                        amount = (double)flutterResponse.data.amount + (double)flutterResponse.data.app_fee + (double)flutterResponse.data.merchant_fee,
                        callBackUrl = payment.callbackUrl,
                        processorCode = flutterResponse.data.processor_response,
                        transactionStatus = flutterResponse.data.status,
                        currencyCode = flutterResponse.data.currency,
                        accountNumber = "",
                        accountNumberMasked = payment.accountNumberMasked,
                        narration = flutterResponse.data.narration,
                        merchantCode = "",
                        message = flutterResponse.message,
                        customerName = flutterResponse.data.customer.name,
                        paymentDate = flutterResponse.data.created_at.ToString(),
                        paymentLink = flutterResponse.meta.authorization.redirect != null ? flutterResponse.meta.authorization.redirect : "",
                    }
                };
                return serviceResponse;
            }
            catch (Exception ex)
            {
                serviceResponse.message = $"{ex.Message}";
                return serviceResponse;
            }


        }
        //////////////////END ----------------///////////////////// CARD PROCESSING  ///////////////////



        //////////////////START ----------------///////////////////// BANK  TRANSFER ///////////////////

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

        //////////////////END ----------------///////////////////// BANK TRANSFER  ///////////////////


       



        //////////////////START ----------------///////////////////// WALLET  ///////////////////


        /// <summary>
        /// Generate a new wallet account
        /// </summary>
        /// <param name="payload"></param>
        /// <returns></returns>

        public async Task<WemaWalletGenerateAccountResponse> GenerateWalletAccount(WemaWalletGenerateAccountRequest payload)
        {
            WemaWalletGenerateAccountResponse response = new WemaWalletGenerateAccountResponse();
            try
            {
                var apiUrl = appUrl.BaseUrl + "/v3/virtual-account-numbers";

                var walletPayload = new
                {
                    email = payload.email,
                    tx_ref = Guid.NewGuid().ToString(),
                    phonenumber = payload.phoneNumber,
                    is_permanent = true,
                    firstname = payload.first_name,
                    lastname = payload.last_name,
                    narration = $"open wallet for {payload.first_name} {payload.last_name}",
                    bvn = payload.bvn,
                };

                var res = await ApiCaller.POST(new StringContent(JsonSerializer.Serialize(walletPayload), Encoding.UTF8, "application/json"), apiUrl, authConfig.clientSecret, headers);
                var flutterResponse = JsonSerializer.Deserialize<FlutterBaseModel<FlutterWallet, meta>>(res);



                response = new WemaWalletGenerateAccountResponse
                {
                    message = flutterResponse.message,
                    status = flutterResponse.status == "success" ? true : false,
                    code = Convert.ToInt32(flutterResponse.data.response_code),
                    data = new WemaWalletGenerateAccountResponseData
                    {
                        flw_ref = flutterResponse.data.flw_ref,
                        order_ref = flutterResponse.data.order_ref,
                        account_number = flutterResponse.data.account_number
                    }
                };

                Wallets wallet = new Wallets
                {
                    account_number = flutterResponse.data.account_number,
                    email = payload.email,
                    phone_number = payload.phoneNumber,
                    account_name = $"{payload.first_name} {payload.last_name}",
                    account_balance = 0.00,
                    account_mandate = 0.00,
                    acccount_trackerRef = flutterResponse.data.flw_ref,
                    acccount_trackerId = flutterResponse.data.order_ref,
                    wallet_provider = ChannelCode.flutterWave,
                    created_at = DateTimeOffset.UtcNow,
                };

                var isCreatedAccount = await walletRepository.CreateWallet(wallet);
                if (!isCreatedAccount)
                {
                throw new Exception("Account creation failed, please try again later.");
               }
            }
            catch (Exception ex)
            {
                response.data = null;
                response.status = false;
                response.message = ex.Message;

            }

            return response;
        }

       public async Task<WemaWalletBankListRresponse> GetAllBanks()
{
    var response = new WemaWalletBankListRresponse
    {
        result = new List<ResultList>(),
        message = "Success"
    };

    try
    {
        var apiUrl = "https://api.flutterwave.com/v3/banks/NG";
        var res = await ApiCaller.GET(apiUrl, authConfig.clientSecret, headers);

        // Deserialize response
        var flutterResponse = JsonSerializer.Deserialize<FlutterBaseModel<List<FlutterBankList>, meta>>(res);

        if (flutterResponse?.data != null)
        {
            foreach (var bank in flutterResponse.data)
            {
                response.result.Add(new ResultList
                {
                    bankLogo = "",  
                    provider_slug = "", 
                    bankCode = bank.code,
                    bankName = bank.name
                });
            }
        }
        else
        {
            response.message = "No bank data found.";
        }
    }
    catch (Exception ex)
    {
        response.message = $"Error: {ex.Message}";
    }

    return response;
}


        public async Task<NipCharges> GetNipCharges(ChannelCode channelCode, double amount, PaymentType payment_type, Currency currency)
        {
            NipCharges nipCharges = new NipCharges
            {
                errorMessage = "",
               result = new NipChargesResult
               {
                chargeFees = new List<NipChargesResultFee>()
               }
                
            };
            try
            {
                var apiUrl = $"https://api.flutterwave.com/v3/transactions/fee?amount={amount}&currency={currency}&payment_type={payment_type}";
                var res = await ApiCaller.GET(apiUrl, authConfig.clientSecret, headers);
                var flutterResponse = JsonSerializer.Deserialize<FlutterBaseModel<ChargeData, meta>>(res);

                nipCharges.result.chargeFees.Add(new NipChargesResultFee
                {
                    id = 1,
                    chargeFeeName = ChannelCode.flutterWave.ToString(),
                    transactionType = 1,
                    charge = flutterResponse.data.fee + flutterResponse.data.flutterwaveFee + flutterResponse.data.merchantFee + flutterResponse.data.stampDutyFee,
                    lower = 0,
                    upper = amount
                });
                nipCharges.errorMessage = flutterResponse.message;
                
            }
            catch (Exception ex)
            {
                nipCharges.errorMessage = $"Error fetching NIP charges: {ex.Message}";

            }
            return nipCharges;
           
        }




        public async Task<string> ProcessInternalTransferFromWalletProviderToBankAccount(WithdrawFromWallet payload)
        {
            try
            {

            var apiUrl = "https://api.flutterwave.com/v3/transfers";
            var res = await ApiCaller.POST(new StringContent(JsonSerializer.Serialize(new
            {
                account_bank = ChannelCode.flutterWave,
                account_number = payload.bank_accountNumber,
                amount = payload.amount,
                currency = "NGN",
                debit_currency = "NGN"
            }), Encoding.UTF8, "application/json"), apiUrl, authConfig.clientSecret, headers);
            var flutterResponse = JsonSerializer.Deserialize<FlutterBaseModel<FlutterWallet, meta>>(res);
            var status = flutterResponse.status == "success" ? flutterResponse.data.flw_ref : flutterResponse.message;
          
                Withdrawals withdrawal = new Withdrawals
                {
                    wallet_accountNumber = payload.wallet_accountNumber,
                    amount = payload.amount,
                    transactionReference = payload.transactionReference,
                    narration = payload.narration,
                    channelCode = ChannelCode.flutterWave,
                    status =  flutterResponse.status == "success" ? CentralPG.Enums.OrderStatus.Successful : CentralPG.Enums.OrderStatus.Failed,
                    processorRef =  flutterResponse.data != null && !string.IsNullOrWhiteSpace(flutterResponse.data.flw_ref) ? flutterResponse.data.flw_ref : "",
                    processorMsg = !string.IsNullOrWhiteSpace(flutterResponse.message) ? flutterResponse.message : "",
                };
                await walletRepository.UpdateWithdrawal(withdrawal);
            return status;
            }
            catch (Exception ex)
            {
                return $"{ex.Message}";
            }
            
        }



        /// <summary>
        /// WEBHOOK
        /// </summary>
        /// <param name="webhooks"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public async Task<string> WebHookNotification(string stream)
        {
            try
            {

                var FlutterWebhook = JsonSerializer.Deserialize<FlutterWebhook>(stream, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

                if (FlutterWebhook.data.payment_type == "bank_transfer")
                {
                    ConfirmWalletTransferStatus confirmWalletTransferStatus = new ConfirmWalletTransferStatus
                    {
                        transfer_reference = FlutterWebhook.data.tx_ref,
                        processor_reference = FlutterWebhook.data.flw_ref,
                        amount = FlutterWebhook.data.amount.GetValueOrDefault(),
                        processor_response = FlutterWebhook.data.processor_response,
                        currency = FlutterWebhook.data.currency,
                        status = FlutterWebhook.data.status,
                        narration = FlutterWebhook.data.narration,
                        created_at = FlutterWebhook.data.created_at,
                        customer_name = FlutterWebhook.data.customer.name,
                        phone_number = FlutterWebhook.data.customer.phone_number,
                        email = FlutterWebhook.data.customer.email,
                        provider = ChannelCode.flutterWave.ToString(),
                    };

                    walletRepository.CreditWallet(confirmWalletTransferStatus);
                   

                    WalletTransactionHistory walletTransactionHistory = new WalletTransactionHistory
                    {
                        originator_accountName = "",
                        destination_accountName = FlutterWebhook.data.fullname,
                        destination_accountNumber = FlutterWebhook.data.account_number,
                        processor_reference = FlutterWebhook.data.flw_ref,
                        transaction_reference = FlutterWebhook.data.tx_ref,
                        amount = FlutterWebhook.data.amount.GetValueOrDefault(),
                        transaction_type = FlutterWebhook.data.payment_type,
                        status = CentralPG.Enums.OrderStatus.Successful,
                        narration = FlutterWebhook.data.narration,
                        transaction_date = FlutterWebhook.data.created_at,
                        provider = ChannelCode.flutterWave,
                    };
                    await walletRepository.CreateWalletTransactionHistory(walletTransactionHistory);
                }
                if (FlutterWebhook.data.payment_type == "card")
                {
                    PaymentTransactions paymentTransactions = new PaymentTransactions
                    {
                        transactionStatus = FlutterWebhook.data.status,
                        paymentReference = FlutterWebhook.data.tx_ref,
                    };
                    await paymentRepository.UpdatePayment(paymentTransactions);
                }
                return "";
            }
            catch (Exception ex)
            {
                return $"{ex.Message}";
            }

        }

    }
}