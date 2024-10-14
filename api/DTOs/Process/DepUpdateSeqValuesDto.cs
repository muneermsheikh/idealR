namespace api.DTOs.Process
{
    public class DepUpdateSeqValuesDto
    {
        public int DepId { get; set; }
        public int DeploySequence { get; set; }
        public int NextSequence { get; set; }
        public DateTime NextSequenceDate { get; set; }
    }
}