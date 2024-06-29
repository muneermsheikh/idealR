namespace api.DTOs.Admin
{
    public class EmploymentDto
    {
        public int SelectionDecisionId { get; set; }    
        public int CvRefId { get; set; }
        public int ApplicationNo { get; set; }
        public string CandidateName {get; set;}
        public string ProfessionName {get; set;}
        public string CompanyName { get; set; }
        public string SelectionStatus { get; set; }
        public DateTime SelectedOn { get; set; }
        public int ChargesFixed {get; set;}
        public int Charges {get; set;}
        public string SalaryCurrency { get; set; }
        public int Salary { get; set; }
        public int ContractPeriodInMonths { get; set; }=24;
        public int WeeklyHours {get; set;}=48;
        public bool HousingProvidedFree { get; set; }
        public bool HousingNotProvided {get; set;}
        public int HousingAllowance { get; set; }
        public bool FoodProvidedFree { get; set; }
        public int FoodAllowance { get; set; }
        public bool FoodNotProvided { get; set; }
        public bool TransportProvidedFree { get; set; }
        public bool TransportNotProvided { get; set; }
        public int TransportAllowance { get; set; }
        public int OtherAllowance { get; set; }
        public int LeavePerYearInDays { get; set; }
        public int LeaveAirfareEntitlementAfterMonths {get; set;}
        public string OfferAccepted { get; set; }
        public DateTime OfferAcceptedOn { get; set; }
       
    }
}