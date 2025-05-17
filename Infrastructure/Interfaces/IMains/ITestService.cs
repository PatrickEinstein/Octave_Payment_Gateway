using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CentralPG.Models;


namespace CentralPG.Infrastructure.Interfaces.IMains
{
    public interface ITestService
    {
        Task<serviceResponse<String>> Test();
    }
}