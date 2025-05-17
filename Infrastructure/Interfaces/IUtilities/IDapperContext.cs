using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;


namespace CentralPG.Infrasturcture.Interfaces.Utilities
{
    public interface IDapperContext
    {
        IDbConnection GetDbConnection();
        IDbConnection GetMerchantDbConnection();
        IDbConnection GetPaymentDbConnection();
    }
}