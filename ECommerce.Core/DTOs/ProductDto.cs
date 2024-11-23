using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Core.DTOs
{
    public class ProductDto
    {
        public int ProductDtoId { get; set; }
        public string Name { get; set; }
        public string? Description { get; set; }
        public decimal Price { get; set; }
        public string? Color { get; set; }
        public string? Size { get; set; }
        public int Stock { get; set; }
        public double AverageRating { get; set; }
        public int RatingCount { get; set; }
        public string? ImageCoverUrl { get; set; }
        [NotMapped]
        public IFormFile ImageCover { get; set; }
        public List<string>? productImagesUrls { get; set; }
        public int BrandId { get; set; }
        public int CategoryId { get; set; }

        [NotMapped]
        public List<IFormFile>? productImages { get; set; }
        public CategoryDto? CategoryDto { get; set; }
        public BrandDto? brandDto { get; set; }

    }
}
