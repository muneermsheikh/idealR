using System.Diagnostics.Contracts;
using api.Entities.Admin.Order;
using api.Extensions;
using api.Helpers;
using api.Interfaces.Admin;
using api.Params.Admin;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace api.Controllers
{
    [Authorize(Policy = "ContractReviewPolicy")]
    public class ContractReviewController : BaseApiController
    {
        private readonly IContractReviewRepository _repo;
        public ContractReviewController(IContractReviewRepository repo)
        {
            _repo = repo;
        }

        [HttpGet("contractreviewspaged")]
        public async Task<ActionResult<PagedList<ContractReviewDto>>> GetContractReviewsPagedList([FromQuery]ContractReviewParams reviewParams)
        {
            var pages = await _repo.GetContractReviews(reviewParams);

            if (pages == null) return NotFound("No matching Contract Reviews found");
            
            Response.AddPaginationHeader(new PaginationHeader(pages.CurrentPage, pages.PageSize, 
                pages.TotalCount, pages.TotalPages));
            
            return Ok(pages);
        }


        [HttpPost("contractreview")]
        public async Task<ActionResult<ContractReview>> AddContractReview(ContractReview contractReview)
        {
            var review = await _repo.AddContractReview(contractReview);

            if (review == null) return  BadRequest("Failed to save the contract review");

            return Ok(review);
        }

        [HttpPut("contractreview")]
        public async Task<ActionResult<ContractReview>> EditContractReview (ContractReview contractReview)
        {
            var review = await _repo.EditContractReview(contractReview); //on return from repo, this has twice the number of OrderItems ???
            if (review == null) return BadRequest("Failed to update the contract review");

            return Ok(review);
        }

        [HttpGet("generate/{orderId}")]
        public async Task<ActionResult<ContractReview>> GenerateContractReview (int orderId)
        {
            var review = await _repo.GenerateContractReviewObject(orderId, User.GetUsername());

            if(review == null) return BadRequest();
            return Ok(review);
        }

        [HttpGet("contractreviewfromOrderId/{orderId}")]
        public async Task<ActionResult<ContractReview>> GetContractReviewFromOrderId(int orderId)
        {
            var review = await _repo.GetContractReviewFromOrderId(orderId);

            if(review == null) return NotFound();

            return Ok(review);
        }

        [HttpDelete("deletebyorderid/{orderid}")]
        public async Task<ActionResult<bool>> DeleteContractReviewFromOrderId(int orderid)
        {
            var deleted = await _repo.DeleteContractReview(orderid);
            if(!deleted) return BadRequest("Failed to delete the contract Review");
            return Ok();

        }

        [HttpDelete("deletereviewitemoforderid/{orderitemid}")]
        public async Task<ActionResult<bool>> DeleteContractReviewItemFromOrderItemId(int orderitemid)
        {
            var deleted = await _repo.DeleteContractReviewItem(orderitemid);
            if(!deleted) return BadRequest("Failed to delete the contract Review Item");
            return Ok();
        }

        [HttpDelete("reviewq/{id}")]     
        public async Task<ActionResult<bool>> DeleteContractReviewItemQ(int id)
        {
            var deleted = await _repo.DeleteReviewQ(id);
            if(!deleted) return BadRequest("Failed to delete the contract Review Question");
            return Ok();
        }

        [HttpGet("reviewstddq")]
        public async Task<ActionResult<ICollection<ContractReviewItemStddQ>>> GetReviewStddQs()
        {
            var qs = await _repo.GetReviewStddQs();

            if(qs==null) return NotFound("No Review standard questions found");

            return Ok(qs);
        }

        [HttpPut("updateorderreviewstatus/{orderid}")]
        public async Task<ActionResult<bool>> UpdateOrderReviewStatus(int orderid)
        {
            var updated = await _repo.UpdateOrderReviewStatus(orderid);
            if(!updated) return BadRequest("Failed to update the review statuses");

            return Ok();

        }

    }
}