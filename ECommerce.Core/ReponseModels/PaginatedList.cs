using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Core.ReponseModels
{
    public class PaginatedList<T>
    {
        public List<T> Items { get; }
        public int TotalItems { get; }
        public int PageIndex { get; }
        public int PageSize { get; }
        public int TotalPages { get; }
        public int? NextPage => PageIndex < TotalPages ? PageIndex + 1 : null;
        public int? PreviousPage => PageIndex > 1 ? PageIndex - 1 : null;

        public PaginatedList(List<T> items, int pageIndex, int pageSize, int totalItems)
        {
            Items = items;
            PageIndex = pageIndex;
            PageSize = pageSize;
            TotalItems = totalItems;
            TotalPages = (int)Math.Ceiling(totalItems / (double)pageSize);
        }

        public Metadata GetMetadata()
        {
            return new Metadata
            {
                CurrentPage = PageIndex,
                NumberOfPages = TotalPages,
                Limit = PageSize,
                NextPage = NextPage,
                PreviousPage = PreviousPage
            };
        }
    }
}
