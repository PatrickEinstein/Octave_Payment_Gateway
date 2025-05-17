using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using CentralPG.Models;
using CentralPG.Infrastructure.Interfaces.IMains;


namespace CentralPG.Controllers
{
    [ApiController]
    [ApiExplorerSettings(IgnoreApi = true)]
    [Route("api/[controller]")]
    public class TestController : ControllerBase
    {
        private readonly ITestService testService;

        public TestController(ITestService testService)
        {
            this.testService = testService;
        }

        [HttpGet("/api/test")]
        public async Task<serviceResponse<String>> Test()
        {
            var res = await testService.Test();
            return res;
        }

    }
}