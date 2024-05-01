using System.ComponentModel.DataAnnotations;

namespace api.Entities.HR
{
    public class UserExp: BaseEntity
    {
        public int CandidateId { get; set; }
        public int SrNo { get; set; }
        public string Employer { get; set; }
        public string Position { get; set; }
        public bool? CurrentJob {get; set;}
        public string SalaryCurrency { get; set; }
        public int? MonthlySalaryDrawn { get; set; }
        [Required]
        public DateOnly? WorkedFrom { get; set; }
        public DateOnly? WorkedUpto {get; set;}
    }
}