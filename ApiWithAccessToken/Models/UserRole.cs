namespace ApiWithAccessToken.Models
{
    public class UserRole
    {
        public int Id { get; set; }
        public int AppUserId { get; set; }
        public string? Role { get; set; }

    }
}
