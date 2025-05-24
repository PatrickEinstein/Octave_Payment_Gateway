using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using CentralPG.Models;
using CentralPG.Data;
using Microsoft.EntityFrameworkCore;
using OCPG.Models;
using CentralPG.Infrasturcture.Interfaces.Utilities;
using CentralPG.Core.Models.Entities;
using CentralPG.Interfaces.IProcessors;
using OCPG.Core.Models;
using OCPG.Infrastructure.Interfaces.IRepositories;
using OCPG.Core.Enums;

namespace CentralPG.Infrastructure.Services.Mains;


//TODO: Unify NewtonSoft and System.JSON

public class ChamsSwitch : IPaymentProcessor
{
    public IApiCaller ApiCaller { get; set; }

    public ChamsSwitch(AppUrl appUrl, IApiCaller apiCaller, AuthConfig authConfig, IPaymentRepository paymentRepository, DataBaseContext dataBaseContext)
    {
        this.ApiCaller = apiCaller;
        this.authConfig = authConfig;
        this.paymentRepository = paymentRepository;
        this.dataBaseContext = dataBaseContext;
        this.appUrl = appUrl;
    }


    private IDictionary<string, string> headers = new Dictionary<string, string>
    {

    };
    private readonly AppUrl appUrl;
    private IDictionary<string, string> header;

    private readonly AuthConfig authConfig;
    private readonly IPaymentRepository paymentRepository;
    private readonly DataBaseContext dataBaseContext;

    public async Task<serviceResponse<AuthTokens>> Login()
    {
        serviceResponse<AuthTokens> serviceResponse = new serviceResponse<AuthTokens>();
        try
        {
            LoginModel request = new LoginModel
            {
                clientId = authConfig.ClientId,
                clientSecret = authConfig.clientSecret
            };
            string apiUrl = $"{appUrl.BaseUrl}/api/Account/login";
            var Load = new StringContent(JsonSerializer.Serialize(request), Encoding.UTF8, "application/json");
            var res = await ApiCaller.POST(Load, apiUrl, "", headers);

            if (!string.IsNullOrEmpty(res))
            {
                var resApi = JsonSerializer.Deserialize<AuthTokens>(res);
                if (resApi != null && resApi.access_token != null)
                {
                    var token = await dataBaseContext.Auths.FirstOrDefaultAsync();

                    if (token != null)
                    {
                        token.access_token = resApi.access_token;
                        token.timeGenerated = resApi.timeGenerated;
                        token.expires_in = resApi.expires_in;
                    }
                    else
                    {
                        await dataBaseContext.Auths.AddAsync(resApi);
                    }

                    await dataBaseContext.SaveChangesAsync();
                    serviceResponse.Data = resApi;
                }
            }
            return serviceResponse;
        }
        catch (Exception ex)
        {
            serviceResponse.Message = $"{ex.Message}";
            return serviceResponse;
        }
    }


    public async Task<serviceResponse<GetAdviceModel>> GetAdvice(string adviceReference)
    {
        serviceResponse<GetAdviceModel> serviceResponse = new serviceResponse<GetAdviceModel>();
        try
        {
            string apiUrl = $"{appUrl.BaseUrl}/Payment/advice?adviceReference={adviceReference}";
            var token = await dataBaseContext.Auths.FirstOrDefaultAsync();
            var accessToken = token.access_token;
            var res = await ApiCaller.GET(apiUrl, accessToken, headers);
            var resApi = JsonSerializer.Deserialize<GetAdviceModel>(res);
            serviceResponse.Data = resApi;
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
            string apiUrl = $"{appUrl.BaseUrl}/Payment/advice";
            var Load = new StringContent(JsonSerializer.Serialize(advice), Encoding.UTF8, "application/json");
            var token = await dataBaseContext.Auths.FirstOrDefaultAsync();
            var accessToken = token.access_token;
            var res = await ApiCaller.POST(Load, apiUrl, accessToken, headers);
            var resApi = JsonSerializer.Deserialize<AdviceResponseModel>(res);
            resApi.responseData.processor = "ChamsSwitch";
            serviceResponse.Data = resApi;
            return serviceResponse;
        }
        catch (Exception ex)
        {
            serviceResponse.Message = $"{ex.Message}";
            return serviceResponse;

        }
    }


