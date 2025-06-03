namespace API.Models
{
    public class ApiResponse
    {

        public ApiResponse(int statusCode, string? message = null)
        {
            StatusCode = statusCode;
            Message = message ?? GetDefaultMessageForStatusCode ( statusCode );
        }

        public int StatusCode { get; set; }
        public string Message { get; set; } = string.Empty;


        private string GetDefaultMessageForStatusCode ( int statusCode )
        {
            return statusCode switch
            {
                400 => "Bad Request",
                401 => "Unauthorized",
                403 => "forbidden access",
                404 => "Not Found",
                422 => "Unprocessable Entity",
                500 => "Internal Server Error",
                _ => "An error occurred"
            };
        }
    }
}
