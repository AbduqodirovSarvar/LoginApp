using LoginApp.DB.Enums;

namespace LoginApp.Models.DTOs;

public class UserCreateDto
{
    public string FirstName { get; set; } = null!;
    public string LastName { get; set; } = null!;
    public string Email { get; set; } = null!;
    public string Password { get; set; } = null!;
    public string Phone { get; set; } = null!;
    public UserRole Role { get; set; } = UserRole.User;
}
