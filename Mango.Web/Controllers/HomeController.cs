using IdentityModel;
using Mango.Web.Models;
using Mango.Web.Service.IService;
using Mango.Web.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Diagnostics;

namespace Mango.Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly IProductService _productService;
        private readonly ICartService _cartService;

        public HomeController(IProductService productService, ICartService cartService)
        {
            _productService = productService;
            _cartService = cartService;
        }


        public async Task<IActionResult> Index()
        {
            List<ProductDto>? products = new List<ProductDto>();

            ResponseDto? response = await _productService.GetAllProductsAsync();

            if (response != null && response.IsSuccess == true)
            {
                string jsonProducts = Convert.ToString(response.Result);

                products = JsonConvert.DeserializeObject<List<ProductDto>>(jsonProducts);
            }
            else
            {
                TempData["Error"] = response?.Message; // Notificación de Toastr
            }

            return View(products);
        }



        [Authorize]
        public async Task<IActionResult> ProductDetails(int productID)
        {
            ProductDto? product = new ProductDto();

            ResponseDto? response = await _productService.GetProductByIdAsync(productID);

            if (response != null && response.IsSuccess == true)
            {
                string jsonProducts = Convert.ToString(response.Result);

                product = JsonConvert.DeserializeObject<ProductDto>(jsonProducts);
            }
            else
            {
                TempData["Error"] = response?.Message; // Notificación de Toastr
            }

            return View(product);
        }



        [Authorize]
        [HttpPost]
        [ActionName("ProductDetails")]
        public async Task<IActionResult> ProductDetails(ProductDto productDto)
        {
            CartDto cartDto = new CartDto()
            {
                CartHeader = new CartHeaderDto()
                {
                    UserId = User.Claims.Where(u => u.Type == JwtClaimTypes.Subject)?.FirstOrDefault()?.Value
                }
            };

            CartDetailsDto cartDetails = new CartDetailsDto()
            {
                ProductId = productDto.ProductId,
                Count = productDto.Count
            };

            List<CartDetailsDto> cartDetailsList = new List<CartDetailsDto>();
            cartDetailsList.Add(cartDetails);
            cartDto.CartDetails = cartDetailsList;

            ResponseDto? response = await _cartService.UpsertCartAsync(cartDto);

            if (response != null && response.IsSuccess == true)
            {
                TempData["Success"] = "Item has been added to the Shopping Cart";
                return RedirectToAction(nameof(Index));
            }
            else
            {
                TempData["Error"] = response?.Message;
            }

            return View(productDto);
        }


        [Authorize(Roles = SD.RoleAdmin)]
        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
