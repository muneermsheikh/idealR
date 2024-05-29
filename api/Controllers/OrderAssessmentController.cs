using api.Entities.Admin.Order;
using api.Entities.HR;
using api.Errors;
using api.Extensions;
using api.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SQLitePCL;

namespace api.Controllers
{
    [Authorize(Policy ="HRMPolicy")]
    public class OrderAssessmentController : BaseApiController
    {
        private readonly IOrderAssessmentRepository _repo;
        public OrderAssessmentController(IOrderAssessmentRepository repo)
        {
            _repo = repo;
        }

        [HttpGet("assessmentStddQs")]
        public async Task<ActionResult<ICollection<AssessmentQStdd>>> GetAssessmentQStdds() 
        {
            var qs = await _repo.GetAssessmentQStdds();
            if(qs==null || qs.Count == 0) return NotFound("No standrd assessment questions on record");

            return Ok(qs);

        }

        [HttpGet("orderitemassessment/{orderItemId}")]
        public async Task<ActionResult<OrderItemAssessment>> GetOrderItemAssessment(int orderItemId)
        {
            var assessment = await _repo.GetOrderAssessmentItem(orderItemId);
            if(assessment==null) return NotFound();
            return Ok(assessment);
        }

        [HttpGet("orderitemAssessmt/{orderId}")]
        public async Task<ActionResult<ICollection<OrderItemAssessment>>> GetOrderAssessment(int orderId)
        {
            var assessment = await _repo.GetOrderAssessment(orderId);
            if(assessment==null) return NotFound();
            return Ok(assessment);
        }

                
        [HttpGet("generateAssessment/{orderItemId}")]
        public async Task<ActionResult<OrderItemAssessment>> GenerateOrderItemAssessment(int orderItemId)
        {
            var assessment = await _repo.GenerateOrderAssessmentItemFromStddQ(orderItemId, User.GetUsername());
            if(assessment == null) return BadRequest("Failed to generate the assessment object for the Order Item");

            return Ok(assessment);
        }

        [HttpPost("assessment")]
        public async Task<ActionResult<OrderAssessment>> CreateOrderAssessment (OrderAssessment orderAssessment)
        {
            var obj = await _repo.SaveNewOrderAssessment(orderAssessment);

            if(obj==null) return BadRequest(new ApiException(500, "Bad Request", "Failed to save the order assessment"));

            return Ok(obj);
        }

        [HttpPost("itemassessment")]
        public async Task<ActionResult<OrderItemAssessment>> InsertOrderItemAssessment(OrderAssessmentItem orderItemAssessment)
        {
            var posted = await _repo.SaveOrderAssessmentItem(orderItemAssessment);
            if(posted == null) return BadRequest("Failed to post the OrderItem Assessment");

            return Ok(posted);
        }

        [HttpPut("itemassessment")]
        public async Task<ActionResult<bool>> UpdateOrderItemAssessment(OrderAssessmentItem orderAssessmentItem)
        {
            var updated = await _repo.EditOrderAssessmentItem(orderAssessmentItem);

            if(!updated) return BadRequest("Failed to update the Order Item Assessment");

            return Ok();
        }


        [HttpPut("assessment")]
        public async Task<ActionResult<bool>> UpdateOrderAssessment(OrderAssessment orderAssessment)
        {
            var updated = await _repo.EditOrderAssessment(orderAssessment, User.GetUsername());

            if(!updated) return BadRequest("Failed to update the Order Assessment");

            return Ok(true);
        }

        [HttpGet("orderitemassessmentQ/{orderitemid}")]
        public async Task<ActionResult<ICollection<OrderItemAssessmentQ>>> GetOrderAssessmentItemQs (int orderitemid)
        {
            var obj = await _repo.GetOrderAssessmentItemQs(orderitemid);

            if(obj == null) return BadRequest(new ApiException(402, "Bad Request", "No assessment questions returned"));

            return Ok(obj); 
        }

        [HttpDelete("itemAssessment/{orderItemId}")]
        public async Task<ActionResult<bool>> DeleteOrderItemAssessment(int orderItemId)
        {
            var deleted = await _repo.DeleteOrderAssessmentItem(orderItemId);
            if(!deleted) return BadRequest("Failed to delete the OrderItem Assessment");
            return Ok(true);
        }

        [HttpDelete("itemAssessmentQ/{questionId}")]
        public async Task<ActionResult<bool>> DeleteOrderAssessmentItemQ(int questionId)
        {
            var deleted = await _repo.DeleteOrderAssessmentItemQ(questionId);
            if(!deleted) return BadRequest("Failed to delete the OrderItem Assessment Question");
            return Ok(true);
        }
        

    }
}