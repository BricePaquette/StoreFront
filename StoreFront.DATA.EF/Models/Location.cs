using System;
using System.Collections.Generic;

namespace StoreFront.DATA.EF.Models
{
    public partial class Location
    {
        public Location()
        {
            Orders = new HashSet<Order>();
        }

        public int LocationId { get; set; }
        public string PlanetName { get; set; } = null!;
        public string? CityName { get; set; }
        public string Description { get; set; } = null!;

        public virtual ICollection<Order> Orders { get; set; }
    }
}
