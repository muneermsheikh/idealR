namespace api.Params
{
    public class UserParams: PaginationParams
    {
        public string CurrentUsername { get; set; }
        public string Gender { get; set; }
        public int MinAge { get; set; }
        public int MaxAge { get; set; }
        public string   OrderBy { get; set; } = "LastActive";
    }

}