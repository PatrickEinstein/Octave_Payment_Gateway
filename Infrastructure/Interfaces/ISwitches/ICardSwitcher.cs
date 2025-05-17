using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CentralPG.Interfaces.IProcessors;
using OCPG.Core.Enums;

namespace OCPG.Infrastructure.Interfaces.ISwitches
{
    public interface ICardSwitcher
    {
         public IPaymentProcessor SwitchCardProcessor(ChannelCode channelCode);
    }
}