namespace Core.Exceptions
{
    public class UnauthorizedException( string message = "User is not authorized" ) : Exception ( message )
    {
    }
}
