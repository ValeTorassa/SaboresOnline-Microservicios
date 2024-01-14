namespace Mango.Services.AuthAPI.Models
{
    public class JwtOptions
    {
        public string Issuer { get; set; } = string.Empty; //entidad que emitió el token
        public string Audience { get; set; } = string.Empty; // entidad que debe recibir y procesar el token
        public string Secret { get; set; } = string.Empty; // This is the secret key that will be used to encrypt and decrypt the token.

        /*
Emisor (Issuer):
Puede ser tu propia aplicación o servicio que emite tokens JWT para la autenticación.
Ejemplo: "Issuer": "miaplicacion.com"

Audiencia (Audience):
Puede ser el nombre de tu aplicación o servicio que espera y consume el token.
Ejemplo: "Audience": "miaplicacioncliente"*/
    }
}
