using System.ComponentModel.DataAnnotations.Schema;

namespace api.Entities.HR
{
    public class SelectionDecision: BaseEntity
    {
        [ForeignKey("CVRefId")]
        public int CVRefId { get; set; }
        public DateTime SelectionDate {get; set;}
        public string SelectionStatus { get; set; }
        public DateTime SelectedOn { get; set; }
        public int Charges {get; set;}
        public string Remarks {get; set;}
        #nullable enable
        public Employment? Employment {get; set;}
        public CVRef? CVRef {get; set;}
    }
}