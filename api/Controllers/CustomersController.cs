using api.DTOs.Admin;
using api.DTOs.Customer;
using api.Entities.Admin.Client;
using api.Entities.Identity;
using api.Errors;
using api.Extensions;
using api.Helpers;
using api.Interfaces;
using api.Params.Admin;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace api.Controllers
{
    [Authorize(Policy = "CustomerPolicy")]      //"HR Manager", "HR Supervisor", "Admin", "Admin Manager", "Document Controller-Admin"));
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

        [Authorize(Policy="CustomerPolicy")]
        [HttpGet]
        public async Task<ActionResult<PagedList<CustomerDto>>> GetCustomers([FromQuery]CustomerParams customerParams)
        { 
            var Customers = await _customerRepo.GetCustomers(customerParams);

            if(Customers == null) return NotFound("No matching customers found");
            Response.AddPaginationHeader(new PaginationHeader(Customers.CurrentPage, Customers.PageSize, 
                Customers.TotalCount, Customers.TotalPages));
            
            return Ok(Customers);
        }

        [HttpGet("list/{customerType}")]
        public async Task<ActionResult<ICollection<CustomerDto>>> GetCustomerList(string customerType)
        { 
            var Customers = await _customerRepo.GetCustomerList(customerType);

            if(Customers == null) return NotFound("No matching customers found");
            
            return Ok(Customers);

        }


        [Authorize(Policy="CustomerPolicy")]
        [HttpPost]
        public async Task<ActionResult<ReturnStringsDto>> CreateCustomer(CreateCustomerDto createDto)
        {
            var newCustomer = _mapper.Map<Customer>(createDto);
            var dto = new ReturnStringsDto();
            var st = await _customerRepo.InsertCustomer(newCustomer);
            if(!string.IsNullOrEmpty(st)) {
                dto.ErrorString = st;
            } else {
                dto.SuccessString = "Customer Created";
            }
            
            return dto;
            
        }

        [HttpGet("customercities/{customerType}")]
        public async Task<ActionResult<ICollection<string>>> GetCustomerCities(string customerType)
        {
            var cities = await _customerRepo.GetCustomerCities(customerType);
            if(cities == null) return NotFound();
            return Ok(cities);
        }

        [Authorize(Policy="CustomerPolicy")]
        [HttpDelete("customer/{id}")]
        public async Task<ActionResult> DeleteCustomer(int id)
        {
            if(await _customerRepo.DeleteCustomer(id)) return Ok();
            
            return BadRequest("Failed to delete the customer");
        }

        [Authorize(Policy="CustomerPolicy")]
        [HttpDelete("official/{id}")]
        public async Task<ActionResult> DeleteCustomerOfficial(int id)
        {
            if(await _customerRepo.DeleteCustomerOfficial(id)) return Ok();
            
            return BadRequest("Failed to delete the customer official");
        }
        

        [Authorize(Policy="CustomerPolicy")]
        [HttpPut("edit")]
        public async Task<ActionResult<ReturnStringsDto>> EditCustomer(Customer customer)
        {
            var edited = await _customerRepo.UpdateCustomer(customer);
            var dto = new ReturnStringsDto();
            if (edited) {
                dto.SuccessString = "Customer Updated successfully";
            } else {
                dto.ErrorString = "Failed to update the customer";
            }
            
            return Ok(dto);
        }

        [HttpGet("customernamefromId/{customerId}")]
        public async Task<ReturnStringsDto> GetCustomerNameFromCustomerId(int customerId)
        {
            var dto = new ReturnStringsDto();
            var cust = await _customerRepo.GetCustomerById(customerId);
            if(cust == null) {
                dto.ErrorString = "Failed to retrieve the customer data";
            } else {
                dto.SuccessString = cust.CustomerName + " retrieved";
            }
            return dto;
        }


        [HttpGet("clientidandnames/{customertype}")]
        public async Task<ICollection<ClientIdAndNameDto>> GetCustomerIdAndName(string customertype)
        {
            var cust = await _customerRepo.GetCustomerIdAndNames(customertype);
            if(cust == null) return null;
            return cust;
        }

        [HttpGet("byid/{id}")]
        public async Task<Customer> GetCustomerById(int id)
        {
            return await _customerRepo.GetCustomerById(id);
        }

        [HttpGet("agentdetails/{customerType}")]
        public async Task<ICollection<CustomerAndOfficialsDto>> GetAgentDetails(string customerType)
        {
            return await _customerRepo.GetCustomerAndOfficials(customerType);
        }

        [Authorize(Policy="CustomerPolicy")]
        [HttpPut("updateofficialwithappuserid")]
        public async Task<ActionResult<ReturnStringsDto>> UpdateCustomerOfficialWithAppUserId(CustomerOfficial official)
        {
            var succeeded = await _customerRepo.UpdateCustomerOfficialWithAppuserId(official);
            if(succeeded) return Ok();
            return BadRequest("failed to  update the data");
        }

        [HttpGet("officialidandcustomernames/{customerType}")]
        public async Task<ActionResult<ICollection<OfficialAndCustomerNameDto>>> GetOfficialIdAndCustomerNames(string customerType)
        {
            var obj = await _customerRepo.GetOfficialsAndCustomerNames(customerType);

            if(obj == null) return BadRequest(new ApiException(404,"Bad Request", "No records returned for customer type " + customerType));

            return Ok(obj);

        }
    }
}