using api.Entities.Admin.Client;
using api.Errors;
using api.Extensions;
using api.Interfaces.Admin;
using api.Interfaces.Customers;
using Microsoft.AspNetCore.Mvc;

namespace api.Controllers
{
    public class CustomerReviewController: BaseApiController
    {
        private readonly ICustomerReviewRepository _custRvwRepo;
        public CustomerReviewController(ICustomerReviewRepository custRvwRepo) => _custRvwRepo = custRvwRepo;


        [HttpGet("customerreview/{customerId}")]
        public async Task<CustomerReview> GetCustomerReview(int customerId)
        {
            return await _custRvwRepo.GetCustomerReview(customerId);
        }

        [HttpPut("customerreview")]
        public async Task<bool> UpdateCustomerReview(CustomerReview customerReview)
        {
            return await _custRvwRepo.UpdateCustomerReview(customerReview, User.GetUsername());
        }

        [HttpGet("customerReviewStatusData")]
        public async Task<ActionResult<ICollection<string>>> GetCustomerReviewStatusData()
        {
            var data = await _custRvwRepo.GetCustomerReviewStatusData();

            if(data.Count > 0) return Ok(data);

            return BadRequest(new ApiException(402, "Bad Request", "No Customer Review Data on record"));
        }
    }
}