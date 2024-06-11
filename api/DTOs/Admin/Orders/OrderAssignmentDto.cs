namespace api.DTOs.Admin.Orders
{
    public class OrderAssignmentDto
    {
        public int OrderId { get; set; }
        public int OrderNo { get; set; }
        public DateTime OrderDate { get; set; }
        public string CityOfWorking { get; set; }
        public int ProjectManagerId { get; set; }
        public string ProjectManagerPosition { get; set; }
        public int OrderItemId { get; set; }
        public string HrExecUsername { get; set; }
        public string CategoryRef { get; set; }
        public int ProfessionId {get; set;}
        public string ProfessionName { get; set; }
        public int CustomerId { get; set; }
        public string CustomerName { get; set; }
        public int Quantity { get; set; }
        public DateTime CompleteBy { get; set; }
       
    }
}