namespace api.DTOs.Admin.Orders
{
    public class OrderItemIdAndHRExecEmpNoDto
    {
        public int OrderItemId { get; set; }
        public int HRExecEmpId { get; set; }
        public string HRExecUsername { get; set; }
        public string HRExecAppUserId { get; set; }
    }
}