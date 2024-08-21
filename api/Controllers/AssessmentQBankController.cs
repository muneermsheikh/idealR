using api.DTOs.HR;
using api.Entities.Admin.Order;
using api.Entities.HR;
using api.Entities.Master;
using api.Errors;
using api.Extensions;
using api.Helpers;
using api.Interfaces.HR;
using api.Params.HR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace api.Controllers
{
    public class AssessmentQBankController : BaseApiController
    {
        private readonly IAssessmentQBankRepository _qBankRepo;
        public AssessmentQBankController(IAssessmentQBankRepository qBankRepo)
        {
            _qBankRepo = qBankRepo;
        }
        
        [HttpGet("existingqbankprofs")]
        public async Task<ICollection<Profession>> ExistingQBankCategories()
        {
            return await _qBankRepo.GetExistingCategoriesInAssessmentQBank();
        }

        [HttpGet("assessmentbankqs")]
        public async Task<ActionResult<PagedList<AssessmentQBankDto>>> GetAssessmentBankQs([FromQuery] AssessmentQBankParams qbankParams)
        {
            var qs = await _qBankRepo.GetAssessmentQBanks(qbankParams);     //includes qbankitems
            if(qs == null) return NotFound("No matching Assessment Questions found from the Question Bank");

            Response.AddPaginationHeader(new PaginationHeader(qs.CurrentPage, qs.PageSize, 
                qs.TotalCount, qs.TotalPages));
            
            return Ok(qs);
        } 

        [HttpGet("assessmentstddqs")]
        public async Task<ActionResult<ICollection<AssessmentQBankDto>>> GetAssessmentBankQList(AssessmentQBankParams qParams)
        {
            var qs = await _qBankRepo.GetAssessmentStddQList(qParams);     
            if(qs == null) return NotFound("No matching Assessment Questions found from the Question Bank");

            return Ok(qs);
        } 


        [HttpGet("catqs/{categoryName}")]
        public async Task<AssessmentQBank> GetAssessmentQsOfCategoryByName(string categoryName)
        {
            return await _qBankRepo.GetAssessmentQsOfACategoryByName(categoryName);
        }

        [HttpGet("catqsbyorderitem/{orderitemid}")]
        public async Task<ICollection<OrderAssessmentItemQ>> GetAssessmentQBankByCategoryId(int orderitemid)
        {
            var q = await _qBankRepo.GetAssessmentQBankByOrderItemId(orderitemid);
            return q;
        }

        [Authorize(Policy = "HRMPolicy")]
        [HttpPost]
        public async Task<ActionResult<AssessmentQBank>> InsertAssessmentQ(AssessmentQBank qbank)
        {
            var q = await _qBankRepo.InsertAssessmentQBank(qbank);
            if (q == null) return BadRequest(new ApiException(400, "Bad Request", 
                "this probably means the Assessment Question for the chosen category already exists"));
            return Ok(q);
        }

        [HttpPost("stddq")]
        public async Task<ActionResult<AssessmentStddQ>> InsertStddQ(AssessmentStddQ stddQ)
        {
            var q = await _qBankRepo.InsertStddQ(stddQ);

            return q;
        }

        [Authorize(Policy ="HRMPolicy")]
        [HttpPut]
        public async Task<AssessmentQBank> UpdateAssessmentQBank(AssessmentQBank qBank)
        {
            var success = await _qBankRepo.UpdateAssessmentQBank(qBank);
            return success;
        }

        [Authorize(Policy ="HRMPoicy")]
        [HttpPut("stddq")]
        public async Task<ActionResult<AssessmentStddQ>> UpdateStddQ(AssessmentStddQ stddQ)
        {
           return await _qBankRepo.UpdateStddQ(stddQ);

        }

        [Authorize(Policy ="HRMPolicy")]
        [HttpDelete("stddq/{questionId}")]
        public async Task<ActionResult<bool>> DeleteStandardQ(int questionId)
        {
            return await _qBankRepo.DeleteAssessmentQBank(questionId);
        }


    }
}