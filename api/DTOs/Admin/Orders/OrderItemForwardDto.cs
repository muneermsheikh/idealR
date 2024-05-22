namespace api.DTOs.Admin.Orders
{
    public class OrderItemForwardDto
    {
         public int Id { get; set; }
         public int OrderItemId { get; set; }
         public bool AssessmentQDesigned { get; set; }
         public int OrderId { get; set; }
         public int OrderNo { get; set; }
         public DateOnly OrderDate { get; set; }
         public string CustomerName { get; set; }
         public string AboutEmployer { get; set; }
         public int ProfessionId { get; set; }
         public string ProfessionName { get; set; }
         public string CategoryRef { get; set; }
         public int Quantity { get; set; }
         public string Status { get; set; }
         public string JobDescription { get; set; }
         public string Remuneration { get; set; }
    }
}