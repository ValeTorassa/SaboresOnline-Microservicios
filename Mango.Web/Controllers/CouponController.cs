using Mango.Web.Models;
using Mango.Web.Service;
using Mango.Web.Service.IService;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace Mango.Web.Controllers
{
    public class CouponController : Controller
    {
        private readonly ICouponService _couponService;

        public CouponController(ICouponService couponService)
        {
            _couponService = couponService;
        }

        public async Task<ActionResult> CouponIndex() 
        {
            List<CouponDto>? coupons = new List<CouponDto>();

            ResponseDto? response = await _couponService.GetAllCouponsAsync();

            if (response != null && response.IsSuccess == true)
            {
                string JsonCoupons = Convert.ToString(response.Result);

                coupons = JsonConvert.DeserializeObject<List<CouponDto>>(JsonCoupons);
            }
            else
            {
                TempData["Error"] = response?.Message;//notificacion de toastr
            }

            return View(coupons);
			//Cuando creas una Razor Page(click derecho y agregar vista),
            //el sistema la asocia automáticamente a una acción del controlador según convenciones de nomenclatura.
            //Si creas una Razor Page desde el método CouponIndex en el CouponController,
            //la página se llamará CouponIndex.cshtml y estará vinculada automáticamente a ese método del controlador.
		}

        public async Task<ActionResult> CouponCreate()
        {
            return View();
        }

        [HttpPost]
        public async Task<ActionResult> CouponCreate(CouponDto model)
        {
            if (ModelState.IsValid)
            {
                ResponseDto? response = await _couponService.CreateCouponAsync(model);

                if (response != null && response.IsSuccess == true)
                {
                    TempData["Success"] = response?.Message; //notificacion de toastr
                    return RedirectToAction(nameof(CouponIndex));
                }
                else
                {
                    TempData["Error"] = response?.Message;//notificacion de toastr
                }
            }

            return View(model);
        }


        public async Task<ActionResult> CouponDelete(int id)
        {

			ResponseDto? response = await _couponService.GetCouponByIdAsync(id);

			if (response != null && response.IsSuccess == true)
			{
                string JsonCoupon = Convert.ToString(response.Result);

				CouponDto? model = JsonConvert.DeserializeObject<CouponDto>(JsonCoupon);

                TempData["Success"] = response?.Message;//notificacion de toastr
                return View(model);
			}
            else
            {
                TempData["Error"] = response?.Message;//notificacion de toastr
            }

            return NotFound();
		}

        [HttpPost]
        public async Task<ActionResult> CouponDelete(CouponDto model)
        {
            ResponseDto? response = await _couponService.DeleteCouponAsync(model.CouponId);

            if (response != null && response.IsSuccess == true)
            {
                TempData["Success"] = response?.Message;//notificacion de toastr
                return RedirectToAction(nameof(CouponIndex));
            }
            else
            {
                TempData["Error"] = response?.Message;//notificacion de toastr
            }

            return View(model);
        }

        public async Task<ActionResult> CouponEdit(int id)
        {
            ResponseDto? response = await _couponService.GetCouponByIdAsync(id);

            if (response != null && response.IsSuccess == true)
            {
                string JsonCoupon = Convert.ToString(response.Result);

                CouponDto? model = JsonConvert.DeserializeObject<CouponDto>(JsonCoupon);

                TempData["Success"] = response?.Message;//notificacion de toastr
                return View(model);
            }
            else
            {
                TempData["Error"] = response?.Message;//notificacion de toastr
            }

            return NotFound();
        }

        [HttpPost]
        public async Task<ActionResult> CouponEdit(CouponDto model)
        {
            if (ModelState.IsValid)
            {
                ResponseDto? response = await _couponService.UpdateCouponAsync(model);

                if (response != null && response.IsSuccess == true)
                {
                    TempData["Success"] = response?.Message;//notificacion de toastr
                    return RedirectToAction(nameof(CouponIndex));
                }
                else
                {
                    TempData["Error"] = response?.Message;//notificacion de toastr
                }
            }

            return View(model);
        }

    }
}
