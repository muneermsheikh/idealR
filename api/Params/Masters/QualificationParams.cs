namespace api.Params.Masters
{
    public class QualificationParams: PaginationParams
    {
        public int Id { get; set; }
        public string QualificationName { get; set; }
    }
}