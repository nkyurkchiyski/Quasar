using System;
using System.Collections.Generic;
using System.Text;

namespace Quasar.Models
{
    public class Image
    {
        public Image()
        {
            this.Id = Guid.NewGuid();
        }

        public Guid Id { get; set; }

        public string PublicId { get; set; }

        public string Path { get; set; }

        public Guid ProductId { get; set; }
        public virtual Product Product { get; set; }
    }
}
