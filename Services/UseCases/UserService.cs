using AutoMapper;
using LoginApp.DB;
using LoginApp.DB.Enums;
using LoginApp.Models.DTOs;
using LoginApp.Models.ViewModels;
using LoginApp.Services.Security;
using Microsoft.EntityFrameworkCore;

namespace LoginApp.Services.UseCases;

public class UserService(
    AppDbContext appDbContext,
    HashService hashService,
    IMapper mapper,
    CurrentUserService currentUserService
    )
{
    private readonly AppDbContext _context = appDbContext;
    private readonly HashService _hashService = hashService;
    private readonly IMapper _mapper = mapper;
    private readonly CurrentUserService _currentUserService = currentUserService;

    public async Task<UserViewModel> GetByEmail(string email)
    {
        var currentUser = await _context.Users.FirstOrDefaultAsync(x => x.Id == _currentUserService.UserId)
                                        ?? throw new Exception("Current User not found");
        if (currentUser.Role != UserRole.Admin)
        {
            throw new Exception("Access Denied");
        }

        return _mapper.Map<UserViewModel>(await _context.Users.FirstOrDefaultAsync(x => x.Email == email)
                             ?? throw new Exception("User not found"));
    }

    public async Task<List<UserViewModel>> GetAll()
    {
        var currentUser = await _context.Users.FirstOrDefaultAsync(x => x.Id == _currentUserService.UserId)
                                        ?? throw new Exception("Current User not found");
        if (currentUser.Role != UserRole.Admin)
        {
            throw new Exception("Access Denied");
        }

        return _mapper.Map<List<UserViewModel>>(await _context.Users.ToListAsync());
    }

    public async Task<UserViewModel> Update(UserUpdateDto dto)
    {
        var currentUser = await _context.Users.FirstOrDefaultAsync(x => x.Id == _currentUserService.UserId)
                                        ?? throw new Exception("Current User not found");
        if (currentUser.Role != UserRole.Admin)
        {
            throw new Exception("Access Denied");
        }

        var user = await _context.Users.FirstOrDefaultAsync(x => x.Id == dto.Id)
                                 ?? throw new Exception("User not found Exception");

        user.FirstName = dto.FirstName ?? user.FirstName;
        user.LastName = dto.LastName ?? user.LastName;
        user.Email = dto.Email ?? user.Email;
        user.Phone = dto.Phone ?? user.Phone;
        user.Role = dto.Role ?? user.Role;
        user.PasswordHash = string.IsNullOrEmpty(dto.Password) ? user.PasswordHash : _hashService.GetHash(dto.Password);

        await _context.SaveChangesAsync();

        return _mapper.Map<UserViewModel>(user);
    }

    public async Task<bool> Delete(Guid Id)
    {
        var currentUser = await _context.Users.FirstOrDefaultAsync(x => x.Id == _currentUserService.UserId)
                                        ?? throw new Exception("Current User not found");
        if (currentUser.Role != UserRole.Admin)
        {
            throw new Exception("Access Denied");
        }

        var user = await _context.Users.FirstOrDefaultAsync(x => x.Id == Id)
                                 ?? throw new Exception("User not found Exception");

        _context.Users.Remove(user);
        return await _context.SaveChangesAsync() > 0;
    }

    public List<EnumViewModel> GetUserRoleEnums()
    {

        return _mapper.Map<List<EnumViewModel>>((UserRole[])Enum.GetValues(typeof(UserRole)));
    }
}
