namespace api.Entities.Admin.Client
{
    public class AgencySpecialty: BaseEntity
    {
        public int CustomerId { get; set; }
		public int ProfessionId { get; set; }
		public int? IndustryId { get; set; }
		public string SpecialtyName { get; set; }
    }
}