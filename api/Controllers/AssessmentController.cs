using api.Entities.HR;
using api.Extensions;
using api.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace api.Controllers
{
    [Authorize(Policy ="HRMPolicy")]
    public class AssessmentController : BaseApiController
    {
        private readonly IAssessmentRepository _repo;
        public AssessmentController(IAssessmentRepository repo)
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
            var assessment = await _repo.GetOrderItemAssessment(orderItemId);
            if(assessment==null) return NotFound();
            return Ok(assessment);
        }

        [HttpGet("orderAssessments/{orderId}")]
        public async Task<ActionResult<OrderItemAssessment>> GetOrderAssessment(int orderId)
        {
            var assessment = await _repo.GetOrderAssessments(orderId);
            if(assessment==null) return NotFound();
            return Ok(assessment);
        }

        [HttpGet("generateAssessment/{orderItemId}")]
        public async Task<ActionResult<OrderItemAssessment>> GenerateOrderItemAssessment(int orderItemId)
        {
            var loggedInUserName = User.GetUsername();
            var assessment = await _repo.GenerateOrderItemAssessmentFromStddQ(orderItemId, loggedInUserName);
            if(assessment == null) return BadRequest("Failed to generate the assessment object for the Order Item");

            return Ok(assessment);
        }

        [HttpPost("assessment")]
        public async Task<ActionResult<OrderItemAssessment>> InsertOrderItemAssessment(OrderItemAssessment orderItemAssessment)
        {
            var posted = await _repo.SaveOrderItemAssessment(orderItemAssessment);
            if(posted == null) return BadRequest("Failed to post the OrderItem Assessment");

            return Ok(posted);
        }

        [HttpPut("assessment")]
        public async Task<ActionResult<bool>> UpdateOrderItemAssessment(OrderItemAssessment orderItemAssessment)
        {
            var updated = await _repo.EditOrderItemAssessment(orderItemAssessment);

            if(!updated) return BadRequest("Failed to update the Order Item Assessment");

            return Ok();
        }

        [HttpDelete("itemAssessment/{orderItemId}")]
        public async Task<ActionResult<bool>> DeleteOrderItemAssessment(int orderItemId)
        {
            var deleted = await _repo.DeleteOrderItemAssessment(orderItemId);
            if(!deleted) return BadRequest("Failed to delete the OrderItem Assessment");
            return Ok(true);
        }

        [HttpDelete("itemAssessmentQ/{orderItemId}")]
        public async Task<ActionResult<bool>> DeleteOrderItemAssessmentQ(int orderItemId)
        {
            var deleted = await _repo.DeleteOrderItemAssessmentQ(orderItemId);
            if(!deleted) return BadRequest("Failed to delete the OrderItem Assessment Question");
            return Ok(true);
        }
        

    }
}