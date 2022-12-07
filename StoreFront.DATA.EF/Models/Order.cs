using System;
using System.Collections.Generic;

namespace StoreFront.DATA.EF.Models
{
    public partial class Order
    {
        public Order()
        {
            OrderProducts = new HashSet<OrderProduct>();
        }

        public int OrderId { get; set; }
        public string UserId { get; set; } = null!;
        public int LocationId { get; set; }
        public DateTime? OrderDate { get; set; }

        public virtual Location Location { get; set; } = null!;
        public virtual UserDetail User { get; set; } = null!;
        public virtual ICollection<OrderProduct> OrderProducts { get; set; }
    }
}
