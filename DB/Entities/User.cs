using LoginApp.DB.Enums;

namespace LoginApp.DB.Entities
{
    public class User
    {
        public Guid Id { get; private set; } = Guid.NewGuid();
        public string FirstName { get; set; } = null!;
        public string LastName { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string PasswordHash { get; set; } = null!;
        public string Phone { get; set; } = null!;
        public UserRole Role { get; set; } = UserRole.User;
        public DateTime CreatedAt { get; private set; } = DateTime.UtcNow;
    }
}
