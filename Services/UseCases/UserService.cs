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
    IMapper mapper,
    CurrentUserService currentUserService
    )
{
    private readonly AppDbContext _context = appDbContext;
    private readonly IMapper _mapper = mapper;
    private readonly CurrentUserService _currentUserService = currentUserService;

    public async Task<UserViewModel> GetByEmail(string email)
    {
        return _mapper.Map<UserViewModel>(await _context.Users.AsNoTracking().FirstOrDefaultAsync(x => x.Email == email)
                             ?? throw new Exception("User not found"));
    }

    public async Task<PaginatedResult<UserViewModel>> GetAll(PaginationParams @params)
    {
        var query = _context.Users.AsNoTracking();
        var totalCount = await query.CountAsync();
        var items = await query.Skip((@params.PageNumber - 1) * @params.PageSize)
                              .Take(@params.PageSize)
                              .ToListAsync();

        return new PaginatedResult<UserViewModel>(
            _mapper.Map<List<UserViewModel>>(items),
            totalCount,
            @params.PageNumber,
            @params.PageSize);
    }

    public async Task<UserViewModel> Update(UserUpdateDto dto)
    {
        var user = await _context.Users.FirstOrDefaultAsync(x => x.Id == dto.Id)
                                 ?? throw new Exception("User not found Exception");

        user.FirstName = dto.FirstName ?? user.FirstName;
        user.LastName = dto.LastName ?? user.LastName;
        user.Email = dto.Email ?? user.Email;
        user.Phone = dto.Phone ?? user.Phone;
        user.Role = dto.Role ?? user.Role;
        user.PasswordHash = dto.Password ?? user.PasswordHash;

        await _context.SaveChangesAsync();

        return _mapper.Map<UserViewModel>(user);
    }

    public async Task<bool> Delete(Guid Id)
    {
        var user = await _context.Users.FirstOrDefaultAsync(x => x.Id == Id)
                                 ?? throw new Exception("User not found Exception");

        _context.Users.Remove(user);
        return await _context.SaveChangesAsync() > 0;
    }

    public async Task<UserViewModel> GetById(Guid id)
    {
        var user = await _context.Users.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id)
                     ?? throw new Exception("User not found");

        return _mapper.Map<UserViewModel>(user);
    }

    public async Task<UserViewModel> GetCurrentUser()
    {
        var user = await _context.Users.AsNoTracking().FirstOrDefaultAsync(x => x.Id == _currentUserService.UserId)
                     ?? throw new Exception("User not found");

        return _mapper.Map<UserViewModel>(user);
    }

    public List<EnumViewModel> GetUserRoleEnums()
    {

        return _mapper.Map<List<EnumViewModel>>((UserRole[])Enum.GetValues(typeof(UserRole)));
    }
}
