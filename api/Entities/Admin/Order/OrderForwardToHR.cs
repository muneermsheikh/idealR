namespace api.Entities.Admin.Order
{
    public class OrderForwardToHR: BaseEntity
    {
        public int OrderId { get; set; }
        public string RecipientUsername { get; set; }   
        public DateTime DateOnlyForwarded { get; set; }
      
    }
}