    /// <summary>
    /// Process Payment using card information
    /// </summary>
    /// <param name="cardDeetails"></param>
    /// <param name="adviceReference"></param>
    /// <returns></returns>

    public async Task<ProcessCardResponseModel> ProcessCardPayment(CardPayment cardDeetails, string adviceReference)
    {
        ProcessCardResponseModel resApi = new ProcessCardResponseModel();
        try
        {
            string apiUrl = $"{appUrl.BaseUrl}/Payment/process/card/{adviceReference}";
            var Load = new StringContent(JsonSerializer.Serialize(cardDeetails), Encoding.UTF8, "application/json");
            var token = await dataBaseContext.Auths.FirstOrDefaultAsync();
            var accessToken = token.access_token;
            var res = await ApiCaller.POST(Load, apiUrl, accessToken, headers);
            resApi = JsonSerializer.Deserialize<ProcessCardResponseModel>(res);
            var domain = await paymentRepository.GetPaymentByAdviceReference(adviceReference);
            resApi.responseData.paymentLink = $"{domain.callbackUrl.Replace("status/", "")}" + "forms/" +
                                                $"?creq={resApi?.responseData?.formData?.formData?.JWT}" +
                                                $"&acsUrl={resApi?.responseData?.formData.url}" ?? "";
            return resApi;
        }
        catch (Exception ex)
        {
            resApi.message = $"{ex.Message}";
            return resApi;
        }
    }

    /// <summary>
    /// Complete Card Payment
    /// </summary>
    /// <param name="cardDeetails"></param>
    /// <returns></returns>
    public async Task<CompletePaymentResponseModel> CompleteCardPayment(CompleteCardPayment cardDeetails)
    {
        CompletePaymentResponseModel serviceResponse = new CompletePaymentResponseModel();
        try
        {
            string apiUrl = $"{appUrl.BaseUrl}/Payment/complete/card";
            var Load = new StringContent(JsonSerializer.Serialize(cardDeetails), Encoding.UTF8, "application/json");
            var token = await dataBaseContext.Auths.FirstOrDefaultAsync();
            var accessToken = token.access_token;
            var res = await ApiCaller.POST(Load, apiUrl, accessToken, headers);
            var resApi = JsonSerializer.Deserialize<CompletePaymentResponseModel>(res);
            var domain = await paymentRepository.GetPaymentByPaymentReference(resApi.responseData.paymentReference);
            resApi.responseData.paymentLink = $"{domain.callbackUrl.Replace("status/", "")}" + "forms/" +
                                   $"?creq={resApi?.responseData?.formData?.formData?.JWT}" +
                                   $"&acsUrl={resApi?.responseData?.formData?.url}" +
                                   $"&paymentReference={resApi?.responseData?.formData?.formData?.PaymentReference}";
            resApi.responseData.processor_message = "";

            return resApi;
        }
        catch (Exception ex)
        {
            serviceResponse.message = $"{ex.Message}";
            return serviceResponse;
        }
    }

    public async Task<CompletePaymentResponseModel> ValidateCardPayment(ValidatePayment cardDeetails)
    {
        throw new NotImplementedException();
    }


    /// <summary>
    /// Process Payment using Bank Information
    /// </summary>
    /// <param name="cardDetails"></param>
    /// <param name="adviceReference"></param>
    /// <returns></returns>
    public async Task<ProcessBankPaymentResponseModel> ProcessBankPayment(BankPayment cardDetails, string adviceReference)
    {
        ProcessBankPaymentResponseModel resApi = new ProcessBankPaymentResponseModel();
        try
        {
            string apiUrl = $"{appUrl.BaseUrl}/Payment/process/bankaccount/{adviceReference}";
            var newLoad = new
            {
                SourceBankCode = cardDetails.bankCode,
                SourceAccountNumber = cardDetails.accountNumber,
                sourceAccountName = cardDetails.nameOnAccount
            };
            var Load = new StringContent(JsonSerializer.Serialize(newLoad), Encoding.UTF8, "application/json");
            var token = await dataBaseContext.Auths.FirstOrDefaultAsync();
            var accessToken = token.access_token;
            var res = await ApiCaller.POST(Load, apiUrl, accessToken, headers);
            resApi = JsonSerializer.Deserialize<ProcessBankPaymentResponseModel>(res);
            return resApi;
        }
        catch (Exception ex)
        {
            resApi.message = $"{ex.Message}";
            return resApi;
        }
    }

