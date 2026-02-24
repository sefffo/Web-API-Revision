using ECommerce.SharedLibirary.CommonResult;
using ECommerce.SharedLibirary.DTO_s.IdentityDTOs;
using System;
using System.Collections.Generic;
using System.Text;

namespace ECommerce.Services.Abstraction
{
    public interface IAuthenticationService
    {
        //login 
        Task<Result<UserDTO>> LoginAsync(LoginDTO loginDTO);



        //register 

        Task<Result<UserDTO>> RegisterAsync(RegisterDTO registerDTO);
    }
}
