namespace api.DTOs.Admin.Orders
{
    public class OrderItemAssignmentDto
    {
        public int OrderId { get; set; }
        public int CustomerId { get; set; }
        public string CustomerName { get; set; }
        public string AboutEmployer { get; set; }
        public int OrderNo { get; set; }
        public DateOnly OrderDate { get; set; }
        
        public int OrderItemId { get; set; }
        public int SrNo { get; set; }
        public int ProfessionId { get; set; }
        public string ProfessionName { get; set; }
        public int Quantity { get; set; }
        public bool Ecnr { get; set; }=false;
        public DateOnly CompleteBefore { get; set; }
        public string Status { get; set; }
        public bool Checked {get; set;}
        public int AssignedToEmpId { get; set; }
        public int AssignedToAppUserId { get; set; }
        public string AssignedToUsername { get; set; }
    }
}