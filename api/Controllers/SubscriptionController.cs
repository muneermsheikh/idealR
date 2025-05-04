using System.Security.Claims;
using api.Entities.Identity;
using api.Entities.Subscriptions;
using api.Errors;
using api.Interfaces.Subscriptions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace api.Controllers
{
    [Authorize(Policy = "Subscription")]
    public class SubscriptionController: BaseApiController
    {
        private readonly ISubscriptionRepository _repo;
        private readonly UserManager<AppUser> _userManager;

        public SubscriptionController(ISubscriptionRepository repo,UserManager<AppUser> userManager)
        {
            _userManager = userManager;
            _repo = repo;
        }        
        
        [HttpGet("status/{appUserId}")]
        public async Task<IActionResult> GetSubscriptionStatus(int appUserId)
        {
            var user = await _userManager.FindByIdAsync(appUserId.ToString());
            var userId = user==null ? 0 : user.Id;

            if(userId==0) return BadRequest(new ApiException(400, "Not Found"));

            var subscription = await _repo.GetSubscriptionAsync(userId);

            if (subscription == null)
                return BadRequest(new ApiException(400, "No subscription found."));

            return Ok(new
            {
                subscription.SubscriptionType,
                subscription.StartDate,
                subscription.EndDate,
                subscription.Status,
                IsActive = await _repo.IsTrialActive(userId)
            });
        }

        [HttpPost("extend-trial({appUserId}/{additionalDays})")]
        public async Task<IActionResult> ExtendTrialAsync(int appUserId, int additionalDays)
        {
            var user = await _userManager.FindByIdAsync(appUserId.ToString());
            var userId = user==null ? 0 : user.Id;

            try
            {
                await _repo.ExtendTrialAsync(userId, additionalDays);
                return Ok("Trial extended successfully.");
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new ApiException(400, ex.Message));
            }
        }
    
        [HttpPost("convert-to-paid")]
        public async Task<IActionResult> ConvertToPaid([FromBody] ConvertToPaidRequest request)
        {
            var user = await _userManager.FindByIdAsync(request.AppUserId.ToString());
            var userId = user==null ? 0 : user.Id;

            try
            {
                await _repo.ConvertToPaidAsync(request);
                return Ok("Subscription converted to paid.");
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }

    public class ExtendTrialRequest
    {
        public int AdditionalDays { get; set; }
    }

    public class ConvertToPaidRequest
    {
        public string TransactionId { get; set; }
        public int AppUserId {get; set;}
        public long Amount {get; set;}
    }

}