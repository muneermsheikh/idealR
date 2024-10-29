namespace api.DTOs.Admin
{
    public class MedicalObjective
    {
        public int DepId { get; set; }
        public DateTime DateSelected { get; set; }
        public DateTime RefForMedicals { get; set; }
        public DateTime MedicalResult { get; set; }
        public int ApplicationNo { get; set; }
        public string CandidateName {get; set;}
        public string OrderRef {get;set;}
        public string CustomerName {get; set;}
    }
}