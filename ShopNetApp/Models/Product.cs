using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace ShopNetApp.Models
{
    //Product
    //========
    [Table("Product")]
    public class Product
    {
        //Id
        [Key]
        public int Id { get; set; }

        //Name
        [Required]
        public string Name { get; set; }

        //ShortDescription
        public string ShortDescription { get; set; }

        //Description
        public string Description { get; set; }

        //Price
        [Required]
        [Range(1,int.MaxValue)]
        public double Price { get; set; }

        //Image
        //(Validate using sweetAlert2 for Create)
        //(No Validation required for Update)
        public string Image { get; set; }


        //------ FOREIGN KEYS ------
        //CATEGORY 
        [Display(Name="Category Type")]
        [ForeignKey("CategoryId")]
        public int CategoryId { get; set; }

        //APPLICATION TYPE ID
        [Display(Name="Application Type")]
        public int ApplicationTypeId { get; set; }
        

        //===== Navigation Properties =======
        //Product -> Category -> Id
        public virtual Category Category { get; set; }

        //Product -> ApplicationType -> Id
        public virtual ApplicationType ApplicationType { get; set; }
    }
}
