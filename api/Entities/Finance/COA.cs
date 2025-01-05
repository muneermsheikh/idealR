using System.ComponentModel.DataAnnotations;

namespace api.Entities.Finance
{
    public class COA: BaseEntity
    {
        [MaxLength(50)]
		public string Section { get; set; }
		[MaxLength(1), Required]    
		public string Divn { get; set; }
		[MaxLength(1), Required]
		public string AccountType { get; set; }
		[Required, MaxLength(100)]
		public string AccountName { get; set; }
		public string AccountClass {get; set;}
		public long OpBalance { get; set; }
    }
}