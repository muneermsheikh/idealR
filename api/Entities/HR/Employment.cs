using System.ComponentModel.DataAnnotations;
using DocumentFormat.OpenXml.Wordprocessing;

namespace api.Entities.HR
{
    public class Employment: BaseEntity
    {
        public int CvRefId { get; set; }
        public int SelectionDecisionId {get; set;}
        public DateTime SelectedOn { get; set; }
        public int ChargesFixed {get; set;}
        public int Charges {get; set;}
        public string SalaryCurrency { get; set; }
        public int Salary { get; set; }
        [Range(1,36)]
        public int ContractPeriodInMonths { get; set; }=24;
        public int WeeklyHours {get; set;}=48;
        public bool HousingProvidedFree { get; set; }
        public bool HousingNotProvided { get; set; }
        public int HousingAllowance { get; set; }
        public bool FoodProvidedFree { get; set; }
        public bool FoodNotProvided { get; set; }
        public int FoodAllowance { get; set; }
        public bool TransportProvidedFree { get; set; }
        public int TransportAllowance { get; set; }
        public bool TransportNotProvided { get; set; }
        public int OtherAllowance { get; set; }
        public int LeavePerYearInDays { get; set; }
        public int LeaveAirfareEntitlementAfterMonths {get; set;}
        //public string ConclusionStatus { get; set; }
        [MaxLength(50)]
        public string OfferAccepted { get; set; }
        public DateTime OfferAcceptedOn { get; set; }
        public string OfferConclusionRegisteredByUsername { get; set; }
        [MaxLength(100)]
        public string OfferAttachmentFileName { get; set; }
        [MaxLength(250)]
        public string OfferAttachmentFullPath { get; set; }
       
    }
}