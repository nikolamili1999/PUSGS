using IdentityApi.Dto;
using IdentityApi.Models;
using System.Collections.Generic;

namespace IdentityApi.Interfaces
{
    public interface IUserService
    {
        UserDto GetUserByEmail(string email);
        UserDto GetUserByUsername(string username);
        UserDto AddUser(RegisterDto registerDto);
        UserDto UpdateUser(string email, UpdateUserDto dto);
        SuccessLoginDto LoginUser(LoginDto loginDto);
        List<UserDto> GetSellers();
        bool UpdateSellerStatus(UpdateStatusDto dto);
        void UpdatePendingStatus(string email, ERequestStatus status);
        SuccessLoginDto LoginGoogle(GoogleLoginDto user);

    }
}
