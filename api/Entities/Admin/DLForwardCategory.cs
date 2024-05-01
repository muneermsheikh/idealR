namespace api.Entities.Admin
{
    public class DLForwardedToAgent: BaseEntity
    {
  		public int CustomerOfficialId { get; set; }
		public int OrderItemId {get; set;}
		public DateOnly DateForwarded {get; set;}
    }
}