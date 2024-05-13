using api.DTOs.Process;

namespace api.DTOs.Admin
{
    public class CVRefDto
    {
        public int Id { get; set; }
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
        public string PPNo {get; set;}
        public DateOnly ReferredOn { get; set; }
        public string RefStatus { get; set; }
        public string SelectionStatus { get; set; }
    }
}