namespace Core.Exceptions
{
    public class CustomValidationException  : Exception
    {
        
        public IEnumerable<string> Errors { get; }
        
        public CustomValidationException  ( IEnumerable<string> errors )
            : base ( "one or more validations errors occured" )
        {
            Errors = errors; 
        }
        public CustomValidationException  ( string error ) 
            : base ( "one or more validations errors occured" )
        {
            Errors = [error];
        }

    }
}
