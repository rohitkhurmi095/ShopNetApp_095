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
        public IActionResult Details(int? Id)
        {
            //Check if id==0?
            if (Id == 0 || Id == null)
            {
                return NotFound();
            }

            //Find product by id + Eager loading of entites
            //Find first matching category else return null(default) (firstOrDefault)
            var product = _dbContext.Product.Include(c => c.Category).Include(a => a.ApplicationType).Where(p => p.Id == Id).FirstOrDefault();

            //Check if category is found
            if (product == null)
            {
                return NotFound();
            }

            ProductDetailsViewModel productDetails = new ProductDetailsViewModel
            {
                Product = product,   //productDetails
                existsInCart = false //(default)
            };

            //return ProductDetailsViewModel -> View
            return View(productDetails);
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
