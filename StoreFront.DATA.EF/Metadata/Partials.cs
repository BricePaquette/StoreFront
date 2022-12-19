using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;//Installed this package for access to ModelMetaDataType
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;//Added for access to DataAnnotations
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StoreFront.DATA.EF.Models//.Metadata
{
    [ModelMetadataType(typeof(CategoryMetadata))]
    public partial class Category
    {

    }
    [ModelMetadataType(typeof(OrderMetadata))]
    public partial class Order
    {

    }
    [ModelMetadataType(typeof(ProductMetadata))]
    public partial class Product
    {
        [NotMapped]
        public IFormFile Image { get; set; }
    }[ModelMetadataType(typeof(UserDetailMetadata))]
    public partial class UserDetail
    {

    }
}
