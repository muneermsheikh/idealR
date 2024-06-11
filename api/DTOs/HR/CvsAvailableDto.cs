using api.Entities.HR;

namespace api.DTOs.HR
{
    public class cvsAvailableDto
    {
        public int CandAssessmentId { get; set; }
        public int CandidateId { get; set; }
        public bool  Checked { get; set; }
        public string Gender { get; set; }
        public int ApplicationNo { get; set; }
        public string FullName {get; set;}
        public string City {get; set;}
        public string OrderCategoryRef { get; set; }
        public int OrderItemId { get; set; }
        public string GradeAssessed { get; set; }
        public DateTime AssessedOn { get; set; }
        public ICollection<UserProfession> userProfessions { get; set; }
    }
}