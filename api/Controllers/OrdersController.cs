using api.DTOs.Admin.Orders;
using api.Entities.Admin.Order;
using api.Errors;
using api.Extensions;
using api.Helpers;
using api.Interfaces;
using api.Interfaces.Orders;
using api.Params.Orders;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace api.Controllers
{
    //[Authorize(Policy = "AdminPolicy")]
    public class OrdersController : BaseApiController
    {
        private readonly IOrdersRepository _repo;
        private readonly IJDAndRemunRepository _jdAndRemunRepo;
        public OrdersController(IOrdersRepository repo, IJDAndRemunRepository jdAndRemunRepo)
        {
            _jdAndRemunRepo = jdAndRemunRepo;
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
        public async Task<ActionResult<PagedList<OrderBriefDto>>> GetOrdersPagedList([FromQuery]OrdersParams orderParams)
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
        
        [HttpGet("ackToClient/{orderid}")]
        public async Task<ActionResult<bool>> GenerateOrderAcknowledgement(int orderid)
        {
            var MsgWithErr =  await _repo.ComposeMsg_AckToClient(orderid);
            
            if(!string.IsNullOrEmpty(MsgWithErr.ErrorString)) 
                return BadRequest(new ApiException(402, "Failed to generate email", MsgWithErr.ErrorString));

            return Ok(true);
        }

        [HttpGet("openorderitemcategories")]
        public async Task<ActionResult<PagedList<OpenOrderItemCategoriesDto>>> OpenOrderItemCategories(OpenOrderItemsParams openitemParams)
        {
            var pagedList = await _repo.GetOpenItemCategories(openitemParams);

            if(pagedList.Count ==0) return BadRequest("No order items on record matching the criteria");

            Response.AddPaginationHeader(new PaginationHeader(pagedList.CurrentPage, 
                pagedList.PageSize, pagedList.TotalCount, pagedList.TotalPages));
            
            return Ok(pagedList);
            
        }

        [HttpGet("openorderitemcategorylist")]
        public async Task<ActionResult<ICollection<OpenOrderItemCategoriesDto>>> OpenOrderItemCategoryList()
        {
            var obj = await _repo.GetOpenItemCategoryList();

            if(obj.Count() ==0) return BadRequest("No order items on record matching the criteria");

            return Ok(obj);
        }

        [HttpGet("openorderitemBriefDto")]
        public async Task<ActionResult<ICollection<OrderItemBriefDto>>> GetBriefOrderDto (int orderid)
        {
            var order = await _repo.GetOrderByIdWithItemsAsyc(orderid);

            if(order != null) return Ok(order);

            return NotFound("Order not found");
        }

        
        [HttpGet("brieforderitemsdto")]
        public async Task<ActionResult<ICollection<OrderItemBriefDto>>> GetOrderWithItemsBriefDto (int orderid)
        {
            var order = await _repo.GetOpenOrderItemsBriefDto();

            if(order != null) return Ok(order);

            return NotFound("Order not found");
        }

        [HttpGet("orderWithItems/{orderid}")]
        public async Task<ActionResult<Order>> GetOrderWithItems (int orderid)
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

        [HttpGet("itembrief/{orderitemid}")]
        public async Task<ActionResult<OrderItemBriefDto>> GetOrderItemDto (int orderitemid)
        {
            var order = await _repo.GetOrderByIdWithItemsAsyc(orderitemid);

            if(order != null) return Ok(order);

            return NotFound("Order not found");
        }

        [HttpGet("orderitemrefcode/{orderitemid}")]
        public async Task<ActionResult<string>> GetOrderItemRefCode (int orderitemid)
        {
            var st = await _repo.GetOrderItemRefCode(orderitemid);
            if(string.IsNullOrEmpty(st)) return "";
            return Ok(st);
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

        [HttpGet("openorderitems")]
        public async Task<ActionResult<PagedList<OrderItemBriefDto>>> GetOpenOrderItems(OpenOrderItemsParams orderParams)
        {
            var items = await _repo.GetOpenOrderItems(orderParams);

            if(items == null || items.Count == 0) return NotFound("No matching items found");

             if(items.Count ==0) return BadRequest("failed to retrieve matching orders");

            Response.AddPaginationHeader(new PaginationHeader(items.CurrentPage, 
                items.PageSize, items.TotalCount, items.TotalPages));
            
            return Ok(items);
        }

        [HttpGet("jd/{OrderItemId}")]
        public async Task<ActionResult<JobDescription>> GetJDofOrderItem (int OrderItemId)
        {
            var jd = await _jdAndRemunRepo.GetJDOfOrderItem(OrderItemId);
            if(jd == null) return NotFound("That Order Item does not have any Job Description defined");
            return Ok(jd);
        }

        [HttpPost("jobDescription")]
        public async Task<ActionResult<JobDescription>> AddJobDescription(JobDescription jobDescription)
        {
            var jd = await _jdAndRemunRepo.AddJobDescription(jobDescription);
            if(jd != null) return Ok(jd);

            return BadRequest("Failed to insert the job description");
        }

        [HttpPut("jd")]
        public async Task<ActionResult<bool>> EditJobDescription(JobDescription jobDescription)
        {
            var edited = await _jdAndRemunRepo.EditJobDescription(jobDescription);
            if(edited) return Ok(edited);
            return BadRequest("Failed to edit the Job Description");
        }

        [HttpDelete("jobdescription/{jobDescriptionId}")]
        public async Task<ActionResult<bool>> DeleteJobDescription(int jobDescriptionId)
        {
            var edited = await _jdAndRemunRepo.DeleteJobDescription(jobDescriptionId);
            if(edited) return Ok(edited);
            return BadRequest("Failed to delete the Job Description");
        }

        [HttpGet("remuneration/{OrderItemId}")]
        public async Task<ActionResult<RemunerationDto>> GetRemunerationofOrderItem (int OrderItemId)
        {
            var remun = await _jdAndRemunRepo.GetRemunerationDtoOfOrderItem(OrderItemId);
            if(remun == null) return NotFound("That Order Item does not have any Remuneration defined");
            return Ok(remun);
        }

        [HttpPost("remuneration")]
        public async Task<ActionResult<Remuneration>> AddRemuneration(Remuneration remuneration)
        {
            var newRemun = await _jdAndRemunRepo.AddRemuneration(remuneration);
            if(newRemun != null) return Ok(newRemun);
            return BadRequest("Failed to save the remuneration");
        }

        [HttpPut("remuneration")]
        public async Task<ActionResult<Remuneration>> EditRemuneration(Remuneration remuneration)
        {
            var edited = await _jdAndRemunRepo.EditRemuneration(remuneration);
            if(edited) return Ok(edited);
            return BadRequest("Failed to edit the remuneration details");
        }

        [HttpDelete("remuneration/{remunerationId}")]
        public async Task<ActionResult<bool>> DeleteRemuneration(int remunerationId)
        {
            var deleted = await _jdAndRemunRepo.DeleteRemuneration(remunerationId);
            if(deleted)  return Ok(deleted);
            return BadRequest("Failed to delete the remuneration");
        }

    }
}