namespace api.DTOs.Admin
{
    public class HRObjective
    {
        public string AssignedToUsername { get; set; }
        public DateTime FromDate { get; set; }
        public DateTime UptoDate { get; set; }
        public int OrderNo { get; set; }
        public int OrderItemId { get; set; }
        public DateTime DateAssigned { get; set; }
        public int TotalCVsAssigned { get; set; }
        public DateTime DateConcluded { get; set; }
        public int TotalCVsConcluded { get; set; }
    }
}