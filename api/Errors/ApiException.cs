namespace api.Errors
{
    public class ApiException
    {
        public ApiException(int statusCode, string message, string detail)
        {
            StatusCode = statusCode;
            Message = message;
            Details = detail;
        }

        public int StatusCode { get; set; }
        public string Message { get; set; }
        public string Details { get; set; }
    }
}