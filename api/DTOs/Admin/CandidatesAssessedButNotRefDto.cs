using api.Entities.HR;

namespace api.DTOs.Admin
{
    //Required inputs for composing email messages
    public class CandidatesAssessedButNotRefDto
    {
        public CandidateAssessment CandidateAssessment { get; set; }
        public int OrderItemId { get; set; }
        public int OrderItemSrNo { get; set; }
        public int ProfessionId { get; set; }
        public int OrderId { get; set; }
        public int OrderNo { get; set; }
        public string CustomerName { get; set; }
        public int CustomerId { get; set; }
        public string ProfessionName { get; set; }
        public int CandidateId { get; set; }
        public string CandidateName { get; set; }
        public int ApplicationNo { get; set; }
        public string PassportNo { get; set; }
        public bool Ecnr { get; set; }
        public int DocControllerAdminTaskId { get; set; }
        public int ChargesAgreed { get; set; }
        public int HRExecId { get; set; }
        public string TaskDescription { get; set; }
        public string Candidatedescription { get; set; }
    }
}