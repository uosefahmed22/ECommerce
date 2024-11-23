using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Core.ReponseModels
{
    public class Metadata
    {
        public int CurrentPage { get; set; }
        public int NumberOfPages { get; set; }
        public int Limit { get; set; }
        public int? NextPage { get; set; }
        public int? PreviousPage { get; set; }
        public Metadata()
        {
        }

        public Metadata(int currentPage, int numberOfPages, int limit, int? nextPage, int? previousPage)
        {
            CurrentPage = currentPage;
            NumberOfPages = numberOfPages;
            Limit = limit;
            NextPage = nextPage;
            PreviousPage = previousPage;
        }
    }
}
