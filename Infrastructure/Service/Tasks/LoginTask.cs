using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using NCrontab;
using CentralPG.Models;

using CentralPG.Core.Models.Entities;
using CentralPG.Interfaces.IProcessors;

namespace CentralPG.Infrastructure.Services.Tasks
{
    public class LoginTask : BackgroundService
    {
        private CrontabSchedule _schedule;
        private DateTime _nextRun;
        private readonly IServiceScopeFactory serviceScopeFactory;
        private readonly ILogger<TestCronJob> logger;

        // private string Schedule => "*/40 */59 */23 * * *"; //Runs every 23:59 time of everyday
        // private string Schedule => "0 0 */6 * * *"; //At 0 minutes past the hour, every 6 hours
        private string Schedule => "* */50 * * * *"; //Runs every 50 minutes
        // private string Schedule => "*/30 * * * * *"; //Runs every 30 seconds


        public LoginTask(IServiceScopeFactory serviceScopeFactory, ILogger<TestCronJob> logger)
        {
            _schedule = CrontabSchedule.Parse(Schedule, new CrontabSchedule.ParseOptions { IncludingSeconds = true });
            _nextRun = _schedule.GetNextOccurrence(DateTime.Now);
            this.serviceScopeFactory = serviceScopeFactory;
            this.logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            do
            {
                var now = DateTime.Now;
                if (now > _nextRun)
                {
                    await Process();
                    _nextRun = _schedule.GetNextOccurrence(DateTime.Now);
                }
                await Task.Delay(5000, stoppingToken); // 5 seconds delay
            }
            while (!stoppingToken.IsCancellationRequested);
        }

        public static string GenerateRandomString(int length = 16)
        {
            Random random = new Random();
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
            StringBuilder result = new StringBuilder(length);
            for (int i = 0; i < length; i++)
            {
                result.Append(chars[random.Next(chars.Length)]);
            }
            return result.ToString();
        }

        private async Task<serviceResponse<AuthTokens>> Process()
        {
            using (var scope = serviceScopeFactory.CreateScope())
            {
                serviceResponse<AuthTokens> res = new serviceResponse<AuthTokens>();
                var service = scope.ServiceProvider.GetRequiredService<IPaymentProcessor>();
                var result = await service.Login();
                logger.LogInformation($"TOKEN ==> {JsonSerializer.Serialize(result)}");
                var resultApi = result.Data;
                res.Data = resultApi;
                logger.LogInformation($"Login in==> {JsonSerializer.Serialize(result)}");
                return res;
            }
        }
    }
}