    /// <summary>
    /// Complete the Bank Payment if need be
    /// </summary>
    /// <param name="cardDetails"></param>
    /// <returns></returns>
    public async Task<serviceResponse<AdviceResponseModel>> CompleteBankPayment(CompleteCardPayment cardDetails)
    {
        serviceResponse<AdviceResponseModel> serviceResponse = new serviceResponse<AdviceResponseModel>();
        try
        {
            string apiUrl = $"{appUrl.BaseUrl}/Payment/bankaccount/{cardDetails.paymentreference}";

            var Load = new StringContent(JsonSerializer.Serialize(new
            {
                value = cardDetails.value
            }), Encoding.UTF8, "application/json");
            var token = await dataBaseContext.Auths.FirstOrDefaultAsync();
            var accessToken = token.access_token;
            var res = await ApiCaller.GET(apiUrl, accessToken, headers);
            var resApi = JsonSerializer.Deserialize<AdviceResponseModel>(res);
            serviceResponse.Data = resApi;
            return serviceResponse;
        }
        catch (Exception ex)
        {
            serviceResponse.Message = $"{ex.Message}";
            return serviceResponse;
        }
    }

    public async Task<BankTransferResponseModel> GenerateNewDynamicAccount(string merchantReference)
    {
        BankTransferResponseModel _ = new BankTransferResponseModel();
        try
        {
            string apiUrl = $"{appUrl.BaseUrl}/Payment/process/banktransfer/{merchantReference}";


            var Load = new StringContent(JsonSerializer.Serialize(new
            {
                bankCode = "035"
            }), Encoding.UTF8, "application/json");
            var res = await ApiCaller.POST(Load, apiUrl, "", headers);
            var __ = JsonSerializer.Deserialize<BankTransferResponseModel>(res);
            return __;
        }
        catch (Exception e)
        {
            _.message = $"An Error occurred while,{e.Message} Contact the developer";
            return _;
        }
    }



    /////////////////////////////////////////WALLET MODULE //////////////////////////////////////////////////WALLET MODULE /////////////////////////////////////////////////


    /////////////////////////////////////////GET DETAILS //////////////////////
    public async Task<WalletAccountNameInquiryResponse> NameEnquiry(string accountNumber)
    {

        WalletAccountNameInquiryResponse _ = new WalletAccountNameInquiryResponse();
        var apiUrl = $"{appUrl.BaseUrl}/debit/name-enquiry/035";
        var Load = new StringContent(JsonSerializer.Serialize(new
        {
        }), Encoding.UTF8, "application/json");
        var token = await dataBaseContext.Auths.FirstOrDefaultAsync();
        var accessToken = token.access_token;
        var res = await ApiCaller.GET(apiUrl, accessToken, headers);
        var resApi = JsonSerializer.Deserialize<WalletAccountNameInquiryResponse>(res);
        return resApi;
    }
    public async Task<CreditWalletRequestResponse> ConfirmClientTransferStatus(ConfirmWalletTransferStatus clientTransactionReference)
    {

        ConfirmCreditWalletTransactionResponse _ = new ConfirmCreditWalletTransactionResponse();
        var apiUrl = $"{appUrl.BaseUrl}/credit/confirm/035";
        var token = await dataBaseContext.Auths.FirstOrDefaultAsync();
        var accessToken = token.access_token;
        var res = await ApiCaller.GET(apiUrl, accessToken, headers);
        var resApi = JsonSerializer.Deserialize<CreditWalletRequestResponse>(res);
        return resApi;
    }
    public async Task<GetAccountDetails> GetAccountDetails(string accountNumber)
    {
        var apiUrl = $"{appUrl.BaseUrl}/manage/details/035";
        var token = await dataBaseContext.Auths.FirstOrDefaultAsync();
        var accessToken = token.access_token;
        var Load = new StringContent(JsonSerializer.Serialize(new
        {
        }), Encoding.UTF8, "application/json");
        var res = await ApiCaller.POST(Load, apiUrl, accessToken, headers);
        var resApi = JsonSerializer.Deserialize<GetAccountDetails>(res);
        return resApi;
    }
    public async Task<string> GetWalletTransactionHistory(WemaAccountTransactionHistoryRequest model)
    {
        var apiUrl = $"{appUrl.BaseUrl}/manage/history/035";
        var str = JsonSerializer.Serialize(model);
        var load = new StringContent(JsonSerializer.Serialize(model), Encoding.UTF8, "application/json");
        var call = await ApiCaller.POST(load, apiUrl, "", header);
        return call;
    }

