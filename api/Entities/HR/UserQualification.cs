namespace api.Entities.HR
{
    public class UserQualification: BaseEntity
    {
        public int CandidateId { get; set; }
        public int QualificationId { get; set; }
        public bool IsMain { get; set; }
    }
}