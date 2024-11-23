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
    public interface IBrandRepository
    {
        Task<ApiResponse> GetBrands();
        Task<ApiResponse> GetBrand(int brandId);
        Task<ApiResponse> CreateBrand(BrandDto brandDto);
        Task<ApiResponse> UpdateBrand(int brandId, BrandDto brandDto);
        Task<ApiResponse> DeleteBrand(int brandId);
    }
}
