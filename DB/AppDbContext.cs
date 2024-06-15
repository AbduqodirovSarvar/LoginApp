using LoginApp.DB.Entities;
using LoginApp.DB.Enums;
using LoginApp.Services.Security;
using Microsoft.EntityFrameworkCore;

namespace LoginApp.DB;

public class AppDbContext(
    DbContextOptions<AppDbContext> options,
    HashService hashService
    ) : DbContext(options)
{
    private readonly HashService _hashService = hashService;
    public DbSet<User> Users { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.Entity<User>(user =>
        {
            user.HasKey(x => x.Id);
            user.HasIndex(x => x.Email).IsUnique();
            user.HasIndex(x => x.Phone).IsUnique();
            user.HasData(
                new User()
                {
                    Email = "admin@gmail.com",
                    FirstName = "Admin",
                    LastName = "Admin",
                    PasswordHash = _hashService.GetHash("Admin12345"),
                    Role = UserRole.Admin,
                    Phone = "+998 97 654 32 10"
                });
        });
    }
}
