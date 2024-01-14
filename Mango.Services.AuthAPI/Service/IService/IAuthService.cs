using Mango.Services.AuthAPI.Models.Dto;

namespace Mango.Services.AuthAPI.Service.IService
{
    public interface IAuthService
    {
        Task<string> Register(RegistrationRequestDto registrationRequestDTO);
        Task<LoginResponseDto> Login(LoginRequestDto loginRequestDTO);
        Task<bool> AssignRole(string email, string roleName);
    }
}
