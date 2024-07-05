namespace api.Entities.Admin.Client
{
    public class AcknowledgeToClient: BaseEntity
    {
        public int OrderId { get; set; }
        public DateTime DateAcknowledged { get; set; }
        public string RecipientUsername { get; set; }
        public string RecipientEmailId { get; set; }
        public string SenderUsername { get; set; }
        public string SenderEmailId { get; set; }
        public int CustomerId { get; set; }
        public string CustomerName { get; set; }
        public string MessageType { get; set; }
       
    }
}