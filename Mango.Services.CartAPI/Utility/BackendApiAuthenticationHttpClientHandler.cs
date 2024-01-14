using Microsoft.AspNetCore.Authentication;
using System.Net.Http.Headers;

namespace Mango.Services.ShoppingCartAPI.Utility
{
    public class BackendApiAuthenticationHttpClientHandler : DelegatingHandler
    {
        /*Un DelegatingHandler en ASP.NET Web API es un filtro que puedes usar para manipular solicitudes y respuestas HTTP 
         * antes de que lleguen a su destino o después de que salgan. Es un middleware
         * Un middleware proporcionando funcionalidades adicionales entre el cliente y el servidor, la diferencia
         * es que los delegating handler estan del lado del cliente*/

        private readonly IHttpContextAccessor _accesor;

        // Constructor que recibe el HttpContextAccessor para acceder al contexto HTTP actual
        public BackendApiAuthenticationHttpClientHandler(IHttpContextAccessor accesor)
        {
            _accesor = accesor;
        }

        // Sobrescribe el método SendAsync de DelegatingHandler
        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            // Obtiene el token de acceso almacenado en las cookies de autenticación
            var accessToken = await _accesor.HttpContext.GetTokenAsync("access_token");

            // Verifica si el token de acceso no es nulo o vacío
            if (!string.IsNullOrEmpty(accessToken))
            {
                // Agrega el token de acceso al encabezado de autorización de la solicitud HTTP
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
            }

            // Llama al siguiente manipulador en la cadena de manipuladores para continuar con el procesamiento de la solicitud
            return await base.SendAsync(request, cancellationToken);
        }

        /*
         *  la clase es un manipulador de HttpClient que se utiliza para interceptar las solicitudes HTTP salientes y 
         *  agregar el token de acceso JWT al encabezado de autorización, 
         *  permitiendo así la autenticación con servicios de backend que requieren autenticación basada en token. 
         *  
         *  Este manipulador facilita la inclusión automática del token de acceso en cada solicitud HTTP saliente.
         */
    }
}
