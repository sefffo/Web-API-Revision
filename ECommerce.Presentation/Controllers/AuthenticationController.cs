using ECommerce.Services.Abstraction;
using ECommerce.SharedLibirary.DTO_s.IdentityDTOs;
using Microsoft.AspNetCore.Mvc;

namespace ECommerce.Presentation.Controllers
{
    public class AuthenticationController(IAuthenticationService authenticationService) : ApiBaseController
    {
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




    }
}
