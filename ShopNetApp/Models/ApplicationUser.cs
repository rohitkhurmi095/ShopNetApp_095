using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ShopNetApp.Models
{
    //ApplicationUser:Identity user
    //Add more fields to [dbo.AspNet.Users] Table
    public class ApplicationUser : IdentityUser
    {
        //FirstName
        public string FirstName { get; set; }

        //LastName
        public string LastName { get; set; }
    }
}
