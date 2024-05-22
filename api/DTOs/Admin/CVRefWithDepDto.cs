using api.DTOs.Process;

namespace api.DTOs.Admin
{
    public class CVRefWithDepDto
    {
        public int CVRefId { get; set; }
        public bool Checked {get; set;}
        public int CustomerId { get; set; }
        public int CandidateId { get; set; }
        public int ApplicationNo { get; set; }
        public string CandidateName { get; set; }

        public string CustomerName { get; set; }
        public int OrderId { get; set; }
        public int OrderNo { get; set; }
        public DateOnly OrderDate {get; set;}
        public int OrderItemId { get; set; }
        public int SrNo {get; set;}
        public string ProfessionName { get; set; }
        public string CategoryRef { get; set; }
       
        //public string PPNo {get; set;}
        public string AgentName {get; set;}
        public DateOnly ReferredOn { get; set; }
        public string ReferralDecision { get; set; }
        public DateTime SelectedOn { get; set; }
        public ICollection<DeployDto> Deployments { get; set; }
    }
}