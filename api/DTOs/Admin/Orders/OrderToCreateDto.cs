using System.ComponentModel.DataAnnotations;

namespace api.DTOs.Admin.Orders
{
    public class OrderToCreateDto
    {
        [Required]
        public DateTime OrderDate { get; set; }
        public string OrderRef { get; set; }
        public DateTime OrderRefDate {get; set;}
        [Required]
        public int CustomerId { get; set; }
        public string Country { get; set; }
        public string CityOfWorking { get; set; }
        public DateTime CompleteBy { get; set; }
        public string Remarks { get; set; }
        public ICollection<OrderItemDto> OrderItems { get; set; }
    }
}