using ECommerce.Core.DTOs;
using ECommerce.Core.Errors;
using ECommerce.Core.IRepositories;
using ECommerce.Repository.Repositories;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace ECommerce.Apis.Controllers
{
    public class BrandController : BaseApiController
    {
        private readonly IBrandRepository _brandRepository;
        public BrandController(IBrandRepository brandRepository)
        {
            _brandRepository = brandRepository;
        }

        [HttpGet("getbrands")]
        public async Task<IActionResult> GetBrandsAsync()
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var result = await _brandRepository.GetBrands();
            if (result.StatusCode == 400)
            {
                return BadRequest(result);
            }
            return Ok(result);
        }

        [HttpGet("getbrand")]
        public async Task<IActionResult> GetBrandAsync(int brandId)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var result = await _brandRepository.GetBrand(brandId);
            if (result.StatusCode == 400)
            {
                return BadRequest(result);
            }
            return Ok(result);
        }

        [HttpPost("createbrand")]
        public async Task<IActionResult> CreateBrandAsync([FromForm] BrandDto brandDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var result = await _brandRepository.CreateBrand(brandDto);
            if (result.StatusCode == 400)
            {
                return BadRequest(result);
            }
            return Ok(result);
        }

        [HttpPut("updatebrand")]
        public async Task<IActionResult> UpdateBrandAsync(int brandId, [FromForm] BrandDto brandDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var result = await _brandRepository.UpdateBrand(brandId, brandDto);
            if (result.StatusCode == 400)
            {
                return BadRequest(result);
            }
            return Ok(result);
        }

        [HttpDelete("deletebrand")]
        public async Task<IActionResult> DeleteBrandAsync(int brandId)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var result = await _brandRepository.DeleteBrand(brandId);
            if (result.StatusCode == 400)
            {
                return BadRequest(result);
            }
            return Ok(result);
        }
    }
}
