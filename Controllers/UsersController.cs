using LoginApp.DB.Enums;
using LoginApp.Models.DTOs;
using LoginApp.Services.UseCases;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace LoginApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class UsersController(
        UserService userService
        ) : ControllerBase
    {
        private readonly UserService _userService = userService;

        [HttpGet]
        [Authorize(Policy = "AdminActions")]
        public async Task<IActionResult> GetAll()
        {
            try
            {
                var response = await _userService.GetAll();
                return Ok(response);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("user-roles")]
        [AllowAnonymous]
        public IActionResult GetAllUserRoles()
        {
            try
            {
                return Ok(_userService.GetUserRoleEnums());
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet]
        [Route("by-email/{email}")]
        [Authorize(Policy = "AdminActions")]
        public async Task<IActionResult> GetByEmail([FromRoute] string email)
        {
            try
            {
                var response = await _userService.GetByEmail(email);
                return Ok(response);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut]
        [Authorize(Policy = "AdminActions")]
        public async Task<IActionResult> Update([FromBody] UserUpdateDto updateDto)
        {
            try
            {
                var response = await _userService.Update(updateDto);
                return Ok(response);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpDelete("{id}")]
        [Authorize(Policy = "AdminActions")]
        public async Task<IActionResult> Delete(Guid id)
        {
            try
            {
                var response = await _userService.Delete(id);
                return Ok(response);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("{id}")]
        [Authorize(Policy = "AdminActions")]
        public async Task<IActionResult> GetById(Guid id)
        {
            try
            {
                var response = await _userService.GetById(id);
                return Ok(response);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("get-me")]
        public async Task<IActionResult> GetMe()
        {
            try
            {
                var response = await _userService.GetCurrentUser();
                return Ok(response);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
