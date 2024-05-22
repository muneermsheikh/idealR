using api.DTOs.Admin.Orders;
using api.Entities.Admin.Order;
using api.Errors;
using api.Extensions;
using api.Interfaces.Admin;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace api.Controllers
{
    [Authorize(Policy = "OrderForwardPolicy")]
    public class OrderForwardController : BaseApiController
    {
        private readonly IOrderForwardRepository _orderFwdRepo;
        private readonly ITaskRepository _taskRepo;
        public OrderForwardController(IOrderForwardRepository orderFwdRepo, ITaskRepository taskRepo)
        {
            _taskRepo = taskRepo;
            _orderFwdRepo = orderFwdRepo;
        }

        [HttpGet("generateOrderFwdToAgent/{orderid}")]
        public async Task<ActionResult<OrderForwardToAgent>> GenerateOrderForwardToAgents(int orderid)
        {
            
            var obj = await _orderFwdRepo.GenerateOrderForwardObjForAgentByOrderId(orderid);
            
            if(obj != null) return Ok(obj);
            
            return BadRequest(new ApiException(404, "Failed to generate the Order Forward Mesasge for AGents"));
        }

        [HttpGet("generateOrderFwdToHR/{orderid}")]
        public async Task<ActionResult<OrderForwardToHR>> GenerateOrderForwardToHR(int orderid)
        {
            var obj = await _orderFwdRepo.GenerateObjToForwardOrderToHR(orderid);
            
            if(obj != null) return Ok(obj);
            
            return BadRequest(new ApiException(404, "Failed to generate the Order Forward object for HR Dept"));
        }



        [HttpPost]
        public async Task<ActionResult<string>> UpdateOrderForwardedToAgents(OrderForwardToAgent orderforward)
        {
            
            var errorString = await _orderFwdRepo.UpdateOrderForwardedToAgents(orderforward );
            if(string.IsNullOrEmpty(errorString)) return Ok();
            
            return BadRequest(new ApiException(404, "Failed to forward Order to Associates", errorString));
        }

        [HttpPost("updateOrderFwdToHR")]
        public async Task<ActionResult<string>> UpdateOrderForwardedToHR(OrderForwardToHR orderFwdHR)
        {
            var errorString = await _orderFwdRepo.UpdateForwardOrderToHR(orderFwdHR, User.GetUsername());
            if(string.IsNullOrEmpty(errorString)) return Ok();
            
            return BadRequest(new ApiException(404, "Failed to forward Order to HR Dept", errorString));
        }

        [HttpPost("updateOrderFwdToAgent")]
        public async Task<ActionResult<string>> UpdateOrderForwardedToAgent(OrderForwardToAgent orderFwd)
        {
            var errorString = await _orderFwdRepo.UpdateOrderForwardedToAgents(orderFwd);
            if(string.IsNullOrEmpty(errorString)) return Ok();
            
            return BadRequest(new ApiException(404, "Failed to forward Order to HR Dept", errorString));
        }

      
        [HttpGet("byorderid/{orderid}")]
        public async Task<ActionResult<ICollection<OrderForwardToAgent>>> GetDLForwardsOfOrderId(int orderid)
        {
            var dto = await _orderFwdRepo.OrderFowardsOfAnOrder(orderid);
            
            if (dto==null) return NotFound(new ApiException(402, "No Forwards exist for the selected order"));
            
            return Ok(dto);
        }

        [HttpGet("associatesforwardedForOrderId/{orderid}")]
        public async Task<ActionResult<OrderForwardToAgentDto>> AssociatesToWhomOrderForwarded (int orderid)
        {
            var forwarded = await _orderFwdRepo.AssociatesOfOrderForwardsOfAnOrder(orderid, User.GetUsername());

            if(forwarded==null) return NotFound(new ApiException(404, "Bad Request", "Categories not forwarded to any agents"));

            return Ok(forwarded);
        }
            
    }
}