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
    public class CategoryRepository : ICategoryRepository
    {
        private readonly AppDbContext _dbContext;
        private readonly IMapper _mapper;
        private readonly IImageService _imageService;

        public CategoryRepository(AppDbContext dbContext, IMapper mapper, IImageService imageService)
        {
            _dbContext = dbContext;
            _mapper = mapper;
            _imageService = imageService;
        }
        public async  Task<ApiResponse> CreateCategory(CategoryDto categoryDto)
        {
            try
            {
                if(categoryDto.Image != null)
                {
                    var uploadResult = await _imageService.UploadImageAsync(categoryDto.Image);
                    if (uploadResult.Item1 != 1)
                    {
                        return new ApiResponse(500, $"Image upload failed: {uploadResult.Item2}");
                    }
                    categoryDto.ImageUrl = uploadResult.Item2;
                }

                var category = _mapper.Map<Category>(categoryDto);
                await _dbContext.categories.AddAsync(category);
                await _dbContext.SaveChangesAsync();
                return new ApiResponse(201, "Category created successfully");
            }
            catch (Exception ex)
            {
                return new ApiResponse(500, $"An error occurred while creating the category: {ex.Message}");
            }
        }
        public async Task<ApiResponse> DeleteCategory(int categoryId)
        {
            try
            {
                var category = await _dbContext.categories.FindAsync(categoryId);
                if (category is null)
                    return new ApiResponse(404, "Category not found");

                if (category.ImageUrl != null)
                {
                    await _imageService.DeleteImageAsync(category.ImageUrl);
                }

                if (category == null)
                    return new ApiResponse(404, "Category not found");
                _dbContext.categories.Remove(category);
                await _dbContext.SaveChangesAsync();
                return new ApiResponse(204, "Category deleted successfully");
            }
            catch (Exception ex)
            {
                return new ApiResponse(500, $"An error occurred while deleting the category: {ex.Message}");
            }
        }
        public async Task<ApiResponse> GetCategories()
        {
            try
            {
                var categories = await _dbContext.categories.ToListAsync();
                return new ApiResponse(200, categories);
            }
            catch (Exception ex)
            {
                return new ApiResponse(500, $"An error occurred while retrieving categories: {ex.Message}");
            }
        }
        public async Task<ApiResponse> UpdateCategory(int categoryId, CategoryDto categoryDto)
        {
            try
            {
                if (categoryDto.Image != null)
                {
                    var uploadResult = await _imageService.UploadImageAsync(categoryDto.Image);
                    if (uploadResult.Item1 != 1)
                    {
                        return new ApiResponse(500, $"Image upload failed: {uploadResult.Item2}");
                    }
                    categoryDto.ImageUrl = uploadResult.Item2;
                }

                var category = await _dbContext.categories.FindAsync(categoryId);
                if (category == null)
                    return new ApiResponse(404, "Category not found");

                _mapper.Map(categoryDto, category);
                _dbContext.categories.Update(category);
                await _dbContext.SaveChangesAsync();
                return new ApiResponse(204, "Category updated successfully");
            }
            catch (Exception ex)
            {
                return new ApiResponse(500, $"An error occurred while updating the category: {ex.Message}");
            }
        }
        public async Task<ApiResponse> GetCategory(int categoryId)
        {
            try
            {
                var category = await _dbContext.categories
                    .Where(c => c.Id == categoryId)
                    .Select(c => new
                    {
                        Id = c.Id,
                        Name = c.Name,
                        Description = c.Description,
                        ImageUrl = c.ImageUrl,
                        Products = c.Products.Select(p => new
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
                        }).ToList()
                    })
                    .FirstOrDefaultAsync();

                if (category == null)
                {
                    return new ApiResponse(404, "Category not found.");
                }

                return new ApiResponse(200, category);
            }
            catch (Exception ex)
            {
                return new ApiResponse(500, $"An error occurred while retrieving the category: {ex.Message}");
            }
        }
    }
}
