using LoginApp.DB.Enums;

namespace LoginApp.Models.ViewModels;

public class UserViewModel
{
    public Guid Id { get; set; }
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public string? Email { get; set; }
    public EnumViewModel? Role { get; set; }
    public string? Password { get; set; }
    public string? Phone { get; set; }
    public DateTime CreatedAt { get; set; }
}
