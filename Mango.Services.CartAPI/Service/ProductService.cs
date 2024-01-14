using Mango.Services.ShoppingCartAPI.Models.Dto;
using Mango.Services.ShoppingCartAPI.Service.IService;
using Newtonsoft.Json;
using System.Reflection.Metadata.Ecma335;

namespace Mango.Services.ShoppingCartAPI.Service
{
    public class ProductService : IProductService
    {
        private readonly IHttpClientFactory _clientFactory;

        public ProductService(IHttpClientFactory clientFactory)
        {
            _clientFactory = clientFactory;
        }

        public async Task<IEnumerable<ProductDto>> GetProducts()
        {
            var client = _clientFactory.CreateClient("Product");
            /*Cuando se utiliza la fábrica de clientes HTTP (_clientFactory), 
             * el nombre proporcionado ("Product" en este caso) se asocia con la configuración 
             * que se haya establecido previamente mediante el método AddHttpClient en el método ConfigureServices
             * y de ahi obtiene la direccion de la API*/

            var response = await client.GetAsync("/api/productAPI");
            var content = await response.Content.ReadAsStringAsync();
            var resp = JsonConvert.DeserializeObject<ResponseDto>(content);

            if (resp.IsSuccess)
            {
                return JsonConvert.DeserializeObject<IEnumerable<ProductDto>>(Convert.ToString(resp.Result));
            }

            return new List<ProductDto>();
        }
    }
}
