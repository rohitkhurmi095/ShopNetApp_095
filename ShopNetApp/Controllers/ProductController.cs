using cloudscribe.Pagination.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ShopNetApp.Data;
using ShopNetApp.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace ShopNetApp.Controllers
{
    public class ProductController : Controller
    {
        //DEPENDENCY INJECTION
        //=====================
        //DbContext
        //----------
        private readonly ApplicationDbContext _dbContext;
        //HostEnvironment - to access wwwroot folder (for fileUpload)
        private readonly IWebHostEnvironment _webHostEnvironment;
        public ProductController(ApplicationDbContext dbContext, IWebHostEnvironment webHostEnvironment)
        {
            _dbContext = dbContext;
            _webHostEnvironment = webHostEnvironment;
        }


        //=================
        //GET All Products
        //=================
        //Include(x=>x.virtualProperty) = EagerLoading of entities
        //pageNumber = currentPageNumber (Default=1)
        //pageSize = no.of.recordsPerPage (Default=5)
        public IActionResult Index(string search = "", int pageNumber = 1, int pageSize = 5)
        {
             //Get List(all products) from DbContext
             List<Product> products;

             //PAGINATION
             //-----------
             //cloudscribe.web.pagination
             //query = query.Skip(pageSize*pageNumber).Take(PageSize).AsNoTracking().ToList();
             //pagedList<T> result = new PagedList<T>(Data,TotalItems,PageNumber,PageSize)
             //TotalPages = TotalItems/PageSize
             var offset = (pageSize * pageNumber) - pageSize;

              //FILTER BY SEARCH by productName
              //-----------------
              //Passing SearchTerm -> formInput
              ViewBag.Search = search;

              //If search term is null or empty => Disply all results
              //Else display only searchResult
              //AsNoTracking() => if we are only reading query it gives better perfomance
              //No need to track the query
               if (!string.IsNullOrEmpty(search))
               {
                    //show searched result with Pagination
                    products = _dbContext.Product.Include(c => c.Category).Include(a => a.ApplicationType).Where(c => c.Name.Contains(search)).Skip(offset).Take(pageSize).AsNoTracking().ToList();
               }
               else
               {
                    //Show all results with pagination
                    products = _dbContext.Product.Include(c=>c.Category).Include(a=>a.ApplicationType).AsNoTracking().Skip(offset).Take(pageSize).ToList();
               }


               //PAGED LIST
               //-----------
               var result = new PagedResult<Product>
               {
                    Data = products,
                    TotalItems = _dbContext.Product.Count(),
                    PageNumber = pageNumber,
                    PageSize = pageSize
               };

               //TotalPages = TotalItems/PageSize
               ViewBag.TotalPages = (int)Math.Ceiling(_dbContext.Product.Count() / (double)pageSize);

               //return PagedList<products> -> View
               return View(result);
        }





        //======================
        //GET Product by Id
        //======================
        //Include(x=>x.virtualProperty) = EagerLoading of entities
        public IActionResult Details(int? Id)
        {
            //Check if id==0?
            if (Id == 0 || Id == null)
            {
                return NotFound();
            }

            //Find category by id
            //Find first matching product else return null(default) (firstOrDefault)
            var product = _dbContext.Product.Include(c => c.Category).Include(a => a.ApplicationType).Where(p => p.Id == Id).FirstOrDefault();

            //Check if category is found
            if (product == null)
            {
                return NotFound();
            }

            //return category -> View
            return View(product);
        }





        //================
        //CREATE Product
        //================
        //1.ADD Product FORM - GET
        //-------------------------
        public IActionResult Create()
        {
            //NOTE: To get Dropdowns for ApplicationType|Category
            //------------------------------------------------------
            //Pass categories|ApplicationTypes -> CREATE Product FORM Page
             ViewBag.Categories = _dbContext.Category.ToList();
             ViewBag.ApplicationTypes = _dbContext.ApplicationType.ToList();

             //Add Product form
             return View();
        }


        //2.ADD Product - POST
       //----------------------
       [HttpPost]
       public IActionResult Create(Product product)
       {
            //If valid Form state
            if (ModelState.IsValid)
            {

                    //=============
                    //IMAGE UPLOAD - wwwroot/images/product
                    //=============
                    //Read file from Form request - using HttpRequest
                    var files = HttpContext.Request.Form.Files;

                    //Get wwwroot folder using - IWebHostEnvironement
                    string webRootPath = _webHostEnvironment.WebRootPath;

                    //FileUpload Settings
                    string upload = webRootPath + WC.ImagePath;                //uploadPath
                    string fileName = Guid.NewGuid().ToString();              //random fileName 
                    string extension = Path.GetExtension(files[0].FileName);  //upload extension

                    //CREATE FileSteam data in UploadPath
                    using (var fileSteam = new FileStream(Path.Combine(upload,fileName+extension),FileMode.Create))
                    {
                        //copy file data -> fileSteam(Upload path)
                        files[0].CopyTo(fileSteam);
                    }

                    //Update Product Image
                    product.Image = fileName + extension;
                    //---------------------
                   
                    //AddProduct to Db + SaveChanges
                    _dbContext.Product.Add(product);
                    //SaveChanges
                    _dbContext.SaveChanges();
               
                    //After adding product -> show all Products
                    return RedirectToAction("Index", "Product");

            }else{
                    //Else return same view
                    return View();
            }
       }






        //================
        //UPDATE Product
        //================
        //1.Edit Product Form with Prefetched Category data
        //--------------------
        public IActionResult Edit(int? Id)
        {
                //Check if id is valid 
                if (Id == null || Id == 0)
                {
                    return NotFound();
                }

                //Find Product by Id
                //SingleProduct (FirstOrDefault() => returns First matching record else null)
                var product = _dbContext.Product.Include(c=>c.Category).Include(a=>a.ApplicationType).Where(p => p.Id == Id).FirstOrDefault();

                //Check if category is found or not
                if (product == null)
                {
                    return NotFound();
                }


                //NOTE: To get Dropdowns for ApplicationType|Category
                //------------------------------------------------------
                //Pass categories|ApplicationTypes -> EDIT Product FORM Page
                ViewBag.Categories = _dbContext.Category.ToList();
                ViewBag.ApplicationTypes = _dbContext.ApplicationType.ToList();

               //Return category(prefetched data) -> Edit Form
                return View(product);
        }

        //2.UPDATE Product - ProductId (from EDIT ProductForm)
        //-----------------------------
        [HttpPost]
         public IActionResult Edit(int? Id, Product p)
         {
                //Find Category by Id
                var product = _dbContext.Product.AsNoTracking().Where(p => p.Id == Id).FirstOrDefault();

                //If product not found
                if (product == null)
                {
                    return NotFound();
                }

                //If valid Form state
                if (ModelState.IsValid)
                {

                    //IMAGE UPDATE - wwwrootfolder/images/products
                    //=============
                    //NOTE: IF image is already uploaded + user tries to upload new Photo => remove oldPhoto + updload NewPhoto

                    //Read file from Form request - using HttpRequest
                    var files = HttpContext.Request.Form.Files;

                    //Get wwwroot folder using - IWebHostEnvironement
                    string webRootPath = _webHostEnvironment.WebRootPath;

                    //If new File has been uploaded(Modified Image)
                    if (files.Count>0)
                    {
                         //FileUpload Settings
                        string upload = webRootPath + WC.ImagePath;                //uploadPath
                        string fileName = Guid.NewGuid().ToString();              //random fileName 
                        string extension = Path.GetExtension(files[0].FileName);  //upload extension

                        //REMOVE OldFile(previously uploaded Image)
                        //OldFile
                        var OldFile = Path.Combine(upload, product.Image);
                        if (System.IO.File.Exists(OldFile))
                        {
                             System.IO.File.Delete(OldFile);
                        }

                        //CREATE FileSteam data in UploadPath(NewFile)
                        using (var fileSteam = new FileStream(Path.Combine(upload, fileName + extension), FileMode.Create))
                        {
                            //copy file data -> fileSteam(Upload path)
                            files[0].CopyTo(fileSteam);
                        }

                        //UPDATE ProductImage in Db
                        p.Image = fileName + extension;
                    }
                    else{
                        //IF Image was not Modified
                        p.Image = product.Image;
                    }

                    //UPDATE product properties In Db
                    product.Name = p.Name;
                    product.Price = p.Price;
                    product.CategoryId = p.CategoryId;
                    product.ApplicationTypeId = p.ApplicationTypeId;
                    product.ShortDescription = p.ShortDescription;
                    product.Description = p.Description;

                    //Save Db
                     _dbContext.Product.Update(p);
                    _dbContext.SaveChanges();

                   //Redirect -> Categories after adding new Category
                    return RedirectToAction("Index", "Product");
                }
                else
                {
                    //Else -> return same view
                    return View();
                }
         }





        //================
        //DELETE Product 
        //================
        //1.DELETE Category Page with Prefetched Category data
        //--------------------
        public IActionResult Delete(int? Id)
          {
            //Check if id is valid 
            if (Id == null || Id == 0)
            {
                return NotFound();
            }

            //Find PRODUCT by Id + EagerLoad entities
            //SingleProduct (FirstOrDefault() => returns First matching record else null)
            var product = _dbContext.Product.Include(c => c.Category).Include(a => a.ApplicationType).Where(p => p.Id == Id).FirstOrDefault();

            //Check if product is found or not
            if (product == null)
            {
                return NotFound();
            }

            //Return product(prefetched data) -> DeletePage
            return View(product);
          }

           //2.DELETE Category - CategoryId (from DeleteCategoryPage)
          //--------------------------------
          [HttpPost]
          public IActionResult Delete(int? Id,Product p)
          {
            //Find category by Id + EagerLoading of entities
            //SingleProduct (FirstOrDefault() => returns First matching record else null)
            var prod = _dbContext.Product.Include(c => c.Category).Include(a => a.ApplicationType).Where(p => p.Id == Id).FirstOrDefault();

            //If product is not found
            if (prod == null)
            {
                return NotFound();
            }


            //REMOVE Image From Folder
            //-------------------------
            string upload = _webHostEnvironment.WebRootPath + WC.ImagePath; //wwwrootFolder ImagePath
            var oldFile = Path.Combine(upload, prod.Image);  //Image PRESENT in the currentProduct

            //remove Image(if exists)
            if (System.IO.File.Exists(oldFile))
            {
                System.IO.File.Delete(oldFile);
            }
            //---------------------------

            //Remove product from Database
            _dbContext.Product.Remove(prod);
            //SaveChanges()
            _dbContext.SaveChanges();

            //After removing product -> goto Products Page
            return RedirectToAction("Index", "Product");
          }
    }
}
