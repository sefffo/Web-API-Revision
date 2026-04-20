using AutoMapper;
using ECommerce.Domain.Entities.IdentityModule;
using ECommerce.Services.Abstraction;
using ECommerce.SharedLibirary.CommonResult;
using ECommerce.SharedLibirary.DTO_s.IdentityDTOs;
using ECommerce.SharedLibirary.DTO_s.OrderDTOs;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace ECommerce.Services.Servicies
{
    public class AuthenticationService(UserManager<AppUser> userManager,RoleManager<IdentityRole> roleManager, IConfiguration configuration, IMapper mapper) : IAuthenticationService
    {
        private string GenerateRefreshToken()
        {
            // generates a cryptographically secure random 32-byte string
            // this is NOT a JWT — it's just a random opaque string stored in DB
            var randomBytes = new byte[32];
            using var rng = System.Security.Cryptography.RandomNumberGenerator.Create();
            rng.GetBytes(randomBytes);
            return Convert.ToBase64String(randomBytes);
        }

        public async Task<bool> CheckEmailAsync(string Email)
        {
            var email = await userManager.FindByEmailAsync(Email);
            if (email == null)
                return false;
            else
                return true;
        }

        public async Task<Result<UserDTO>> GetCurrentUserAsync(string email, string Token)
        {
            var User = await userManager.FindByEmailAsync(email);
            if (User == null)
                return Error.NotFound("User Not Found", $"User With Email {email} is not Found");

            var userDto = new UserDTO(email, User.DisplayName, Token, RefreshToken: User.RefreshToken!);
            return Result<UserDTO>.Ok(userDto);
        }

        public async Task<Result<AddressDTO>> UpdateUserAddressAsync(string email, AddressDTO NewAddress)
        {
            var user = await userManager.Users
              .Include(u => u.Address)
              .FirstOrDefaultAsync(u => u.Email == email);

            if (user == null)
                return Error.NotFound("User Not Found", $"User with email {email} is not found");

            var addressEntity = mapper.Map<Address>(NewAddress);
            user.Address = addressEntity;

            var result = await userManager.UpdateAsync(user);

            if (!result.Succeeded)
                return Result<AddressDTO>.Fail(
                    result.Errors.Select(e => Error.Validation(e.Code, e.Description)).ToList());

            return Result<AddressDTO>.Ok(NewAddress);
        }

        public async Task<Result<AddressDTO>> GetUserAddressAsync(string email)
        {
            var User = await userManager.Users.Include(u => u.Address).FirstOrDefaultAsync(e => e.Email == email);

            if (User == null)
                return Error.NotFound("User Not Found", $"User With Email {email} is not Found");

            if (User.Address == null)
                return Error.NotFound("Address Not Found", "This user has no address set yet");

            var MappedAddress = mapper.Map<AddressDTO>(User.Address);
            return Result<AddressDTO>.Ok(MappedAddress);
        }

        public async Task<Result<UserDTO>> LoginAsync(LoginDTO loginDTO)
        {
            var user = await userManager.FindByEmailAsync(loginDTO.Email);
            if (user == null)
                return Error.InvalidCredentials("Invalid email or password", "Invalid email or password");

            var isPasswordCorrect = await userManager.CheckPasswordAsync(user, loginDTO.Password);
            if (!isPasswordCorrect)
                return Error.InvalidCredentials("Invalid email or password", "Invalid email or password");

            var refreshToken = GenerateRefreshToken();
            user.RefreshToken = refreshToken;
            user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(7);
            await userManager.UpdateAsync(user);

            var userDTO = new UserDTO(Email: user.Email!, user.DisplayName, Token: await GenerateJWTToken(user), RefreshToken: refreshToken);
            return Result<UserDTO>.Ok(userDTO);
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
                await userManager.AddToRoleAsync(user, role: "User");

                var refreshToken = GenerateRefreshToken();
                user.RefreshToken = refreshToken;
                user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(7);
                await userManager.UpdateAsync(user);

                return Result<UserDTO>.Ok(new UserDTO(
                    Email: user.Email!,
                    DisplayName: user.DisplayName,
                    Token: await GenerateJWTToken(user),
                    RefreshToken: refreshToken));
            }
            else
            {
                return Result<UserDTO>.Fail(
                    result.Errors.Select(e => Error.Validation(code: e.Code, description: e.Description)).ToList());
            }
        }

        public async Task<string> GenerateJWTToken(AppUser appUser)
        {
            var Roles = await userManager.GetRolesAsync(appUser);

            var Claims = new List<Claim>
            {
                new Claim(ClaimTypes.Email, appUser.Email),
                new Claim(ClaimTypes.NameIdentifier, appUser.Id),
                new Claim(ClaimTypes.Name, appUser.UserName),
            };

            foreach (var role in Roles)
                Claims.Add(new Claim(ClaimTypes.Role, role));

            var securityKey = configuration["JwtOptions:securityKey"];
            if (string.IsNullOrEmpty(securityKey))
                throw new InvalidOperationException("JwtOptions:securityKey missing from configuration.");

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(securityKey));
            var SigningCredentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var issuer = configuration["JwtOptions:Issuer"];
            var audience = configuration["JwtOptions:Audience"];

            var token = new JwtSecurityToken(
                claims: Claims,
                expires: DateTime.Now.AddHours(2),
                signingCredentials: SigningCredentials,
                issuer: issuer,
                audience: audience
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public async Task<Result<UserDTO>> RefreshTokenAsync(RefreshTokenDTO refreshTokenDTO)
        {
            var tokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidIssuer = configuration["JwtOptions:Issuer"],
                ValidAudience = configuration["JwtOptions:Audience"],
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(
                    Encoding.UTF8.GetBytes(configuration["JwtOptions:securityKey"]!)),
                ValidateLifetime = false // allow expired tokens here
            };

            ClaimsPrincipal principal;
            try
            {
                principal = new JwtSecurityTokenHandler()
                    .ValidateToken(refreshTokenDTO.AccessToken, tokenValidationParameters, out _);
            }
            catch
            {
                return Error.InvalidCredentials("Invalid token", "Access token is invalid");
            }

            var email = principal.FindFirstValue(ClaimTypes.Email);
            var user = await userManager.FindByEmailAsync(email!);

            if (user is null ||
                user.RefreshToken != refreshTokenDTO.RefreshToken ||
                user.RefreshTokenExpiryTime <= DateTime.UtcNow)
            {
                return Error.InvalidCredentials("Invalid refresh token", "Refresh token is invalid or expired");
            }

            var newAccessToken = await GenerateJWTToken(user);
            var newRefreshToken = GenerateRefreshToken();

            user.RefreshToken = newRefreshToken;
            user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(7);
            await userManager.UpdateAsync(user);

            return Result<UserDTO>.Ok(new UserDTO(user.Email!, user.DisplayName, newAccessToken, newRefreshToken));
        }

        public async Task<Result<UserDTO>> HandleGoogleLoginAsync(string email, string name, string googleId)
        {
            var user = await userManager.FindByEmailAsync(email);

            if (user is null)
            {
                // --- NEW USER PATH ---
                user = new AppUser
                {
                    UserName = email,
                    Email = email,
                    DisplayName = name,
                    EmailConfirmed = true
                };

                var createResult = await userManager.CreateAsync(user);
                if (!createResult.Succeeded)
                    return Result<UserDTO>.Fail(
                        createResult.Errors.Select(e => Error.Validation(e.Code, e.Description)).ToList());

                await userManager.AddToRoleAsync(user, "User");

                await userManager.AddLoginAsync(user,
                    new UserLoginInfo("Google", googleId, "Google"));

                // Re-fetch from DB to get the latest ConcurrencyStamp.
                // AddLoginAsync calls UpdateAsync internally which rotates the stamp.
                // If we don't re-fetch, our local `user` object has a stale stamp
                // and the UpdateAsync below will silently fail (no rows updated),
                // leaving RefreshToken as NULL in the database.
                user = (await userManager.FindByEmailAsync(email))!;
            }
            else
            {
                // --- EXISTING USER PATH ---
                // Self-heal: assign User role if missing (edge case from earlier runs)
                var roles = await userManager.GetRolesAsync(user);
                if (!roles.Any())
                    await userManager.AddToRoleAsync(user, "User");
            }

            // At this point `user` is always fresh from DB — safe to UpdateAsync
            var refreshToken = GenerateRefreshToken();
            user.RefreshToken = refreshToken;
            user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(7);
            await userManager.UpdateAsync(user);

            var accessToken = await GenerateJWTToken(user);

            return Result<UserDTO>.Ok(new UserDTO(
                Email: user.Email!,
                DisplayName: user.DisplayName,
                Token: accessToken,
                RefreshToken: refreshToken
            ));
        }

        public async Task<Result<string>> AssignRoleAsync(AssignRoleDTO assignRoleDTO)
        {
            var User = await userManager.FindByEmailAsync(assignRoleDTO.UserEmail);

            if (User == null)
                return Error.NotFound("User Not Found", $"User With Email {assignRoleDTO.UserEmail} is not Found");

            var RoleExists = await roleManager.RoleExistsAsync(assignRoleDTO.RoleName);

            if (!RoleExists)
                return Error.NotFound("Role Not Found", $"Role With Name {assignRoleDTO.RoleName} is not Found");

            var result = await userManager.AddToRoleAsync(User, assignRoleDTO.RoleName);

            if (!result.Succeeded)
                return Error.Failure("Failed to assign role", $"Failed to assign role {assignRoleDTO.RoleName} to user {assignRoleDTO.UserEmail}");

            return Result<string>.Ok($"Role {assignRoleDTO.RoleName} assigned to user {assignRoleDTO.UserEmail} successfully");
        }

        public async Task<Result<IEnumerable<UserDTO>>> GetAllUsersAsync()
        {
            var users = await userManager.Users.ToListAsync();
            

            var userDTOs = users.Select(u => new UserDTO(
                Email: u.Email!,
                DisplayName: u.DisplayName,
                Token: string.Empty, // No token for listing users
                RefreshToken: string.Empty // No refresh token for listing users
            )).ToList();


            return Result<IEnumerable<UserDTO>>.Ok(userDTOs);
        }
    }
}
