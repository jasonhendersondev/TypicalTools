using System.ComponentModel.DataAnnotations;

namespace TypicalTechTools.Models
{
    public class Comment
    {

        public int Id { get; set; }
        [Display(Name = "Comment")]
        public string Text { get; set; }
        [Display(Name = "Product Code")]
        public int ProductId { get; set; }

        /// <summary>
        /// Return a CSV formatted string of the a comment object
        /// </summary>
        /// <returns></returns>
        /// 

        public string? SessionId { get; set; }
        public DateTime CreatedDate { get; set; }

        //Defines relationship between comments and product tables. Each comment can only have one product.
        public virtual Product? Product { get; set; }

        public string ToCSVString()
        {
            return $"{Id},{Text},{ProductId}";
        }

    }
}
