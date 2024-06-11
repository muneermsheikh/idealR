namespace api.DTOs.Admin.Orders
{
    public class OrderItemIdAndHRExecEmpNoDto
    {
        public int OrderItemId { get; set; }
        public int HrExecEmpId { get; set; }
        public string HrExecUsername { get; set; }
        public string HrExecAppUserId { get; set; }
    }
}