using ECommerce.Core.DTOs;
using ECommerce.Core.Errors;
using ECommerce.Core.Models;
using ECommerce.Core.ReponseModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Core.IRepositories
{
    public interface IProductRepository
    {
        Task<ProductPagedResponse> GetProducts(int pageIndex, int pageSize);
        Task<ApiResponse> GetProduct(int productId);
        Task<ApiResponse> CreateProduct(ProductDto productDto);
        Task<ApiResponse> UpdateProduct(int productId, ProductDto productDto);
        Task<ApiResponse> DeleteProduct(int productId);
        Task<ApiResponse> GetBestSellingProducts();
        Task<ApiResponse> GetNewProducts();
        Task<ApiResponse> RateProduct(RatingDto ratingDto);
    }
}
