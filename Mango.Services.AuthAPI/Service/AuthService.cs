using Mango.Services.AuthAPI.Data;
using Mango.Services.AuthAPI.Models;
using Mango.Services.AuthAPI.Models.Dto;
using Mango.Services.AuthAPI.Service.IService;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;

namespace Mango.Services.AuthAPI.Service
{
    public class AuthService : IAuthService
    {
        private readonly AppDbContext _db;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IJwtTokenGenerator _jwtTokenGenerator;

        public AuthService(AppDbContext db, UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager, IJwtTokenGenerator jwtTokenGenerator )
        {
            _db = db;
            _userManager = userManager;
            _roleManager = roleManager;
            _jwtTokenGenerator = jwtTokenGenerator;
        }



        public async Task<LoginResponseDto> Login(LoginRequestDto loginRequestDTO)
        {
            // Busca al usuario en la base de datos por nombre de usuario
            var user = _db.ApplicationUsers.Where(u => u.UserName.ToLower() == loginRequestDTO.UserName.ToLower()).FirstOrDefault();

            // Verifica si la contraseña proporcionada es válida para el usuario
            bool isValid = await _userManager.CheckPasswordAsync(user, loginRequestDTO.Password);

            // Si el usuario no existe o la contraseña no es válida, devuelve una respuesta vacía
            if (user == null || !isValid)
            {
                return new LoginResponseDto() { User = null, Token = "" };
            }

            var roles = await _userManager.GetRolesAsync(user);

            // Genera un token JWT utilizando el generador de tokens
            var token = _jwtTokenGenerator.GenerateToken(user, roles);

            // Crea un objeto UserDto con la información relevante del usuario
            UserDto userDto = new UserDto()
            {
                Email = user.Email,
                Name = user.Name,
                Id = user.Id,
                PhoneNumber = user.PhoneNumber
            };

            // Crea un objeto LoginResponseDto con el objeto UserDto y el token JWT
            LoginResponseDto loginResponseDto = new LoginResponseDto()
            {
                User = userDto,
                Token = token //este token se adjunta a las solicitudes posteriores al servidor para demostrar que ha iniciado sesión
                              //El servidor puede validar este token para autorizar al usuario en operaciones protegidas
            };

            // Devuelve la respuesta con el objeto LoginResponseDto
            return loginResponseDto;
        }




        public async Task<string> Register(RegistrationRequestDto registrationRequestDTO)
        {
            // Crear una instancia de ApplicationUser con los datos proporcionados en el DTO
            ApplicationUser user = new ApplicationUser()
            {
                UserName = registrationRequestDTO.Email,
                Email = registrationRequestDTO.Email,
                NormalizedEmail = registrationRequestDTO.Email.ToUpper(),
                Name = registrationRequestDTO.Name,
                PhoneNumber = registrationRequestDTO.PhoneNumber
            };

            try
            {
                // Intentar crear el usuario utilizando UserManager
                var result = await _userManager.CreateAsync(user, registrationRequestDTO.Password);

                // Verificar si la creación del usuario fue exitosa
                if (result.Succeeded)
                {
                    // Obtener el usuario recién creado desde la base de datos
                    var userToReturn = _db.ApplicationUsers
                        .Where(u => u.UserName == registrationRequestDTO.Email)
                        .FirstOrDefault();

                    // Crear un objeto UserDto para devolver información del usuario registrado
                    UserDto userDto = new UserDto()
                    {
                        Email = userToReturn.Email,
                        Name = userToReturn.Name,
                        Id = userToReturn.Id,
                        PhoneNumber = userToReturn.PhoneNumber
                    };

                    return "";
                }
                else
                {
                    return result.Errors.FirstOrDefault().Description;
                }
            } 
            catch(Exception ex)
            {

            }

            return "Error Encountered";
        }

        /*
            El método CreateAsync del UserManager de ASP.NET Identity lleva a cabo varias validaciones de contraseña 

            Longitud Mínima (Password.RequiredLength)

            Caracteres Especiales (Password.RequireNonAlphanumeric): La contraseña debe contener al menos  un símbolo o un carácter especial.

            Letras Mayúsculas (Password.RequireUppercase)

            Letras Minúsculas (Password.RequireLowercase)

            Dígitos (Password.RequireDigit): La contraseña debe contener al menos un dígito numérico.*/



        // Método para asignar un rol a un usuario basado en su dirección de correo electrónico
        public async Task<bool> AssignRole(string email, string roleName)
        {
            // Buscar al usuario por su dirección de correo electrónico en la base de datos
            var user = _db.ApplicationUsers.Where(u => u.Email.ToLower() == email.ToLower()).FirstOrDefault();

            // Verificar si se encontró al usuario
            if (user != null)
            {
                // Verificar si el rol ya existe
                if (!_roleManager.RoleExistsAsync(roleName).GetAwaiter().GetResult())
                {
                    // Si el rol no existe, crearlo
                    _roleManager.CreateAsync(new IdentityRole(roleName)).GetAwaiter().GetResult();
                }

                // Asignar el usuario al rol especificado
                await _userManager.AddToRoleAsync(user, roleName);

                // La operación fue exitosa, devolver true
                return true;
            }

            // El usuario no fue encontrado, devolver false
            return false;
        }

    }
}
