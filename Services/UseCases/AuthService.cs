using AutoMapper;
using LoginApp.DB;
using LoginApp.DB.Entities;
using LoginApp.DB.Enums;
using LoginApp.Models.DTOs;
using LoginApp.Models.ViewModels;
using LoginApp.Services.Security;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace LoginApp.Services.UseCases
{
    public class AuthService
    {
        private readonly AppDbContext _context;
        private readonly HashService _hashService;
        private readonly TokenService _tokenService;
        private readonly CurrentUserService _currentUserService;
        private readonly IMapper _mapper;

        public AuthService(
            AppDbContext appDbContext,
            HashService hashService,
            TokenService tokenService,
            CurrentUserService currentUserService,
            IMapper mapper)
        {
            _context = appDbContext;
            _hashService = hashService;
            _tokenService = tokenService;
            _currentUserService = currentUserService;
            _mapper = mapper;
        }

        public async Task<LoginViewModel> Login(LoginDto dto)
        {
            var user = await _context.Users.FirstOrDefaultAsync(x => x.Email == dto.Email)
                         ?? throw new UnauthorizedAccessException("Invalid login or password");

            if (!_hashService.VerifyHash(dto.Password, user.PasswordHash))
            {
                throw new UnauthorizedAccessException("Invalid login or password");
            }

            var claims = new List<Claim>
            {
                new(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new(ClaimTypes.Role, user.Role.ToString())
            };

            return new LoginViewModel
            {
                User = _mapper.Map<UserViewModel>(user),
                AccessToken = _tokenService.GetAccessToken([..claims])
            };
        }

        public async Task<UserViewModel> Register(UserCreateDto dto)
        {
            var currentUser = await _context.Users
                .FirstOrDefaultAsync(x => x.Id == _currentUserService.UserId)
                ?? throw new UnauthorizedAccessException("Current user not found");

            if (currentUser.Role != UserRole.Admin)
            {
                throw new UnauthorizedAccessException("Access denied");
            }

            var userExists = await _context.Users.AnyAsync(x => x.Email == dto.Email || x.Phone == dto.Phone);
            if (userExists)
            {
                throw new InvalidOperationException("User already exists");
            }

            var user = new User
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
}
