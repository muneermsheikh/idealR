using api.DTOs.HR;
using api.Entities.Admin.Order;
using api.Entities.HR;
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
        
   
        [HttpGet("questionsFromQBank/{professionid}")]
        public async Task<ActionResult<AssessmentBank>> GetAssessmentQsFromQBank(int professionid) 
        {
            var qs = await _qBankRepo.GetOrCreateCustomAssessmentQsForAProfession(professionid);

            if(qs==null) return NotFound(new ApiException(404,"No assessment questions found in the Question Bank matching the given profession", "No Questions in Question Bank"));

            return Ok(qs);

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
            var qs = await _qBankRepo.GetAssessmentQBanks(qParams);     
            if(qs == null) return NotFound("No matching Assessment Questions found from the Question Bank");

            return Ok(qs);
        } 


        [HttpGet("catqsbyorderitem/{orderitemid}")]
        public async Task<ICollection<OrderAssessmentItemQ>> GetAssessmentQBankByCategoryId(int orderitemid)
        {
            var q = await _qBankRepo.GetAssessmentQBankByOrderItemId(orderitemid);
            return q;
        }

        [Authorize(Policy = "HRMPolicy")]
        [HttpPost]
        public async Task<ActionResult<AssessmentBankQ>> InsertAssessmentQ(AssessmentBankQ qbank)
        {
            var q = await _qBankRepo.InsertStddQ(qbank);
            if (q == null) return BadRequest(new ApiException(400, "Bad Request", 
                "this probably means the Assessment Question for the chosen category already exists"));
            return Ok(q);
        }

        [HttpPost("stddq")]
        public async Task<ActionResult<AssessmentBankQ>> InsertStddQ(AssessmentBankQ stddQ)
        {
            var q = await _qBankRepo.InsertStddQ(stddQ);

            return q;
        }

        [Authorize(Policy ="HRMPolicy")]
        [HttpPut]
        public async Task<AssessmentBankQ> UpdateAssessmentQBank(AssessmentBankQ qBank)
        {
            var success = await _qBankRepo.UpdateStddQ(qBank);
            return success;
        }

        [Authorize(Policy ="HRMPolicy")]
        [HttpPut("assessmentBank")]
        public async Task<bool> UpdateAssessmentBank(AssessmentBank qBank)
        {
            if(qBank.Id == 0) {
                return await _qBankRepo.InsertAssessmentBank(qBank);
            } else {
                return await _qBankRepo.UpdateAssessmentBank(qBank);
            }
            
        }

        [Authorize(Policy ="HRMPolicy")]
        [HttpPost("assessmentBank")]
        public async Task<bool> InsertAssessmentBank(AssessmentBank qBank)
        {
            return  await _qBankRepo.InsertAssessmentBank(qBank);
        }

        [Authorize(Policy ="HRMPoicy")]
        [HttpPut("stddq")]
        public async Task<ActionResult<AssessmentBankQ>> UpdateStddQ(AssessmentBankQ stddQ)
        {
           return await _qBankRepo.UpdateStddQ(stddQ);

        }

        [Authorize(Policy ="HRMPolicy")]
        [HttpDelete("stddq/{questionId}")]
        public async Task<ActionResult<bool>> DeleteStandardQ(int questionId)
        {
            return await _qBankRepo.DeleteAssessmentBankQ(questionId);
        }

        
        [HttpGet("qBankPaged")]
        public async Task<ActionResult<PagedList<AssessmentBank>>> GetQBankPaginated([FromQuery] AssessmentQBankParams qbankParams)
        {
            var qs = await _qBankRepo.GetQBankPaginated(qbankParams);     //includes qbankitems
            if(qs == null) return NotFound("No matching Assessment Questions found from the Question Bank");

            Response.AddPaginationHeader(new PaginationHeader(qs.CurrentPage, qs.PageSize, 
                qs.TotalCount, qs.TotalPages));
            
            return Ok(qs);
        } 

    }
}