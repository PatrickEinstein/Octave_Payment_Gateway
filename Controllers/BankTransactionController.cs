using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CentralPG.Controllers;
using CentralPG.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using OCPG.Core.Enums;
using OCPG.Infrastructure.Interfaces.IManagers;

namespace OCPG.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BankTransactionController : ControllerBase
    {

        private readonly IPaymentManager paymentManager;
        private readonly ILogger<BankTransactionController> logger;

        public BankTransactionController(IPaymentManager paymentManager, ILogger<BankTransactionController> logger)
        {
            this.paymentManager = paymentManager;
            this.logger = logger;
        }


        [HttpPost("/Payment/processpayment/bank-debit/{channelCode}")]
        public async Task<IActionResult> ProcessPaymentBank(BankPayment bankDetails, string adviceReference,  ChannelCode channelCode)
        {
            return Ok(await paymentManager.ProcessBankPayment(bankDetails, adviceReference, channelCode));
        }
        [HttpPost("/Payment/generateDynamicacoount/bank-transfer/{channelCode}")]
        public async Task<IActionResult> GenerateDynamicAccount(GenerateBankAccount model,ChannelCode channelCode)
        {
            return Ok(await paymentManager.GenerateBankAccount(model, channelCode));
        }


    }
}