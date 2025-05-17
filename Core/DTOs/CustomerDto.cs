
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CentralPG.Core.Dtos;

public record CustomerDto
{
    public int Id { get; set; }

    public string FirstName { get; set; }

    public string LastName { get; set; }

    public string PhoneNumber { get; set; }

    public string Address { get; set; }

    public string Email { get; set; }



}

public record CustomerLoginModel
{
    public string Username { get; set; }
    public string Email { get; set; }
    public string Password { get; set; }

}

public record CreateCustomerRequestModel
{
    public string FirstName { get; set; }

    public string LastName { get; set; }

    public string PhoneNumber { get; set; }

    public string Address { get; set; }

    public string Email { get; set; }

    public string Password { get; set; }
}

