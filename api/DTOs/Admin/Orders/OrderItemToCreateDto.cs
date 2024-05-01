using System.ComponentModel.DataAnnotations;
using api.Entities.Admin.Order;

namespace api.DTOs.Admin.Orders
{
    public class OrderItemToCreateDto
    {
        [Required]
        public int OrderId {get; set;}
        [Required]
        public int SrNo {get; set;}
        [Required]
        public int ProfessionId { get; set; }
        [Required]
        public int Quantity { get; set; }
        public int MinCVs { get; set; }
        public int MaxCVs { get; set; }
        public string SourceFrom { get; set; }
        public bool Ecnr { get; set; }
        public DateOnly CompleteBefore { get; set; }
        public JobDescription JobDescription {get; set;}
        public Remuneration remuneration { get; set; }
        
    }
}