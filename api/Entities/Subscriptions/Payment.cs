using System.ComponentModel.DataAnnotations;

namespace api.Entities.Subscriptions
{
    public class Payment: BaseEntity
    {
        public int SubscriptionId {get; set;}
        [MaxLength(15)]
        public string PaymentProvider {get; set;}   //stripe, paypal, manual
        public string TransactionId {get; set;}
        public long Amount {get; set;}
        public DateTime PaymentDate {get; set;}
        [MaxLength(15)]
        public string Status {get; set;}    //success, failed

    }
}