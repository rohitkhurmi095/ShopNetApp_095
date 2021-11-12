using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ShopNetApp.Models.ViewModel
{
    //PRODUCT Details ViewModel
    //===========================
    //Single Product + CartInfo
    public class ProductDetailsViewModel
    {
        //Product Details
        public Product Product { get; set; }

        //Product AddedTo Cart or not? (true/false)
        public bool existsInCart { get; set; }
    }
}
