using Mango.Services.ProductAPI.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Mango.Services.ProductAPI.Models.Dto;
using Mango.Services.ProductAPI.Data;

namespace Mango.Services.ProductAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductAPIController : Controller
    {
        private readonly AppDbContext _db;
        private ResponseDto _response;

        public ProductAPIController(AppDbContext db)
        {
            _db = db;
            _response = new ResponseDto();
        }

        [HttpGet]
        public ResponseDto Get()
        {
            try
            {
                IEnumerable<Product> products = _db.Products.ToList();
                // Realizar mapeo manual de Product a ProductDto
                _response.Result = products.Select(p => new ProductDto
                {
                    ProductId = p.ProductId,
                    Name = p.Name,
                    Price = p.Price,
                    Description = p.Description,
                    CategoryName = p.CategoryName,
                    ImageUrl = p.ImageUrl
                });
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.Message = ex.Message;
            }
            return _response;
        }

        [HttpGet("{id}")]
        public ResponseDto Get(int id)
        {
            try
            {
                var product = _db.Products.FirstOrDefault(p => p.ProductId == id);
                if (product == null)
                {
                    _response.IsSuccess = false;
                    _response.Message = "Product not found";
                }
                else
                {
                    // Realizar mapeo manual de Product a ProductDto
                    _response.Result = new ProductDto
                    {
                        ProductId = product.ProductId,
                        Name = product.Name,
                        Price = product.Price,
                        Description = product.Description,
                        CategoryName = product.CategoryName,
                        ImageUrl = product.ImageUrl
                    };
                }
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.Message = ex.Message;
            }
            return _response;
        }

        [HttpGet("GetByName/{name}")]
        public ResponseDto GetByName(string name)
        {
            try
            {
                var product = _db.Products.First(u => u.Name == name);
                if (product == null)
                {
                    _response.IsSuccess = false;
                    _response.Message = "Coupon not found";
                }
                _response.Result = new ProductDto
                {
                    ProductId = product.ProductId,
                    Name = product.Name,
                    Price = product.Price,
                    Description = product.Description,
                    CategoryName = product.CategoryName,
                    ImageUrl = product.ImageUrl
                };
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.Message = ex.Message;
            }
            return _response;
        }

        [HttpPost]
        [Authorize(Roles = "ADMIN")]
        public ResponseDto Post([FromBody] ProductDto productDto)
        {
            try
            {
                // Realizar mapeo manual de ProductDto a Product
                Product product = new Product
                {
                    Name = productDto.Name,
                    Price = productDto.Price,
                    Description = productDto.Description,
                    CategoryName = productDto.CategoryName,
                    ImageUrl = productDto.ImageUrl
                };

                _db.Products.Add(product);
                _db.SaveChanges();

                // Realizar mapeo manual de Product a ProductDto
                _response.Result = new ProductDto
                {
                    ProductId = product.ProductId,
                    Name = product.Name,
                    Price = product.Price,
                    Description = product.Description,
                    CategoryName = product.CategoryName,
                    ImageUrl = product.ImageUrl
                };
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.Message = ex.Message;
            }
            return _response;
        }

        [HttpPut]
        [Authorize(Roles = "ADMIN")]
        public ResponseDto Put([FromBody] ProductDto productDto)
        {
            try
            {
                // Realizar mapeo manual de ProductDto a Product
                Product product = new Product
                {
                    ProductId = productDto.ProductId,
                    Name = productDto.Name,
                    Price = productDto.Price,
                    Description = productDto.Description,
                    CategoryName = productDto.CategoryName,
                    ImageUrl = productDto.ImageUrl
                };

                _db.Products.Update(product);
                _db.SaveChanges();

                // Realizar mapeo manual de Product a ProductDto
                _response.Result = new ProductDto
                {
                    ProductId = product.ProductId,
                    Name = product.Name,
                    Price = product.Price,
                    Description = product.Description,
                    CategoryName = product.CategoryName,
                    ImageUrl = product.ImageUrl
                };
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.Message = ex.Message;
            }
            return _response;
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "ADMIN")]
        public ResponseDto Delete(int id)
        {
            try
            {
                Product product = _db.Products.FirstOrDefault(p => p.ProductId == id);
                if (product != null)
                {
                    _db.Products.Remove(product);
                    _db.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.Message = ex.Message;
            }
            return _response;
        }
    }
}
