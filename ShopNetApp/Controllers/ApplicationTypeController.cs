using cloudscribe.Pagination.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ShopNetApp.Data;
using ShopNetApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ShopNetApp.Controllers
{
    public class ApplicationTypeController : Controller
    {
        //DbContext
        //----------
        private readonly ApplicationDbContext _dbContext;
        public ApplicationTypeController(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }


        //========================
        //GET All ApplicationTypes + SEARCH
        //========================
        //pageNumber = currentPageNumber (Default=1)
        //pageSize = no.of.recordsPerPage (Default=5)
        public IActionResult Index(string search = "", int pageNumber = 1, int pageSize = 5)
        {
            //Get List(all applicationTypes) from DbContext
            List<ApplicationType> applicationTypes;

            //PAGINATION
            //-----------
            //cloudscribe.web.pagination
            //query = query.Skip(pageSize*pageNumber).Take(PageSize).AsNoTracking().ToList();
            //pagedList<T> result = new PagedList<T>(Data,TotalItems,PageNumber,PageSize)
            //TotalPages = TotalItems/PageSize
            var offset = (pageSize * pageNumber) - pageSize;

            //FILTER BY SEARCH by applicationTypeName
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
                applicationTypes = _dbContext.ApplicationType.Where(a => a.Name.Contains(search)).Skip(offset).Take(pageSize).AsNoTracking().ToList();
            }
            else
            {
                //Show all results with pagination
                applicationTypes = _dbContext.ApplicationType.AsNoTracking().Skip(offset).Take(pageSize).ToList();
            }


            //PAGED LIST
            //-----------
            var result = new PagedResult<ApplicationType>
            {
                Data = applicationTypes,
                TotalItems = _dbContext.ApplicationType.Count(),
                PageNumber = pageNumber,
                PageSize = pageSize
            };

            //TotalPages = TotalItems/PageSize
            ViewBag.TotalPages = (int)Math.Ceiling(_dbContext.ApplicationType.Count() / (double)pageSize);

            //return PagedList<categories> -> View
            return View(result);
        }




        //==========================
        //GET ApplicationType by Id
        //==========================
        public IActionResult Details(int? Id)
        {
            //Check if id==0?
            if (Id == 0 || Id == null)
            {
                return NotFound();
            }

            //Find applicationType by id
            //Find first matching category else return null(default) (firstOrDefault)
            var applicationType = _dbContext.ApplicationType.Where(a => a.Id == Id).FirstOrDefault();

            //Check if applicationType is found
            if (applicationType == null)
            {
                return NotFound();
            }

            //return category -> View
            return View(applicationType);
        }




        //=======================
        //CREATE ApplicationType
        //=======================
        //1.ADD ApplicationType FORM - GET
        //-------------------------
        public IActionResult Create()
        {
            //Add ApplicationType form
            return View();
        }

        //2.ADD ApplicationType - POST
        //----------------------
        [HttpPost]
        public IActionResult Create(ApplicationType applicationType)
        {
            //If valid Form state
            if (ModelState.IsValid)
            {
                //Add category to Db
                _dbContext.ApplicationType.Add(applicationType);
                //Save Db
                _dbContext.SaveChanges();

                //Redirect -> All Types after adding new ApplicationType
                return RedirectToAction("Index", "ApplicationType");
            }
            else
            {
                //Else return same view
                return View(applicationType);
            }
        }





        //=======================
        //UPDATE ApplicationType
        //=======================
        //1.Edit ApplicationType Form with Prefetched ApplicationType  data
        //--------------------
        public IActionResult Edit(int? Id)
        {
            //Check if id is valid 
            if (Id == null || Id == 0)
            {
                return NotFound();
            }

            //Find applicationType by Id
            //SingleProduct (FirstOrDefault() => returns First matching record else null)
            var applicationType = _dbContext.ApplicationType.Where(a => a.Id == Id).FirstOrDefault();

            //Check if applicationType is found or not
            if (applicationType == null)
            {
                return NotFound();
            }

            //Return applicationType(prefetched data) -> Edit Form
            return View(applicationType);
        }

        //2.UPDATE ApplicationType - ApplicationTypeId (from EDIT ApplicationTypeForm)
        //-----------------------------
        [HttpPost]
        public IActionResult Edit(int? Id, ApplicationType appType)
        {
            //Find ApplicationType by Id
            var applicationType = _dbContext.ApplicationType.AsNoTracking().Where(a => a.Id == Id).FirstOrDefault();

            //If category not found
            if (applicationType == null)
            {
                return NotFound();
            }

            //If valid Form state
            if (ModelState.IsValid)
            {
                //UPDATE entities In Db
                applicationType.Name = appType.Name;

                //UpdateDb 
                _dbContext.ApplicationType.Update(appType);
                //Save Db
                _dbContext.SaveChanges();

                //Redirect ->  after ApplicationTypes new Category
                return RedirectToAction("Index", "ApplicationType");
            }
            else
            {
                //Else -> return same view
                return View(applicationType);
            }
        }





        //=======================
        //DELETE ApplicationType
        //=======================
        //1.DELETE ApplicationType Page with Prefetched ApplicationType data
        //--------------------
        public IActionResult Delete(int? Id)
        {
            //Check if id is valid 
            if (Id == null || Id == 0)
            {
                return NotFound();
            }

            //Find applicationType by Id
            //SingleProduct (FirstOrDefault() => returns First matching record else null)
            var applicationType = _dbContext.ApplicationType.Where(a => a.Id == Id).FirstOrDefault();

            //Check if applicationType is found or not
            if (applicationType == null)
            {
                return NotFound();
            }

            //Return applicationType(prefetched data) -> DeletePage
            return View(applicationType);
        }


        //2.DELETE ApplicationType - ApplicationTypeId (from DeleteApplicationPage)
        //-----------------------------
        [HttpPost]
        public IActionResult Delete(int? Id, ApplicationType appType)
        {
            //Find applicationType by Id
            //SingleProduct (FirstOrDefault() => returns First matching record else null)
            var applicationType = _dbContext.ApplicationType.Where(a => a.Id == Id).FirstOrDefault();

            //If category is not found
            if(applicationType == null)
            {
                return NotFound();
            }

            //Remove category from Database
            _dbContext.ApplicationType.Remove(applicationType);
            //SaveChanges()
            _dbContext.SaveChanges();

            //After removing applicationType -> goto ApplicationType Page
            return RedirectToAction("Index", "ApplicationType");
        }





    }
}
