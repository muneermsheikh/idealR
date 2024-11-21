using System.Net;
using api.DTOs.Admin;
using api.DTOs.HR;
using api.Entities.Admin.Client;
using api.Errors;
using api.Extensions;
using api.Helpers;
using api.Interfaces.Admin;
using api.Interfaces.HR;
using api.Params;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace api.Controllers
{
    [Authorize(Policy ="FeedbackPolicy")]   //RequireRole("Candidate", "Official", "Document Controller-Processing", "Admin", "Document Controller-Admin"));
    /*
        1. feedback question parameters are in table feedbackStddQs
        2. when a feedback question is to be sent to the customer, a questionnaire is created WITHOUT SAVING IN DATABASE
            - CreateNewFeedbackForm below.
        3. This questionnaire is edited by the user (from RA), then sent to the server to write to the database.  
            - RegisterFeedback.  The data is now ready to be formatted at the client end, to send to the customer
        4. Once the feedback questionnaire is written to the database, it is formatted to a layout in the client section.
        5. A link of this form is sent to the client, who then updates te form with his inputs.
        6. The client then submits the form, which is received by the api.
    */
    public class FeedbackController : BaseApiController
    {
        private readonly IFeedbackRepository _repo;
        private readonly ITaskRepository _taskRepo;
        public FeedbackController(IFeedbackRepository repo, ITaskRepository taskRepo)
        {
            _taskRepo = taskRepo;
            _repo = repo;
        }

        [HttpGet("pagedlist")]
        public async Task<ActionResult<PagedList<FeedbackDto>>> GetPagedList([FromQuery]FeedbackParams fParams)
        {
            var feedbacks = await _repo.GetFeedbackList(fParams);

            if(feedbacks == null) return NotFound("No matching Feedbacks found");
            Response.AddPaginationHeader(new PaginationHeader(feedbacks.CurrentPage, feedbacks.PageSize, 
                feedbacks.TotalCount, feedbacks.TotalPages));
            
            return Ok(feedbacks);
        }
        
        [HttpGet("FeedbackFromId/{id}")]        
        public async Task<CustomerFeedback> GetFeedbackAsync(int id)
        {
            var feedback = await _repo.GetFeedbackWithItems(id);
            return feedback;
        }

        [HttpDelete("delete/{id}")]
        public async Task<bool> DeleteFeedbackAsync(int id)
        {
            return await _repo.DeleteFeedback(id);
        }

        [HttpPut]
        public async Task<ActionResult<string>> EditFeedbackAsync(CustomerFeedback feedback)
        {
            var sError = await _repo.EditFeedback(feedback);
            if(int.TryParse(sError, out _)) return Ok(sError);

            return BadRequest(new ApiException(400, sError, "Not Found"));
        }

        [HttpGet("newfeedback/{feedbackId}/{customerid}")]
        public async Task<CustomerFeedback> GenerateOrGetFeedbackObject(int feedbackId, int customerid)
        {
            var obj = await _repo.GenerateOrGetFeedbackFromId(feedbackId, customerid);
            return obj;
        }

        [HttpGet("generatenewfeedback/{customerid}")]
        public async Task<CustomerFeedback> GenerateNewFeedbackObject(int customerid)
        {
            var obj = await _repo.GenerateNewFeedbackOfCustomer(customerid);
            return obj;
        }

        

        [HttpGet("history/{customerId}")]
        public async Task<ActionResult<ICollection<FeedbackHistoryDto>>> GetFeedbackHistory(int customerId) {

            var hist = await _repo.CustomerFeedbackHistory(customerId);
            //if(hist==null || hist.Count == 0) return BadRequest(new ApiException(400, "Not Found", "No feedback history available"));
            return Ok(hist);
        }

        [HttpPost("saveFeedback")]
        public async Task<CustomerFeedback> SaveFeedback(CustomerFeedback feedbk)
        {
            if(feedbk.FeedbackItems==null) return null;
            
            return await _repo.SaveNewFeedback(feedbk);
        }

        [HttpGet("stddqs")]
        public async Task<ICollection<FeedbackQ>> GetStandardFeedbackQs()
        {
            return await _repo.GetFeedbackStddQs();
        }

        [HttpGet("sendfeedback/{id}")]
        public async Task<ActionResult<string>> sendFeedbackToClientOnline(int id)
        {
            var err = await _repo.SendFeedbackEmailToCustomer(id, User.GetUsername());

            if(string.IsNullOrEmpty(err)) return Ok("");

            return BadRequest(new ApiException(400, "Failed to generate email to client", err));

        }

        [HttpGet("MedicalObjectives")]
        public ActionResult<string> GetMedicalObjectiveData()
        {
            return "reached api";
            /*var pagedList = await _taskRepo.GetMedicalObjectives();

            if(pagedList.Count ==0 || pagedList == null) return BadRequest("No Objectives data available during the dates mentioned");

            Response.AddPaginationHeader(new PaginationHeader(pagedList.CurrentPage, 
                pagedList.PageSize, pagedList.TotalCount, pagedList.TotalPages));
            
            return Ok(pagedList);
            */
        }
    
    }
}