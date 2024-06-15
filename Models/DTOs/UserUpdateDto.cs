using LoginApp.DB.Enums;
using System.ComponentModel.DataAnnotations;

namespace LoginApp.Models.DTOs;

public class UserUpdateDto
{
    [Required]
    public Guid Id { get; set; }
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public string? Email { get; set; }
    public string? Password { get; set; }
    public string? Phone { get; set; }
    public UserRole? Role { get; set; }
}
