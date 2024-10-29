namespace api.Entities.Admin
{
    public class IntervwAttendance: BaseEntity
    {
        public IntervwAttendance()
        {
        }


        public int IntervwItemCandidateId { get; set; }
        public int AttendanceStatusId { get; set; }
        public string Status { get; set; }
        public DateTime StatusDate { get; set; }
    }
}