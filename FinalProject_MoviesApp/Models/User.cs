namespace FinalProject_MoviesApp.Models
{
    public class User
    {
        public int UserId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string City { get; set; } = string.Empty;
        public string Email { get; set; }
        public string Password { get; set; }
        public string Phone { get; set; } = string.Empty; 
        public string Role { get; set; } = string.Empty;
    }
}
