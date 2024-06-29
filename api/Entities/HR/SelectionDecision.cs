using System.ComponentModel.DataAnnotations.Schema;
using api.Entities.Deployments;
using api.Entities.Finance;

namespace api.Entities.HR
{
    public class SelectionDecision: BaseEntity
    {
        [ForeignKey("CVRefId")]
        public int CvRefId { get; set; }
        public int OrderItemId { get; set; }
        public int ApplicationNo { get; set; }
        public string CandidateName { get; set; }
        public int CandidateId { get; set; }
        public int ProfessionId { get; set; }
        public string Gender { get; set; }
        public string SelectedAs { get; set; }
        //public DateTime SelectionDate {get; set;}
        public int CustomerId { get; set; }
        public string CustomerName { get; set; }
        public string CityOfWorking { get; set; }
        public string SelectionStatus { get; set; }
        //public string RejectionReason { get; set; }
        public DateTime SelectedOn { get; set; }
        public int Charges {get; set;}
        public string Remarks {get; set;}
        
        #nullable enable
        //public Employment? Employment {get; set;}
        //public Dep? Dep { get; set; }
        public CVRef? CVRef {get; set;}
        
    }
}