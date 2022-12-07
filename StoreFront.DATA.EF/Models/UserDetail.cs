using System;
using System.Collections.Generic;

namespace StoreFront.DATA.EF.Models
{
    public partial class UserDetail
    {
        public UserDetail()
        {
            Orders = new HashSet<Order>();
        }

        public string UserId { get; set; } = null!;
        public string UserFirstName { get; set; } = null!;
        public string UserLastName { get; set; } = null!;
        public bool IsCrewmate { get; set; }

        public virtual ICollection<Order> Orders { get; set; }
    }
}
