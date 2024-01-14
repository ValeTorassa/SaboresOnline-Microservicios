using AutoMapper;
using Mango.MessageBus;
using Mango.Services.ShoppingCartAPI.Data;
using Mango.Services.ShoppingCartAPI.Models;
using Mango.Services.ShoppingCartAPI.Models.Dto;
using Mango.Services.ShoppingCartAPI.Service.IService;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Mango.Services.ShoppingCartAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CartAPIController : Controller
    {
        private readonly AppDbContext _db;
        private readonly ResponseDto _response;
        private readonly IMapper _mapper;
        private readonly IProductService _productService;
        private readonly ICouponService _couponService;
        private readonly IMessageBus _messageBus;
        private readonly IConfiguration _configuration;

        public CartAPIController(AppDbContext db, IMapper mapper, IProductService productService, ICouponService couponService, IMessageBus messageBus, IConfiguration configuration)
        {
            _db = db;
            _response = new ResponseDto();
            _mapper = mapper;
            _productService = productService;
            _couponService = couponService;
            _messageBus = messageBus;
            _configuration = configuration;
        }

        [HttpGet("GetCart/{userId}")]
        public async Task<ResponseDto> GetCart(string userId)
        {
            try
            {
                CartDto cartDto = new CartDto()
                {
                    CartHeader = _mapper.Map<CartHeaderDto>(await _db.CartHeaders.FirstOrDefaultAsync(u => u.UserId == userId))
                };

                cartDto.CartDetails = _mapper.Map<IEnumerable<CartDetailsDto>>(_db.CartDetails.Where
                                                                            (u => u.CartHeaderId == cartDto.CartHeader.CartHeaderId));

                IEnumerable<ProductDto> products = await _productService.GetProducts();

                foreach (var item in cartDto.CartDetails)
                {
                    item.Product = products.FirstOrDefault(u => u.ProductId == item.ProductId);
                    cartDto.CartHeader.CartTotal += (item.Product.Price * item.Count);
                }


                if (!string.IsNullOrEmpty(cartDto.CartHeader.CouponCode))
                {
                    CouponDto coupon = await _couponService.GetCoupon(cartDto.CartHeader.CouponCode);
                    if(coupon != null && cartDto.CartHeader.CartTotal > coupon.MinAmount)
                    {
                        cartDto.CartHeader.CartTotal = cartDto.CartHeader.CartTotal - coupon.DiscountAmount;
                        cartDto.CartHeader.Discount = coupon.DiscountAmount;
                    }
                }


                _response.Result = cartDto;
            }
            catch (Exception ex)
            {
                _response.Message = ex.Message.ToString();
                _response.IsSuccess = false;
            }
            return _response;
        }



        [HttpPost("CartUpsert")]
        public async Task<ResponseDto> CartUpsert(CartDto cartDto)
        {
            try
            {
                // Buscar el encabezado del carrito en la base de datos basado en el ID del usuario.
                var cartHeaderFromDb = await _db.CartHeaders.AsNoTracking().FirstOrDefaultAsync(u => u.UserId == cartDto.CartHeader.UserId);

                if (cartHeaderFromDb == null)
                {
                    // Mapear el objeto DTO del encabezado del carrito a la entidad del modelo de datos usando AutoMapper
                    CartHeader cartHeader = _mapper.Map<CartHeader>(cartDto.CartHeader);
                    _db.CartHeaders.Add(cartHeader);
                    await _db.SaveChangesAsync();

                    // Asignar el ID del encabezado del carrito recién creado al primer elemento en la colección de detalles del carrito en el DTO
                    //esto se hace solo con el primer elemento ya que solo se puede añadir de a un producto
                    cartDto.CartDetails.First().CartHeaderId = cartHeader.CartHeaderId;
                    _db.CartDetails.Add(_mapper.Map<CartDetails>(cartDto.CartDetails.First()));
                    await _db.SaveChangesAsync();

                }
                else
                {
                    // Suponiendo que se asume agregar un solo producto a la vez al carrito.

                    // Extraer el ProductId del primer elemento en la colección cartDto.CartDetails.
                    var productIdToAdd = cartDto.CartDetails.First().ProductId;

                    // Buscar detalles del carrito en la base de datos basados en ProductId y CartHeaderId.
                    var cartDetailsFromDb = await _db.CartDetails.AsNoTracking().FirstOrDefaultAsync(u =>
                        u.ProductId == productIdToAdd &&
                        u.CartHeaderId == cartHeaderFromDb.CartHeaderId);


                    if (cartDetailsFromDb == null)
                    {
                        //crear detalle añadiendo el id del encabezado del carrito
                        cartDto.CartDetails.First().CartHeaderId = cartHeaderFromDb.CartHeaderId;
                        _db.CartDetails.Add(_mapper.Map<CartDetails>(cartDto.CartDetails.First()));
                        await _db.SaveChangesAsync();
                    }
                    else
                    {
                        //actualizar cantidad en el detalles ya que el detalle sober ese producto ya existe
                        cartDto.CartDetails.First().Count += cartDetailsFromDb.Count;

                        // Asignar el ID del encabezado 
                        cartDto.CartDetails.First().CartHeaderId = cartHeaderFromDb.CartHeaderId;

                        // Asignar el ID del detalle
                        cartDto.CartDetails.First().CartDetailsId = cartDetailsFromDb.CartDetailsId;

                        _db.CartDetails.Update(_mapper.Map<CartDetails>(cartDto.CartDetails.First()));
                        await _db.SaveChangesAsync();
                    }
                }
                _response.Result = cartDto;
            }
            catch (Exception ex)
            {
                _response.Message = ex.Message.ToString();
                _response.IsSuccess = false;
            }
            return _response;
        }
        //as no tracking
        /*Esto puede ser beneficioso en escenarios donde solo se están leyendo datos y no se planea actualizar ni guardar cambios en esas entidades.
         y puede evitar errores como que no se pueda actualizar una entidad:

        Si se recupera una entidad del contexto y luego se intenta actualizarla fuera del contexto (DbContext), 
        podría generar un error si el objeto está en el estado EntityState.Modified. Al usar AsNoTracking,
        se evita que las entidades estén en ese estado, y se pueden realizar cambios sin preocuparse por el estado de seguimiento.
         */




        [HttpPost("RemoveCart")]
        public async Task<ResponseDto> RemoveCart([FromBody] int cartDetailsId)
        {
            try
            {
                CartDetails cartDetails = await _db.CartDetails.FirstOrDefaultAsync(u => u.CartDetailsId == cartDetailsId);

                int totalCountOfCartItems = _db.CartDetails.Where(u => u.CartHeaderId == cartDetails.CartHeaderId).Count();
                _db.CartDetails.Remove(cartDetails);

                if (totalCountOfCartItems == 1)
                {
                    var cartHeaderToRemove = await _db.CartHeaders.FirstOrDefaultAsync(u => u.CartHeaderId == cartDetails.CartHeaderId);

                    _db.CartHeaders.Remove(cartHeaderToRemove);
                }

                await _db.SaveChangesAsync();

                _response.Result = cartDetails;
            }
            catch (Exception ex)
            {
                _response.Message = ex.Message.ToString();
                _response.IsSuccess = false;
            }
            return _response;
        }


        [HttpPost("ApplyCoupon")]
        public async Task<ResponseDto> ApplyCoupon([FromBody] CartDto cartDto)
        {
            try
            {
                // Buscar el encabezado del carrito en la base de datos basado en el ID del usuario.
                var cartHeaderFromDb = await _db.CartHeaders.FirstAsync(u => u.UserId == cartDto.CartHeader.UserId);
                //insertamos el codigo del cupon en el encabezado del carrito
                cartHeaderFromDb.CouponCode = cartDto.CartHeader.CouponCode;

                _db.CartHeaders.Update(cartHeaderFromDb);
                await _db.SaveChangesAsync();

                _response.Result = true;
            }
            catch (Exception ex)
            {
                _response.Message = ex.Message.ToString();
                _response.IsSuccess = false;
            }
            return _response;
        }


        [HttpPost("RemoveCoupon")]
        public async Task<ResponseDto> RemoveCoupon([FromBody] CartDto cartDto)
        {
            try
            {
                // Buscar el encabezado del carrito en la base de datos basado en el ID del usuario.
                var cartHeaderFromDb = await _db.CartHeaders.FirstAsync(u => u.UserId == cartDto.CartHeader.UserId);
                //insertamos el codigo del cupon en el encabezado del carrito
                cartHeaderFromDb.CouponCode = "";

                _db.CartHeaders.Update(cartHeaderFromDb);
                await _db.SaveChangesAsync();

                _response.Result = true;
            }
            catch (Exception ex)
            {
                _response.Message = ex.Message.ToString();
                _response.IsSuccess = false;
            }
            return _response;
        }


        [HttpPost("EmailCartRequest")]
        public async Task<ResponseDto> EmailCartRequest([FromBody] CartDto cartDto)
        {
            try
            {
                await _messageBus.PublishMessage(cartDto, _configuration.GetValue<string>("QueueNames:EmailShoppingCart"));

                _response.Result = true;
            }
            catch (Exception ex)
            {
                _response.Message = ex.Message.ToString();
                _response.IsSuccess = false;
            }
            return _response;
        }
    }
}


