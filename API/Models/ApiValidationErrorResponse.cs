namespace API.Models
{
    public class ApiValidationErrorResponse : ApiResponse
    {
        public ApiValidationErrorResponse (string? message = null  ) : base ( 400, message )
        {
            Errors = [];
        }

        public IEnumerable<string> Errors { get; set; }
    }
}
