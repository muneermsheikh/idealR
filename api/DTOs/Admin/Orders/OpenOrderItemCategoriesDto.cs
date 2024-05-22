namespace api.DTOs.Admin.Orders
{
    public class OpenOrderItemCategoriesDto
    {
        public bool Checked { get; set; }
        public int CustomerId { get; set; }
        public int OrderId { get; set; }
        public int OrderNo { get; set; }
        public int OrderItemId { get; set; }
        public int ProfessionId { get; set; }
        public string CustomerName { get; set; }
        public DateOnly OrderDate { get; set; }
        public string CategoryRefAndName { get; set; }
        public int Quantity { get; set; }
    }
}