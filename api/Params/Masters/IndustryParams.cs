namespace api.Params.Masters
{
    public class IndustryParams: PaginationParams
    {
        public int Id { get; set; }
        public string IndustryName { get; set; }
        public string IndustryGroup { get; set; }
        public string IndustryClass { get; set; }
    }
}