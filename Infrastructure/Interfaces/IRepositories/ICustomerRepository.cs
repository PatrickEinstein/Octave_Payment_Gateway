using CentralPG.Core.Dtos;
using CentralPG.Core.Models.Entities;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CentralPG.Infrasturcture.Interfaces.IRepositories
{
    public interface ICustomerRepository
    {
        public CustomerDto GetById(int id);

        public CustomerDto GetByEmail(string email);

        public CustomerDto Create(Customer customer);

        public CustomerDto Update(Customer customer);

        public void Delete(Customer customer);

        public List<CustomerDto> GetAll();

        public bool Exists(string email);

        public Customer Find(int id);

        //public Customer Login(CustomerLoginModel model);
    }
}
