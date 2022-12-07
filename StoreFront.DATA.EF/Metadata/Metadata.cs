using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StoreFront.DATA.EF.Models//.Metadata
{
    public class CategoryMetadata
    {
        [Display(Name = "Name")]
        [Required]
        [StringLength(50, ErrorMessage = "*Must not exceed 50 characters")]
        public string CategoryName { get; set; } = null!;

        [Display(Name ="Description")]
        [StringLength(300, ErrorMessage = "*Must not exceed 300 characters")]
        public string? CategoryDescription { get; set; }
    }
    public class LocationMetadata
    {
        [Display(Name = "Planet Name")]
        [Required]
        [StringLength(50, ErrorMessage = "*Must not exceed 50 characters")]
        public string PlanetName { get; set; } = null!;

        [Display(Name = "City Name")]
        [StringLength(50, ErrorMessage = "*Must not exceed 50 characters")]
        public string? CityName { get; set; }

        [Display(Name = "Description")]
        [Required]
        [StringLength(500, ErrorMessage = "*Must not exceed 500 characters")]
        public string Description { get; set; } = null!;
    }
    public class OrderMetadata
    {
        [Display(Name = "Date Ordered")]
        [DisplayFormat(DataFormatString = "{0:d}")]
        [DataType(DataType.DateTime)]
        public DateTime? OrderDate { get; set; }
    }
    public class ProductMetadata
    {
        [Display(Name = "Name")]
        [Required]
        [StringLength(50, ErrorMessage = "*Must not exceed 50 characters")]
        public string ProductName { get; set; } = null!;

        [Display(Name = "Description")]
        [StringLength(350, ErrorMessage = "*Must not exceed 350 characters")]
        public string? ProductDescription { get; set; }

        [Display(Name = "Discontinued")]
        public bool? IsDiscontinued { get; set; }

        [Display(Name = "# In Stock")]
        [Range(0, int.MaxValue)]
        public int? ProductCount { get; set; }

        [Display(Name = "Price")]
        [Required]
        [Range(0, (double)decimal.MaxValue)]
        [DisplayFormat(DataFormatString = "{0:c}")]
        public decimal ProductPrice { get; set; }

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

        [Display(Name = "Crewmate")]
        [Required]
        public bool IsCrewmate { get; set; }
    }
}
