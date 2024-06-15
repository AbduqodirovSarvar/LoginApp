using LoginApp.Models.DTOs;
using LoginApp.Services.UseCases;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace LoginApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController(
        AuthService authService
        ) : ControllerBase
    {
        private readonly AuthService _authService = authService;

        [HttpPost("sign-in")]
        public async Task<IActionResult> SignIn([FromBody] LoginDto loginDto)
        {
            try
            {
                var response = await _authService.Login(loginDto);
                return Ok(response);
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }

        [HttpPost("sign-up")]
        [Authorize(Policy = "AdminActions")]
        public async Task<IActionResult> SignUp([FromBody] UserCreateDto createDto)
        {
            try
            {
                var response = await _authService.Register(createDto);
                return Ok(response);
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }

    }
}
