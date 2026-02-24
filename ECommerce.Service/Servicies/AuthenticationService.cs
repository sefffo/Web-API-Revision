using ECommerce.Domain.Entities.IdentityModule;
using ECommerce.Services.Abstraction;
using ECommerce.SharedLibirary.CommonResult;
using ECommerce.SharedLibirary.DTO_s.IdentityDTOs;
using Microsoft.AspNetCore.Identity;

namespace ECommerce.Services.Servicies
{
    public class AuthenticationService(UserManager<AppUser> userManager) : IAuthenticationService
    {
        public async Task<Result<UserDTO>> LoginAsync(LoginDTO loginDTO)
        {
            //check first if the user exist in the database by email

            var user = await userManager.FindByEmailAsync(loginDTO.Email);
            if (user == null)
            {
                return Error.InvalidCredentials("Invalid email or password", "Invalid email or password");
            }
            //if not exist return fail result with error message "Invalid email or password"
            //if exist check if the password is correct
            var isPasswordCorrect = await userManager.CheckPasswordAsync(user, loginDTO.Password);
            if (!isPasswordCorrect)
            {
                return Error.InvalidCredentials("Invalid email or password", "Invalid email or password");
            }
            var userDTO = new UserDTO(user.Email!, user.DisplayName, "Test");
            return Result<UserDTO>.Ok(userDTO);
            //if not correct return fail result with error message "Invalid email or password"
            //if correct generate a token for the user and return ok result with the userDTO

        }

        public async Task<Result<UserDTO>> RegisterAsync(RegisterDTO registerDTO)
        {
            var user = new AppUser
            {
                Email = registerDTO.Email,
                DisplayName = registerDTO.DisplayName,
                UserName = registerDTO.UserName,
                PhoneNumber = registerDTO.PhoneNumber
            };
            var result = await userManager.CreateAsync(user, registerDTO.Password);
            if (result.Succeeded)
            {
                return Result<UserDTO>.Ok(new UserDTO(Email: user.Email!, DisplayName : user.DisplayName, Token: "Test"));
            }
            else
            {
               return Result<UserDTO>.Fail(result.Errors.Select(e => Error.Validation(code: e.Code, description: e.Description)).ToList());
            }

        }
    }
}
