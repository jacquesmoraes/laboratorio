﻿namespace API.Models
{
    public class ApiException ( int statusCode, string? message = null, string? details = null ) : ApiResponse( statusCode, message )
    {
        public string Details { get; set; } = details ?? string.Empty;
    }
    
}
