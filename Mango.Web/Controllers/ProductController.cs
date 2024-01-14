using Mango.Web.Models;
using Mango.Web.Service.IService;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace Mango.Web.Controllers
{
    public class ProductController : Controller
    {
        private readonly IProductService _productService;

        public ProductController(IProductService productService)
        {
            _productService = productService;
        }

        public async Task<ActionResult> ProductIndex()
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

        public async Task<ActionResult> ProductCreate()
        {
            return View();
        }

        [HttpPost]
        public async Task<ActionResult> ProductCreate(ProductDto model)
        {
            if (ModelState.IsValid)
            {
                ResponseDto? response = await _productService.CreateProductAsync(model);

                if (response != null && response.IsSuccess == true)
                {
                    TempData["Success"] = response?.Message; // Notificación de Toastr
                    return RedirectToAction(nameof(ProductIndex));
                }
                else
                {
                    TempData["Error"] = response?.Message; // Notificación de Toastr
                }
            }

            return View(model);
        }

        public async Task<ActionResult> ProductDelete(int id)
        {
            ResponseDto? response = await _productService.GetProductByIdAsync(id);

            if (response != null && response.IsSuccess == true)
            {
                string jsonProduct = Convert.ToString(response.Result);

                ProductDto? model = JsonConvert.DeserializeObject<ProductDto>(jsonProduct);

                TempData["Success"] = response?.Message; // Notificación de Toastr
                return View(model);
            }
            else
            {
                TempData["Error"] = response?.Message; // Notificación de Toastr
            }

            return NotFound();
        }

        [HttpPost]
        public async Task<ActionResult> ProductDelete(ProductDto model)
        {
            ResponseDto? response = await _productService.DeleteProductAsync(model.ProductId);

            if (response != null && response.IsSuccess == true)
            {
                TempData["Success"] = response?.Message; // Notificación de Toastr
                return RedirectToAction(nameof(ProductIndex));
            }
            else
            {
                TempData["Error"] = response?.Message; // Notificación de Toastr
            }

            return View(model);
        }

        public async Task<ActionResult> ProductEdit(int id)
        {
            ResponseDto? response = await _productService.GetProductByIdAsync(id);

            if (response != null && response.IsSuccess == true)
            {
                string jsonProduct = Convert.ToString(response.Result);

                ProductDto? model = JsonConvert.DeserializeObject<ProductDto>(jsonProduct);

                TempData["Success"] = response?.Message; // Notificación de Toastr
                return View(model);
            }
            else
            {
                TempData["Error"] = response?.Message; // Notificación de Toastr
            }

            return NotFound();
        }

        [HttpPost]
        public async Task<ActionResult> ProductEdit(ProductDto model)
        {
            if (ModelState.IsValid)
            {
                ResponseDto? response = await _productService.UpdateProductAsync(model);

                if (response != null && response.IsSuccess == true)
                {
                    TempData["Success"] = response?.Message; // Notificación de Toastr
                    return RedirectToAction(nameof(ProductIndex));
                }
                else
                {
                    TempData["Error"] = response?.Message; // Notificación de Toastr
                }
            }

            return View(model);
        }
    }
}
