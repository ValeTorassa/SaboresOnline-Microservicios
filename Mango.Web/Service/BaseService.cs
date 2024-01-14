using Mango.Web.Models;
using Mango.Web.Service.IService;
using Newtonsoft.Json;
using System.Net.Http;
using System.Text;
using static Mango.Web.Utility.SD;

namespace Mango.Web.Service
{
    public class BaseService : IBaseService
    {
        // IHttpClientFactory se utiliza para crear instancias de HttpClient de manera eficiente
        private readonly IHttpClientFactory _clientFactory;

        private readonly ITokenProvider _tokenProvider;

        public BaseService(IHttpClientFactory httpClientFactory, ITokenProvider tokenProvider)
        {
            _clientFactory = httpClientFactory;
            _tokenProvider = tokenProvider;
        }


        public async Task<ResponseDto?> SendAsync(RequestDto requestDto, bool withBearer = true)
        {
            try
            {
                // Se utiliza IHttpClientFactory para crear una instancia de HttpClient llamada "MangoAPI".
                HttpClient client = _clientFactory.CreateClient("MangoAPI");

                // Se crea un mensaje de solicitud HTTP.
                HttpRequestMessage message = new HttpRequestMessage();


                //la inclusión del encabezado "Accept: application/json" es una buena práctica en el cliente para indicar sus
                //preferencias de formato de respuesta, pero la respuesta real dependerá de cómo esté implementada la lógica del servidor en la API.
                message.Headers.Add("Accept", "application/json");
               
                //token
                if (withBearer)
                {
                    // Se obtiene el token de la sesión actual.
                    var accessToken = _tokenProvider.GetToken();
                    // Se agrega el token de acceso al encabezado de autorización de la solicitud HTTP.
                    message.Headers.Add("Authorization", $"Bearer {accessToken}");
                }

                message.RequestUri = new Uri(requestDto.Url);

                // Si hay datos en la solicitud, se serializan a formato JSON y se establecen como contenido de la solicitud.
                if (requestDto.Data != null)
                {
                    message.Content = new StringContent(JsonConvert.SerializeObject(requestDto.Data), Encoding.UTF8, "application/json");
                }

                // Se establece el método HTTP de la solicitud.
                HttpResponseMessage? apiResponse = null;
                //declara una variable que puede contener la respuesta de una solicitud HTTP (si se asigna más adelante),
                //pero inicialmente se establece como nula


                // Asignar el método HTTP correspondiente al objeto HttpRequestMessage
                switch (requestDto.ApiType)
                {
                    case ApiType.POST:
                        message.Method = HttpMethod.Post;
                        break;
                    case ApiType.PUT:
                        message.Method = HttpMethod.Put;
                        break;
                    case ApiType.DELETE:
                        message.Method = HttpMethod.Delete;
                        break;
                    default:
                        message.Method = HttpMethod.Get;
                        break;
                }

                // Se envía la solicitud HTTP y se almacena la respuesta en la variable apiResponse.
                apiResponse = await client.SendAsync(message);

                // Se comprueba el código de estado de la respuesta HTTP.
                switch (apiResponse.StatusCode)
                {
                    case System.Net.HttpStatusCode.NotFound:
                        return new ResponseDto { IsSuccess = false, Message = "Not Found" };
                    case System.Net.HttpStatusCode.Unauthorized:
                        return new ResponseDto { IsSuccess = false, Message = "Unauthorized" };
                    case System.Net.HttpStatusCode.Forbidden:
                        return new ResponseDto { IsSuccess = false, Message = "Access Denied" };
                    case System.Net.HttpStatusCode.InternalServerError:
                        return new ResponseDto { IsSuccess = false, Message = "Internal Server Error" };
                    default:
                        // Se lee el contenido de la respuesta HTTP y se deserializa a un objeto ResponseDto.
                        var apiContent = await apiResponse.Content.ReadAsStringAsync();
                        var apiResponseDto = JsonConvert.DeserializeObject<ResponseDto>(apiContent);
                        return apiResponseDto;
                }
            }
            catch (Exception ex)
            {
                var dto = new ResponseDto
                {
                    IsSuccess = false,
                    Message = ex.ToString()
                };
                return dto;
            }
           
        }
    }
}
