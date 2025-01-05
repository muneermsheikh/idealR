namespace api.DTOs.Admin
{
    public class OrderAssessmentItemHeaderDto
    {
        public int Id { get; set; }
        public string CustomerName { get; set; }
        public string ProfessionName { get; set; }
        public string CategoryRef { get; set; }
        public int OrderItemId { get; set; }
        public DateTime DateDesigned { get; set; }
    }
}