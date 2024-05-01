namespace api.Entities.HR
{
    public class UserProfession: BaseEntity
    {
        public int CandidateId { get; set; }
        public int ProfessionId { get; set; }
        public int IndustryId { get; set; }
        public bool IsMain { get; set; }
    }
}