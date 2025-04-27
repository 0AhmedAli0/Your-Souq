using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Specifications
{
    public class ProductSpecParameters
    {
		private const int MaxPageSize = 50;
		public int pageIndex { get; set; } = 1;

		private int PageSize = 6;

		public int pageSize
        {
			get => PageSize;
			set => PageSize = (value > MaxPageSize) ? MaxPageSize : value;
		}


		private List<string> Brands = [];

		public List<string> brands
		{
			get => Brands;
			set 
			{ 
				Brands = value.SelectMany(x => x.Split(',', StringSplitOptions.RemoveEmptyEntries)).ToList();
				//set { Brands = value.ToString().Split(',').ToList(); }
			}
		}
		
		private List<string> Types = [];

		public List<string> types
		{
			get => Types;
			set 
			{ 
				Types = value.SelectMany(x => x.Split(',', StringSplitOptions.RemoveEmptyEntries)).ToList();
			}
		}

        public string? Sort { get; set; }

    }
}
