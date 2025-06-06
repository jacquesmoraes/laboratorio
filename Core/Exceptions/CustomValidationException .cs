namespace Core.Exceptions
{
    public class CustomValidationException  : Exception
    {
        
        public IEnumerable<string> Errors { get; }
        
        public CustomValidationException  ( IEnumerable<string> errors )
            : base ( errors.FirstOrDefault() ?? "Validation error" )
        {
            Errors = errors; 
        }
        public CustomValidationException  ( string error ) 
            : base ( error)
        {
            Errors = [error];
        }

    }
}
