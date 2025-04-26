using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Specifications
{
    public class ProductSpecParameters
    {
		private List<string> Brands;

		public List<string> brands
		{
			get => Brands;
			set 
			{ 
				Brands = value.SelectMany(x => x.Split(',', StringSplitOptions.RemoveEmptyEntries)).ToList();
				//set { Brands = value.ToString().Split(',').ToList(); }
			}
		}

	}
}
