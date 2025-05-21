using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

using CentralPG.Models;
using OCPG.Infrastructure.Interfaces.IManagers;
using OCPG.Core.Enums;
using System.IO;
using Microsoft.Extensions.Logging;
using System.Text.Json;


namespace CentralPG.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CardTransaction : ControllerBase
    {
        private readonly IPaymentManager paymentManager;
        private readonly ILogger<CardTransaction> logger;

        public CardTransaction(IPaymentManager paymentManager, ILogger<CardTransaction> logger)
        {
            this.paymentManager = paymentManager;
            this.logger = logger;
        }


        [HttpPost("/Payment/processpayment/card/{channel}")]
        public async Task<IActionResult> ProcessPaymentCard(CardPayment cardDetails, string adviceReference, ChannelCode channel)
        {
            return Ok(await paymentManager.ProcessCardPayment(cardDetails, adviceReference, channel));
        }
        [HttpPost("/Payment/completepayment/card/{channel}")]
        public async Task<IActionResult> CompletePaymentCard(CompleteCardPayment cardDeetails, ChannelCode channel)
        {
            return Ok(await paymentManager.CompleteCardPayment(cardDeetails, channel));
        }

        [HttpPost("/Payment/validate/card/{channel}")]
        public async Task<IActionResult> ValidatePaymentCard(ValidatePayment paymentDetails, ValidateCardPaymentChannelCode channel)
        {
            return Ok(await paymentManager.ValidateCardPayment(paymentDetails, channel));
        }


    }
}