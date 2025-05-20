using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CentralPG.Data;
using CentralPG.Infrastructure.Services.Mains;
using CentralPG.Infrasturcture.Interfaces.Utilities;
using CentralPG.Interfaces.IProcessors;
using CentralPG.Models;
using Microsoft.EntityFrameworkCore.Storage;
using OCPG.Core.Enums;
using OCPG.Core.Models;
using OCPG.Infrastructure.Interfaces.ICryptographies;
using OCPG.Infrastructure.Interfaces.IRepositories;
using OCPG.Infrastructure.Interfaces.ISwitches;
using OCPG.Infrastructure.Service.Processors;


namespace accessFT.Infrastructures.Services.Switches
{
    public class CardSwitcher : ICardSwitcher
    {

        private readonly Dictionary<string, IPaymentProcessor> cardProcessors = new();

        private readonly AppUrl appUrl;
        private readonly DataBaseContext dataBaseContext;
        private readonly PaystackAuthConfig paystackAuthConfig;
        private readonly PayStackAppUrls payStackAppUrls;
        private readonly FlutterAuthConfig flutterAuthConfig;
        private readonly FlutterWaveAppUrls flutterWaveAppUrls;
        private readonly IFlutterCryptography flutterCryptography;
        private readonly ICardRepository cardRepository;
        private readonly AuthConfig authConfig;
        private readonly IPaymentRepository paymentRepository;

        public IApiCaller ApiCaller { get; set; }

        public CardSwitcher(AppUrl appUrl, IApiCaller apiCaller,
        AuthConfig authConfig, IPaymentRepository paymentRepository,
        DataBaseContext dataBaseContext,
        PaystackAuthConfig paystackAuthConfig,
        PayStackAppUrls payStackAppUrls,
        FlutterAuthConfig flutterAuthConfig,
        FlutterWaveAppUrls flutterWaveAppUrls,
        IFlutterCryptography flutterCryptography,
        ICardRepository cardRepository
        )
        {
            this.ApiCaller = apiCaller;
            this.authConfig = authConfig;
            this.paymentRepository = paymentRepository;
            this.dataBaseContext = dataBaseContext;
            this.paystackAuthConfig = paystackAuthConfig;
            this.payStackAppUrls = payStackAppUrls;
            this.flutterAuthConfig = flutterAuthConfig;
            this.flutterWaveAppUrls = flutterWaveAppUrls;
            this.flutterCryptography = flutterCryptography;
            this.cardRepository = cardRepository;
            this.appUrl = appUrl;
        }


        public IPaymentProcessor SwitchCardProcessor(ChannelCode channelCode)
        {
            if (cardProcessors.ContainsKey(channelCode.ToString()))
            {
                return cardProcessors[channelCode.ToString()];
            }

            IPaymentProcessor cardProcessor = channelCode.ToString() switch
            {
                "chamsSwitch" => new ChamsSwitch(appUrl, ApiCaller, authConfig, paymentRepository, dataBaseContext),
                "paystack" => new PayStack(payStackAppUrls, ApiCaller, paystackAuthConfig, paymentRepository, dataBaseContext),
                "flutterWave" => new FlutterWave(flutterWaveAppUrls, ApiCaller, flutterAuthConfig, paymentRepository,dataBaseContext, flutterCryptography, cardRepository),
                _ => null,
            };

            if (cardProcessors != null)
            {
                cardProcessors[channelCode.ToString()] = cardProcessor;
            }

            return cardProcessor;
        }
    }
}