namespace api.DTOs.Process
{
    public class DepItemToAddDto
    {
        public int DepId { get; set; }
        public DateOnly TransactionDate { get; set; }   
        public int Sequence { get; set; }
        
    }
}