    /////////////////////////////////////////CREATE WALLET MODULE //////////////////////

    public async Task<WemaWalletGenerateAccountResponse> GenerateWalletAccount(WemaWalletGenerateAccountRequest payload)
    {
        var apiUrl = $"{appUrl.BaseUrl}/create/035";
        var str = JsonSerializer.Serialize(payload);
        var Load = new StringContent(JsonSerializer.Serialize(payload), Encoding.UTF8, "application/json");
        var token = await dataBaseContext.Auths.FirstOrDefaultAsync();
        var accessToken = token.access_token;
        var res = await ApiCaller.POST(Load, apiUrl, accessToken, headers);
        var response = JsonSerializer.Deserialize<WemaWalletGenerateAccountResponse>(res);
        return response;
    }
    public async Task<WemaWalletValidateOTPResponse> ValidateAccountwithOtp(WemaWalletValidateOTPRequest payload)
    {

        WemaWalletValidateOTPResponse _ = new WemaWalletValidateOTPResponse();
        var apiUrl = $"{appUrl.BaseUrl}/create/validate/035";
        var str = JsonSerializer.Serialize(payload);
        var Load = new StringContent(JsonSerializer.Serialize(payload), Encoding.UTF8, "application/json");
        var token = await dataBaseContext.Auths.FirstOrDefaultAsync();
        var accessToken = token.access_token;
        var res = await ApiCaller.POST(Load, apiUrl, accessToken, headers);
        _ = JsonSerializer.Deserialize<WemaWalletValidateOTPResponse>(res);
        return _;
    }

    /////////////////////////////////////////DEBIT WALLET MODULE //////////////////////
    public async Task<WemaWalletBankListRresponse> GetAllBanks()
    {

        WalletAccountNameInquiryResponse _ = new WalletAccountNameInquiryResponse();
        var apiUrl = $"{appUrl.BaseUrl}/debit/get-banks/035";
        var token = await dataBaseContext.Auths.FirstOrDefaultAsync();
        var accessToken = token.access_token;
        var res = await ApiCaller.GET(apiUrl, accessToken, headers);
        var resApi = JsonSerializer.Deserialize<WemaWalletBankListRresponse>(res);
        return resApi;
    }


    public async Task<NipCharges> GetNipCharges()
    {
        var apiUrl = $"{appUrl.BaseUrl}/debit/charges/035";
        var token = await dataBaseContext.Auths.FirstOrDefaultAsync();
        var accessToken = token.access_token;
        var res = await ApiCaller.GET(apiUrl, accessToken, headers);
        var resApi = JsonSerializer.Deserialize<NipCharges>(res);
        return resApi;
    }
    public async Task<CreditWalletRequestResponse> ProcessClientTransfer(ClientTransferRequest model)
    {

        CreditWalletRequestResponse _ = new CreditWalletRequestResponse();

        var apiUrl = $"{appUrl.BaseUrl}/debit/process/035";
        var Load = new StringContent(JsonSerializer.Serialize(model), Encoding.UTF8, "application/json");
        var token = await dataBaseContext.Auths.FirstOrDefaultAsync();
        var accessToken = token.access_token;
        var res = await ApiCaller.POST(Load, apiUrl, accessToken, headers);
        var resApi = JsonSerializer.Deserialize<CreditWalletRequestResponse>(res);
        return resApi;
    }

   public async Task<string> WebHookNotification(string stream)
    {
        serviceResponse<WebHookRequestModel> response = new serviceResponse<WebHookRequestModel>();
        return JsonSerializer.Serialize(response);
    }

}




