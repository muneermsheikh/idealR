using api.DTOs.Admin;
using api.DTOs.Orders;
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

        
        //OrderAssessmentItem
        [HttpGet("orderassessmentitem/{orderItemId}")]
        public async Task<ActionResult<OrderAssessmentItemDto>> GetOrderItemAssessment(int orderItemId)
        {
            var assessment = await _repo.GetOrCreateOrderAssessmentItem(orderItemId, User.GetUsername());
            if(!string.IsNullOrEmpty(assessment.Error)) return BadRequest(new ApiException(400, "Error in getting OrderAssessmentItem", assessment.Error));
            
            return Ok(assessment.orderAssessmentItemDto);
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
        public async Task<ActionResult<OrderAssessmentItem>> InsertOrderItemAssessment(OrderAssessmentItem orderItemAssessment)
        {
            var posted = await _repo.SaveOrderAssessmentItem(orderItemAssessment);
            if(posted == null) return BadRequest("Failed to post the OrderItem Assessment");

            return Ok(posted);
        }

        [HttpPut("assessmentitem")]
        public async Task<ActionResult<bool>> UpdateOrderItemAssessment(OrderAssessmentItem orderAssessmentItem)
        {
            if(orderAssessmentItem.Id == 0) {
                var posted = await _repo.SaveOrderAssessmentItem(orderAssessmentItem);
                if(posted == null) return BadRequest("Failed to post the OrderItem Assessment");      
                return Ok(true);
            } else {
                var updated = await _repo.EditOrderAssessmentItem(orderAssessmentItem, User.GetUsername());
                if(!updated) return BadRequest("Failed to update the Order Item Assessment");
                return Ok(true);
            }
            
        }


        [HttpPut("assessment")]
        public async Task<ActionResult<string>> UpdateOrderAssessment(OrderAssessment orderAssessment)
        {
            var strErr = await _repo.EditOrderAssessment(orderAssessment, User.GetUsername());

            if(!string.IsNullOrEmpty(strErr)) return BadRequest(new ApiException(400,"Faild to update the order assessment", strErr));

            return Ok("");
        }

        [HttpGet("orderassessmentItemQsFromOrderItemId/{orderitemid}")]
        public async Task<ActionResult<ICollection<OrderAssessmentItemQ>>> GetOrderAssessmentItemQsFromOrderItemId (int orderitemid)
        {
            var obj = await _repo.GetOrderAssessmentItemQsFromOrderItemId(orderitemid);

            if(obj == null) return BadRequest(new ApiException(402, "Bad Request", "No assessment questions returned"));

            return Ok(obj); 
        }

        [HttpGet("assessmentItemHeaders/{professionGroup}")]
        public async Task<ActionResult<ICollection<OrderAssessmentItemHeaderDto>>> GetOrderAssessmentItemHeaders (string professionGroup)
        {
            var dtos = await _repo.GetOrderAssessmentHeaders(professionGroup);

            //if (dtos == null || dtos.Count == 0) return BadRequest(new ApiException(400, "No record returned", "No matching order assessment exist for the selection Profession Group"));

            return Ok(dtos);
        }

        [HttpGet("orderassessmentItemQs/{assessmentitemid}")]
        public async Task<ActionResult<ICollection<OrderAssessmentItemQ>>> GetOrderAssessmentItemQsFromId (int assessmentitemid)
        {
            var obj = await _repo.GetOrderAssessmentItemQsFromId(assessmentitemid);

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

 
        [HttpGet("getAndSetProfessionGroup/{ProfessionId}/{OrderAssessmentItemId}")]
        public async Task<ActionResult<SingleStringDto>> SetOrderAssessmentItemFromProfId(int ProfessionId, int OrderAssessmentItemId)
        {
            var st = await _repo.GetAndSetProfessionGroupFromProfessionId(ProfessionId, OrderAssessmentItemId);
            
            return Ok(st);
        }   
    }
}