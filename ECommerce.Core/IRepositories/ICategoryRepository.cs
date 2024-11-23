using ECommerce.Core.DTOs;
using ECommerce.Core.Errors;
using ECommerce.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Core.IRepositories
{
    public interface ICategoryRepository
    {
        Task<ApiResponse> GetCategories();
        Task<ApiResponse> GetCategory(int categoryId);
        Task<ApiResponse> CreateCategory(CategoryDto categoryDto);
        Task<ApiResponse> UpdateCategory(int categoryId, CategoryDto categoryDto);
        Task<ApiResponse> DeleteCategory(int categoryId);
    }
}
