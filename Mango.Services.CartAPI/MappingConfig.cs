
using AutoMapper;
using Mango.Services.ShoppingCartAPI.Models;
using Mango.Services.ShoppingCartAPI.Models.Dto;

namespace Mango.Services.ShoppinCartAPI
{
    // Clase encargada de configurar los mapeos entre las clases Coupon y CouponDto
    //El mapeo automático es el proceso de asignar automáticamente los valores de propiedades de un objeto a otro
    public class MappingConfig
    {
        // Método estático que registra y configura los mapeos utilizando AutoMapper
        public static MapperConfiguration RegisterMaps()
        {
            // Se crea una nueva configuración de AutoMapper
            var mappingConfig = new MapperConfiguration(config =>
            {
                config.CreateMap<CartHeader,CartHeaderDto>();
                config.CreateMap<CartHeaderDto, CartHeader>();

                config.CreateMap<CartDetails, CartDetailsDto>();
                config.CreateMap<CartDetailsDto, CartDetails>();
            });

            // Se devuelve la configuración de mapeo para su uso posterior
            return mappingConfig;
        }
    }
}
