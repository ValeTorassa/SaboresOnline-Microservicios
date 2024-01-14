using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Runtime.CompilerServices;
using System.Text;

namespace Mango.Services.CouponAPI.Extensions
{
    public static class WebApplicationBuilderExtensions
    {
        public static WebApplicationBuilder AddAppAuthentication(this WebApplicationBuilder builder)
        {
            /*
        Se obtienen los parámetros necesarios para la configuración y validación de tokens JWT desde la sección "ApiSettings" 
            */

            var settingsSection = builder.Configuration.GetSection("ApiSettings");

            // Configuración de parámetros JWT desde el archivo de configuración
            var secret = settingsSection.GetValue<string>("Secret");
            var issuer = settingsSection.GetValue<string>("Issuer");
            var audience = settingsSection.GetValue<string>("Audience");


            // Obtiene la clave en formato de bytes a partir del secreto
            var key = Encoding.UTF8.GetBytes(secret);


            /*
             Se configura el sistema de autenticación para utilizar JWT. 
            Esto asegura que las solicitudes se autentiquen correctamente utilizando tokens JWT, 
            y se especifican los parámetros de validación necesarios.
             */

            // Configuración de autenticación JWT
            builder.Services.AddAuthentication(x =>
            {
                // Establece el esquema de autenticación predeterminado para JwtBearer
                x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(x =>
            {
                // Configuración de validación de tokens JWT
                x.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = true,
                    ValidIssuer = issuer,
                    ValidateAudience = true,
                    ValidAudience = audience
                };
            });

            return builder;
        }
    }
}
