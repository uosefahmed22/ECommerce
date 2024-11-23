using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Core.ReponseModels
{
    public class ProductCartApiResponse
    {
        public int StatusCode { get; set; }
        public string Message { get; set; }
        public int NumOfCartItems { get; set; }
        public object Data { get; set; }

        public ProductCartApiResponse(int statusCode, string message, object data)
        {
            StatusCode = statusCode;
            Message = message;
            Data = data;
        }
        public ProductCartApiResponse(int statusCode, string message)
        {
            StatusCode = statusCode;
            Message = message;
        }
        public ProductCartApiResponse(int statusCode, string message, int numOfCartItems, object data)
        {
            StatusCode = statusCode;
            Message = message;
            NumOfCartItems = numOfCartItems;
            Data = data;
        }
    }
}
