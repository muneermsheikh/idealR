using api.DTOs.Admin;
using api.Entities.Admin;
using api.Errors;
using api.Extensions;
using api.Helpers;
using api.Interfaces.Admin;
using api.Params.Admin;
using Microsoft.AspNetCore.Mvc;

namespace api.Controllers
{
    public class VisasController : BaseApiController
    {
        private readonly IVisaRepository _visaRepo;
        public VisasController(IVisaRepository visaRepo)
        {
            _visaRepo = visaRepo;
        }

    
        [HttpGet("PagedList")]
        public async Task<ActionResult<PagedList<VisaBriefDto>>> GetPagedList([FromQuery] VisaParams vParams)
        {
            var pagedlist = await _visaRepo.GetVisaPagedList(vParams);
            if (pagedlist.Count == 0) return Ok(null);    //  return BadRequest(new ApiException(400,"Bad Request", "failed to retrieve matching Visa records"));

            Response.AddPaginationHeader(new PaginationHeader(pagedlist.CurrentPage, 
            pagedlist.PageSize, pagedlist.TotalCount, pagedlist.TotalPages));
        
            return Ok(pagedlist);
        }

        [HttpGet("VisaHeader/{visaId}")]
        public async Task<ActionResult<VisaHeader>> GetVisaHeader(int visaId)
        {
            return await _visaRepo.GetVisaHeader(visaId);
        }

        [HttpPut("editVisa")]
        public async Task<ActionResult<VisaHeader>> UpdateVisa(VisaHeader vHeader)
        {
            var dto = await _visaRepo.EditVisa(vHeader);

            if(dto != null) return BadRequest(new ApiException(404, "Failure", "Failed to update the visa"));

            return Ok(dto);
        }

        [HttpPost("insertNewVisa")]
        public async Task<ActionResult<VisaHeader>> InsertNewVisa(VisaHeader visaHeader)
        {
            var header = await _visaRepo.InsertVisa(visaHeader);

            if(header==null) return BadRequest(new ApiException(400, "Failure", "failed to insert the Visa"));

            return Ok(header);
        }

        [HttpDelete("visa/{visaid}")]
        public async Task<ActionResult<bool>> DeleteVisa(int visaid)
        {
            var deleted = await _visaRepo.DeleteVisa(visaid);
            return Ok(deleted);
        }

        [HttpPost("insertNewVisaitem")]
        public async Task<ActionResult<VisaHeader>> InsertVisaItem(VisaItem visaItem)
        {
            var item = await _visaRepo.InsertVisaItem (visaItem);

            if(item==null) return BadRequest(new ApiException(400, "Failure", "failed to insert the Visa item"));

            return Ok(item);
        }

        [HttpDelete("visaItem/{visaitemid}")]
        public async Task<ActionResult<bool>> DeleteVisaItem(int visaitemid)
        {
            var deleted = await _visaRepo.DeleteVisaItem(visaitemid);
            return Ok(deleted);
        }
    
        [HttpPost("transaction")]
        public async Task<ActionResult<VisaTransaction>> InsertVisaTransaction(VisaTransaction vTransaction)
        {
            var obj = await _visaRepo.InsertVisaTransaction(vTransaction);
            if(obj == null) return BadRequest(new ApiException(400, "Error", "Failed to insert the transaction"));

            return Ok(obj);
        }

        [HttpPut("editTransaction")]
        public async Task<ActionResult<VisaTransaction>> EditVisaTransaction(VisaTransaction vTransaction)
        {
            var obj = await _visaRepo.EditVisaTransaction(vTransaction);

            if(obj == null) return BadRequest(new ApiException(400, "Error", "Failed to edit the transaction"));

            return Ok(obj);
        }
    
    
        [HttpDelete("ItemTransaction/{visaitemid}")]
        public async Task<ActionResult<bool>> DeleteVisaTransaction(int visaTransactionid)
        {
            var deleted = await _visaRepo.DeleteVisaTransaction(visaTransactionid);
            return Ok(deleted);
        }

        [HttpGet("TransactionsPagedList")]
        public async Task<ActionResult<PagedList<VisaTransaction>>> GetVisaTransactionPagedList([FromQuery] VisaParams vParams)
        {
            var pagedlist = await _visaRepo.GetVisaTransactionPagedList(vParams);
            if (pagedlist.Count == 0) return Ok(null);    //  return BadRequest(new ApiException(400,"Bad Request", "failed to retrieve matching Visa records"));

            Response.AddPaginationHeader(new PaginationHeader(pagedlist.CurrentPage, 
            pagedlist.PageSize, pagedlist.TotalCount, pagedlist.TotalPages));
        
            return Ok(pagedlist);
        }

        [HttpGet("OpenOrderItemsForCustomer/{CustomerId}")]
        public async Task<ActionResult<ICollection<OrderItemForVisaAssignmentDto>>> GetOrderItemsAvailableForCustomer(int CustomerId)
        {
            var dto = await _visaRepo.GetOpenOrderItemsForCustomer(CustomerId);
            //if(dto.Count == 0) return NotFound(new ApiException(400, "No record found", "The Customer has no unassigned quantity"));
            return Ok(dto);
        }

        [HttpPost("assignVisas")]
        public async Task<ActionResult<ICollection<VisaAssignment>>> AssignVisasToOrderIems(ICollection<VisaAssignment> visaAssignments)
        {
            var obj = await _visaRepo.InsertVisaAssignments(visaAssignments);
            if(obj.Count == visaAssignments.Count) return Ok(obj);

            return BadRequest(new ApiException(400, "Failure to isert", "Failed to insert the visa assignments"));
        }

    }
}