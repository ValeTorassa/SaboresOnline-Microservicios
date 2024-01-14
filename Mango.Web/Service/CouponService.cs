using Mango.Web.Models;
using Mango.Web.Service.IService;
using Mango.Web.Utility;

namespace Mango.Web.Service
{
    public class CouponService : ICouponService
    {
        // Campo privado que almacena una instancia de IBaseService (interfaz para operaciones comunes)
        private readonly IBaseService _baseService;

        public CouponService(IBaseService baseService)
        {
            _baseService = baseService;
        }
        //se ha diseñado para depender de un servicio más genérico (IBaseService)
        //La inyección de dependencias se utiliza para proporcionar la implementación concreta de IBaseService

        public async Task<ResponseDto?> CreateCouponAsync(CouponDto couponDto)
        {
            return await _baseService.SendAsync(new RequestDto()
            {
                ApiType = SD.ApiType.POST,
                Url = SD.CouponAPIBase + "/api/couponAPI",
                Data = couponDto
            });
        }

        public async Task<ResponseDto?> DeleteCouponAsync(int id)
        {
            return await _baseService.SendAsync(new RequestDto()
            {
                ApiType = SD.ApiType.DELETE,
                Url = SD.CouponAPIBase + "/api/couponAPI/" + id,
            });
        }

        public async Task<ResponseDto?> GetCouponByIdAsync(int id)
        {
            return await _baseService.SendAsync(new RequestDto()
            {
                ApiType = SD.ApiType.GET,
                Url = SD.CouponAPIBase + "/api/couponAPI/" + id,
            });
        }

        public async Task<ResponseDto?> GetAllCouponsAsync()
        {
            return await _baseService.SendAsync(new RequestDto()
            {
                ApiType = SD.ApiType.GET,
                Url = SD.CouponAPIBase + "/api/couponAPI",
            });
        }

        public async Task<ResponseDto?> GetCouponAsync(string couponCode)
        {
            return await _baseService.SendAsync(new RequestDto()
            {
                ApiType = SD.ApiType.GET,
                Url = SD.CouponAPIBase + "/api/couponAPI/GetByCode/" + couponCode,
            });
        }

        public async Task<ResponseDto?> UpdateCouponAsync(CouponDto couponDto)
        {
            return await _baseService.SendAsync(new RequestDto()
            {
                ApiType = SD.ApiType.PUT,
                Url = SD.CouponAPIBase + "/api/couponAPI",
                Data = couponDto
            });
        }
    }
}
