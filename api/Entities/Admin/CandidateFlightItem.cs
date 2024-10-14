namespace api.Entities.Admin
{
    public class CandidateFlightItem: BaseEntity
    {
        public int CandidateFlightGrpId { get; set; }
        public int DepId { get; set; }
        public int CvRefId { get; set; }
        public int DepItemId { get; set; }
        public int ApplicationNo { get; set; }
        public string CategoryName { get; set; }
        public string CandidateName { get; set; }
        public string CustomerName { get; set; }
        public string CustomerCity { get; set; }
        
    }
}