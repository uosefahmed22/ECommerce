using ECommerce.Core.Errors;
using ECommerce.Core.IServices;
using ECommerce.Core.Models.Auth;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace ECommerce.Apis.Controllers
{
    public class AuthController : BaseApiController
    {

        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }
        /// <summary>
        /// تسجيل مستخدم جديد.
        /// </summary>
        /// <remarks>
        /// هذا الإجراء يقوم بتسجيل مستخدم جديد في النظام.
        /// </remarks>
        /// <response code="200">تم التسجيل بنجاح.</response>
        /// <response code="400">طلب غير صالح.</response>
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] Register model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var result = await _authService.RigsterAsync(model, GenerateCallBackUrl);
            if (result.StatusCode == 400)
            {
                return BadRequest(result);
            }
            return Ok(result);
        }

        /// <summary>
        /// تسجيل دخول المستخدم.
        /// </summary>
        /// <remarks>
        /// هذا الإجراء يقوم بتسجيل دخول المستخدم.
        /// </remarks>
        /// <response code="200">تم تسجيل الدخول بنجاح.</response>
        /// <response code="400">طلب غير صالح.</response>
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] Login model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var result = await _authService.LoginAsync(model);
            if (result.StatusCode == 400)
            {
                return BadRequest(result);
            }
            return Ok(result);
        }

        /// <summary>
        /// تحديث رمز الوصول.
        /// </summary>
        /// <remarks>
        /// هذا الإجراء يقوم بتحديث رمز الوصول باستخدام رمز التحديث.
        /// </remarks>
        /// <response code="200">تم تحديث الرمز بنجاح.</response>
        /// <response code="400">طلب غير صالح.</response>
        [HttpPost("refresh-token")]
        public async Task<IActionResult> RefreshToken([FromBody] TokenRequest model)
        {
            var result = await _authService.RefreshToken(model);
            if (result.StatusCode == 400)
            {
                return BadRequest(result);
            }
            return Ok(result);
        }

        /// <summary>
        /// إلغاء رمز التحديث.
        /// </summary>
        /// <remarks>
        /// هذا الإجراء يقوم بإلغاء رمز التحديث المحدد.
        /// </remarks>
        /// <response code="200">تم إلغاء الرمز بنجاح.</response>
        /// <response code="400">طلب غير صالح.</response>
        [HttpPost("revoke-token")]
        public async Task<IActionResult> RevokeToken([FromBody] TokenRequest model)
        {
            var result = await _authService.RevokeToken(model);
            if (result.StatusCode == 400)
            {
                return BadRequest(result);
            }
            return Ok(result);
        }

        /// <summary>
        /// نسيان كلمة المرور.
        /// </summary>
        /// <remarks>
        /// هذا الإجراء يقوم بإرسال تعليمات استعادة كلمة المرور إلى البريد الإلكتروني.
        /// </remarks>
        /// <response code="200">تم إرسال البريد الإلكتروني بنجاح.</response>
        /// <response code="400">طلب غير صالح.</response>
        [HttpPost("forget-password")]
        public async Task<IActionResult> ForgetPassword(string email)
        {
            var result = await _authService.ForgetPassword(email);
            if (result.StatusCode == 400)
            {
                return BadRequest(result);
            }
            return Ok(result);
        }

        /// <summary>
        /// التحقق من رمز OTP.
        /// </summary>
        /// <remarks>
        /// هذا الإجراء يقوم بالتحقق من صحة رمز OTP المرسل.
        /// </remarks>
        /// <response code="200">تم التحقق بنجاح.</response>
        /// <response code="400">طلب غير صالح.</response>
        [HttpPost("verify-otp")]
        public IActionResult VerifyOtp([FromBody] VerifyOtp model)
        {
            var result = _authService.VerfiyOtp(model);
            if (result.StatusCode == 400)
            {
                return BadRequest(result);
            }
            return Ok(result);
        }

        /// <summary>
        /// إعادة تعيين كلمة المرور.
        /// </summary>
        /// <remarks>
        /// هذا الإجراء يقوم بإعادة تعيين كلمة المرور للمستخدم.
        /// </remarks>
        /// <response code="200">تم إعادة تعيين كلمة المرور بنجاح.</response>
        /// <response code="400">طلب غير صالح.</response>
        [HttpPost("reset-password")]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPassword model)
        {
            var result = await _authService.ResetPasswordAsync(model);
            if (result.StatusCode == 400)
            {
                return BadRequest(result);
            }
            return Ok(result);
        }

        /// <summary>
        /// إعادة إرسال بريد التأكيد.
        /// </summary>
        /// <remarks>
        /// هذا الإجراء يقوم بإعادة إرسال بريد التأكيد إلى المستخدم.
        /// </remarks>
        /// <response code="200">تم إرسال البريد الإلكتروني بنجاح.</response>
        /// <response code="400">طلب غير صالح.</response>
        [HttpPost("resend-confirmation-email")]
        public async Task<IActionResult> ResendConfirmationEmail(string email)
        {
            var result = await _authService.ResendConfirmationEmailAsync(email, GenerateCallBackUrl);
            if (result.StatusCode == 400)
            {
                return BadRequest(result);
            }
            return Ok(result);
        }

        /// <summary>
        /// تأكيد البريد الإلكتروني.
        /// </summary>
        /// <remarks>
        /// هذا الإجراء يقوم بتأكيد البريد الإلكتروني للمستخدم باستخدام معرف المستخدم ورمز التأكيد.
        /// </remarks>
        /// <response code="200">تم تأكيد البريد الإلكتروني بنجاح.</response>
        /// <response code="400">فشل تأكيد البريد الإلكتروني.</response>
        [HttpGet("confirm-email")]
        public async Task<IActionResult> ConfirmEmail(string userId, string confirmationToken)
        {
            var result = await _authService.ConfirmUserEmailAsync(userId!, confirmationToken!);

            if (result)
            {
                return RedirectPermanent(@"https://uosefahmed22.github.io/ECommerceInAngular/login");
            }
            else
            {
                return BadRequest("Failed to confirm user email.");
            }
        }

        /// <summary>
        /// تغيير كلمة المرور.
        /// </summary>
        /// <remarks>
        /// هذا الإجراء يقوم بتغيير كلمة المرور الحالية للمستخدم.
        /// </remarks>
        /// <response code="200">تم تغيير كلمة المرور بنجاح.</response>
        /// <response code="400">طلب غير صالح.</response>
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "User")]
        [HttpPost("change-password")]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePassword model)
        {
            var email = User.FindFirst(ClaimTypes.Email)?.Value;

            if (string.IsNullOrEmpty(email))
            {
                return BadRequest(new ApiResponse(400, "حدث خطأ ما"));
            }

            var response = await _authService.ChangePasswordAsync(model, email);

            if (response.StatusCode == 200)
            {
                return Ok(response);
            }

            return BadRequest(response);
        }

        private string GenerateCallBackUrl(string token, string userId)
        {
            var encodedToken = Uri.EscapeDataString(token);
            var encodedUserId = Uri.EscapeDataString(userId);
            var callBackUrl = $"{Request.Scheme}://{Request.Host}/api/Auth/confirm-email?userId={encodedUserId}&confirmationToken={encodedToken}";
            return callBackUrl;
        }
    }
}
