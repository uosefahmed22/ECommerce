using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Core.Models
{
    public class Product
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string? Description { get; set; }
        public decimal Price { get; set; }
        public string? Color { get; set; }
        public string? Size { get; set; }
        public int Stock { get; set; }
        public int TotalSold { get; set; }
        public DateTime CreatedAt { get; set; }= DateTime.Now;
        public string? ImageCoverUrl { get; set; }
        [NotMapped]
        public IFormFile ImageCover { get; set; }
        public List<string>? productImagesUrls { get; set; }
        [NotMapped]
        public List<IFormFile>? productImages { get; set; }
        public int CategoryId { get; set; }
        [ForeignKey("CategoryId")]
        public Category Category { get; set; }
        public int BrandId { get; set; }
        [ForeignKey("BrandId")]
        public Brand Brand { get; set; }
    }
}