using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Specifications
{
    public class pagingParams
    {
        private const int MaxPageSize = 50;
        public int pageIndex { get; set; } = 1;

        private int PageSize = 6;

        public int pageSize
        {
            get => PageSize;
            set => PageSize = (value > MaxPageSize) ? MaxPageSize : value;
        }
    }
}
