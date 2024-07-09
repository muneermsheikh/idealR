using api.DTOs;
using api.DTOs.Admin;
using api.DTOs.HR;
using api.Entities.HR;
using api.Errors;
using api.Extensions;
using api.Helpers;
using api.Interfaces.Admin;
using api.Interfaces.HR;
using api.Params.Admin;
using api.Params.HR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace api.Controllers
{
    [Authorize(Policy = "CVRefPolicy")]
    public class CVRefController : BaseApiController
    {
          private readonly ICVRefRepository _cvrefRepo;
          private readonly IComposeMessagesAdminRepository _adminMsgRepo;
          private readonly ICandidateAssessentRepository _candAssessRepo;
          public CVRefController(ICVRefRepository cvrefRepo, ICandidateAssessentRepository candAssessRepo, IComposeMessagesAdminRepository adminMsgRepo)
          {
            this._candAssessRepo = candAssessRepo;
          this._adminMsgRepo = adminMsgRepo;
               _cvrefRepo = cvrefRepo;
          }
          
          [HttpGet("referralsbyParams")]
          public async Task<ActionResult<PagedList<CVRefDto>>> GetReferralsOfOrderItemId(CVRefParams refParams)
          {
               var refs = await _cvrefRepo.GetCVReferrals(refParams);
               if (refs == null) return NotFound(new ApiException(404, "No record found"));

               Response.AddPaginationHeader(new PaginationHeader(refs.CurrentPage, refs.PageSize, 
               refs.TotalCount, refs.TotalPages));
          
               return Ok(refs);
          
          }

     
          [HttpGet("cvref/{cvrefid}")]
          public async Task<ActionResult<CVRefDto>> GetCVRef(int cvrefid)
          {
               var cvref = await _cvrefRepo.GetCVRefDto(cvrefid);

               if (cvref == null)  return NotFound(new ApiException(404, "Not Found"));

               return cvref;
          }

        
          [HttpGet("selDecisionReminder/{CustomerId}")]
          public async Task<ActionResult<bool>> SelDecisionReminderMessage(int CustomerId )
          {
               var msg = await _adminMsgRepo.ComposeSelDecisionRemindersToClient(CustomerId, User.GetUsername());
               if(msg == null) 
                    return BadRequest(new ApiException(400, "Bad Request", "Failed to compose reminder message"));

               return Ok("Selection Reminder message composed and saved");
          }
          
          [HttpGet("cvrefwithdeploys/{cvrefid}")]
          public async Task<ActionResult<CVRefWithDepDto>> GetCVRefWithDeploys(int cvrefid)
          {
               var dto = await _cvrefRepo.GetCVRefWithDeploys(cvrefid);

               return Ok(dto);
          }

          [HttpPost]
          public async Task<ActionResult<string>> MakeReferrals(ICollection<int> CVReviewIds)
          {
               var msgWithErr = await _cvrefRepo.MakeReferrals(CVReviewIds, User.GetUsername());

               if(!string.IsNullOrEmpty(msgWithErr.ErrorString)) return BadRequest(new ApiException(400, "Failed to forward the CVs to the client", msgWithErr.ErrorString));

               if(!string.IsNullOrEmpty(msgWithErr.Notification)) return Ok("CVs refferred, but with some errors: " + msgWithErr.Notification);
               
               return Ok("");
          }

          //[Authorize(Roles="DocumentControllerAdmin, HRManager")]
          [HttpPut]
          public async Task<ActionResult<bool>> EditAReferral(CVRef cvref)
          {
               return await _cvrefRepo.EditReferral(cvref);
          }

          //[Authorize(Roles="Admin, DocumentControllerAdmin, HRManager, HRSupervisor, HRExecutive, HRTrainee")]
          [HttpGet("cvsreferred")]
          public async Task<ActionResult<PagedList<CVRefDto>>> CustomerReferralsPending([FromQuery]CVRefParams cvrefParams)
          {
               //cvrefParams.RefStatus = "Referred";

               var pendings = await _cvrefRepo.GetCVReferralsPending(cvrefParams);

               if (pendings==null && pendings.Count == 0) return NotFound(new ApiException(402, "No CVs pending for forwarding to customers"));
               
               Response.AddPaginationHeader(new PaginationHeader(pendings.CurrentPage, pendings.PageSize, 
                    pendings.TotalCount, pendings.TotalPages));
            
               return Ok(pendings);

          }

          [HttpGet("cvsavailabletorefer")]
          public async Task<ActionResult<PagedList<CandidateAssessedDto>>> CandidatesReadyToRefer(CandidateAssessmentParams candAssessParam)
          {
               var dto = await _candAssessRepo.GetCandidateAssessments(candAssessParam);
               if (dto==null && dto.Count == 0) return NotFound(new ApiException(402, "No CVs pending for forwarding to customers"));
               
               Response.AddPaginationHeader(new PaginationHeader(dto.CurrentPage, dto.PageSize, 
                    dto.TotalCount, dto.TotalPages));
            
               return Ok(dto);

          }
          
          [HttpPut("updatecandidatesassesswithcvrefid")]
          public async Task<ActionResult<int>> UpdateCandidateAssessmentsWithCVRefId()
          {
               return await _cvrefRepo.UpdateCandidateAssessmentWithCVRefId();
          }

          [HttpDelete("deletecvref/{cvrefid}")]
          public async Task<ActionResult<bool>> DeleteCVRef(int cvrefid)
          {
               return await _cvrefRepo.DeleteReferral(cvrefid);
          }
    }
}