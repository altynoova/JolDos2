using Microsoft.AspNetCore.Identity;

namespace JolDos2.Data
{
    public class ApplicationUser:IdentityUser
    {
        public string? Firstname { get; set; }
        public string? Lastname { get; set; }
        public string? Gender { get; set; }
        public string? Role { get; set; }
        public float? Rating { get; set; }
        //public string? ProfilePicture { get; set; }
    }
}
