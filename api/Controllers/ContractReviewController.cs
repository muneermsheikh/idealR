using System.Reflection.Metadata.Ecma335;
using api.DTOs.Admin;
using api.DTOs.Order;
using api.Entities.Admin.Order;
using api.Errors;
using api.Extensions;
using api.Helpers;
using api.Interfaces.Admin;
using api.Params.Admin;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace api.Controllers
{
    [Authorize(Policy = "ContractReviewPolicy")]        //Role-Contract Review
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

    

        [HttpPut("reviewitem")]
        public async Task<ActionResult<ContractReviewItem>> UpdateReviewItem(ContractReviewItem reviewitem)
        {
            var item = await _repo.EditContractReviewItem(reviewitem, false);   //the false flag causes updates to be written to DB
            return item;
        }

        [HttpPost("reviewitem")]
        public async Task<ActionResult<ContractReviewItem>> PostNewReviewItem(ContractReviewItem reviewItem)
        {
            var obj = await _repo.SaveNewContractReviewItem(reviewItem);

            if (obj == null) return BadRequest(new ApiException(400, "Bad Request", "Failed to create the Contract Review Item"));

            return Ok(obj);
        }

        [HttpPost("contractreview")]
        public async Task<ActionResult<ContractReview>> AddContractReview(ContractReview contractReview)
        {
            var review = await _repo.AddContractReview(contractReview);

            if (review == null) return  BadRequest("Failed to save the contract review");

            return Ok(review);
        }

        [HttpPut("contractreview")]
        public async Task<ActionResult<bool>> EditContractReview (ContractReview contractReview)
        {
            var succeeded = await _repo.EditContractReview(contractReview); //on return from repo, this has twice the number of OrderItems ???
            if (!succeeded) return BadRequest("Failed to update the contract review");

            return Ok(succeeded);
        }

        [HttpGet("reviewitem/{orderitemid}")]
        public async Task<ActionResult<ContractReviewItemDto>> GetContractReviewItem(int orderitemid)
        {
                var item = await _repo.GetOrGenerateContractReviewItem(orderitemid, User.GetUsername());
                if (item == null) return NotFound(new ApiException(400, "Bad Request", "Failed to retrieve the contract review item"));

                return Ok(item);
        }
        
        [HttpGet("reviewitems/{orderid}")]
        public async Task<ActionResult<ICollection<ContractReviewItem>>> GetContractReviewItems(int orderid)
        {
            var obj = await _repo.GetContractReviewItems(orderid);
            if (obj == null) return NotFound();

            return Ok(obj);
        }
        
        [HttpGet("getOrGenerate/{orderId}")]
        public async Task<ActionResult<ContractReview>> GenerateContractReview (int orderId)
        {
            var review = await _repo.GetOrGenerateContractReviewNOSAVE(orderId, User.GetUsername());

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

        [HttpGet("updateorderreviewstatus/{id}")]
        public async Task<ActionResult<string>> UpdateOrderReviewStatus(int id)
        {
            var status = await _repo.UpdateOrderReviewStatusWITHSAVE(id,0);
            
            return Ok(status);

        }

    }
}