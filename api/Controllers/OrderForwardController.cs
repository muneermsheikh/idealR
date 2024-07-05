using api.DTOs.Admin.Orders;
using api.Entities.Admin.Order;
using api.Errors;
using api.Extensions;
using api.Helpers;
using api.Interfaces.Admin;
using api.Params.Orders;
using Microsoft.AspNetCore.Mvc;

namespace api.Controllers
{
    //[Authorize(Policy = "OrderForwardPolicy")]
    public class OrderForwardController : BaseApiController
    {
        private readonly IOrderForwardRepository _orderFwdRepo;
        private readonly ITaskRepository _taskRepo;
        public OrderForwardController(IOrderForwardRepository orderFwdRepo, ITaskRepository taskRepo)
        {
            _taskRepo = taskRepo;
            _orderFwdRepo = orderFwdRepo;
        }

        [HttpGet("pagedlist")]
        public async Task<ActionResult<PagedList<OrderForwardToAgentDto>>> GetOrderForwardsPagedList([FromQuery] OrderFwdParams fParams)
        {
            var fwds = await _orderFwdRepo.GetPagedListOfOrderFwds(fParams);

            if(fwds == null) return NotFound("No matching Order Forward records found");

            Response.AddPaginationHeader(new PaginationHeader(fwds.CurrentPage, fwds.PageSize, 
                fwds.TotalCount, fwds.TotalPages));
            
            return Ok(fwds);
        }

        [HttpGet("getOrGenerateOrderFwdToAgent/{orderid}")]
        public async Task<ActionResult<OrderForwardToAgent>> GenerateOrderForwardToAgents(int orderid)
        {
            
            var obj = await _orderFwdRepo.GenerateOrderForwardToAgent(orderid);
            
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

        [HttpPost("updateOrderFwdToHR/{orderid}")]
        public async Task<ActionResult<string>> UpdateOrderForwardedToHR(int orderid)
        {
            var errorString = await _orderFwdRepo.UpdateForwardOrderToHR(orderid, User.GetUsername());
            if(string.IsNullOrEmpty(errorString)) return Ok();
            
            return BadRequest(new ApiException(404, "Failed to forward Order to HR Dept", errorString));
        }

        [HttpPut("updateOrderFwdToAgent")]
        public async Task<ActionResult<string>> UpdateOrderForwardedToAgent(OrderForwardToAgent fwd)
        {
            var errorString = await _orderFwdRepo.UpdateOrderForwardedToAgents (fwd, User.GetUsername());
            
            if(string.IsNullOrEmpty(errorString)) return Ok();
            
            return BadRequest(new ApiException(404, "Failed to forward Order to Associates", errorString));
        }

        [HttpPost("insertOrderFwdToAgent")]
        public async Task<ActionResult<string>> InsertOrderForwardedToAgent(OrderForwardToAgent orderFwd)
        {
            var errorString = await _orderFwdRepo.InsertOrderForwardedToAgents(orderFwd, User.GetUsername());
            if(string.IsNullOrEmpty(errorString)) return Ok();
            
            return BadRequest(new ApiException(404, "Failed to forward Order to selected Associates", errorString));
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
            
        [HttpDelete("deleteOrderFwd/{orderid}")]
        public async Task<ActionResult<bool>> DeleteOrderForward(int orderid) {
            var succeeded = await _orderFwdRepo.DeleteOrderForward(orderid);
            return succeeded;
        }

        [HttpDelete("deleteOrderFwdCategory/{orderitemid}")]
        public async Task<ActionResult<bool>> DeleteOrderForwardCategory(int orderitemid) {
            var succeeded = await _orderFwdRepo.DeleteOrderForwardCategory(orderitemid);
            return succeeded;
        }

        [HttpDelete("deleteOrderFwdCatOfficial/{catofficialid}")]
        public async Task<ActionResult<bool>> DeleteOrderForwardCategoryOfficial(int catofficialid) {
            var succeeded = await _orderFwdRepo.DeleteOrderForwardCategoryOfficial(catofficialid);
            return succeeded;
        }

    }
}