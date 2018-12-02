using System;

namespace Quasar.Models
{
    public class UserProduct
    {
        public UserProduct()
        {
            this.Id = Guid.NewGuid();
        }

        public Guid Id { get; set; }

        public Guid ProductId { get; set; }
        public virtual Product Product { get; set; }

        public string UserId { get; set; }
        public virtual User User { get; set; }
    }
}
