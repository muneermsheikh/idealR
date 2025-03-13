namespace api.Entities.Admin
{
    public class VisaAssignment: BaseEntity
    {
        public int OrderItemId { get; set; }
        public int VisaItemId { get; set; }
        public int VisaQntyAssigned { get; set; }
        public DateTime DateAssigned { get; set; }
    }
}