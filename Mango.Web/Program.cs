using Mango.Web.Service;
using Mango.Web.Service.IService;
using Mango.Web.Utility;
using Microsoft.AspNetCore.Authentication.Cookies;

namespace Mango.Web
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddControllersWithViews();
            builder.Services.AddHttpContextAccessor(); // información del contexto HTTP, como la solicitud actual y la autenticación del usuario.
            builder.Services.AddHttpClient();// realizar solicitudes HTTP desde la aplicación

            // Agregar el servicio HttpClient con interfaz específica y su implementación al contenedor de inyección de dependencias.
            builder.Services.AddHttpClient<ICouponService, CouponService>();
            builder.Services.AddHttpClient<IAuthService, AuthService>();
            builder.Services.AddHttpClient<IProductService, ProductService>();
            builder.Services.AddHttpClient<ICartService, CartService>();

            // Asignar la URL base de la API de cupones desde la configuración de la aplicación a la propiedad CouponAPIBase en SD
            SD.CouponAPIBase = builder.Configuration["ServiceUrls:CouponAPI"];

            SD.AuthAPIBase = builder.Configuration["ServiceUrls:AuthAPI"];

            SD.ProductAPIBase = builder.Configuration["ServiceUrls:ProductAPI"];

            SD.ShoppingCartAPIBase = builder.Configuration["ServiceUrls:ShoppingCartAPI"];

            // Agregar el servicio al contenedor de inyección de dependencias
            //creará una instancia única de BaseService para cada ámbito de solicitud HTTP.
            builder.Services.AddScoped<IBaseService, BaseService>();
            builder.Services.AddScoped<ICouponService, CouponService>();
            /*Estás registrando una implementación concreta para la interfaz IAuthService en el contenedor de inyección de dependencias. 
             * En este caso, cuando se solicite una instancia de IAuthService, el contenedor de inyección de dependencias
             * proporcionará una instancia de la clase AuthService.
            
            principio de inversión de control (IoC) y la inyección de dependencias. 
            Al registrar interfaces con sus implementaciones concretas, puedes cambiar fácilmente la implementación 
            subyacente simplemente modificando la configuración de la inyección de dependencias*/
            builder.Services.AddScoped<IAuthService, AuthService>();
            builder.Services.AddScoped<ITokenProvider, TokenProvider>();
            builder.Services.AddScoped<IProductService, ProductService>();
            builder.Services.AddScoped<ICartService, CartService>();

            //ACLARACION: al añadir un servicio al contenedor de inyección de dependencias,
            //estás diciendo a ASP.NET Core que se encargue de crear y
            //proporcionar instancias de ese servicio cuando sea necesario en diferentes partes de tu aplicación. 

            //Al agregar algo al contenedor de inyección de dependencias, básicamente estás configurando cómo se debe crear y
            //proporcionar una instancia de ese objeto cuando se necesite en tu aplicación.
            //Esto elimina la necesidad de instanciar manualmente el objeto en cada clase específica que lo utiliza.


            // Configuración de la autenticación usando el esquema de autenticación basado en cookies.
            builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
                .AddCookie(options =>
                {
                    // Ruta de inicio de sesión, a la que se redirige cuando un usuario no autenticado intenta acceder a una acción protegida.
                    options.LoginPath = "/Auth/Login";

                    // Ruta de acceso denegado, a la que se redirige cuando un usuario autenticado no tiene permisos suficientes para acceder a una acción.
                    options.AccessDeniedPath = "/Auth/AccessDenied";

                    // Tiempo de expiración de la cookie de autenticación después de la última interacción del usuario.
                    options.ExpireTimeSpan = TimeSpan.FromMinutes(10);
                });




            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthentication();// para habilitar la autenticacion a traves de las cookies
            app.UseAuthorization();

            //esta configuración establece la ruta predeterminada para que, si no se especifica ninguna ruta en la URL,
            //se use el controlador "Home" y la acción "Index" como valores predeterminados. 
            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");

            app.Run();
        }
    }
}
