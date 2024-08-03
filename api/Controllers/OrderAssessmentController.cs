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
    [Authorize(Policy ="HRMPolicy")]    //RequireRole("HR Manager", "HR Supervisor", "HR Executive", "Admin", "Admin Manager"));
    public class OrderAssessmentController : BaseApiController
    {
        private readonly IOrderAssessmentRepository _repo;
        public OrderAssessmentController(IOrderAssessmentRepository repo)
        {
            _repo = repo;
        }

        //stdd questions
        [HttpGet("assessmentStddQs")]
        public async Task<ActionResult<ICollection<OrderAssessmentItemQ>>> GetAssessmentQStdds() 
        {
            var qs = await _repo.GetAssessmentQStdds();
            if(qs==null || qs.Count == 0) return NotFound(new ApiException(404,"No standrd assessment questions on record", "Not Found Error"));

            return Ok(qs);

        }

        [HttpGet("questionsFromQBank/{professionid}")]
        public async Task<ActionResult<ICollection<OrderAssessmentItemQ>>> GetAssessmentQsFromQBank(int professionid) 
        {
            var qs = await _repo.GetCustomAssessmentQsForAProfession(professionid);

            if(qs==null || qs.Count == 0) return NotFound(new ApiException(404,"No assessment questions found in the Question Bank matching the given profession", "No Questions in Question Bank"));

            return Ok(qs);

        }

        
        //OrderAssessmentItem
        [HttpGet("orderassessmentitem/{orderItemId}")]
        public async Task<ActionResult<OrderItemAssessment>> GetOrderItemAssessment(int orderItemId)
        {
            var assessment = await _repo.GetOrCreateOrderAssessmentItem(orderItemId, User.GetUsername());
            if(assessment==null) return NotFound();
            return Ok(assessment);
        }

        [HttpGet("orderAssessment/{orderId}")]
        public async Task<ActionResult<OrderAssessment>> GetOrderAssessment(int orderId)
        {
            var assessment = await _repo.GetOrderAssessment(orderId, User.GetUsername());
            if(assessment==null) return NotFound(new ApiException(404, "The Order Assessment Record not found", "Not Found Error"));
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

        [HttpPut("assessmentitem")]
        public async Task<ActionResult<bool>> UpdateOrderItemAssessment(OrderAssessmentItem orderAssessmentItem)
        {
            var updated = await _repo.EditOrderAssessmentItem(orderAssessmentItem);

            if(!updated) return BadRequest("Failed to update the Order Item Assessment");

            return Ok(true);
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