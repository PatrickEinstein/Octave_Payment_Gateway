using CentralPG.Core.Dtos;
using CentralPG.Core.Models.Entities;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CentralPG.Infrastructure.Interfaces.IMains
{
    public interface ICustomerService
    {
        Task<CustomerDto> Register(CreateCustomerRequestModel model);
        Task<Customer> Find(int id);
        Task<CustomerDto> GetById(int id);
        Task<CustomerDto> GetByEmail(string email);
        Task<List<CustomerDto>> GetAll();
        Task<AuthTokens> Login(CustomerLoginModel model);
    }
}
