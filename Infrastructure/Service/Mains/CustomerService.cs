
using CentralPG.Core.Dtos;
using CentralPG.Core.Models.Entities;
using CentralPG.Infrastructure.Interfaces.IMains;
using CentralPG.Infrasturcture.Interfaces.IRepositories;


using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CentralPG.Infrastructure.Sevices.Mains;

public class CustomerService : ICustomerService
{
    private readonly ICustomerRepository _customerRepository;

    public CustomerService(ICustomerRepository customerRepository)
    {
        _customerRepository = customerRepository;
    }

    public async Task<Customer> Find(int id)
    {
        return _customerRepository.Find(id);
    }

    public async Task<List<CustomerDto>> GetAll()
    {
        return _customerRepository.GetAll();
    }

    public async Task<CustomerDto> GetByEmail(string email)
    {
        return _customerRepository.GetByEmail(email);
    }

    public async Task<CustomerDto> GetById(int id)
    {
        return _customerRepository.GetById(id);
    }

    public async Task<CustomerDto> Register(CreateCustomerRequestModel model)
    {
        if (_customerRepository.Exists(model.Email))
        {
            //Throw custom already exit exception...
            return null;
        }
        else
        {
            var customer = new Customer
            {
                Email = model.Email,
                Password = model.Password,
                FirstName = model.FirstName,
                LastName = model.LastName,
                Address = model.Address,
                PhoneNumber = model.PhoneNumber
            };

            return _customerRepository.Create(customer);
        }
    }

    public async Task<AuthTokens> Login(CustomerLoginModel model)
    {
        var res = this.GetByEmail(model.Email);
        if (res != null)
        {
            return new AuthTokens
            {

            };
        }
        else
        {
            return new AuthTokens
            {
                status = false,
            };
        }
    }
}

