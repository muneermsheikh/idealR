using api.Entities.Admin;
using api.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace api.Controllers
{

    /*
        1. feedback question parameters are in table feedbackStddQs
        2. when a feedback question is to be sent to the customer, a questionnaire is created WITHOUT SAVING IN DATABASE
            - CreateNewFeedbackForm below.
        3. This questionnaire is edited by the user (from RA), then sent to the server to write to the database.  
            - RegisterFeedback.  The data is now ready to be formatted at the client end, to send to the customer
        4. Once the feedback questionnaire is written to the database, it is formatted to a layout in the client section.
        5. A link of this form is sent to the client, who then updates te form with his inputs.
        6. The client then submits the form, which is received by the api server.
    */
    public class FeedbackController : BaseApiController
    {
        private readonly IFeedbackRepository _repo;
        public FeedbackController(IFeedbackRepository repo)
        {
            _repo = repo;
        }

        [HttpGet("FeedbackFrommId/{id}")]        
        public async Task<Feedback> GetFeedbackAsync(int id)
        {
            var feedback = await _repo.GetFeedbackFromId(id);
            return feedback;
        }

        [HttpDelete("delete/{id}")]
        public async Task<bool> DeleteFeedbackAsync(int id)
        {
            return await _repo.DeleteFeedback(id);

        }

        [HttpPut]
        public async Task<bool> EditFeedbackAsync(Feedback feedback)
        {
            return await _repo.EditFeedback(feedback);
        }

        [HttpPost("newfeedback/{customerId}")]
        public async Task<Feedback> CreateNewFeedbackForm(int customerId)
        {
            var form = await _repo.GenerateNewFeedback(customerId);
            return form;
        }


        [HttpPost("registerfeedback")]
        public async Task<Feedback> RegisterFeedback(Feedback feedback)
        {
            return await _repo.InsertFeedback(feedback);
        }

        [HttpPost("stddq")]
        public async Task<ICollection<FeedbackStddQ>> InsertFeedbackStddQ(ICollection<FeedbackStddQ> feedbackQs)
        {
            return await _repo.InsertFeedbackStddQs(feedbackQs);
        }

        [HttpGet("stddqs")]
        public async Task<ICollection<FeedbackStddQ>> GetStandardFeedbackQs()
        {
            return await _repo.GetFeedbackStddQs();
        }

    }
}