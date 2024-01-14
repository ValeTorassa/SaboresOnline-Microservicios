using Mango.Web.Models;
using Mango.Web.Service.IService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.IdentityModel.Tokens.Jwt;

namespace Mango.Web.Controllers
{
    public class CartController : Controller
    {
        private readonly ICartService _cartService;

        public CartController(ICartService cartService)
        {
            this._cartService = cartService;
        }

        [Authorize]
        public async Task<IActionResult> CartIndex()
        {
            return View(await LoadCartDtoBasedOnLoggedInUser());
        }

        public async Task<IActionResult> Remove(int cartDetailId)
        {
            //el método sabe de qué carrito tiene que borrar un detalle porque se basa en el identificador único (cartDetailsId)
            //proporcionado como parámetro, el cual se utiliza para identificar el detalle de carrito específico 
            var response = await _cartService.RemoveFromCartAsync(cartDetailId);

            if (response != null && response.IsSuccess)
            {
                TempData["Success"] = "Item removed from cart successfully";
                return RedirectToAction(nameof(CartIndex));
            }
            return View();
        }


        private async Task<CartDto> LoadCartDtoBasedOnLoggedInUser()
        {
            //var userId = User.Claims.Where(u => u.Type == "sub").FirstOrDefault().Value;
            var userId = User.Claims.Where(u => u.Type == JwtRegisteredClaimNames.Sub)?.FirstOrDefault()?.Value;

            var response = await _cartService.GetCartByUserIdAsync(userId);

            if (response != null && response.IsSuccess)
            {
                CartDto cartDto = JsonConvert.DeserializeObject<CartDto>(Convert.ToString(response.Result));

                return cartDto;
            }
            return new CartDto();
        }

        [HttpPost]
        public async Task<IActionResult> ApplyCoupon(CartDto cartDto)
        {

            var response = await _cartService.ApplyCouponAsync(cartDto);

            if (response != null && response.IsSuccess)
            {
                TempData["Success"] = "Coupon Added Successfully";
                return RedirectToAction(nameof(CartIndex));
            }
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> RemoveCoupon(CartDto cartDto)
        {
            cartDto.CartHeader.CouponCode = "";

            var response = await _cartService.ApplyCouponAsync(cartDto);

            if (response != null && response.IsSuccess)
            {
                TempData["Success"] = "Coupon Removed Successfully";
                return RedirectToAction(nameof(CartIndex));
            }
            return View();
        }


        [HttpPost]
        public async Task<IActionResult> EmailCart(CartDto cartDto)
        {
            CartDto cart = await LoadCartDtoBasedOnLoggedInUser();

            cart.CartHeader.Email = User.Claims.Where(u => u.Type == JwtRegisteredClaimNames.Email)?.FirstOrDefault()?.Value;

            var response = await _cartService.EmailCart(cart);

            if (response != null && response.IsSuccess)
            {
                TempData["Success"] = "Email will be processed and sent shortly";
                return RedirectToAction(nameof(CartIndex));
            }
            return View();
        }

    }
}
