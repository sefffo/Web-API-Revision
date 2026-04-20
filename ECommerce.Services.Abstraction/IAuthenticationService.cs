using ECommerce.Domain.Entities.IdentityModule;
using ECommerce.SharedLibirary.CommonResult;
using ECommerce.SharedLibirary.DTO_s.IdentityDTOs;
using ECommerce.SharedLibirary.DTO_s.OrderDTOs;

namespace ECommerce.Services.Abstraction
{
    public interface IAuthenticationService
    {

        // Role Management
        Task<Result<string>> AssignRoleAsync(AssignRoleDTO assignRoleDTO);


        //login 
        Task<Result<UserDTO>> LoginAsync(LoginDTO loginDTO);



        //register 

        Task<Result<UserDTO>> RegisterAsync(RegisterDTO registerDTO);

        //generating JWT Token


        Task<string> GenerateJWTToken(AppUser appUser);



        //check email 

        Task<bool> CheckEmailAsync(string Email); //==> get it from teh token 

        //get all users 

        Task<Result<IEnumerable<UserDTO>>> GetAllUsersAsync();

        // delete a user by email (SuperAdmin)
        Task<Result<string>> DeleteUserAsync(string email);

        // revoke a user's refresh token by email (SuperAdmin)
        Task<Result<string>> RevokeRefreshTokenAsync(string email);

        //get current user address
        Task<Result<AddressDTO>> GetUserAddressAsync(string email);

        Task<Result<AddressDTO>> UpdateUserAddressAsync(string email, AddressDTO NewAddress); //take the email from the token and the new addressDto ==> return updated address

        //get the current user 
        //get the mail from the token and return the user name and email

        Task<Result<UserDTO>> GetCurrentUserAsync(string email, string Token);


        Task<Result<UserDTO>> RefreshTokenAsync(RefreshTokenDTO refreshTokenDTO);


        #region Explaining RefreshToken 

        /*
         * 
         * 
         *  What the Client Actually Needs
            When the client calls /refresh-token, it needs to receive a complete response it can use immediately.
                If you returned just Task<string>, you'd only return the new access token — but the client also needs:
            The new refresh token (for the next rotation)
            The user's email and display name (to re-render the UI if needed)
         * 
         * 
         * 
         * 
         */

        #endregion


        //Adding OAuth Service 

        Task<Result<UserDTO>> HandleGoogleLoginAsync(string email, string name, string googleId);

    }
}
