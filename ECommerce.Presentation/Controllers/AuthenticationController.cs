using ECommerce.SharedLibirary.CommonResult;
using ECommerce.SharedLibirary.DTO_s.IdentityDTOs;
using ECommerce.SharedLibirary.DTO_s.OrderDTOs;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace ECommerce.Presentation.Controllers
{
    public class AuthenticationController(Services.Abstraction.IAuthenticationService authenticationService) : ApiBaseController
    {

        [HttpPost("assign-role")]
        [Authorize(Roles = "SuperAdmin")]
        public async Task<ActionResult> AssignRole(AssignRoleDTO assignRoleDTO)
        {
            var result = await authenticationService.AssignRoleAsync(assignRoleDTO);
            if (result.isSuccess)
                return Ok($"Role {assignRoleDTO.RoleName} assigned to user {assignRoleDTO.UserEmail}");
            return HandleResult(result);
        }

        [HttpPost("login")]
        public async Task<ActionResult<UserDTO>> Login(LoginDTO loginDTO)
        {
            var result = await authenticationService.LoginAsync(loginDTO);
            return HandleResult(result);
        }

        [HttpPost("Register")]
        public async Task<ActionResult<UserDTO>> Register(RegisterDTO registerDTO)
        {
            var result = await authenticationService.RegisterAsync(registerDTO);
            return HandleResult(result);
        }

        [HttpGet("check-email")]
        public async Task<ActionResult<bool>> CheckEmail([FromQuery] string email)
        {
            var exists = await authenticationService.CheckEmailAsync(email);
            return Ok(exists);
        }

        [HttpGet("CurrentUser")]
        [Authorize]
        public async Task<ActionResult<Result<UserDTO>>> GetCurrentUserAsync()
        {
            var email = User.FindFirstValue(ClaimTypes.Email);
            var token = await HttpContext.GetTokenAsync("access_token");
            return HandleResult(await authenticationService.GetCurrentUserAsync(email!, token!));
        }

        [HttpGet("address")]
        [Authorize]
        public async Task<ActionResult<AddressDTO>> GetAddressAsync()
        {
            var email = User.FindFirstValue(ClaimTypes.Email);
            return HandleResult(await authenticationService.GetUserAddressAsync(email!));
        }

        [HttpPut("address")]
        [Authorize]
        public async Task<ActionResult<AddressDTO>> UpdateAddressAsync(AddressDTO addressDto)
        {
            var email = User.FindFirstValue(ClaimTypes.Email);
            return HandleResult(await authenticationService.UpdateUserAddressAsync(email!, addressDto));
        }

        [HttpPost("refresh-token")]
        public async Task<ActionResult<UserDTO>> RefreshToken(RefreshTokenDTO refreshTokenDTO)
        {
            var result = await authenticationService.RefreshTokenAsync(refreshTokenDTO);
            return HandleResult(result);
        }

        [Authorize(Roles = "SuperAdmin")]
        [HttpGet("users")]
        public async Task<ActionResult<IEnumerable<UserDTO>>> GetAllUsersAsync()
        {
            return HandleResult(await authenticationService.GetAllUsersAsync());
        }

        [HttpDelete("users/{email}")]
        [Authorize(Roles = "SuperAdmin")]
        public async Task<ActionResult<string>> DeleteUserAsync(string email)
        {
            var result = await authenticationService.DeleteUserAsync(email);
            return HandleResult(result);
        }

        [HttpPost("users/{email}/revoke-token")]
        [Authorize(Roles = "SuperAdmin")]
        public async Task<ActionResult<string>> RevokeRefreshTokenAsync(string email)
        {
            var result = await authenticationService.RevokeRefreshTokenAsync(email);
            return HandleResult(result);
        }

        [HttpGet("google-login")]
        public IActionResult GoogleLogin()
        {
            var redirectUri = Url.Action(nameof(GoogleCallback), "Authentication", null, Request.Scheme);
            var properties = new AuthenticationProperties { RedirectUri = redirectUri };
            return Challenge(properties, GoogleDefaults.AuthenticationScheme);
        }

        [HttpGet("google-callback")]
        public async Task<ActionResult<UserDTO>> GoogleCallback()
        {
            var result = await HttpContext.AuthenticateAsync(CookieAuthenticationDefaults.AuthenticationScheme);

            if (!result.Succeeded)
                return Unauthorized(new { message = "Google authentication failed" });

            var email = result.Principal!.FindFirstValue(ClaimTypes.Email)!;
            var name = result.Principal!.FindFirstValue(ClaimTypes.Name)!;
            var googleId = result.Principal!.FindFirstValue(ClaimTypes.NameIdentifier)!;

            var response = await authenticationService.HandleGoogleLoginAsync(email, name, googleId);
            return HandleResult(response);
        }
    }
}
