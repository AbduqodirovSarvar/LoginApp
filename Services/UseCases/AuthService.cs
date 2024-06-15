using AutoMapper;
using LoginApp.DB;
using LoginApp.DB.Entities;
using LoginApp.DB.Enums;
using LoginApp.Models.DTOs;
using LoginApp.Models.ViewModels;
using LoginApp.Services.Security;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace LoginApp.Services.UseCases;

public class AuthService(
    AppDbContext appDbContext,
    HashService hashService,
    TokenService tokenService,
    CurrentUserService currentUserService,
    IMapper mapper
    )
{
    private readonly AppDbContext _context = appDbContext;
    private readonly HashService _hashService = hashService;
    private readonly TokenService _tokenService = tokenService;
    private readonly CurrentUserService _currentUserService = currentUserService;
    private readonly IMapper _mapper = mapper;

    public async Task<LoginViewModel> Login(LoginDto dto)
    {
        var user = await _context.Users.FirstOrDefaultAsync(x => x.Email == dto.Email)
                                ?? throw new Exception("Login or Password incorrect");

        if (!_hashService.VerifyHash(dto.Password, user.PasswordHash))
        {
            throw new Exception("Login or Password incorrect");
        }

        var claims = new List<Claim>
                {
                    new(ClaimTypes.NameIdentifier, user.Id.ToString()),
                    new(ClaimTypes.Role, user.Role.ToString())
                };

        return new LoginViewModel()
        {
            User = _mapper.Map<UserViewModel>(user),
            AccessToken = _tokenService.GetAccessToken([.. claims])
        };
    }

    public async Task<UserViewModel> Register(UserCreateDto dto)
    {
        var currentUser = await _context.Users.FirstOrDefaultAsync(x => x.Id == _currentUserService.UserId)
                                        ?? throw new Exception("Current User not found");
        if (currentUser.Role != UserRole.Admin)
        {
            throw new Exception("Access Denied");
        }

        var user = await _context.Users.FirstOrDefaultAsync(x => x.Email == dto.Email || x.Phone == dto.Phone);
        if (user != null)
        {
            throw new Exception("User already exists");
        }

        user = new User()
        {
            Email = dto.Email,
            FirstName = dto.FirstName,
            LastName = dto.LastName,
            Phone = dto.Phone,
            PasswordHash = _hashService.GetHash(dto.Password),
            Role = dto.Role
        };

        await _context.Users.AddAsync(user);
        await _context.SaveChangesAsync();
        return _mapper.Map<UserViewModel>(user);
    }
}
