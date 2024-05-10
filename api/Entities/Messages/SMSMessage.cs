namespace api.Entities.Messages
{
    public class SMSMessage: BaseEntity
    {
        public int SequenceNo { get; set; }
        public int UserId { get; set; }
        public DateTime ComposedOn {get; set;}
        public DateTime SMSDateTime { get; set; }
        public string PhoneNo { get; set; }
        public string SMSText { get; set; }
        public string DeliveryResult { get; set; }
    }
}