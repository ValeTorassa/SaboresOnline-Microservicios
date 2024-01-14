namespace Mango.Services.AuthAPI.Models.Dto
{
    public class RegistrationRequestDto
    {
        public string Email { get; set; }
        public string Name { get; set; }
        public string PhoneNumber { get; set; }
        public string Password { get; set; }
        public string? Role { get; set; }

        /*que modela la información necesaria para registrar a un nuevo usuario en el sistema. 
         * En este caso, la clase tiene propiedades que representan los datos asociados con el registro de un usuario*/
    }
}
