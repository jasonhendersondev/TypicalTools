using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TypicalTechTools.Models
{
    public class Product
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public decimal Price { get; set; }
        public string Description { get; set; }

        //Defines the relationship between comments and products. Each product can have many comments.
        public virtual ICollection<Comment>? Comments { get; set; }
        public DateTime UpdatedDate { get; set; }
    }
}
