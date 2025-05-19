using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
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
    public class WebHooksController : ControllerBase
    {
        private readonly IPaymentManager paymentManager;
        private readonly ILogger<WebHooksController> logger;

        public WebHooksController(IPaymentManager paymentManager, ILogger<WebHooksController> logger)
        {
            this.paymentManager = paymentManager;
            this.logger = logger;
        }

        [HttpPost("/webhook/{channel}")]
        public async Task<IActionResult> Webhook(ChannelCode channel)
        {
            string bodyString = await new StreamReader(Request.Body).ReadToEndAsync();

            logger.LogInformation($"Wallet Transaction Notification Callback Response: {bodyString}");

            if (channel == ChannelCode.chamsSwitch)
            {
                var payload = JsonSerializer.Deserialize<WebHookRequestModel>(bodyString);

                if (payload != null)
                {
                    var res = await paymentManager.WebHookNotification(payload, channel);
                    return Ok(res);
                }
                else
                {
                    logger.LogError("Deserialization of WebHookRequestModel failed.");
                    return BadRequest("Invalid payload.");
                }
            }
            return BadRequest("Invalid channel.");
        }
    }
}