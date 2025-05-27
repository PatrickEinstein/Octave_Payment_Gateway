using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CentralPG.Interfaces.IProcessors;
using CentralPG.Models;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using NCrontab;
using OCPG.Infrastructure.Interfaces.IManagers;
using OCPG.Infrastructure.Interfaces.IRepositories;

namespace OCPG.Infrastructure.Service.Tasks
{
    public class WalletTransferTask : BackgroundService
    {
         private CrontabSchedule _schedule;
        private DateTime _nextRun;
        private readonly IServiceScopeFactory serviceScopeFactory;
        private readonly ILogger<WalletTransferTask> logger;

        // private string Schedule => "*/40 */59 */23 * * *"; //Runs every 23:59 time of everyday
        // private string Schedule => "0 0 */6 * * *"; //At 0 minutes past the hour, every 6 hours
        // private string Schedule => "* */50 * * * *"; //Runs every 50 minutes
        private string Schedule => "*/30 * * * * *"; //Runs every 30 seconds


        public WalletTransferTask(IServiceScopeFactory serviceScopeFactory, ILogger<WalletTransferTask> logger)
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

      

        private async Task<string> Process()
        {
            using (var scope = serviceScopeFactory.CreateScope())
            {
                
                var paymentManager = scope.ServiceProvider.GetRequiredService<IPaymentManager>();
                var walletRepo = scope.ServiceProvider.GetRequiredService<IWalletRepository>();

                var pendingWithdrawals = await walletRepo.GetAllPendingWithdrawals();

                foreach (var withdrawal in pendingWithdrawals)
                {
                    try
                    {
                        WithdrawFromWallet withdrawalRequest = new WithdrawFromWallet
                        {
                            wallet_accountNumber = withdrawal.wallet_accountNumber,
                            amount = withdrawal.amount,
                            transactionReference = withdrawal.transactionReference,
                            narration = withdrawal.narration,
                            bank_accountNumber = withdrawal.bank_accountNumber,
                        };
                        await paymentManager.ProcessInternalTransferFromWalletProviderToBankAccount(withdrawalRequest, withdrawal.channelCode);
                   
                    }
                    catch (Exception ex)
                    {
                        logger.LogError(ex, "Error processing withdrawal for account {AccountNumber}", withdrawal.wallet_accountNumber);
                    }
                }
                return "";
            }
        }
    }
}