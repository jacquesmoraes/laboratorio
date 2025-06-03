namespace API.Models
{
    public class ApiValidationErrorResponse : ApiResponse
    {
        public ApiValidationErrorResponse (  ) : base ( 400 )
        {
            Errors = [];
        }

        public IEnumerable<string> Errors { get; set; }
    }
}
