﻿namespace Applications.Dtos.Identity
{
    public class RegisterClientUserDto
    {
        public int ClientId { get; set; }
        public string DisplayName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string ConfirmPassword { get; set; } = string.Empty;
    }
}
