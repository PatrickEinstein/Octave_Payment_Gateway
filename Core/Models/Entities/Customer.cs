using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace CentralPG.Core.Models.Entities
{
    public class Customer
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "FirstName is required")]
        [DisplayName("FirstName")]
        [StringLength(160)]
        public string FirstName { get; set; }

        [Required(ErrorMessage = "LastName is required")]
        [DisplayName("LastName")]
        [StringLength(160)]
        public string LastName { get; set; }

        [Required(ErrorMessage = "PhoneNumber is required")]
        [DataType(DataType.PhoneNumber)]
        [StringLength(24)]
        public string PhoneNumber { get; set; }

        [Required(ErrorMessage = "Address is required")]
        [StringLength(300)]
        public string Address { get; set; }

        [Required(ErrorMessage = "Email is required")]
        [DisplayName("Email Address")]
        [RegularExpression(@"[A-Za-z0-9._%+-]+@[A-Za-z0-9.-]+\.[A-Za-z]{2,4}",
          ErrorMessage = "Email is is not valid.")]
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }


        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }
    }
}
