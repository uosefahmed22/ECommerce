using ECommerce.Core.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Core.DTOs
{
    public class RatingDto
    {
        public int RatingValue { get; set; }
        public int ProductId { get; set; }
        public string UserId { get; set; }
    }
}
