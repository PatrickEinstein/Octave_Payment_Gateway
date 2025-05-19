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

        /// <summary>
        /// use the default value dc8063c591dc48f4952456c6664ca510
        /// </summary>
        /// <param name="adviceReference"></param>
        /// <returns></returns>
        [HttpGet("/Payment/advice/{adviceReference}")]
        public async Task<IActionResult> GetAdvice(string adviceReference = "dc8063c591dc48f4952456c6664ca510")
        {
            return Ok(await paymentManager.GetTransactionStatus(adviceReference));
        }

      
        [HttpPost("/Payment/advice/{channel}")]
        public async Task<IActionResult> InitiateTransaction(AdviceModelReq request, ChannelCode channel)
        {
            return Ok(await paymentManager.InitiateTransaction(request, channel));
        }
    }
}