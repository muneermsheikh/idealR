namespace api.DTOs.Process
{
    public class DepItemInsertedIdDto
    {
        public int DepItemId { get; set; }
        public int DepId { get; set; }
        public int Sequence { get; set; }
        public int NextSequence { get; set; }
        public DateTime NextSequenceDate { get; set; }
    }
}