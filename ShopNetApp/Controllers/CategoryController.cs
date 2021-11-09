using Microsoft.AspNetCore.Mvc;
using ShopNetApp.Data;
using ShopNetApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ShopNetApp.Controllers
{
    public class CategoryController : Controller
    {
        //DbContext
        //----------
        private ApplicationDbContext _dbContext;
        public CategoryController(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }


        //===================
        //GET All Categories + SEARCH
        //===================
        public IActionResult Index(string search="")
        {
            //Get List(all categories) from DbContext
            List<Category> categories;

            //FILTER BY SEARCH by categoryName
            //-----------------
            //Passing SearchTerm -> formInput
            ViewBag.Search = search;

            //If search term is null or empty => Disply all results
            //Else display only searchResult
            if (!string.IsNullOrEmpty(search)){ 
                categories = _dbContext.Category.Where(c => c.Name.Contains(search)).ToList();
            }
            else
            {
                categories = _dbContext.Category.ToList();
            }

            //return categories -> View
            return View(categories);
        }

        //======================
        //GET Category by Id
        //======================
        public IActionResult Details(int? Id)
        {
            //Check if id==0?
            if (Id == 0 || Id == null)
            {
                return NotFound();
            }

            //Find category by id
            //Find first matching category else return null(default) (firstOrDefault)
            var category = _dbContext.Category.Where(c => c.Id == Id).FirstOrDefault();

            //Check if category is found
            if (category == null)
            {
                return NotFound();
            }

            //return category -> View
            return View(category);
        }




        //================
        //CREATE Category
        //================
        //1.ADD Category FORM - GET
        //-------------------------
        public IActionResult Create()
        {
            //Add Category form
            return View();
        }


        //2.ADD Category - POST
        //----------------------
        [HttpPost]
        public IActionResult Create(Category category)
        {
            //If valid Form state
            if (ModelState.IsValid)
            {
                //Add category to Db
                _dbContext.Category.Add(category);
                //Save Db
                _dbContext.SaveChanges();

                //Redirect -> Categories after adding new Category
                return RedirectToAction("Index", "Category");
            }
            else
            {
                //Else return same view
                return View(category);
            }
        }




        //================
        //UPDATE Category
        //================
        //1.Edit Category Form with Prefetched Category data
        //--------------------
        public IActionResult Edit(int? Id)
        {
            //Check if id is valid 
            if (Id == null || Id == 0)
            {
                return NotFound();
            }

            //Find category by Id
            //SingleProduct (FirstOrDefault() => returns First matching record else null)
            var category = _dbContext.Category.Where(c => c.Id == Id).FirstOrDefault();

            //Check if category is found or not
            if (category == null)
            {
                return NotFound();
            }

            //Return category(prefetched data) -> Edit Form
            return View(category);
        }

        //2.UPDATE Category - CategoryId (from EDIT CategoryForm)
        //-----------------------------
        [HttpPost]
        public IActionResult Edit(int? Id,Category c)
        {
            //Find Category by Id
            var category = _dbContext.Category.Where(c => c.Id == Id).FirstOrDefault();

            //If category not found
            if(category == null)
            {
                return NotFound();
            }

            //If valid Form state
            if (ModelState.IsValid)
            {
                //UPDATE category In Db
                category.Name = c.Name;
                category.DisplayOrder = c.DisplayOrder;
                
                //Save Db
                _dbContext.SaveChanges();

                //Redirect -> Categories after adding new Category
                return RedirectToAction("Index", "Category");
            }
            else
            {
                //Else -> return same view
                return View(category);
            }
        }




        //=================
        //DELETE Category
        //=================
        //1.DELETE Category Page with Prefetched Category data
        //--------------------
        public IActionResult Delete(int? Id)
        {
            //Check if id is valid 
            if (Id == null || Id == 0)
            {
                return NotFound();
            }

            //Find category by Id
            //SingleProduct (FirstOrDefault() => returns First matching record else null)
            var category = _dbContext.Category.Where(c => c.Id == Id).FirstOrDefault();

            //Check if category is found or not
            if (category == null)
            {
                return NotFound();
            }

            //Return category(prefetched data) -> DeletePage
            return View(category);
        }


        //2.DELETE Category - CategoryId (from DeleteCategoryPage)
        //-----------------------------
        [HttpPost]
        public IActionResult Delete(int? Id, Category c)
        {
            //Find category by Id
            //SingleProduct (FirstOrDefault() => returns First matching record else null)
            var catg = _dbContext.Category.Where(c => c.Id == Id).FirstOrDefault();

            //If category is not found
            if (catg == null)
            {
                return NotFound();
            }

            //Remove category from Database
            _dbContext.Category.Remove(catg);
            //SaveChanges()
            _dbContext.SaveChanges();

            //After removing category -> goto Categories Page
            return RedirectToAction("Index", "Category");
        }



    }
}
