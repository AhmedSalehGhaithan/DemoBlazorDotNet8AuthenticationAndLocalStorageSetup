﻿namespace BlazorWebAssemblyApp.Authentication
{
    public class AuthenticationModel
    {
        public string? Token { get; set; }
        public string? Username { get; set; }
        public string? RefreshToken { get; set; }
    }
}
