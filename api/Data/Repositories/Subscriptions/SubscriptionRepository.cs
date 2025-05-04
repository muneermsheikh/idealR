using api.Controllers;
using api.Entities.Identity;
using api.Entities.Subscriptions;
using api.Interfaces.Subscriptions;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace api.Data.Repositories.Subscriptions
{
    public class SubscriptionRepository : ISubscriptionRepository
    {
        private readonly DataContext _context;
        private readonly UserManager<AppUser> _userManager;
        public SubscriptionRepository(DataContext context, UserManager<AppUser> userManager)
        {
            _userManager = userManager;
            _context = context;
        }

        public async Task ConvertToPaidAsync(ConvertToPaidRequest request)
        {
            var subscription = await _context.Subscriptions
            .FirstOrDefaultAsync(s => s.AppUserId == request.AppUserId);

            if (subscription == null)
            {
                throw new InvalidOperationException("No subscription found for user.");
            }

            subscription.SubscriptionType = "Paid";
            subscription.EndDate = DateTime.UtcNow.AddYears(1); // Example: 1-year paid subscription
            subscription.Status = "Active";
            subscription.LastUpdated = DateTime.UtcNow;

            // Log payment
            var payment = new Payment
            {
                SubscriptionId = subscription.Id,
                PaymentProvider = "Manual", // Example Stripe, Manual
                TransactionId = request.TransactionId,
                Amount = request.Amount,    //99.99m, // Example amount
                PaymentDate = DateTime.UtcNow,
                Status = "Success"
            };

            _context.Payments.Add(payment);

            await _context.SaveChangesAsync();
        }

        public async Task ExtendTrialAsync(int userId, int additionalDays)
        {
            var subscription = await _context.Subscriptions
            .FirstOrDefaultAsync(s => s.AppUserId == userId && s.SubscriptionType == "Trial");

            if (subscription == null)
            {
                // Create new trial subscription if none exists
                subscription = new Subscription
                {
                    AppUserId = userId,
                    SubscriptionType = "Trial",
                    StartDate = DateTime.UtcNow,
                    EndDate = DateTime.UtcNow.AddDays(additionalDays),
                    Status = "Active"
                };
                _context.Subscriptions.Add(subscription);
            }
            else if (subscription.Status == "Active" || subscription.Status == "Expired")
            {
                // Extend trial
                subscription.EndDate = (subscription.EndDate < DateTime.UtcNow 
                    ? DateTime.UtcNow : subscription.EndDate).AddDays(additionalDays);
                subscription.Status = "Active";
                subscription.LastUpdated = DateTime.UtcNow;
            }
            else
            {
                throw new InvalidOperationException("Cannot extend trial for this subscription.");
            }

            await _context.SaveChangesAsync();
        }

        public async Task<Subscription> GetSubscriptionAsync(int userId)
        {
            return await _context.Subscriptions
            .FirstOrDefaultAsync(s => s.AppUserId == userId);
        }

        public async Task<bool> IsTrialActive(int userId)
        {
            var subscription = await _context.Subscriptions
            .FirstOrDefaultAsync(s => s.AppUserId == userId && s.SubscriptionType == "Trial");
        
            if (subscription == null) return false;
            return subscription.Status == "Active" && subscription.EndDate >= DateTime.UtcNow;
        }

        /*public async Task<IActionResult> CreateCheckoutSession()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var options = new SessionCreateOptions
            {
                PaymentMethodTypes = new List<string> { "card" },
                LineItems = new List<SessionLineItemOptions>
                {
                    new SessionLineItemOptions
                    {
                        Price = "price_12345", // Stripe Price ID
                        Quantity = 1
                    }
                },
                Mode = "subscription",
                SuccessUrl = "https://your-app/success",
                CancelUrl = "https://your-app/cancel"
            };

            var service = new SessionService();
            var session = await service.CreateAsync(options);
            return Ok(new { sessionId = session.Id });
        }*/
    }
}