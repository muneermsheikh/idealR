namespace api.DTOs.Admin
{
    public class OrderItemForVisaAssignmentDto
    {
        public int OrderItemId { get; set; }
        public string CustomerInBrief { get; set; }
        public string Category { get; set; }
        public int Quantity { get; set; }
        public int Assigned { get; set; }
        public int Unassigned { get; set; }
    }
}