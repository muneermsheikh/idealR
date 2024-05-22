using api.Entities.Admin.Order;

namespace api.DTOs.Admin.Orders
{
    public class RemunerationDto
    {
        public string CustomerName { get; set; }
        public string CategoryName { get; set; }
        public DateOnly OrderDate { get; set; }
        public int OrderNo { get; set; }
        public int ProfessionId { get; set; }   
        public Remuneration Remuneration { get; set; }     
    }
}