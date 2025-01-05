using api.DTOs.Admin.Orders;
using api.DTOs.Customer;
using api.Entities.Admin.Order;
using api.Errors;
using api.Extensions;
using api.Helpers;
using api.Interfaces.Admin;
using api.Params.Orders;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace api.Controllers
{
    [Authorize(Policy = "OrderForwardPolicy")]      //Role-Order Forward
    public class OrderForwardController : BaseApiController
    {
        private readonly IOrderForwardRepository _orderFwdRepo;
        private readonly ITaskRepository _taskRepo;
        public OrderForwardController(IOrderForwardRepository orderFwdRepo, ITaskRepository taskRepo)
        {
            _taskRepo = taskRepo;
            _orderFwdRepo = orderFwdRepo;
        }

        /*[HttpGet("pagedlist")]
        public async Task<ActionResult<PagedList<OrderForwardCategory>>> GetOrderForwardsPagedList([FromQuery] OrderFwdParams fParams)
        {
            var fwds = await _orderFwdRepo.GetPagedList(fParams);

            if(fwds == null) return NotFound("No matching Order Forward records found");

            Response.AddPaginationHeader(new PaginationHeader(fwds.CurrentPage, fwds.PageSize, 
                fwds.TotalCount, fwds.TotalPages));
            
            return Ok(fwds);
        }
        */

        [HttpGet("pagedlist")]
        public async Task<ActionResult<PagedList<OrderForwardCategory>>> GetOrderForwardsPagedList([FromQuery] OrderFwdParams fParams)
        {
            var fwds = await _orderFwdRepo.GetPagedListDLForwarded(fParams);

            if(fwds == null) return NotFound("No matching Order Forward records found");

            Response.AddPaginationHeader(new PaginationHeader(fwds.CurrentPage, fwds.PageSize, 
                fwds.TotalCount, fwds.TotalPages));
            
            return Ok(fwds);
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
            if(string.IsNullOrEmpty(errorString)) return Ok("");
            
            return BadRequest(new ApiException(404, "Failed to forward Order to HR Dept", errorString));
        }

        [HttpPost("forwardToAgents/{orderid}")]
        public async Task<ActionResult<bool>> ForwardOrderToAgents(int orderid, ICollection<OfficialAndCustomerNameDto> offsandCustNamedtos)
        {
            var stErr = await _orderFwdRepo.InsertOrUpdateOrderForwardToAgents(offsandCustNamedtos, orderid, User.GetUsername());

            if(string.IsNullOrEmpty(stErr)) return Ok("");

            return BadRequest(new ApiException(400, "Failed to forward the requirements", stErr));
        }

        [HttpPut("updateOrderFwdToAgent")]
        public async Task<ActionResult<string>> UpdateOrderForwardedToAgent(ICollection<OrderForwardCategory> fwd)
        {
            var errorString = await _orderFwdRepo.EditOrderForwardCategories (fwd, User.GetUsername());
            
            if(string.IsNullOrEmpty(errorString)) return Ok();
            
            return BadRequest(new ApiException(404, "Failed to forward Order to Associates", errorString));
        }

        [HttpGet("associatesforwardedForOrderId/{orderid}")]
        public async Task<ActionResult<ICollection<OrderForwardCategoryDto>>> AssociatesToWhomOrderForwarded (int orderid)
        {
            var forwarded = await _orderFwdRepo.AssociatesOfOrderForwardsOfAnOrder(orderid, User.GetUsername());

            if(forwarded==null) return NotFound(new ApiException(404, "Bad Request", "Categories not forwarded to any agents"));

            return Ok(forwarded);
        }
            
        [HttpDelete("deleteOrderFwd/{orderforwardcategoryid}")]
        public async Task<ActionResult<bool>> DeleteOrderForward(int orderforwardcategoryid) {
            var succeeded = await _orderFwdRepo.DeleteOrderForwardCategory(orderforwardcategoryid);
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