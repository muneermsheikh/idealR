using System.ComponentModel.DataAnnotations;

namespace api.DTOs.Admin.Orders
{
    public class OrderToCreateDto
    {
        [Required]
        public DateOnly OrderDate { get; set; }
        public string OrderRef { get; set; }
        public DateOnly OrderRefDate {get; set;}
        [Required]
        public int CustomerId { get; set; }
        public string Country { get; set; }
        public string CityOfWorking { get; set; }
        public DateOnly CompleteBy { get; set; }
        public string Remarks { get; set; }
        public ICollection<OrderItemDto> OrderItems { get; set; }
    }
}