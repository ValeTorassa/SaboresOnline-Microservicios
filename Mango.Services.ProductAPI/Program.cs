
using AutoMapper;
using Mango.Services.ProductAPI.Extensions;
using Mango.Services.ProductAPI.Data;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;

namespace Mango.Services.ProductAPI
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            // agregar el contexto de base de datos al contenedor
            builder.Services.AddDbContext<AppDbContext>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));


            builder.Services.AddControllers();
            builder.Services.AddEndpointsApiExplorer();
            /*SwaggerGen se configura para incluir detalles sobre cómo se debe manejar la autenticación JWT en Swagger UI. 
             Esto incluye la forma de proporcionar el token en las solicitudes y los requerimientos de seguridad asociados.
             */

            builder.Services.AddSwaggerGen(option =>
            {
                option.AddSecurityDefinition(name: JwtBearerDefaults.AuthenticationScheme, securityScheme: new OpenApiSecurityScheme
                {
                    // Configuración de cómo se debe proporcionar el token JWT en Swagger UI
                    Name = "Authorization",
                    Description = "Enter the Bearer Authorization string as following: 'Bearer Generated-JWT-Token'",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.ApiKey,
                    Scheme = "Bearer"
                });

                // Configuración de requerimientos de seguridad para Swagger UI
                option.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = JwtBearerDefaults.AuthenticationScheme
                            }
                        },
                        new string[]{ }
                    }
                });
            });


            builder.AddAppAuthentication(); // Agrega el servicio de autenticación de la carpeta extensions

            // Agrega el servicio de autorización
            builder.Services.AddAuthorization();


            var app = builder.Build();

            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();
            app.UseAuthentication(); // para que el usuario necesite autenticarse para poder acceder a esta API
            app.UseAuthorization();


            app.MapControllers();
            ApplyMigrations();
            app.Run();

            void ApplyMigrations()
            {
                using (var scope = app.Services.CreateScope())
                {
                    var _db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

                    if (_db.Database.GetPendingMigrations().Count() > 0)
                    {
                        _db.Database.Migrate();
                    }
                }
            }
        }
    }
}
