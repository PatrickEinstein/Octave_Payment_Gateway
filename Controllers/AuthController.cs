using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CentralPG.Core.Dtos;
using CentralPG.Infrastructure.Interfaces.IMains;
using Microsoft.AspNetCore.Mvc;


namespace OCPG.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly ICustomerService _customerService;

        public AuthController(ICustomerService customerService)
        {
            this._customerService = customerService;
        }


        [HttpPost("/register")]
        public async Task<IActionResult> Login([FromBody] CreateCustomerRequestModel model)
        {
            var res = await _customerService.Register(model);
            return Ok(res);
        }


        [HttpPost("/login")]
        public async Task<IActionResult> Register([FromBody] CustomerLoginModel model)
        {
            var res = await _customerService.Login(model);

            return Ok(res);
        }

    }
}