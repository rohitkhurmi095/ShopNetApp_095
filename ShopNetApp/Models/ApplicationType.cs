using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace ShopNetApp.Models
{
    //Application Type
    //=================
    [Table("ApplicationType")]
    public class ApplicationType
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "ApplicationType Name can't be empty!")]
        public string Name { get; set; }
    }
}
