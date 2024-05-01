using System.ComponentModel.DataAnnotations;

namespace api.Entities.Admin.Order
{
    public class Remuneration: BaseEntity
    {
        public int OrderItemId {get; set;}
        public int WorkHours { get; set; }
        [Required]
        [MinLength(3), MaxLength(3)]
        public string SalaryCurrency { get; set; }
        public int SalaryMin { get; set; }
        public int SalaryMax { get; set; }
        
        [Range(1,36)]
        public int ContractPeriodInMonths { get; set; }=24;
        public bool HousingProvidedFree { get; set; }
        public int HousingAllowance { get; set; }
        public bool HousingNotProvided { get; set; }
        public bool FoodProvidedFree { get; set; }
        public int FoodAllowance { get; set; }
        public bool FoodNotProvided { get; set; }
        public bool TransportProvidedFree { get; set; }
        public int TransportAllowance { get; set; }
        public bool TransportNotProvided { get; set; }
        public int OtherAllowance { get; set; }
        public int LeavePerYearInDays { get; set; }
        public int LeaveAirfareEntitlementAfterMonths { get; set; }
    }
}