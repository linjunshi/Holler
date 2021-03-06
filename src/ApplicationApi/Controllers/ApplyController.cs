﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using ApplicationApi.Models;
using ApplicationApi.Repositories;
using System.Text.RegularExpressions;

namespace ApplicationApi.Controllers
{
    [Route("api/[controller]/[action]")]
    public class ApplyController : Controller
    {
        ICustomersRepo _repo = new CustomersRepo(new CustomerContext());

        // GET api/values
        [HttpGet]
        public ICollection<Customer> Get()
        {
            return _repo.get_all_customers();
        }

        // GET api/values/5
        //[HttpGet("{email}")]
        //public Customer Get(string email)
        //{
        //    return _repo.get_customer(email);
        //}

        // helper method
        private bool customer_role_exists(CustomerViewModel custView)
        {
            return _repo.get_customer(custView) != null;
        }

        // POST api/values
        [HttpPost]
        public async Task<IActionResult> register(CustomerViewModel custView)
        {
            custView.contact = Regex.Replace(custView.contact, @"\s+", "");
            if (ModelState.IsValid)
            {
                custView.PostTime = DateTime.Now;
                if (customer_role_exists(custView)) {
                    return await this.update_user(custView);
                } else {
                    await _repo.create_customer(custView);
                }
                return Ok(custView);
            }
            return BadRequest(ModelState);
        }

        // PUT api/values/5
        [HttpPut("{CustomerViewModel}")]
        public async Task<IActionResult> update_user(CustomerViewModel custView)
        {
            // TO-DO
            if (ModelState.IsValid)
            {
                Customer c = _repo.get_customer(custView);
                // if the request is less than 60s
                if (c.PostTime.AddSeconds(10) > DateTime.Now) {
                    return BadRequest($"Please try submit the form later!");
                }
                await _repo.update_customer(custView);
                return Ok(custView);
            }
            return BadRequest(ModelState);
        }

        // DELETE api/values/5
        [HttpDelete("{email}")]
        public void Delete(string email)
        {
        }
    }
}
