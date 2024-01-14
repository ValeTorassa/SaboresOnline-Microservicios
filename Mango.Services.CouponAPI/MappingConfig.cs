
using AutoMapper;

namespace Mango.Services.CouponAPI
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
                // Se define un mapeo de la clase Coupon a la clase CouponDto
                config.CreateMap<Models.Coupon, Models.Dto.CouponDto>();

                // Se define un mapeo inverso de la clase CouponDto a la clase Coupon
                config.CreateMap<Models.Dto.CouponDto, Models.Coupon>();
            });

            // Se devuelve la configuración de mapeo para su uso posterior
            return mappingConfig;
        }
    }
}
