namespace api.Params
{
    public class UserParams
    {
        private const int MaxPageSize=20;
        private int _pageSize=10;
        public int pageNumber { get; set; }=1;
        public int PageSize 
        { 
            get => _pageSize;
            set => _pageSize = (value > MaxPageSize ? MaxPageSize : value); 
        }
        public string CurrentUsername { get; set; }
        public string Gender { get; set; }
        public int MinAge { get; set; }
        public int MaxAge { get; set; }
        public string   OrderBy { get; set; } = "LastActive";
    }

}