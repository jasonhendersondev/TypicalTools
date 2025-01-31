using Microsoft.EntityFrameworkCore;
using TypicalTechTools.Models.Data;

namespace TypicalTechTools.Models.Repositories
{
    public class CommentRepository : ICommentRepository
    {

        //Create a readonly field to hold a reference to our context class
        private readonly TypistTechToolsDBContext _context;

        //Request the context from the dependency injection by naming it in the constructor
        public CommentRepository(TypistTechToolsDBContext context)
        {
            _context = context;
        }

        public void CreateComment(Comment comment)
        {

            //Pass the comment to the context class to be added to the DBset
            _context.Comments.Add(comment);
            //Save all DBset changes to the database.
            _context.SaveChanges();
        }

        public void DeleteSingleComment(int id)
        {
            //Remove comment where provided id matches the appropriate comment id in the database
            var comment = _context.Comments.Where(c => c.Id == id).FirstOrDefault();
            _context.Comments.Remove(comment);
            _context.SaveChanges();
        }

        public List<Comment> GetAllComments(int productId)
        {
            //Return all comments where the product ID FK of the comment matches the product PK
            //return _context.Comments
            //                        .Include(c => c.Text).ToList();

            if (productId == 0)
            {
                return null;
            }

            //var allComments = ParseComments();

            return _context.Comments.Where(c => c.ProductId == productId).ToList();

            // Return all comments where the productcode matches the provided product code
          



        }

        public Comment GetSingleComment(int commentId)
        {
            return _context.Comments.Where(c => c.Id == commentId).Include(c=>c.Product).FirstOrDefault();
        }

        public void UpdateComment(Comment comment)
        {
            //Take the provided comment and update existing record and save changes.
            _context.Comments.Update(comment);
            _context.SaveChanges();
        }
    }
}
