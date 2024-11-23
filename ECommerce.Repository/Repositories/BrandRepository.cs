using AutoMapper;
using ECommerce.Core.DTOs;
using ECommerce.Core.Errors;
using ECommerce.Core.IRepositories;
using ECommerce.Core.IServices;
using ECommerce.Core.Models;
using ECommerce.Repository.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Repository.Repositories
{
    public class BrandRepository : IBrandRepository
    {
        private readonly AppDbContext _dbContext;
        private readonly IMapper _mapper;
        private readonly IImageService _imageService;

        public BrandRepository(AppDbContext dbContext, IMapper mapper, IImageService imageService)
        {
            _dbContext = dbContext;
            _mapper = mapper;
            _imageService = imageService;
        }
        public async Task<ApiResponse> CreateBrand(BrandDto brandDto)
        {
            try
            {
                var uploadResult = await _imageService.UploadImageAsync(brandDto.Image);
                if (uploadResult.Item1 != 1)
                {
                    return new ApiResponse(500, $"Image upload failed: {uploadResult.Item2}");
                }
                brandDto.ImageUrl = uploadResult.Item2;

                var brand = _mapper.Map<Brand>(brandDto);
                await _dbContext.brands.AddAsync(brand);
                await _dbContext.SaveChangesAsync();

                return new ApiResponse(201, "Brand created successfully");
            }
            catch (Exception ex)
            {
                return new ApiResponse(500, $"An error occurred while creating the brand: {ex.Message}");
            }
        }
        public async Task<ApiResponse> DeleteBrand(int brandId)
        {
            try
            {
                var brand = await _dbContext.brands.FindAsync(brandId);

                var deleteResult =  _imageService.DeleteImageAsync(brand.ImageUrl);

                if (brand == null) 
                    return new ApiResponse(404, "Brand not found");

                _dbContext.brands.Remove(brand);
                await _dbContext.SaveChangesAsync();

                return new ApiResponse(204, "Brand deleted successfully");
            }
            catch (Exception ex)
            {
                return new ApiResponse(500, $"An error occurred while deleting the brand: {ex.Message}");
            }
        }
        public async Task<ApiResponse> GetBrand(int brandId)
        {
            try
            {
                var brand = await _dbContext
                    .brands
                    .Where(b => b.Id == brandId)
                    .Select(b => new
                    {
                        Id = b.Id,
                        Name = b.Name,
                        Description = b.Description,
                        ImageUrl = b.ImageUrl,
                        Products = b.Products.Select(p => new
                        {
                            Id = p.Id,
                            Name = p.Name,
                            Description = p.Description,
                            Price = p.Price,
                            ImageCoverUrl = p.ImageCoverUrl,
                            AverageRating = _dbContext.Ratings
                                .Where(r => r.ProductId == p.Id)
                                .Average(r => (double?)r.RatingValue) ?? 0,
                            RatingCount = _dbContext.Ratings
                                .Where(r => r.ProductId == p.Id)
                                .Count()
                        })
                    }).FirstOrDefaultAsync();

                if (brand == null)
                    return new ApiResponse(404, "Brand not found");

                return new ApiResponse(200, brand);
            }
            catch (Exception ex)
            {
                return new ApiResponse(500, $"An error occurred while retrieving the brand: {ex.Message}");
            }
        }
        public async Task<ApiResponse> GetBrands()
        {
            try
            {
                var brands =await _dbContext
                    .brands
                    .Select(b => new
                    {
                        Id = b.Id,
                        Name = b.Name,
                        Description = b.Description,
                        ImageUrl = b.ImageUrl,
                        Products = b.Products.Select(p => new
                        {
                            Id = p.Id,
                            Name = p.Name,
                            Description = p.Description,
                            Price = p.Price,
                            ImageCoverUrl = p.ImageCoverUrl
                        })
                    })
                    .ToListAsync();
                return new ApiResponse(200, brands);
            }
            catch (Exception ex)
            {
                return new ApiResponse(500, $"An error occurred while retrieving the brands: {ex.Message}");
            }
        }
        public async Task<ApiResponse> UpdateBrand(int brandId, BrandDto brandDto)
        {
            try
            {
                if (brandDto.Image != null)
                {
                    var uploadResult = await _imageService.UploadImageAsync(brandDto.Image);
                    if (uploadResult.Item1 != 1)
                    {
                        return new ApiResponse(500, $"Image upload failed: {uploadResult.Item2}");
                    }
                    brandDto.ImageUrl = uploadResult.Item2;
                }
                var brand = await _dbContext.brands.FindAsync(brandId);
                if (brand == null)
                    return new ApiResponse(404, "Brand not found");

                _mapper.Map(brandDto, brand);
                _dbContext.brands.Update(brand);
                await _dbContext.SaveChangesAsync();
                return new ApiResponse(200, "Brand updated successfully");
            }
            catch (Exception ex)
            {
                return new ApiResponse(500, $"An error occurred while updating the brand: {ex.Message}");
            }
        }
    }
}
