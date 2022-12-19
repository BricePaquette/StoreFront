using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StoreFront.DATA.EF.Models//.Metadata
{
    public class CategoryMetadata
    {
        [Display(Name = "Category")]
        public int CategoryId { get; set; }

        [Display(Name = "Name")]
        [Required]
        [StringLength(50, ErrorMessage = "*Must not exceed 50 characters")]
        public string CategoryName { get; set; } = null!;

        [Display(Name ="Description")]
        [StringLength(300, ErrorMessage = "*Must not exceed 300 characters")]
        [DisplayFormat(NullDisplayText = "[N/A]")]
        public string? CategoryDescription { get; set; }
    }
    public class OrderMetadata
    {
        //no metadata needed for FKs - as they are represented in a View by a dropdown list
        public string UserId { get; set; } = null!;
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:d}")]//0:d => MM/dd/yyyy
        [Display(Name = "Order Date")]
        [Required]
        public DateTime OrderDate { get; set; }

        [Required, StringLength(50, ErrorMessage = "*Must not exceed 50 characters"), Display(Name = "Planet Name")]
        public string PlanetName { get; set; } = null!;
        [StringLength(100)]
        [Display(Name = "Ship To")]
        [Required]
        public string ShipToName { get; set; } = null!;

        [StringLength(50)]
        [Display(Name = "City")]
        [Required]
        public string ShipCity { get; set; } = null!;

        [StringLength(2)]
        [Display(Name = "State")]
        public string? ShipState { get; set; }

        [StringLength(5)]
        [Display(Name = "Zip")]
        [Required]
        [DataType(DataType.PostalCode)]
        public string ShipZip { get; set; } = null!;
    }
    public class ProductMetadata
    {
        [Display(Name = "Name")]
        [Required]
        [StringLength(50, ErrorMessage = "*Must not exceed 50 characters")]
        public string ProductName { get; set; } = null!;

        [Display(Name = "Description")]
        [StringLength(350, ErrorMessage = "*Must not exceed 350 characters")]
        [DisplayFormat(NullDisplayText = "[N/A]")]
        public string? ProductDescription { get; set; }

        [Required]
        [Display(Name = "Discontinued")]
        [DisplayFormat(NullDisplayText = "[N/A]")]
        public bool? IsDiscontinued { get; set; }

        [Display(Name = "# In Stock")]
        [Range(0, int.MaxValue)]
        public int? ProductCount { get; set; }

        [Display(Name = "Price")]
        [Required]
        [Range(0, (double)decimal.MaxValue)]
        [DisplayFormat(DataFormatString = "{0:c}")]
        public decimal ProductPrice { get; set; }

        //public int CategoryId { get; set; }
        [Display(Name = "Image")]
        [StringLength(100, ErrorMessage = "*Must not exceed 100 characters")]
        public string? ProductImage { get; set; }

        [Display(Name = "# Ordered")]
        [Range(0, int.MaxValue)]
        public int? UnitsOnOrder { get; set; }
    }
    public class UserDetailMetadata
    {
        [Display(Name = "First Name")]
        [StringLength(50, ErrorMessage = "*Must not exceed 50 characters")]
        [Required]
        public string UserFirstName { get; set; } = null!;

        [Display(Name = "Last Name")]
        [StringLength(50, ErrorMessage = "*Must not exceed 50 characters")]
        [Required]
        public string UserLastName { get; set; } = null!;

        [Required]
        [StringLength(150)]
        public string? Address { get; set; }
        [StringLength(50)]
        [Required]
        public string? City { get; set; }
        [StringLength(2)]
        
        public string? State { get; set; }
        [StringLength(5)]
        [Required]
        [DataType(DataType.PostalCode)]
        public string? Zip { get; set; }
        [StringLength(24)]
        [DataType(DataType.PhoneNumber)]
        
        public string? Phone { get; set; }

        [Required, StringLength(50, ErrorMessage = "*Must not exceed 50 characters"), Display(Name = "Planet Name")]
        public string PlanetName { get; set; } = null!;
    }
}
