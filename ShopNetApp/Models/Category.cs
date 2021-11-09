using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace ShopNetApp.Models
{
    //Category
    //=========
    [Table("Category")]
    public class Category
    {
        [Key]
        //CategoryId
        public int Id { get; set; }

        //CategoryName
        public string Name { get; set; }

        //DisplayOrder
        [DisplayName("Display Order")]
        public int DisplayOrder { get; set; }
    }
}
