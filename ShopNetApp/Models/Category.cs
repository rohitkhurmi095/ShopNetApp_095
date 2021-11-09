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
        [Required(ErrorMessage = "CategoryName can't be empty!")]
        public string Name { get; set; }

        //DisplayOrder
        [Required(ErrorMessage = "DisplayOrderId can't be empty!")]
        [Range(1,int.MaxValue,ErrorMessage="Display Order for category must be greater than 0")]
        [DisplayName("Display Order")]
        public int? DisplayOrder { get; set; }
    }
}
