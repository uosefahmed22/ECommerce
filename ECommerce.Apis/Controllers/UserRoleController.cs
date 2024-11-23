using ECommerce.Core.Errors;
using ECommerce.Core.IServices;
using ECommerce.Core.Models.Auth;
using ECommerce.Repository.Data;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace ECommerce.Apis.Controllers
{
    public class UserRoleController : BaseApiController
    {
        private readonly AppDbContext _dbcontext;
        private readonly UserManager<AppUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IUserRoleService _userRoleService;

        public UserRoleController(AppDbContext dbcontext
            , UserManager<AppUser> userManager
            , RoleManager<IdentityRole> roleManager,
            IUserRoleService userRoleService
            )
        {
            _dbcontext = dbcontext;
            _userManager = userManager;
            _roleManager = roleManager;
            _userRoleService = userRoleService;
        }

        /// <summary>
        /// استرجاع جميع الأدوار.
        /// </summary>
        /// <remarks>
        /// هذا الإجراء يقوم بجلب جميع الأدوار الموجودة في النظام.
        /// </remarks>
        /// <response code="200">تم استرجاع الأدوار بنجاح.</response>
        /// <response code="400">طلب غير صالح.</response>
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Admin")]
        [HttpGet("GetRoles")]
        public async Task<IActionResult> GetRoles()
        {
            var roles = await _userRoleService.GetRolesAsync();
            return Ok(roles);
        }

        /// <summary>
        /// إنشاء دور جديد.
        /// </summary>
        /// <remarks>
        /// هذا الإجراء يقوم بإنشاء دور جديد في النظام.
        /// </remarks>
        /// <response code="200">تم إنشاء الدور بنجاح.</response>
        /// <response code="400">طلب غير صالح.</response>
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Admin")]
        [HttpPost("CreateRole")]
        public async Task<IActionResult> CreateRole(string roleName)
        {
            var role = await _userRoleService.CreateRole(roleName);
            return Ok(role);
        }

        /// <summary>
        /// حذف دور.
        /// </summary>
        /// <remarks>
        /// هذا الإجراء يقوم بحذف دور معين من النظام.
        /// </remarks>
        /// <response code="200">تم حذف الدور بنجاح.</response>
        /// <response code="400">طلب غير صالح.</response>
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Admin")]
        [HttpDelete("DeleteRole")]
        public async Task<IActionResult> DeleteRole(string roleName)
        {
            var role = await _userRoleService.DeleteRole(roleName);
            return Ok(role);
        }

        /// <summary>
        /// إضافة مستخدم إلى دور.
        /// </summary>
        /// <remarks>
        /// هذا الإجراء يقوم بإضافة مستخدم معين إلى دور محدد.
        /// </remarks>
        /// <response code="200">تم إضافة المستخدم بنجاح إلى الدور.</response>
        /// <response code="400">طلب غير صالح.</response>
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Admin")]
        [HttpPost("AddUserToRole")]
        public async Task<IActionResult> AddUserToRole(string email, string roleName)
        {
            var result = await _userRoleService.AddUserToRole(email, roleName);
            return Ok(result);
        }

        /// <summary>
        /// إزالة مستخدم من دور.
        /// </summary>
        /// <remarks>
        /// هذا الإجراء يقوم بإزالة مستخدم معين من دور محدد.
        /// </remarks>
        /// <response code="200">تم إزالة المستخدم من الدور بنجاح.</response>
        /// <response code="400">طلب غير صالح.</response>
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Admin")]
        [HttpPost("RemoveUserFromRole")]
        public async Task<IActionResult> RemoveUserFromRole(string email, string roleName)
        {
            var result = await _userRoleService.RemoveUserFromRole(email, roleName);
            return Ok(result);
        }

        /// <summary>
        /// استرجاع جميع المستخدمين.
        /// </summary>
        /// <remarks>
        /// هذا الإجراء يقوم بجلب جميع المستخدمين المسجلين في النظام.
        /// </remarks>
        /// <response code="200">تم استرجاع المستخدمين بنجاح.</response>
        /// <response code="400">طلب غير صالح.</response>
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "User")]
        [HttpGet("GetUsers")]
        public async Task<IActionResult> GetUsers()
        {
            var users = await _userRoleService.GetUsers();
            return Ok(users);
        }

        /// <summary>
        /// استرجاع الأدوار حسب المستخدم.
        /// </summary>
        /// <remarks>
        /// هذا الإجراء يقوم بجلب جميع الأدوار المرتبطة بمستخدم معين.
        /// </remarks>
        /// <response code="200">تم استرجاع الأدوار بنجاح.</response>
        /// <response code="400">طلب غير صالح.</response>
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Admin")]
        [HttpGet("GetRolesByUser")]
        public async Task<IActionResult> GetRolesByUser(string email)
        {
            var roles = await _userRoleService.GetRolesByUser(email);
            return Ok(roles);
        }

        /// <summary>
        /// إضافة صورة للملف الشخصي.
        /// </summary>
        /// <remarks>
        /// هذا الإجراء يقوم بإضافة صورة للملف الشخصي للمستخدم.
        /// </remarks>
        /// <response code="200">تم إضافة الصورة بنجاح.</response>
        /// <response code="400">طلب غير صالح.</response>
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "User")]
        [HttpPatch("AddProfileImage")]
        public async Task<IActionResult> AddProfileImage(IFormFile image)
        {
            var email = User.FindFirst(ClaimTypes.Email)?.Value;
            var result = await _userRoleService.AddProfileImage(image, null, email);
            return Ok(result);
        }

        /// <summary>
        /// استرجاع بيانات المستخدم.
        /// </summary>
        /// <remarks>
        /// هذا الإجراء يقوم بجلب بيانات المستخدم الحالي.
        /// </remarks>
        /// <response code="200">تم استرجاع بيانات المستخدم بنجاح.</response>
        /// <response code="400">طلب غير صالح.</response>
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "User")]
        [HttpGet("GetUser")]
        public async Task<IActionResult> GetUser()
        {
            var email = User.FindFirst(ClaimTypes.Email)?.Value;
            var result = await _userRoleService.GetUser(email);
            return Ok(result);
        }

        /// <summary>
        /// حذف مستخدم.
        /// </summary>
        /// <remarks>
        /// هذا الإجراء يقوم بحذف الحساب الحالي للمستخدم.
        /// </remarks>
        /// <response code="200">تم حذف الحساب بنجاح.</response>
        /// <response code="400">طلب غير صالح.</response>
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "User , Admin")]
        [HttpDelete("deleteUser")]
        public async Task<IActionResult> DeleteUser()
        {
            var email = User.FindFirst(ClaimTypes.Email)?.Value;
            if (email == null)
            {
                return BadRequest(new ApiResponse(400, "Invalid user"));
            }
            var result = await _userRoleService.DeleteUser(email);
            return Ok(result);
        }

    }
}
