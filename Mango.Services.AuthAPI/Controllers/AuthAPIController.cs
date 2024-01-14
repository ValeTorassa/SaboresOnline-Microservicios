using Mango.Services.AuthAPI.Models.Dto;
using Mango.Services.AuthAPI.Service.IService;
using Microsoft.AspNetCore.Mvc;

namespace Mango.Services.AuthAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthAPIController : ControllerBase
    {
        private readonly IAuthService _authService;
        protected ResponseDto _response;

        public AuthAPIController(IAuthService authService)
        {
            _authService = authService;
            _response = new ResponseDto();
        }


        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegistrationRequestDto model)
        {
            // Llamar al servicio de autenticación para realizar el registro del usuario
            var errorMessage = await _authService.Register(model);

            // Verificar si hay un mensaje de error después del registro
            if (!string.IsNullOrEmpty(errorMessage))
            {
                // Configurar la respuesta como no exitosa y establecer el mensaje de error
                _response.IsSuccess = false;
                _response.Message = errorMessage;

                // Devolver una respuesta HTTP BadRequest con la respuesta no exitosa y el mensaje de error
                return BadRequest(_response);
            }

            // Si el registro es exitoso, devolver una respuesta HTTP Ok con la respuesta exitosa
            return Ok(_response);
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequestDto model)
        {
            var loginResponse = await _authService.Login(model);

            if (loginResponse == null)
            {
                _response.IsSuccess = false;
                _response.Message = "Usuario o contraseña incorrectos.";

                return BadRequest(_response);
            }

            _response.Result = loginResponse;
            return Ok(_response);
        }



        [HttpPost("AssignRole")]
        public async Task<IActionResult> AssignRole([FromBody] RegistrationRequestDto model)
        {
            var loginResponse = await _authService.AssignRole(model.Email, model.Role.ToUpper());

            if (loginResponse == null)
            {
                _response.IsSuccess = false;
                _response.Message = "Erro Encontrado";

                return BadRequest(_response);
            }

            _response.Result = loginResponse;
            return Ok(_response);
        }
    }
}
