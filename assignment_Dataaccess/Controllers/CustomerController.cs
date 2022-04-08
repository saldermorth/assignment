﻿using assignment_Dataaccess.Models;
using assignment_Dataaccess.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace assignment_Dataaccess.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CustomerController : ControllerBase
    {
        private readonly ICustomerService _customerService;

        public CustomerController(ICustomerService customerService)
        {
            _customerService = customerService;
        }
        #region Create
        [HttpPost]
        public async Task CreateProduct(Customer cust) //Om man vill kan man här konvertera objektet här
        {
            await _customerService.CreateAsync(cust);
        }
        #endregion
        #region Read
        #endregion
        #region Update
        #endregion
        #region Delete
        #endregion
    }
}
