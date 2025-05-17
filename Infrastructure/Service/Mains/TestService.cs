using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using CentralPG.Models;
using CentralPG.Infrastructure.Interfaces.IMains;


namespace CentralPG.Infrastructure.Services.Mains
{
    public class TestService:ITestService
    {
        public ILogger<TestService> Logger { get; }
        public TestService(ILogger<TestService> logger)
        {
            Logger = logger;  //don't you think _logger is easier to differentiate??
        }


        public async Task<serviceResponse<String>> Test()
        {
            serviceResponse<String> response = new serviceResponse<string>();
            response.Message = "Hello World!";
            Logger.LogInformation($"{JsonSerializer.Serialize(response)}");

            return response;
        }
    }
}