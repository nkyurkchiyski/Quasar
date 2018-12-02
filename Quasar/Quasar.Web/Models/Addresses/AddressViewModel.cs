using System.ComponentModel.DataAnnotations;

namespace Quasar.Web.Models.Addresses
{
    public class AddressViewModel
    {
        [Required]
        [DataType(DataType.Text)]
        [Display(Name = "Country")]
        public string Country { get; set; }

        [Required]
        [DataType(DataType.Text)]
        [Display(Name = "City")]
        public string City { get; set; }

        [Required]
        [DataType(DataType.Text)]
        [Display(Name = "Street")]
        public string Street { get; set; }

        [Required]
        [DataType(DataType.Text)]
        [Display(Name = "Post Code")]
        public string PostCode { get; set; }
    }
}
