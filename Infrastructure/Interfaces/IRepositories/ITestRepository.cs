using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CentralPG.Infrasturcture.Interfaces.IRepositories
{
    public interface ITestRepository
    {
       Task<List<T>> TestGeManyt<T>(string foo);

       Task<T> TestGetSingle<T>(string foo);
       Task<bool> TestUpdate(string foo);

    }
}