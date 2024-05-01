using System.ComponentModel.DataAnnotations;

namespace api.Entities.Admin.Order
{
    public class JobDescription: BaseEntity
    {
        public int OrderItemId { get; set; }    //foreign key
        [Required, MaxLength(250)]
        public string JobDescInBrief { get; set; }
        public string QualificationDesired { get; set; }
        [Range(0,40)]
        public int ExpDesiredMin { get; set; }
        [Range(0,40)]
        public int ExpDesiredMax { get; set; }
        [Range(18,80)]
        public int MinAge { get; set; }
        [Range(18,70)]
        public int MaxAge { get; set; }
        
    }
}