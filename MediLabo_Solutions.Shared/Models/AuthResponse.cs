using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace MediLabo_Solutions.Shared.Models
{
    public class AuthResponse
    {
        public string Token { get; set; } = string.Empty;
        public DateTime Expiration { get; set; }
        public string Username { get; set; } = string.Empty;
    }
}
