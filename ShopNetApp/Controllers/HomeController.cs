using cloudscribe.Pagination.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using ShopNetApp.Data;
using ShopNetApp.Models;
using ShopNetApp.Models.ViewModel;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
//SessionExtensions
using ShopNetApp.Utilities;

namespace ShopNetApp.Controllers
{
    public class HomeController : Controller
    {
        //DbContext
        private readonly ApplicationDbContext _dbContext;
        //Logger
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger, ApplicationDbContext dbContext)
        {
            _logger = logger;
            _dbContext = dbContext;
        }

        //==========================
        //ALL PRODUCTS With Filters
        //==========================
        public IActionResult Index()
        {
            //HomeViewModel
            //Return All Products(card)+Categories(filter) -> Home
            HomeViewModel homeView = new HomeViewModel()
            {
                Products = _dbContext.Product.Include(c => c.Category).Include(a => a.ApplicationType).AsNoTracking().ToList(),
                Categories = _dbContext.Category.ToList()
            };

            //return ViewModel -> Home
            return View(homeView);
        }




        //============================
        //Single Product Details Page
        //============================
        //GET Single Product by Id (Form containing Add to Cart Button)
        //------------------------
        public IActionResult Details(int Id)
        {
            //CHECK If currentProduct exists in Cart or not?
            //If currentProduct exists in cart => existsInCart = true (Show RemoveFromCart button)
            //Get shoppingCartList from session + search productId in session 
            List<ShoppingCart> shoppingCartList = new List<ShoppingCart>();

            //1.check if session exists & sessionCart is not empty
            if (HttpContext.Session.Get<List<ShoppingCart>>(WC.SessionCart) != null
                && HttpContext.Session.Get<List<ShoppingCart>>(WC.SessionCart).Count() > 0)
            {
                //Get shoppingCartList from session
                shoppingCartList = HttpContext.Session.Get<List<ShoppingCart>>(WC.SessionCart);
            }


            ProductDetailsViewModel productDetails = new ProductDetailsViewModel()
            {
                //ProductDetails
                Product = _dbContext.Product.Include(c => c.Category).Include(a => a.ApplicationType).Where(p => p.Id == Id).FirstOrDefault(),
                //cartDetails
                existsInCart = false
            };

            //2.CHECK If Product exists in ShoppingCartList (in session) 
            foreach(var item in shoppingCartList)
            {
                if (item.ProductId == Id)
                {
                    //current state
                    productDetails.existsInCart = true;
                }
            }


            //return ProductDetailsViewModel -> View
            return View(productDetails);
        }


        //____________________________
        //POST: Add To Cart (Session)
        //____________________________
        //POST: Single Product Added to Cart(SESSION)
        //ActionName = Details**
        [HttpPost,ActionName("Details")]
        public IActionResult DetailsPost(int Id)
        {
            //CREATE list of ShoppingCartItems (contains cart ProductId's)
            List<ShoppingCart> shoppingCartList = new List<ShoppingCart>();

            //Check if session Exists 
            //(sessison contains ShoppingCartItems List + List is not empty)
            if(HttpContext.Session.Get<List<ShoppingCart>>(WC.SessionCart)!=null
                && HttpContext.Session.Get<List<ShoppingCart>>(WC.SessionCart).Count() > 0)
            {
                //retrive the Cartsession(ProductId's) - in list
                shoppingCartList = HttpContext.Session.Get<List<ShoppingCart>>(WC.SessionCart);
            }

            //Add item(productId) to list(ProductId's)
            shoppingCartList.Add(new ShoppingCart
            {
                //Id of current product
                ProductId = Id
            });

            //Set list again in session
            HttpContext.Session.Set(WC.SessionCart, shoppingCartList);


            //redirect to HomePage/Index
            return RedirectToAction(nameof(Index));
        }



        //___________________________
        //REMOVE From Cart (Session)
        //___________________________
        public IActionResult RemoveFromCart(int Id)
        {
            //Get shoppingCartList from Session
            List<ShoppingCart> shoppingCartList = new List<ShoppingCart>();

            if(HttpContext.Session.Get<List<ShoppingCart>>(WC.SessionCart)!=null
                && HttpContext.Session.Get<List<ShoppingCart>>(WC.SessionCart).Count() > 0)
            {
                shoppingCartList = HttpContext.Session.Get<List<ShoppingCart>>(WC.SessionCart);
            }

            //Id = id of item to be removed
            //find Item with the id in shoppingCartList
            var itemToRemove = shoppingCartList.Where(p=>p.ProductId==Id).SingleOrDefault();

            //If item to remove != null => Remove from cart
            if (itemToRemove != null)
            {
                shoppingCartList.Remove(itemToRemove);
            };

            //Set shoppingListCart again in Session
            HttpContext.Session.Set(WC.SessionCart, shoppingCartList);

            //redirect to HomePage/Index
            return RedirectToAction(nameof(Index));
        }




        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
