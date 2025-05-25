namespace Core.Helpers
{
    public static class Guard
    {
        public static void AgainstNegativeOrZero(decimal value, string paramName = "value")
        {
            if (value <= 0)
                throw new ArgumentException($"{paramName} must be greater than zero.");
        }

        public static void AgainstNull<T>(T? value, string paramName = "value") where T : class
        {
            if (value == null)
                throw new ArgumentNullException(paramName);
        }

        
    }
}
