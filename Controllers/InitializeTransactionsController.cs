using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CentralPG.Models;
using Microsoft.AspNetCore.Mvc;
using OCPG.Core.Enums;
using OCPG.Infrastructure.Interfaces.IManagers;

namespace OCPG.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class InitializeTransactions : ControllerBase
    {
        private readonly IPaymentManager paymentManager;

        public InitializeTransactions(IPaymentManager paymentManager)
        {
            this.paymentManager = paymentManager;
        }
        [HttpGet("/Payment/advice/{channel}/{adviceReference}")]
        public async Task<IActionResult> GetAdvice(string adviceReference, ChannelCode channel)
        {
            return Ok(await paymentManager.GetTransactionStatus(adviceReference, channel));
        }
        [HttpPost("/Payment/{channel}/advice")]
        public async Task<IActionResult> InitiateTransaction(AdviceModelReq request, ChannelCode channel)
        {
            return Ok(await paymentManager.InitiateTransaction(request, channel));
        }
    }
}