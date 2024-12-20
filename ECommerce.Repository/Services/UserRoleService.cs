﻿using ECommerce.Core.DTOs;
using ECommerce.Core.Errors;
using ECommerce.Core.IServices;
using ECommerce.Core.Models.Auth;
using ECommerce.Repository.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Repository.Services
{
    public class UserRoleService : IUserRoleService
    {
        private readonly AppDbContext _dbContext;
        private readonly UserManager<AppUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IImageService _imageService;

        public UserRoleService(AppDbContext dbContext,
            UserManager<AppUser> userManager,
            RoleManager<IdentityRole> roleManager,
            IImageService imageService)
        {
            _dbContext = dbContext;
            _userManager = userManager;
            _roleManager = roleManager;
            _imageService = imageService;
        }

        public async Task<ApiResponse> AddProfileImage(IFormFile? image, string? ImageUrl, string email)
        {
            try
            {
                var user = await _userManager.FindByEmailAsync(email);
                if (user == null)
                {
                    return new ApiResponse(404, "المستخدم غير موجود");
                }

                if (image == null)
                {
                    if (!string.IsNullOrEmpty(user.ImageUrl))
                    {
                        await _imageService.DeleteImageAsync(user.ImageUrl);
                        user.ImageUrl = null;
                        await _userManager.UpdateAsync(user);
                    }
                    return new ApiResponse(200, "تم حذف الصورة بنجاح");
                }

                var fileResult = await _imageService.UploadImageAsync(image);
                if (fileResult.Item1 == 1)
                {
                    user.ImageUrl = fileResult.Item2;
                    await _userManager.UpdateAsync(user);
                    return new ApiResponse(200, "تم اضافة الصورة بنجاح");
                }
                else
                {
                    return new ApiResponse(400, fileResult.Item2);
                }
            }
            catch (Exception ex)
            {
                return new ApiResponse(400, ex.Message);
            }
        }
        public async Task<ApiResponse> AddUserToRole(string email, string roleName)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
            {
                return new ApiResponse(404, "المستخدم غير موجود");
            }

            var roleExists = await _roleManager.RoleExistsAsync(roleName);
            if (!roleExists)
            {
                return new ApiResponse(404, "الصلاحية غير موجودة");
            }

            var isInRole = await _userManager.IsInRoleAsync(user, roleName);
            if (isInRole)
            {
                return new ApiResponse(400, "المستخدم موجود بالفعل في الصلاحية");
            }

            var result = await _userManager.AddToRoleAsync(user, roleName);
            if (!result.Succeeded)
            {
                var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                return new ApiResponse(400, $"فشل في اضافة المستخدم الى الصلاحية: {errors}");
            }

            return new ApiResponse(200, "تم اضافة المستخدم الى الصلاحية بنجاح");
        }
        public async Task<ApiResponse> CreateRole(string roleName)
        {
            try
            {
                var role = await _roleManager.RoleExistsAsync(roleName);
                if (!role)
                {
                    var roleResult = await _roleManager.CreateAsync(new IdentityRole(roleName));
                    return new ApiResponse(200, "الصلاحية تم انشائها بنجاح");
                }
                return new ApiResponse(400, "الصلاحية موجودة مسبقا");
            }
            catch (Exception ex)
            {
                return new ApiResponse(400, ex.Message);
            }
        }
        public async Task<ApiResponse> DeleteRole(string roleName)
        {
            try
            {
                var role = await _roleManager.FindByNameAsync(roleName);
                if (role == null)
                {
                    return new ApiResponse(404, "الصلاحية غير موجودة");
                }
                var result = await _roleManager.DeleteAsync(role);
                if (result.Succeeded)
                {
                    return new ApiResponse(200, "تم حذف الصلاحية بنجاح");
                }
                return new ApiResponse(400, "فشل في حذف الصلاحية");
            }
            catch (Exception ex)
            {
                return new ApiResponse(400, ex.Message);
            }
        }
        public async Task<ApiResponse> DeleteUser(string email)
        {
            try
            {
                var user = _userManager.FindByEmailAsync(email);
                if (user == null)
                {
                    return new ApiResponse(404, "المستخدم غير موجود");
                }
                var result = await _userManager.DeleteAsync(user.Result);
                if (result.Succeeded)
                {
                    return new ApiResponse(200, "تم حذف المستخدم بنجاح");
                }
                return new ApiResponse(400, "فشل في حذف المستخدم");
            }
            catch (Exception ex)
            {
                return new ApiResponse(400, ex.Message);
            }
        }
        public async Task<ApiResponse> GetRolesAsync()
        {
            try
            {
                var roles = await _roleManager.Roles
                    .Select(x => x.Name)
                    .ToListAsync();
                return new ApiResponse(200, roles);
            }
            catch (Exception ex)
            {
                return new ApiResponse(400, ex.Message);
            }
        }
        public async Task<ApiResponse> GetRolesByUser(string email)
        {
            try
            {
                var user = await _userManager.FindByEmailAsync(email);
                if (user == null)
                {
                    return new ApiResponse(404, "المستخدم غير موجود");
                }
                var roles = await _userManager.GetRolesAsync(user);
                return new ApiResponse(200, roles);
            }
            catch (Exception ex)
            {
                return new ApiResponse(400, ex.Message);
            }
        }
        public async Task<ApiResponse> GetUser(string email)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
            {
                return new ApiResponse(404, "المستخدم غير موجود");
            }

            var roles = await _userManager.GetRolesAsync(user);

            var userDto = new UserDto
            {
                Email = user.Email,
                FullName = user.FullName,
                ImageUrl = user.ImageUrl,
                UserRole = (List<string>)roles
            };

            return new ApiResponse(200, userDto);
        }
        public async Task<ApiResponse> GetUsers()
        {
            try
            {
                var users = await _userManager.Users
                    .Select(x => new UserDto
                    {
                        Email = x.Email,
                        FullName = x.FullName,
                        ImageUrl = x.ImageUrl
                    })
                    .ToListAsync();

                foreach (var user in users)
                {
                    var appUser = await _userManager.FindByEmailAsync(user.Email);
                    user.UserRole = (List<string>)await _userManager.GetRolesAsync(appUser);
                }

                return new ApiResponse(200, users);
            }
            catch (Exception ex)
            {
                return new ApiResponse(400, ex.Message);
            }
        }
        public async Task<ApiResponse> RemoveUserFromRole(string email, string roleName)
        {
            try
            {
                var user = await _userManager.FindByEmailAsync(email);
                if (user == null)
                {
                    return new ApiResponse(404, "المستخدم غير موجود");
                }
                var role = await _roleManager.RoleExistsAsync(roleName);
                if (!role)
                {
                    return new ApiResponse(404, "الصلاحية غير موجودة");
                }
                var result = await _userManager.RemoveFromRoleAsync(user, roleName);
                if (result.Succeeded)
                {
                    return new ApiResponse(200, "تم حذف المستخدم من الصلاحية بنجاح");
                }
                return new ApiResponse(400, "فشل في حذف المستخدم من الصلاحية");
            }
            catch (Exception ex)
            {
                return new ApiResponse(400, ex.Message);
            }
        }

    }

}
