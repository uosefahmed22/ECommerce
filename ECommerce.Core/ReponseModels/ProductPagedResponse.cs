using ECommerce.Core.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Core.ReponseModels
{
    public class ProductPagedResponse
    {
        public int Status { get; set; }
        public Metadata Metadata { get; set; }
        public List<ProductDto> Data { get; set; }

        public ProductPagedResponse(int status, Metadata metadata, List<ProductDto> data)
        {
            Status = status;
            Metadata = metadata;
            Data = data;
        }
    }
}
