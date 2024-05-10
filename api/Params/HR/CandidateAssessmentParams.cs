namespace api.Params.HR
{
    public class CandidateAssessmentParams: PaginationParams
    {

        public int Id { get; set; }
        public int CandidateId { get; set; }
        public int OrderItemId { get; set; }
        public DateOnly AssessedOn { get; set; }
        public string AssessedByEmployeeName { get; set; }
        public string AssessResult { get; set; }
        public bool RequireInternalReview { get; set; }
        public int  CVRefId { get; set; }
    }
}