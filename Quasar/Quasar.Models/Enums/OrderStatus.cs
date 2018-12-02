using System;
using System.Collections.Generic;
using System.Text;

namespace Quasar.Models.Enums
{
    public enum OrderStatus
    {
        AwaitingUser = 0,
        Pending = 1,
        Shipped = 2,
        Delivered = 3,
        Acquired = 4
    }
}
