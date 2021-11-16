using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using ShopNetApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ShopNetApp.Data
{
    //==========
    //DbContext
    //==========
    //EntityFramework Migrations:
    //add-migration "migrationName" -o Data/Migrations
    //update-database

    //Changing To Idnetity Db
    public class ApplicationDbContext : IdentityDbContext
    {
        //Pass ApplicationDbContext options -> EF Core DbContext
        //query data using DbContext instance
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {

        }


        //======
        //DbSet (Tables in Database)
        //======
        //SHOP 
        //-----
        public DbSet<Category> Category { get; set; }
        public DbSet<ApplicationType> ApplicationType { get; set; }
        public DbSet<Product> Product { get; set; }

        //IDENTITY 
        //---------
        //dbo.AspNet.Users
        public DbSet<ApplicationUser> ApplicationUser { get; set; }

    }
}
