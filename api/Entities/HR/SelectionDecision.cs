using System.ComponentModel.DataAnnotations.Schema;
using api.Entities.Deployments;
using api.Entities.Finance;

namespace api.Entities.HR
{
    public class SelectionDecision: BaseEntity
    {
        [ForeignKey("CVRefId")]
        public int CVRefId { get; set; }
        public int OrderItemId { get; set; }
        public int ProfessionId { get; set; }
        public string ProfessionName { get; set; }
        //public DateTime SelectionDate {get; set;}
        public string SelectionStatus { get; set; }
        public string RejectionReason { get; set; }
        public DateTime SelectedOn { get; set; }
        public int Charges {get; set;}
        public string Remarks {get; set;}
        
        #nullable enable
        //public Employment? Employment {get; set;}
        //public Dep? Dep { get; set; }
        public CVRef? CVRef {get; set;}
        
    }
}