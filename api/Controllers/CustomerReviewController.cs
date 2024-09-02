using api.Entities.Admin.Client;
using api.Errors;
using api.Extensions;
using api.Interfaces.Customers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace api.Controllers
{
    [Authorize(Policy = "AdminPolicy")]     //ROLES: Admin
    public class CustomerReviewController: BaseApiController
    {
        private readonly ICustomerReviewRepository _custRvwRepo;
        public CustomerReviewController(ICustomerReviewRepository custRvwRepo) => _custRvwRepo = custRvwRepo;


        [AllowAnonymous]
        [HttpGet("customerreview/{customerId}")]
        public async Task<CustomerReview> GetCustomerReview(int customerId)
        {
            //return await _custRvwRepo.GetCustomerReview(customerId);
            return await _custRvwRepo.GetOrCreateCustomerReviewObject(customerId, User.GetUsername());
        }

        
        [AllowAnonymous]
        [HttpGet("getOrCreateObject/{customerid}")]
        public async Task<CustomerReview> GetOrCreateCustomerReviewObject(int customerid) {
            return await _custRvwRepo.GetOrCreateCustomerReviewObject(customerid, User.GetUsername());
        }

        [HttpPost("insertnew")]
        public async Task<bool> GetCustomerReview(CustomerReview review)
        {
            return await _custRvwRepo.InsertNewCustomerReview(review);
        }

        [HttpPut("customerreview")]
        public async Task<bool> UpdateCustomerReview(CustomerReview customerReview)
        {
            return await _custRvwRepo.UpdateCustomerReview(customerReview, User.GetUsername());
        }

        [HttpPut("approveReviewItem/{reviewitemid}")] 
        public async Task<ActionResult<bool>> ApproveReviewItem(int reviewitemid) {
            var updated = await _custRvwRepo.ApproveReviewItem(reviewitemid, User.GetUsername());

            if(!updated) return BadRequest(new ApiException(400, "Failed to update", "Failed to update the customer review item"));

            return Ok(true);
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