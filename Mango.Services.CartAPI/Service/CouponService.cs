using Mango.Services.ShoppingCartAPI.Models.Dto;
using Mango.Services.ShoppingCartAPI.Service.IService;
using Newtonsoft.Json;

namespace Mango.Services.ShoppingCartAPI.Service
{
    public class CouponService : ICouponService
    {
        private readonly IHttpClientFactory _clientFactory;

        public CouponService(IHttpClientFactory clientFactory)
        {
            _clientFactory = clientFactory;
        }

        public async Task<CouponDto> GetCoupon(string couponCode)
        {
            var client = _clientFactory.CreateClient("Coupon");

            var response = await client.GetAsync($"/api/CouponAPI/GetByCode/" + couponCode);
            var content = await response.Content.ReadAsStringAsync();
            var resp = JsonConvert.DeserializeObject<ResponseDto>(content);

            if (resp!=null && resp.IsSuccess)
            {
                return JsonConvert.DeserializeObject<CouponDto>(Convert.ToString(resp.Result));
            }

            return new CouponDto();
        }
    }
}
