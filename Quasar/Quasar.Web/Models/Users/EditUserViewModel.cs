using Quasar.Web.Models.Addresses;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Quasar.Web.Models.Users
{
    public class EditUserViewModel
    {
        [Required]
        [DataType(DataType.Text)]
        [Display(Name = "Username")]
        [MinLength(3)]
        [RegularExpression(@"^[\w\-\.\*\~]*$")]
        public string UserName { get; set; }

        [Required]
        [DataType(DataType.Text)]
        [Display(Name = "First Name")]
        public string FirstName { get; set; }

        [Required]
        [DataType(DataType.Text)]
        [Display(Name = "LastName")]
        public string LastName { get; set; }

        [Required]
        [DataType(DataType.EmailAddress)]
        [Display(Name = "Email")]
        public string Email { get; set; }

        [Required]
        public AddressViewModel Address { get; set; }
    }
}
