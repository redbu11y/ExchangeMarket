using ExchangeMarket.Api.DTOs.Auth;
using ExchangeMarket.Api.Models.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace ExchangeMarket.Api.Controllers;

[ApiController]
[Route("api/auth")]
[Authorize]
public class AuthController(UserManager<ApplicationUser> userManager) : ControllerBase
{
    [HttpGet("me")]
    public async Task<ActionResult<UserDto>> Me()
    {
        // check user id
        var user = await userManager.GetUserAsync(User);
        if (user is null) return Unauthorized();

        var roles = await userManager.GetRolesAsync(user);
        return Ok(new UserDto(user.Id, user.Email!, user.FullName, roles));
    }

    [HttpPut("me)")]
    public async Task<ActionResult> UpdateMe(UpdateProfileDto dto)
    {
        var user = await userManager.GetUserAsync(User);
        if (user is null) return Unauthorized();

        user.FullName = dto.FullName.Trim();

        var result = await userManager.UpdateAsync(user);
        if (!result.Succeeded)
        {
            foreach (var error in result.Errors)
                ModelState.AddModelError(error.Code, error.Description);

            return ValidationProblem();
        }

        return NoContent();
    }
}