
using AutoMapper;
using Mango.MessageBus;
using Mango.Services.ShoppinCartAPI;
using Mango.Services.ShoppingCartAPI.Data;
using Mango.Services.ShoppingCartAPI.Extensions;
using Mango.Services.ShoppingCartAPI.Service;
using Mango.Services.ShoppingCartAPI.Service.IService;
using Mango.Services.ShoppingCartAPI.Utility;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;

namespace Mango.Services.ShoppingCartAPI
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            // agregar el contexto de base de datos al contenedor
            builder.Services.AddDbContext<AppDbContext>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
            
            //agregar automapper
            IMapper mapper = MappingConfig.RegisterMaps().CreateMapper();
            builder.Services.AddSingleton(mapper);
            builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

            builder.Services.AddScoped<IProductService, ProductService>();

            //Agrega el servicio HttpContextAccessor al contenedor de servicios.
            //Este servicio proporciona acceso al contexto HTTP actual, que incluye información sobre la solicitud HTTP en curso.
            builder.Services.AddHttpContextAccessor(); 

            builder.Services.AddScoped<BackendApiAuthenticationHttpClientHandler>();

            builder.Services.AddScoped<ICouponService, CouponService>();

            builder.Services.AddScoped<IMessageBus, Mango.MessageBus.MessageBus>();

            // Agrega un cliente HTTP llamado "Product" al contenedor de servicios
            // Configura la dirección base para las solicitudes del cliente HTTP "Product"
            // La dirección base se obtiene de "ServiceUrls:ProductAPI" en el json
            builder.Services.AddHttpClient("Product", u => u.BaseAddress = 
            new Uri(builder.Configuration["ServiceUrls:ProductAPI"])).AddHttpMessageHandler<BackendApiAuthenticationHttpClientHandler>();


            // agrega BackendApiAuthenticationHttpClientHandler como un delegado que se ejecuta antes de cada solicitud HTTP saliente
            // y agrega el token de acceso JWT al encabezado de autorización, permitiendo la autenticación en las API.
            builder.Services.AddHttpClient("Coupon", u => u.BaseAddress = 
            new Uri(builder.Configuration["ServiceUrls:CouponAPI"])).AddHttpMessageHandler<BackendApiAuthenticationHttpClientHandler>();

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
