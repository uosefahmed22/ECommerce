using ECommerce.Core.DTOs;
using ECommerce.Core.Errors;
using ECommerce.Core.IRepositories;
using ECommerce.Core.Models.Auth;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace ECommerce.Apis.Controllers
{
    public class ProductController : BaseApiController
    {
        private readonly IProductRepository _productRepository;
        private readonly UserManager<AppUser> _userManager;

        public ProductController(IProductRepository productRepository,UserManager<AppUser> userManager)
        {
            _productRepository = productRepository;
            _userManager = userManager;
        }
        [HttpGet("getproducts")]
        public async Task<IActionResult> GetProductsAsync(int pageIndex, int pageSize)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var result = await _productRepository.GetProducts(pageIndex, pageSize);
            return Ok(result);
        }
        [HttpGet("getproduct")] 
        public async Task<IActionResult> GetProductAsync(int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var result = await _productRepository.GetProduct(id);
            if (result.StatusCode == 400)
            {
                return BadRequest(result);
            }
            return Ok(result);
        }
        [HttpPost("createproduct")]
        public async Task<IActionResult> CreateProductAsync([FromForm] ProductDto productDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var result = await _productRepository.CreateProduct(productDto);
            if (result.StatusCode == 400)
            {
                return BadRequest(result);
            }
            return Ok(result);
        }
        [HttpPut("updateproduct")]
        public async Task<IActionResult> UpdateProductAsync(int productId, [FromForm] ProductDto productDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var result = await _productRepository.UpdateProduct(productId, productDto);
            if (result.StatusCode == 400)
            {
                return BadRequest(result);
            }
            return Ok(result);
        }
        [HttpDelete("deleteproduct")]
        public async Task<IActionResult> DeleteProductAsync(int productId)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var result = await _productRepository.DeleteProduct(productId);
            if (result.StatusCode == 400)
            {
                return BadRequest(result);
            }
            return Ok(result);
        }

        [HttpGet("getbestsellingproducts")]
        public async Task<IActionResult> GetBestSellingProductsAsync()
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var result = await _productRepository.GetBestSellingProducts();
            return Ok(result);
        }

        [HttpGet("getnewproducts")]
        public async Task<IActionResult> GetNewProductsAsync()
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var result = await _productRepository.GetNewProducts();
            return Ok(result);
        }

        [HttpPost("rateproduct")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "User")]
        public async Task<IActionResult> RateProductAsync(int productId, int ratingValue)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var email = User.FindFirst(ClaimTypes.Email)?.Value;
            if (email == null)
            {
                return BadRequest(new ApiResponse(400, "Invalid user"));
            }
            var user = await _userManager.FindByEmailAsync(email);

            var ratingDto = new RatingDto
            {
                UserId = user.Id,
                ProductId = productId,
                RatingValue = ratingValue
            };
            var result = await _productRepository.RateProduct(ratingDto);
            if (result.StatusCode == 400)
            {
                return BadRequest(result);
            }
            return Ok(result);
        }
    }
}