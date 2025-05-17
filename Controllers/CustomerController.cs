using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CentralPG.Core.Dtos;
using CentralPG.Infrastructure.Interfaces.IMains;

namespace CentralPG.Controllers;

[Route("api/v1/[controller]")]
[ApiController]
public class CustomerController : ControllerBase
{
    private readonly ICustomerService _customerService;

    public CustomerController(ICustomerService customerService)
    {
        _customerService = customerService;
    }


    // [HttpGet("{id}")]
    // public IActionResult GetCustomer([FromRoute] int id)
    // {
    //     return Ok(_customerService.GetById(id));
    // }

    // [HttpGet]
    // public IActionResult GetAll()
    // {
    //     return Ok(_customerService.GetAll());
    // }
}

