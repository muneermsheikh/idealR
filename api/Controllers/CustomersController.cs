using api.DTOs.Admin;
using api.Entities.Admin.Client;
using api.Entities.Identity;
using api.Extensions;
using api.Helpers;
using api.Interfaces;
using api.Params.Admin;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace api.Controllers
{
    [Authorize(Policy="AdminPolicy")]
    public class CustomersController : BaseApiController
    {
        private readonly ICustomerRepository _customerRepo;
        private readonly UserManager<AppUser> _userManager;
        private readonly IMapper _mapper;
        public CustomersController(ICustomerRepository customerRepo, UserManager<AppUser> userManager, IMapper mapper)
        {
            _mapper = mapper;
            _userManager = userManager;
            _customerRepo = customerRepo;
        }

        [HttpGet]
        public async Task<ActionResult<PagedList<CustomerDto>>> GetCustomers([FromQuery]CustomerParams customerParams)
        { 
            /*var Username = User.GetUsername();
            if(string.IsNullOrEmpty(Username)) return Unauthorized();
            var user = await _userManager.Users
                .SingleOrDefaultAsync(x => x.UserName == Username.ToLower());
            
            if(user==null) return Unauthorized("invalid credentials");
            */
            var Customers = await _customerRepo.GetCustomers(customerParams);

            if(Customers == null) return NotFound("No matching customers found");
            Response.AddPaginationHeader(new PaginationHeader(Customers.CurrentPage, Customers.PageSize, 
                Customers.TotalCount, Customers.TotalPages));
            
            return Ok(Customers);

        }

        [HttpPost]
        public async Task<ActionResult> CreateCustomer(CreateCustomerDto createDto)
        {
            var username = User.GetUsername();
            var newCustomer = _mapper.Map<Customer>(createDto);

            if (await _customerRepo.InsertCustomer(newCustomer)) return Ok();
            
            return BadRequest("Failed to create the Customer Object");
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteCustomer(int id)
        {
            if(await _customerRepo.DeleteCustomer(id)) return Ok();
            
            return BadRequest("Failed to delete the customer");
        }

        [HttpPut("edit")]
        public async Task<ActionResult> EditCustomer(Customer customer)
        {
            var edited = await _customerRepo.UpdateCustomer(customer);
            if(edited) return Ok();
            return BadRequest("Failed to edit the customer");
        }

        [HttpGet("byid/{id}")]
        public async Task<Customer> GetCustomerById(int id)
        {
            return await _customerRepo.GetCustomerById(id);
        }
    }
}