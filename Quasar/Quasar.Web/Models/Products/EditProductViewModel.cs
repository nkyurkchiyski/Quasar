﻿using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Quasar.Web.Models.Products
{
    public class EditProductViewModel
    {
        public Guid Id { get; set; }

        [Required]
        [DataType(DataType.Text)]
        [Display(Name = "Product Name")]
        public string Name { get; set; }

        [Required]
        [DataType(DataType.MultilineText)]
        [Display(Name = "Description")]
        public string Description { get; set; }

        [Required]
        [Range(1, int.MaxValue)]
        [Display(Name = "Quantity")]
        public int TotalQuantity { get; set; }

        [Required]
        [Range(0.01, double.MaxValue)]
        [DataType(DataType.Currency)]
        [Display(Name = "Price")]
        public decimal Price { get; set; }

        [Required]
        [DataType(DataType.Text)]
        [Display(Name = "Product Type")]
        public string Type { get; set; }

        [Required]
        [DataType(DataType.Text)]
        [Display(Name = "Product Category")]
        public string Category { get; set; }

        [Required]
        [DataType(DataType.Text)]
        [Display(Name = "Product Platform")]
        public string Platform { get; set; }
        
        public IFormFile NewCover { get; set; }
        
        public IFormFile[] NewImages { get; set; }
        
        public ImageViewModel Cover { get; set; }

        public ICollection<ImageViewModel> Images { get; set; }
        
    }
}
