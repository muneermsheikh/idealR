namespace api.DTOs.Admin
{
    public class JDDto
    {
        public int Id { get; set; }
        public string CompanyName { get; set; }
        public string CategoryName { get; set; }
        public int OrderNo { get; set; }
        public DateTime OrderDate { get; set; }
        public int OrderItemId { get; set; }    //foreign key
        public string JobDescInBrief { get; set; }
        public string QualificationDesired { get; set; }
        public int ExpDesiredMin { get; set; }
        public int ExpDesiredMax { get; set; }
        public int MinAge { get; set; }
        public int MaxAge { get; set; }
    }
}