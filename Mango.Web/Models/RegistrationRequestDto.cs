using System.ComponentModel.DataAnnotations;

namespace Mango.Web.Models
{
    public class RegistrationRequestDto
    {
        [Required]
        public string Email { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        public string PhoneNumber { get; set; }
        [Required]
        public string Password { get; set; }
        public string? Role { get; set; }

        /*que modela la información necesaria para registrar a un nuevo usuario en el sistema. 
         * En este caso, la clase tiene propiedades que representan los datos asociados con el registro de un usuario*/
    }
}
