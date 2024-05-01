using System.ComponentModel.DataAnnotations;
using api.Entities.Admin.Order;

namespace api.DTOs
{
    public class OrderItemDto
    {
        public int Id { get; set; }
        public int OrderId { get; set; }
        [Required]
        public int SrNo { get; set; }
        [Required]
        public int ProfessionId { get; set; }
        [Required]
        public string SourceFrom { get; set; }
        [Required]
        public int Quantity { get; set; }
        [Required]
        public int MinCVs { get; set; }
        public int MaxCVs { get; set; }
        public bool Ecnr { get; set; }=false;
        public DateOnly CompleteBefore { get; set; }
        public string Status { get; set; }
        public JobDescription JobDescription { get; set; }
        public Remuneration Remuneration { get; set; }
        //public ContractReviewItem ContractReviewItem { get; set; }
    }
}