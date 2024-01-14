
using AutoMapper;
using Mango.Services.AuthAPI.Data;
using Mango.Services.AuthAPI.Models;
using Mango.Services.AuthAPI.Service;
using Mango.Services.AuthAPI.Service.IService;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Mango.Services.AuthAPI
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddDbContext<AppDbContext>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));


            //Esto es com�nmente utilizado en ASP.NET Core para extraer y asignar valores de configuraci�n a objetos espec�ficos
            //La secci�n en la configuraci�n deber�a contener propiedades que coincidan
            builder.Services.Configure<JwtOptions>(builder.Configuration.GetSection("ApiSettings:JwtOptions"));


            builder.Services.AddIdentity<ApplicationUser, IdentityRole>().AddEntityFrameworkStores<AppDbContext>().AddDefaultTokenProviders();
            /*
            AddIdentity<IdentityUser, IdentityRole>(): Agrega el servicio de autenticaci�n Identity a la colecci�n de servicios. 
            IdentityUser representa un usuario y IdentityRole representa un rol en el sistema de Identity.
           
            AddEntityFrameworkStores<AppDbContext>(): Configura Identity para utilizar Entity Framework para almacenar informaci�n 
            sobre usuarios y roles en la base de datos.
            
            AddDefaultTokenProviders(): Agrega proveedores de tokens predeterminados necesarios para la recuperaci�n de contrase�as, confirmaci�n de correo, etc..
             */
            builder.Services.AddScoped<IJwtTokenGenerator, JwtTokenGenerator>();


            //registramos el IAuthService en el contenedor de dependencias
            builder.Services.AddScoped<IAuthService, AuthService>();

            builder.Services.AddControllers();
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            var app = builder.Build();


            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            //esto se agrega nuevo para esta API
            app.UseAuthentication(); //Habilita el middleware de autenticaci�n en la canalizaci�n de solicitud

            app.UseAuthorization();

            app.MapControllers();

            ApplyMigration();

            app.Run();

            void ApplyMigration()
            {
                using (var scope = app.Services.CreateScope())
                {
                    var _db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

                    if(_db.Database.GetPendingMigrations().Count() > 0)
                    {
                        _db.Database.Migrate();
                    }
                }
            }
        }
    }
}
