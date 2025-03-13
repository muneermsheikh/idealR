namespace api.DTOs.HR
{
    public class CandidateDto
    {
        public int CandidateId { get; set; }
        public string CandidateName { get; set; }
        public int ApplicationNo { get; set; }
        public string CoreProfession { get; set; }
        public ICollection<CandidateHistoryDto> CandidateHistoriesDto { get; set; }
        public long AmountDue { get; set; }
    }

    public class CandidateHistoryDto
    {
        public int OrderItemId { get; set; }
        public string CategoryRef { get; set; }
        public string CustomerName { get; set; }
        public DateTime ReferredOn { get; set; }
        public string SelectionStatus { get; set; }
        public DateTime SelectionStatusDate { get; set; }
        public string DeploymentStatus { get; set; }
        public DateTime DeploymentStatusDate { get; set; }
    }
}