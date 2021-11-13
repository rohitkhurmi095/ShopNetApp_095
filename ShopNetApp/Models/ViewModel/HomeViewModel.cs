using cloudscribe.Pagination.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ShopNetApp.Models.ViewModel
{
    public class HomeViewModel
    {
        //Products
        public List<Product> Products { get; set; }
        //Categories
        public List<Category> Categories { get; set; }
    }
}
