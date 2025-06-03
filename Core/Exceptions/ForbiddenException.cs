namespace Core.Exceptions
{
    public class ForbiddenException(string message = "Access to this resource is forbidden" ) : Exception ( message )
    {
    }
}
