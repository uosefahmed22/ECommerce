using AutoMapper;
using ECommerce.Core.DTOs;
using ECommerce.Core.Errors;
using ECommerce.Core.IRepositories;
using ECommerce.Core.IServices;
using ECommerce.Core.Models;
using ECommerce.Core.ReponseModels;
using ECommerce.Repository.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Repository.Repositories
{
    public class ProductRepository : IProductRepository
    {
        private readonly AppDbContext _dbContext;
        private readonly IMapper _mapper;
        private readonly IImageService _imageService;

        public ProductRepository(AppDbContext dbContext, IMapper mapper, IImageService imageService)
        {
            _dbContext = dbContext;
            _mapper = mapper;
            _imageService = imageService;
        }
        public async Task<ApiResponse> CreateProduct(ProductDto productDto)
        {
            try
            {
                var uploadResult = await _imageService.UploadImageAsync(productDto.ImageCover);
                if (uploadResult.Item1 != 1)
                {
                    return new ApiResponse(500, $"Image upload failed: {uploadResult.Item2}");
                }
                productDto.ImageCoverUrl = uploadResult.Item2;

                if (productDto.productImages != null)
                {
                    var imageUrls = new List<string>();
                    foreach (var image in productDto.productImages)
                    {
                        var imageUploadResult = await _imageService.UploadImageAsync(image);
                        if (imageUploadResult.Item1 != 1)
                        {
                            return new ApiResponse(500, $"Image upload failed: {imageUploadResult.Item2}");
                        }
                        imageUrls.Add(imageUploadResult.Item2);
                    }
                    productDto.productImagesUrls = imageUrls;
                }

                var product = _mapper.Map<Product>(productDto);
                await _dbContext.products.AddAsync(product);
                await _dbContext.SaveChangesAsync();
                return new ApiResponse(201, "Product created successfully");
            }
            catch (Exception ex)
            {
                return new ApiResponse(500, $"An error occurred while creating the product: {ex.Message}");
            }
        }
        public async Task<ApiResponse> DeleteProduct(int productId)
        {
            try
            {
                var product = await _dbContext.products.FindAsync(productId);
                if (product == null)
                    return new ApiResponse(404, "Product not found");

                if (product.ImageCoverUrl != null)
                {
                    await _imageService.DeleteImageAsync(product.ImageCoverUrl);
                }

                if (product.productImagesUrls != null)
                {
                    foreach (var image in product.productImagesUrls)
                    {
                        await _imageService.DeleteImageAsync(image);
                    }
                }

                _dbContext.products.Remove(product);
                await _dbContext.SaveChangesAsync();
                return new ApiResponse(204, "Product deleted successfully");
            }
            catch (Exception ex)
            {
                return new ApiResponse(500, $"An error occurred while deleting the product: {ex.Message}");
            }
        }
        public async Task<ApiResponse> GetBestSellingProducts()
        {
            try
            {
                var products = await _dbContext.products
                    .OrderByDescending(p => p.TotalSold)
                    .Take(10)
                    .Select(p => new
                    {
                        Id = p.Id,
                        Name = p.Name,
                        Description = p.Description,
                        Price = p.Price,
                        Color = p.Color,
                        Size = p.Size,
                        Stock = p.Stock,
                        ImageCoverUrl = p.ImageCoverUrl,
                        AverageRating = GetAverageRating(p.Id, _dbContext),
                        RatingCount = GetRatingCount(p.Id, _dbContext),
                        Category = new
                        {
                            Id = p.Category.Id,
                            Name = p.Category.Name
                        },
                        Brand = new
                        {
                            Id = p.Brand.Id,
                            Name = p.Brand.Name
                        }
                    }).ToListAsync();

                return new ApiResponse(200, products);
            }
            catch (Exception ex)
            {
                return new ApiResponse(500, $"An error occurred while retrieving the best selling products: {ex.Message}");
            }
        }
        public async Task<ApiResponse> GetNewProducts()
        {
            try
            {
                var products = await _dbContext.products
                    .OrderByDescending(p => p.CreatedAt)
                    .Take(10)
                    .Select(p => new
                    {
                        Id = p.Id,
                        Name = p.Name,
                        Description = p.Description,
                        Price = p.Price,
                        Color = p.Color,
                        Size = p.Size,
                        Stock = p.Stock,
                        ImageCoverUrl = p.ImageCoverUrl,
                        AverageRating = GetAverageRating(p.Id, _dbContext),
                        RatingCount = GetRatingCount(p.Id, _dbContext),
                        Category = new
                        {
                            Id = p.Category.Id,
                            Name = p.Category.Name
                        },
                        Brand = new
                        {
                            Id = p.Brand.Id,
                            Name = p.Brand.Name
                        }
                    }).ToListAsync();
                return new ApiResponse(200, products);
            }
            catch (Exception ex)
            {
                return new ApiResponse(500, $"An error occurred while retrieving the new products: {ex.Message}");
            }

        }
        public async Task<ApiResponse> GetProduct(int productId)
        {
            try
            {
                var product = await _dbContext
                    .products
                    .Where(p => p.Id == productId)
                    .Select(p => new
                    {
                        Id = p.Id,
                        Name = p.Name,
                        Description = p.Description,
                        Price = p.Price,
                        Color = p.Color,
                        Size = p.Size,
                        Stock = p.Stock,
                        ImageCoverUrl = p.ImageCoverUrl,
                        productImagesUrls = p.productImagesUrls,
                        AverageRating = GetAverageRating(p.Id, _dbContext),
                        RatingCount = GetRatingCount(p.Id, _dbContext),
                        Category = new
                        {
                            Id = p.Category.Id,
                            Name = p.Category.Name
                        },
                        Brand = new
                        {
                            Id = p.Brand.Id,
                            Name = p.Brand.Name
                        }
                    }).FirstOrDefaultAsync();

                if (product == null)
                    return new ApiResponse(404, "Product not found");

                return new ApiResponse(200, product);
            }
            catch (Exception ex)
            {
                return new ApiResponse(500, $"An error occurred while retrieving the product: {ex.Message}");
            }
        }
        public async Task<ProductPagedResponse> GetProducts(int pageIndex, int pageSize)
        {
            try
            {
                var totalItems = await _dbContext.products.CountAsync();

                var products = await _dbContext
                    .products
                    .OrderBy(p => p.Id)
                    .Skip((pageIndex - 1) * pageSize)
                    .Take(pageSize)
                    .Select(p => new ProductDto
                    {
                        ProductDtoId = p.Id,
                        Name = p.Name,
                        Description = p.Description,
                        Price = p.Price,
                        Color = p.Color,
                        Size = p.Size,
                        Stock = p.Stock,
                        ImageCoverUrl = p.ImageCoverUrl,
                        AverageRating = GetAverageRating(p.Id, _dbContext),
                        RatingCount = GetRatingCount(p.Id, _dbContext),
                        CategoryDto = new CategoryDto
                        {
                            Id = p.Category.Id,
                            Name = p.Category.Name,
                            Description = p.Category.Description,
                            ImageUrl = p.Category.ImageUrl
                        },
                        brandDto = new BrandDto
                        {
                            Id = p.Brand.Id,
                            Name = p.Brand.Name,
                            Description = p.Brand.Description,
                            ImageUrl = p.Brand.ImageUrl
                        }
                    }).ToListAsync();

                var paginatedList = new PaginatedList<ProductDto>(products, pageIndex, pageSize, totalItems);

                return new ProductPagedResponse(
                        status: paginatedList.TotalItems,
                        metadata: paginatedList.GetMetadata(),
                        data: paginatedList.Items);

            }
            catch (Exception ex)
            {
                return new ProductPagedResponse(500, null, null);
            }
        }
        public async Task<ApiResponse> UpdateProduct(int productId, ProductDto productDto)
        {
            try
            {
                if (productDto.ImageCover != null)
                {
                    var uploadResult = await _imageService.UploadImageAsync(productDto.ImageCover);
                    if (uploadResult.Item1 != 1)
                    {
                        return new ApiResponse(500, $"Image upload failed: {uploadResult.Item2}");
                    }
                    productDto.ImageCoverUrl = uploadResult.Item2;
                }
                if (productDto.productImages != null)
                {
                    var imageUrls = new List<string>();
                    foreach (var image in productDto.productImages)
                    {
                        var imageUploadResult = await _imageService.UploadImageAsync(image);
                        if (imageUploadResult.Item1 != 1)
                        {
                            return new ApiResponse(500, $"Image upload failed: {imageUploadResult.Item2}");
                        }
                        imageUrls.Add(imageUploadResult.Item2);
                    }
                    productDto.productImagesUrls = imageUrls;
                }
                var product = await _dbContext.products.FindAsync(productId);
                if (product == null)
                    return new ApiResponse(404, "Product not found");
                _mapper.Map(productDto, product);
                _dbContext.products.Update(product);
                await _dbContext.SaveChangesAsync();
                return new ApiResponse(204, "Product updated successfully");
            }
            catch (Exception ex)
            {
                return new ApiResponse(500, $"An error occurred while updating the product: {ex.Message}");
            }
        }
        public async Task<ApiResponse> RateProduct(RatingDto ratingDto)
        {
            try
            {
                var product = await _dbContext.products.FindAsync(ratingDto.ProductId);
                if (product == null)
                    return new ApiResponse(404, "Product not found");

                var rating = new Rating
                {
                    ProductId = ratingDto.ProductId,
                    UserId = ratingDto.UserId,
                    RatingValue = ratingDto.RatingValue
                };
                await _dbContext.Ratings.AddAsync(rating);
                await _dbContext.SaveChangesAsync();
                return new ApiResponse(201, "Product rated successfully");
            }
            catch (Exception ex)
            {
                return new ApiResponse(500, $"An error occurred while rating the product: {ex.Message}");
            }

        }
        private static double GetAverageRating(int productId, AppDbContext dbContext)
        {
            var ratings = dbContext.Ratings
                .Where(r => r.ProductId == productId);

            if (!ratings.Any())
            {
                return 0;
            }

            var averageRating = ratings.Average(r => r.RatingValue);

            if (averageRating < 0)
            {
                averageRating = 0;
            }
            else if (averageRating > 5)
            {
                averageRating = 5;
            }

            return averageRating;
        }
        private static int GetRatingCount(int productId, AppDbContext dbContext)
        {
            var ratingCount = dbContext.Ratings
                .Where(r => r.ProductId == productId)
                .Count();
            return ratingCount;
        }
    }
}
