using Mango.Web.Models;
using Mango.Web.Service.IService;
using Mango.Web.Utility;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace Mango.Web.Controllers
{
    public class AuthController : Controller
    {
        private readonly IAuthService _authService;

        private readonly ITokenProvider _tokenProvider;

        public AuthController(IAuthService authService, ITokenProvider tokenProvider)
        {
            _authService = authService;
            _tokenProvider = tokenProvider;
        }

        [HttpGet]
        public IActionResult Login()
        {
            LoginRequestDto loginRequestDto = new LoginRequestDto();
            return View(loginRequestDto);
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginRequestDto loginRequestDto)
        {
            // Intenta autenticar al usuario llamando al servicio de autenticación
            ResponseDto response = _authService.LoginAsync(loginRequestDto).Result;

            if (response != null && response.IsSuccess)
            {
                // Autenticación exitosa: deserializa el resultado y realiza acciones adicionales
                LoginResponseDto loginResponseDto = JsonConvert.DeserializeObject<LoginResponseDto>(Convert.ToString(response.Result));

                // Realiza la autenticación del usuario en el sistema
                await SignInUser(loginResponseDto);

                // Establece el token JWT en algún lugar del sistema
                _tokenProvider.SetToken(loginResponseDto.Token);

                // Establece un mensaje de éxito para mostrar en la interfaz de usuario
                TempData["Success"] = "you have logged in successfully";

                // Redirige al usuario a la acción "Index" del controlador "Home"
                return RedirectToAction("Index", "Home");
            }
            else
            {
                TempData["Error"] = response.Message;
                return View(loginRequestDto);
            }

            return View();
        }




        [HttpGet]
        public IActionResult Register()
        {
            // Crear una lista de SelectListItem para representar elementos en un control de lista desplegable
            var roleList = new List<SelectListItem>
            {
                // Primer elemento con texto "Admin" y valor "Admin"
                new SelectListItem { Text = SD.RoleAdmin, Value = SD.RoleAdmin },
                // Segundo elemento con texto "Customer" y valor "Customer"
                new SelectListItem { Text = SD.RoleCustomer, Value = SD.RoleCustomer }
            };

            // Almacenar la lista en ViewBag para que esté disponible en la vista
            ViewBag.RoleList = roleList;
            /*
            La asignación de valores a ViewBag en el controlador es un mecanismo para compartir datos entre el controlador y la vista 
            sin necesidad de pasarlos como parámetros en el método de acción. 
             */

            return View();
        }


        [HttpPost]
        public async Task<IActionResult> Register(RegistrationRequestDto registrationRequestDto)
        {
            ResponseDto responseDtoRegister = _authService.RegisterAsync(registrationRequestDto).Result;

            ResponseDto responseDtoAssignRole;

            if(responseDtoRegister != null && responseDtoRegister.IsSuccess)
            {
                if (string.IsNullOrEmpty(registrationRequestDto.Role))
                {
                    registrationRequestDto.Role = SD.RoleCustomer;
                }
                responseDtoAssignRole = await _authService.AssignRoleAsync(registrationRequestDto);

                if(responseDtoAssignRole != null && responseDtoAssignRole.IsSuccess)
                {
                    //TempData se encuentra en Views/Shared/_Notifications.cshtml
                    TempData["Success"] = "Successful";
                    return RedirectToAction(nameof(Login));
                }else
                {
                    TempData["Error"] = responseDtoRegister.Message;
                }
            }
            var roleList = new List<SelectListItem>
            {
                new SelectListItem { Text = SD.RoleAdmin, Value = SD.RoleAdmin },
                new SelectListItem { Text = SD.RoleCustomer, Value = SD.RoleCustomer }
            };

            ViewBag.RoleList = roleList;


            return View();
        }


        public IActionResult Logout()
        {
            // Cierra la sesión del usuario en el sistema local
            HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

            // Elimina el token JWT del sistema
            _tokenProvider.ClearToken();

            // Establece un mensaje de éxito para mostrar en la interfaz de usuario
            TempData["Success"] = "you have logged out successfully";

            // Redirige al usuario a la acción "Index" del controlador "Home"
            return RedirectToAction("Index", "Home");
        }


        // Método que realiza el inicio de sesión del usuario en el sistema local
        // utilizando la información del token JWT generado durante la autenticación.
        private async Task SignInUser(LoginResponseDto loginResponse)
        {
            // Manejador de tokens JWT para leer y procesar el token recibido.
            var handler = new JwtSecurityTokenHandler();
            var jwt = handler.ReadJwtToken(loginResponse.Token);

            // Creación de una identidad del usuario utilizando las reclamaciones (claims) del token JWT.
            var identity = new ClaimsIdentity(jwt.Claims, CookieAuthenticationDefaults.AuthenticationScheme);

            // Adición de reclamaciones específicas a la identidad. Ejemplo: correo electrónico, sub (subject), nombre.
            //las claimes sirven para almacenar informacion de la sesion del usuario con datos adicionales como los reclamados a continuacion
            identity.AddClaim(new Claim(JwtRegisteredClaimNames.Email, jwt.Claims.FirstOrDefault(u => u.Type == JwtRegisteredClaimNames.Email).Value));
            identity.AddClaim(new Claim(JwtRegisteredClaimNames.Sub, jwt.Claims.FirstOrDefault(u => u.Type == JwtRegisteredClaimNames.Sub).Value));
            identity.AddClaim(new Claim(JwtRegisteredClaimNames.Name, jwt.Claims.FirstOrDefault(u => u.Type == JwtRegisteredClaimNames.Name).Value));

            // Adición de una reclamación adicional para el nombre del usuario utilizando ClaimTypes.
            identity.AddClaim(new Claim(ClaimTypes.Name, jwt.Claims.FirstOrDefault(u => u.Type == JwtRegisteredClaimNames.Email).Value));

            // Adición de una reclamación adicional para el rol del usuario utilizando ClaimTypes.
            identity.AddClaim(new Claim(ClaimTypes.Role, jwt.Claims.FirstOrDefault(u => u.Type == "role").Value));

            // Creación del principal que representa al usuario con la identidad construida.
            var principal = new ClaimsPrincipal(identity);

            // Establecimiento de la sesión del usuario en el sistema local.
            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);
        }
    }
}
