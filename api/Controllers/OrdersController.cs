using api.DTOs.Admin.Orders;
using api.Entities.Admin.Order;
using api.Extensions;
using api.Helpers;
using api.Interfaces;
using api.Params.Orders;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace api.Controllers
{
    [Authorize(Policy = "AdminPolicy")]
    public class OrdersController : BaseApiController
    {
        private readonly IOrdersRepository _repo;
        public OrdersController(IOrdersRepository repo)
        {
            _repo = repo;
        }

        [HttpPost("neworder")]  
        public async Task<ActionResult<Order>> CreateNewOrder(OrderToCreateDto createDto)
        {
            var newOrder = await _repo.CreateOrderAsync(createDto);
            if (newOrder != null) return Ok(newOrder);
            return BadRequest("Failed to create new order");
        }

        [HttpPut("editorder")]
        public async Task<ActionResult<bool>> EditOrder(Order order)
        {
            var edited = await _repo.EditOrder(order);
            if (edited) return Ok();
            return BadRequest("Failed to update the order");
        }
        
        [HttpDelete("deleteorder/{orderid}")]
        public async Task<ActionResult<bool>> DeleteOrder(int orderid)
        {
            var deleted = await _repo.DeleteOrder(orderid);
            if (deleted) return Ok();

            return BadRequest("failed to delete the order");
        }

        [HttpGet("pagedlist")]
        public async Task<ActionResult<PagedList<OrderBriefDto>>> GetOrdersPagedList(OrdersParams orderParams)
        {
            var pagedList = await _repo.GetOrdersAllAsync(orderParams);

            if(pagedList.Count ==0) return BadRequest("failed to retrieve matching orders");

            Response.AddPaginationHeader(new PaginationHeader(pagedList.CurrentPage, 
                pagedList.PageSize, pagedList.TotalCount, pagedList.TotalPages));
            
            return Ok(pagedList);
            
        }

        [HttpGet("orderWithAllRelatedProperties/{orderid}")]
        public async Task<ActionResult<OrderDisplayDto>> GetOrderWithAllRelatedProperties(int orderid)
        {
            var order = await _repo.GetOrderByIdWithAllRelatedProperties(orderid);

            if(order == null) return BadRequest("Failed to return the Order object");

            return Ok(order);

        }

        [HttpGet("orderWithItems/{orderid}")]
        public async Task<ActionResult<OrderDisplayWithItemsDto>> GetOrderWithItems (int orderid)
        {
            var order = await _repo.GetOrderByIdWithItemsAsyc(orderid);

            if(order != null) return Ok(order);

            return NotFound("Order not found");
        }

        [HttpPost("addOrderItem")]
        public async Task<ActionResult<OrderItem>> AddOrderItem(OrderItemToCreateDto itemToCreate)
        {
            var item = await _repo.AddOrderItem(itemToCreate);
            if(item != null) return Ok(item);

            return BadRequest("Failed to add the Order Item");
        }

        [HttpPut("editOrderItem")]
        public async Task<ActionResult<bool>> EditOrderItem (OrderItem orderItem)
        {
            var edited = await _repo.EditOrderItem(orderItem, false);
            if(!edited) return BadRequest("Faied to edit the Order item");
            return Ok();
        }

        [HttpDelete("deleteOrderItem/{orderItemId}")]
        public async Task<ActionResult<bool>> DeleteOrderItem(int orderItemId)
        {
            var edited = await _repo.DeleteOrderItem(orderItemId);
            if(!edited) return BadRequest("Failed to delete the Order Item");
            return Ok();
        }

        [HttpGet("orderItemsMatchingWithProfession/{professionId}")]
        public async Task<ActionResult<ICollection<OrderItemBriefDto>>> GetOrderItemsMatchingProfessionId(int professionId)
        {
            var items = await _repo.GetOpenOrderItemsMatchingAProfession(professionId);

            if(items == null || items.Count == 0) return NotFound("No matching items found");

            return Ok(items);
        }

        [HttpGet("jdoforderitem/{OrderItemId}")]
        public async Task<ActionResult<JobDescription>> GetJDofOrderItem (int OrderItemId)
        {
            var jd = await _repo.GetJDOfOrderItem(OrderItemId);
            if(jd == null) return NotFound("That Order Item does not have any Job Description defined");
            return Ok(jd);
        }

        [HttpPost("jobDescription")]
        public async Task<ActionResult<JobDescription>> AddJobDescription(JobDescription jobDescription)
        {
            var jd = await _repo.AddJobDescription(jobDescription);
            if(jd != null) return Ok(jd);

            return BadRequest("Failed to insert the job description");
        }

        [HttpPut("jobdescription")]
        public async Task<ActionResult<bool>> EditJobDescription(JobDescription jobDescription)
        {
            var edited = await _repo.EditJobDescription(jobDescription);
            if(edited) return Ok(edited);
            return BadRequest("Failed to edit the Job Description");
        }

        [HttpDelete("jobdescription/{jobDescriptionId}")]
        public async Task<ActionResult<bool>> DeleteJobDescription(int jobDescriptionId)
        {
            var edited = await _repo.DeleteJobDescription(jobDescriptionId);
            if(edited) return Ok(edited);
            return BadRequest("Failed to delete the Job Description");
        }

        [HttpGet("remunerationbyorderitemid/{OrderItemId}")]
        public async Task<ActionResult<Remuneration>> GetRemunerationofOrderItem (int OrderItemId)
        {
            var remun = await _repo.GetRemuneratinOfOrderItem(OrderItemId);
            if(remun == null) return NotFound("That Order Item does not have any Remuneration defined");
            return Ok(remun);
        }

        [HttpPost("remuneration")]
        public async Task<ActionResult<Remuneration>> AddRemuneration(Remuneration remuneration)
        {
            var newRemun = await _repo.AddRemuneration(remuneration);
            if(newRemun != null) return Ok(newRemun);
            return BadRequest("Failed to save the remuneration");
        }

        [HttpPut("remuneration")]
        public async Task<ActionResult<Remuneration>> EditRemuneration(Remuneration remuneration)
        {
            var edited = await _repo.EditRemuneration(remuneration);
            if(edited) return Ok(edited);
            return BadRequest("Failed to edit the remuneration details");
        }

        [HttpDelete("remuneration/{remunerationId}")]
        public async Task<ActionResult<bool>> DeleteRemuneration(int remunerationId)
        {
            var deleted = await _repo.DeleteRemuneration(remunerationId);
            if(deleted)  return Ok(deleted);
            return BadRequest("Failed to delete the remuneration");
        }

    }
}