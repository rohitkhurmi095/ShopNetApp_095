using Microsoft.AspNetCore.Mvc;
using ShopNetApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
//Session Extensions
using ShopNetApp.Utilities;
using ShopNetApp.Data;

namespace ShopNetApp.Controllers
{
    public class CartController : Controller
    {
        //DbContext
        //----------
        private readonly ApplicationDbContext _dbContext;
        public CartController(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }


        //SHopping Cart Products
        //=======================
        //1.ShoppingCart List(Session) Get -> ProductId's list
        //2.Find distict producs using productId's: [.Select(x=>x.ProductId)]
        //3.get Products from Database using sessionProductsId: [.Where(p=>distinctProductsList.Contains(p.Id))]
        public IActionResult Index()
        {
            //All CartItems (ProudctId's)
            List<ShoppingCart> shoppingCartItems = new List<ShoppingCart>();

            //Get CartItems from session
            if(HttpContext.Session.Get<List<ShoppingCart>>(WC.SessionCart)!=null
                && HttpContext.Session.Get<List<ShoppingCart>>(WC.SessionCart).Count() > 0)
            {
                //shoppingCartItems Product Id's list (contains duplicated id's)
                shoppingCartItems = HttpContext.Session.Get<List<ShoppingCart>>(WC.SessionCart);
            }

            //REMOVE DUPLICATES from ProductId's (Session) List
            //Distict ProductId's list
            List<int> productInCart = shoppingCartItems.Select(p => p.ProductId).ToList();

            //Get Products data from Db whose id is in Distinct ProductId's list 
            List<Product> products = _dbContext.Product.Where(p => productInCart.Contains(p.Id)).ToList();


            //Pass shoppingCartItems -> cartView
            return View(products);
        }
    }
}
