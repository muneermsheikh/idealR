using System.ComponentModel.DataAnnotations.Schema;
using api.Entities.Identity;

namespace api.Entities.Subscriptions
{
    public class Subscription: BaseEntity
    {
        [ForeignKey("UserId")]
        public int AppUserId {get; set;}
        public string SubscriptionType {get; set;}      //Trial, Paid
        public DateTime StartDate {get; set;} = DateTime.UtcNow;
        public DateTime EndDate {get; set;} = DateTime.UtcNow;
        public string Status {get; set;}                //Active, Expired, Canceled
        public DateTime CreatedOn {get; set;} = DateTime.UtcNow;
        public DateTime LastUpdated {get; set;} = DateTime.UtcNow;
        public AppUser AppUser {get; set;}
    }
}