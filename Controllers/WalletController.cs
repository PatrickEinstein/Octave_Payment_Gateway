using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CentralPG.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using OCPG.Core.Enums;
using OCPG.Core.Models;
using OCPG.Infrastructure.Interfaces.IManagers;

namespace OCPG.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class WalletController : ControllerBase
    {
        private readonly IPaymentManager paymentManager;
        private readonly ILogger<WalletController> logger;

        public WalletController(IPaymentManager paymentManager, ILogger<WalletController> logger)
        {
            this.paymentManager = paymentManager;
            this.logger = logger;
        }

        [HttpPost("/create-wallet/{channelCode}")]
        public async Task<IActionResult> GenerateWallet(WemaWalletGenerateAccountRequest model, ChannelCode channelCode )
        {
            return Ok(await paymentManager.GenerateWalletAccount(model, channelCode));
        }

        [HttpGet("/name-enquiry/{channelCode}")]
        public async Task<IActionResult> NameEnquiry(string accountNumber, ChannelCode channelCode )
        {
            return Ok(await paymentManager.NameEnquiry(accountNumber, channelCode));
        }

        [HttpPost("/wallet-details/{channelCode}")]
        public async Task<IActionResult> GetAccountDetails(string accountNumber, ChannelCode channelCode )
        {
            return Ok(await paymentManager.GetAccountDetails(accountNumber, channelCode));
        }
        [HttpPost("/transaction-history/{channelCode}")]
        public async Task<IActionResult> GetWalletTransactionHistory(WemaAccountTransactionHistoryRequest model,ChannelCode channelCode )
        {
            return Ok(await paymentManager.GetWalletTransactionHistory(model,channelCode));
        }
        [HttpGet("/get-all-banks/{channelCode}")]
        public async Task<IActionResult> GetAllBanks(ChannelCode channelCode)
        {
            return Ok(await paymentManager.GetAllBanks(channelCode));
        }
       [HttpGet("/get-charges")]
public async Task<IActionResult> GetNipCharges(
    [FromQuery] ChannelCode channelCode,
    [FromQuery] double amount = 0.00,
    [FromQuery] PaymentType payment_type = PaymentType.bank_transfer,
    [FromQuery] Currency currency = Currency.NGN)
{

           if (channelCode == ChannelCode.flutterWave && amount <= 0 )
    {
        return BadRequest("If Flutterwave is the channel, please input valid amount, currency, and payment_type.");
    }

       


            var result = await paymentManager.GetNipCharges(channelCode, amount, payment_type, currency);
    return Ok(result);
}


        [HttpPost("/fund-transfer/{channelCode}")]
        public async Task<IActionResult> ProcessClientTransfer(ClientTransferRequest model, ChannelCode channelCode )
        {
            return Ok(await paymentManager.ProcessClientTransfer(model,channelCode));
        }
    }
}