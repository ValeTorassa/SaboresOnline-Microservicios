﻿namespace Mango.Web.Models
{
    public class LoginResponseDto
    {
        public UserDto User { get; set; }
        public string Token { get; set; }

        //información que se envía como respuesta después de un exitoso inicio de sesión
    }
}