using Microsoft.EntityFrameworkCore;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CentralPG.Data;
using CentralPG.Infrasturcture.Interfaces.IRepositories;
using CentralPG.Core.Dtos;
using CentralPG.Core.Models.Entities;

namespace CentralPG.Infrastructure.Services.Repositories;

public class CustomerRepository : ICustomerRepository
{
    private readonly DataBaseContext _context;

    public CustomerRepository(DataBaseContext context)
    {
        _context = context;
    }

    public CustomerDto Create(Customer customer)
    {
        _context.Customers.Add(customer);
        _context.SaveChanges();
        return new CustomerDto
        {
            Id = customer.Id,

            FirstName = customer.FirstName,

            LastName = customer.LastName,

            PhoneNumber = customer.PhoneNumber,

            Address = customer.Address,

            Email = customer.Email
        };
    }

    public void Delete(Customer customer)
    {
        _context.Customers.Remove(customer);
        _context.SaveChanges();
    }

    public bool Exists(string email)
    {
        return _context.Customers.Any(c => c.Email == email);
    }

    public Customer Find(int id)
    {
        return _context.Customers.Find(id);
    }

    public List<CustomerDto> GetAll()
    {
        return _context.Customers
            .Select(customer => new CustomerDto
            {
                Id = customer.Id,

                FirstName = customer.FirstName,

                LastName = customer.LastName,

                PhoneNumber = customer.PhoneNumber,

                Address = customer.Address,

                Email = customer.Email,

            }).ToList();
    }

    public CustomerDto GetByEmail(string email)
    {
        return _context.Customers.Where(c => c.Email == email)
            .Select(customer => new CustomerDto
            {
                Id = customer.Id,

                FirstName = customer.FirstName,

                LastName = customer.LastName,

                PhoneNumber = customer.PhoneNumber,

                Address = customer.Address,

                Email = customer.Email,

            }).SingleOrDefault();
    }

    public CustomerDto GetById(int id)
    {
        return _context.Customers.Where(c => c.Id == id)
            .Select(customer => new CustomerDto
            {
                Id = customer.Id,

                FirstName = customer.FirstName,

                LastName = customer.LastName,

                PhoneNumber = customer.PhoneNumber,

                Address = customer.Address,

                Email = customer.Email,

            }).SingleOrDefault();
    }

    public CustomerDto Update(Customer customer)
    {
        _context.Customers.Update(customer);
        _context.SaveChanges();
        return new CustomerDto
        {
            Id = customer.Id,

            FirstName = customer.FirstName,

            LastName = customer.LastName,

            PhoneNumber = customer.PhoneNumber,

            Address = customer.Address,

            Email = customer.Email
        };

    }
}
