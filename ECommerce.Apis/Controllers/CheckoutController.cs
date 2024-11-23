using ECommerce.Core.DTOs;
using ECommerce.Core.Errors;
using ECommerce.Core.IServices;
using ECommerce.Core.Models.Auth;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace ECommerce.Apis.Controllers
{
    public class CheckoutController : BaseApiController
    {
        private readonly ICheckoutService _checkoutService;
        private readonly UserManager<AppUser> _userManager;
        public CheckoutController(ICheckoutService checkoutService, UserManager<AppUser> userManager)
        {
            _checkoutService = checkoutService;
            _userManager = userManager;
        }
        [HttpPost("createcheckout")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "User")]
        public async Task<IActionResult> CreateCheckoutAsync(string cartId, OrderModelDto orderModelDto)
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
            var result = await _checkoutService.CreateCheckoutAsync(user.Id, cartId, orderModelDto);
            if (result.StatusCode == 400)
            {
                return BadRequest(result);
            }
            return Ok(result);
        }

        [HttpGet("getallcheckouts")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "User")]
        public async Task<IActionResult> GetAllCheckoutsOfUserAsync()
        {
            var email = User.FindFirst(ClaimTypes.Email)?.Value;
            if (email == null)
            {
                return BadRequest(new ApiResponse(400, "Invalid user"));
            }
            var user = await _userManager.FindByEmailAsync(email);
            var result = await _checkoutService.GetAllCheckoutsOfUserAsync(user.Id);
            return Ok(result);
        }
    }
}
