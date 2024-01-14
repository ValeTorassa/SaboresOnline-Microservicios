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
            builder.Services.AddHttpContextAccessor(); // informaci�n del contexto HTTP, como la solicitud actual y la autenticaci�n del usuario.
            builder.Services.AddHttpClient();// realizar solicitudes HTTP desde la aplicaci�n

            // Agregar el servicio HttpClient con interfaz espec�fica y su implementaci�n al contenedor de inyecci�n de dependencias.
            builder.Services.AddHttpClient<ICouponService, CouponService>();
            builder.Services.AddHttpClient<IAuthService, AuthService>();
            builder.Services.AddHttpClient<IProductService, ProductService>();
            builder.Services.AddHttpClient<ICartService, CartService>();

            // Asignar la URL base de la API de cupones desde la configuraci�n de la aplicaci�n a la propiedad CouponAPIBase en SD
            SD.CouponAPIBase = builder.Configuration["ServiceUrls:CouponAPI"];

            SD.AuthAPIBase = builder.Configuration["ServiceUrls:AuthAPI"];

            SD.ProductAPIBase = builder.Configuration["ServiceUrls:ProductAPI"];

            SD.ShoppingCartAPIBase = builder.Configuration["ServiceUrls:ShoppingCartAPI"];

            // Agregar el servicio al contenedor de inyecci�n de dependencias
            //crear� una instancia �nica de BaseService para cada �mbito de solicitud HTTP.
            builder.Services.AddScoped<IBaseService, BaseService>();
            builder.Services.AddScoped<ICouponService, CouponService>();
            /*Est�s registrando una implementaci�n concreta para la interfaz IAuthService en el contenedor de inyecci�n de dependencias. 
             * En este caso, cuando se solicite una instancia de IAuthService, el contenedor de inyecci�n de dependencias
             * proporcionar� una instancia de la clase AuthService.
            
            principio de inversi�n de control (IoC) y la inyecci�n de dependencias. 
            Al registrar interfaces con sus implementaciones concretas, puedes cambiar f�cilmente la implementaci�n 
            subyacente simplemente modificando la configuraci�n de la inyecci�n de dependencias*/
            builder.Services.AddScoped<IAuthService, AuthService>();
            builder.Services.AddScoped<ITokenProvider, TokenProvider>();
            builder.Services.AddScoped<IProductService, ProductService>();
            builder.Services.AddScoped<ICartService, CartService>();

            //ACLARACION: al a�adir un servicio al contenedor de inyecci�n de dependencias,
            //est�s diciendo a ASP.NET Core que se encargue de crear y
            //proporcionar instancias de ese servicio cuando sea necesario en diferentes partes de tu aplicaci�n. 

            //Al agregar algo al contenedor de inyecci�n de dependencias, b�sicamente est�s configurando c�mo se debe crear y
            //proporcionar una instancia de ese objeto cuando se necesite en tu aplicaci�n.
            //Esto elimina la necesidad de instanciar manualmente el objeto en cada clase espec�fica que lo utiliza.


            // Configuraci�n de la autenticaci�n usando el esquema de autenticaci�n basado en cookies.
            builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
                .AddCookie(options =>
                {
                    // Ruta de inicio de sesi�n, a la que se redirige cuando un usuario no autenticado intenta acceder a una acci�n protegida.
                    options.LoginPath = "/Auth/Login";

                    // Ruta de acceso denegado, a la que se redirige cuando un usuario autenticado no tiene permisos suficientes para acceder a una acci�n.
                    options.AccessDeniedPath = "/Auth/AccessDenied";

                    // Tiempo de expiraci�n de la cookie de autenticaci�n despu�s de la �ltima interacci�n del usuario.
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

            //esta configuraci�n establece la ruta predeterminada para que, si no se especifica ninguna ruta en la URL,
            //se use el controlador "Home" y la acci�n "Index" como valores predeterminados. 
            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");

            app.Run();
        }
    }
}
